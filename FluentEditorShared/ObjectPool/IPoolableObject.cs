// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace FluentEditorShared.ObjectPool
{
    // Implementations of IPoolableObject should not handle their own thread syncronization for IsLive / Reset / Activate state. The owning pool will handle that if any is needed.
    public interface IPoolableObject
    {
        bool IsLive { get; }
        void Reset();
        void Activate();
    }
}
