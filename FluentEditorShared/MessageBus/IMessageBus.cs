// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace FluentEditorShared.MessageBus
{
    public interface IMessageBusListener
    {
        void OnMessage<T>(in T message) where T : struct;
    }

    public interface IReadOnlyMessageBus
    {
        // The listener is stored as a WeakReference
        void AddListener<T>(IMessageBusListener listener) where T : struct;
        // The delegate d is stored as a strong reference. It is up to the caller to make sure that the delegate isn't strongly capturing anything it shouldn't.
        void AddListener<T>(Action<T> d) where T : struct;
        void RemoveListener<T>(IMessageBusListener listener) where T : struct;
        void RemoveListener<T>(Action<T> d) where T : struct;
        void PruneListeners();
    }

    public interface IMessageBus
    {
        // The listener is stored as a WeakReference
        void AddListener<T>(IMessageBusListener listener) where T : struct;
        // The delegate d is stored as a strong reference. It is up to the caller to make sure that the delegate isn't strongly capturing anything it shouldn't.
        void AddListener<T>(Action<T> d) where T : struct;
        void RemoveListener<T>(IMessageBusListener listener) where T : struct;
        void RemoveListener<T>(Action<T> d) where T : struct;
        void PruneListeners();

        void BroadcastMessage<T>(in T message) where T : struct;
    }
}
