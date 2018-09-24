// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentEditorShared.MessageBus
{
    public class MessageBus : IMessageBus, IReadOnlyMessageBus
    {
        #region Listener wrappers

        private interface ISubscriberAdapter
        {
            bool NeedsPruning();
            bool Lock();
            void Unlock();
            bool Broadcast<T>(in T message, bool unlock = true) where T : struct;
            bool CompareTarget(object target);
            ISubscriberAdapter Clone();
        }

        // Not thread safe, relies on the messagebus itself for synchronization
        private class ListenerAdapter : ISubscriberAdapter
        {
            public ListenerAdapter(WeakReference<IMessageBusListener> listener)
            {
                _listener = listener;
            }

            private readonly WeakReference<IMessageBusListener> _listener = null;
            private IMessageBusListener _liveRef = null;

            public bool Lock()
            {
                if (_liveRef != null)
                {
                    return true;
                }
                return _listener.TryGetTarget(out _liveRef);
            }

            public void Unlock()
            {
                _liveRef = null;
            }

            public bool NeedsPruning()
            {
                var retVal = !Lock();
                Unlock();
                return retVal;
            }

            public bool Broadcast<T>(in T message, bool unlock = true) where T : struct
            {
                if (_liveRef == null)
                {
                    if (!Lock())
                    {
                        return false;
                    }
                }

                _liveRef.OnMessage<T>(message);
                if (unlock)
                {
                    Unlock();
                }
                return true;
            }

            public bool CompareTarget(object target)
            {
                if (target == null)
                {
                    return false;
                }
                if (_liveRef != null)
                {
                    return _liveRef == target;
                }
                IMessageBusListener liveRef = null;
                if (!_listener.TryGetTarget(out liveRef))
                {
                    return false;
                }
                return liveRef == target;
            }

            public ISubscriberAdapter Clone()
            {
                var retVal = new ListenerAdapter(_listener);
                retVal._liveRef = _liveRef;
                return retVal;
            }
        }

        // Not thread safe, relies on the messagebus itself for synchronization
        private class DelegateAdapter<T> : ISubscriberAdapter where T : struct
        {
            public DelegateAdapter(Action<T> targetDelegate)
            {
                if (targetDelegate == null)
                {
                    throw new ArgumentNullException("targetDelegate");
                }
                _delegate = targetDelegate;
            }

            private readonly Action<T> _delegate;

            public bool Lock() { return true; }

            public void Unlock() { /* Deliberately empty */ }

            public bool NeedsPruning() { return false; }

            public bool Broadcast<U>(in U message, bool unlock = true) where U : struct
            {
                if (_delegate is Action<U> d)
                {
                    d(message);
                    return true;
                }
                return false;
            }

            public bool CompareTarget(object target)
            {
                if (target == null || _delegate == null)
                {
                    return false;
                }
                return target.Equals(_delegate);
            }

            public ISubscriberAdapter Clone()
            {
                return new DelegateAdapter<T>(_delegate);
            }
        }

        #endregion

        public MessageBus() { }

        private object _lock = new object();
        private Dictionary<Type, List<ISubscriberAdapter>> _subscriptions = new Dictionary<Type, List<ISubscriberAdapter>>();

        public void AddListener<T>(IMessageBusListener listener) where T : struct
        {
            lock (_lock)
            {
                var messageType = typeof(T);
                if (!_subscriptions.ContainsKey(messageType))
                {
                    var subList = new List<ISubscriberAdapter>(1);
                    subList.Add(new ListenerAdapter(new WeakReference<IMessageBusListener>(listener)));
                    _subscriptions.Add(messageType, subList);
                }
                else
                {
                    var subList = _subscriptions[typeof(T)];
                    if (null == subList.FirstOrDefault((a) => a.CompareTarget(listener)))
                    {
                        subList.Add(new ListenerAdapter(new WeakReference<IMessageBusListener>(listener)));
                    }
                }
            }
        }

        public void AddListener<T>(Action<T> d) where T : struct
        {
            lock (_lock)
            {
                var messageType = typeof(T);
                if (!_subscriptions.ContainsKey(messageType))
                {
                    var subList = new List<ISubscriberAdapter>(1);
                    subList.Add(new DelegateAdapter<T>(d));
                    _subscriptions.Add(messageType, subList);
                }
                else
                {
                    var subList = _subscriptions[typeof(T)];
                    if (null == subList.FirstOrDefault((a) => a.CompareTarget(d)))
                    {
                        subList.Add(new DelegateAdapter<T>(d));
                    }
                }
            }
        }

        public void RemoveListener<T>(IMessageBusListener listener) where T : struct
        {
            lock (_lock)
            {
                var messageType = typeof(T);
                if (_subscriptions.ContainsKey(messageType))
                {
                    var subList = _subscriptions[messageType];
                    subList.RemoveAll((a) => a.CompareTarget(listener));
                }
            }
        }

        public void RemoveListener<T>(Action<T> d) where T : struct
        {
            lock (_lock)
            {
                var messageType = typeof(T);
                if (_subscriptions.ContainsKey(messageType))
                {
                    var subList = _subscriptions[messageType];
                    subList.RemoveAll((a) => a.CompareTarget(d));
                }
            }
        }

        // Currently this generates some garbage per broadcast in the form of an array holding the recipients of the message (needs to make a copy of the listener list in case listeners Remove themselves in response to the message)
        // this could be mitigated at the cost of some complexity if it becomes a problem
        public void BroadcastMessage<T>(in T message) where T : struct
        {
            List<ISubscriberAdapter> broadcastList = null;

            lock (_lock)
            {
                var messageType = typeof(T);
                if (!_subscriptions.ContainsKey(messageType))
                {
                    return;
                }
                var subList = _subscriptions[messageType];
                if (subList.Count == 0)
                {
                    return;
                }
                broadcastList = new List<ISubscriberAdapter>(subList.Count);
                foreach (var sub in subList)
                {
                    if (sub.Lock())
                    {
                        broadcastList.Add(sub.Clone());
                    }
                    sub.Unlock();
                }
            }

            if (broadcastList.Count > 0)
            {
                foreach (var subscriber in broadcastList)
                {
                    subscriber.Broadcast(in message, true);
                }
            }
        }

        public void PruneListeners()
        {
            lock (_lock)
            {
                foreach (var type in _subscriptions.Keys)
                {
                    var subList = _subscriptions[type];
                    subList.RemoveAll((a) => a.NeedsPruning());
                }
            }
        }
    }
}
