// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace FluentEditorShared.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T> where T : IPoolableObject
    {
        public ObjectPool(int initialSize, Func<T> factory, double growthFactor = 1.5)
        {
            if (initialSize <= 1)
            {
                throw new ArgumentException("initialSize must be at least 2");
            }
            if(growthFactor <= 1f)
            {
                throw new ArgumentException("growthFactor must be greater than 1.0");
            }
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            _factory = factory;
            _growthFactor = growthFactor;
            _pool = new T[initialSize];
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = _factory();
                _pool[i].Reset();
            }
            _currentIndex = 0;
        }

        private object _lock = new object();

        private readonly Func<T> _factory;
        private readonly double _growthFactor;
        private T[] _pool;

        private int _currentIndex;

        public T GetObject()
        {
            lock (_lock)
            {
                if (!_pool[_currentIndex].IsLive)
                {
                    T target = _pool[_currentIndex];
                    _currentIndex++;
                    if (_currentIndex >= _pool.Length)
                    {
                        _currentIndex = 0;
                    }
                    target.Activate();
                    return target;
                }
                for (int i = 0; i < _pool.Length; i++)
                {
                    int index = (i + _currentIndex) % _pool.Length;

                    if (!_pool[index].IsLive)
                    {
                        T target = _pool[index];
                        _currentIndex = index + 1;
                        if (_currentIndex >= _pool.Length)
                        {
                            _currentIndex = 0;
                        }
                        target.Activate();
                        return target;
                    }
                }

                // Resize needed. If this happens at all frequently then initialSize and / or growthFactor are too small. 
                int firstNewIndex = _pool.Length;

                double rawNewSize = Math.Ceiling((double)_pool.Length * _growthFactor);
                if (rawNewSize >= (double)int.MaxValue)
                {
                    throw new Exception("Object pool attempting to grow larger than int.MaxValue");
                }
                int newSize = (int)rawNewSize;

                Array.Resize(ref _pool, newSize);
                for(int i=firstNewIndex;i<_pool.Length;i++)
                {
                    _pool[i] = _factory();
                    _pool[i].Reset();
                }

                T resizedTarget = _pool[firstNewIndex];
                _currentIndex = firstNewIndex + 1;
                if(_currentIndex >= _pool.Length)
                {
                    _currentIndex = 0;
                }
                resizedTarget.Activate();
                return resizedTarget;
            }
        }

        public void ReturnObject(T target)
        {
            lock (_lock)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("target");
                }
                target.Reset();
            }
        }
    }
}
