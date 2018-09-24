// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace FluentEditorShared.ObjectPool
{
    // Wrapper for making object pools of types which do not themselves implement IPoolableObject
    // This is not thread safe and assumes the object pool will handle any synchronization
    public class PoolableWrapper<T> : IPoolableObject
    {
        public PoolableWrapper(T payload, Action<T> resetHandler = null, Action<T> activationHandler = null)
        {
            _payload = payload;
            _resetHandler = resetHandler;
            _activationHandler = activationHandler;
        }

        private bool _isLive = false;
        public bool IsLive
        {
            get { return _isLive; }
        }

        private readonly Action<T> _resetHandler = null;
        public void Reset()
        {
            if(_resetHandler != null)
            {
                _resetHandler(_payload);
            }
            _isLive = false;
        }

        private readonly Action<T> _activationHandler = null;
        public void Activate()
        {
            if(_activationHandler != null)
            {
                _activationHandler(_payload);
            }
            _isLive = true;
        }

        private readonly T _payload = default;
        public T Payload
        {
            get { return _payload; }
        }
    }
}
