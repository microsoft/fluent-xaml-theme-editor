// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace FluentEditorShared.DependencyContainer
{
    // It is common for a dependency container to use Type as its key. In this case we want to be able to have multiple dependencies of the same type so an explicit Id is needed.
    // This implementation also does not allow for removal of dependencies once added. That is not a needed feature at this time and would require that users of such a container subscribe to events and handle cases of their dependencies being removed at runtime.
    public class DependencyContainer : IDependencyContainer
    {
        private object _lock = new object();
        private readonly Dictionary<string, object> _dependencies = new Dictionary<string, object>();

        public void AddDependency<T>(string id, T dependency) where T : class
        {
            lock (_lock)
            {
                if (_dependencies.ContainsKey(id))
                {
                    throw new Exception(string.Format("A dependency with id {0} already exists", id));
                }
                _dependencies.Add(id, dependency);
            }
        }

        public T GetDependency<T>(string id) where T : class
        {
            lock (_lock)
            {
                if (!_dependencies.ContainsKey(id))
                {
                    return null;
                }
                var d = _dependencies[id];
                if (d is T retVal)
                {
                    return retVal;
                }
                else
                {
                    throw new Exception(string.Format("A dependency with id {0} exists but is of type {1} not {2}", id, d.GetType().ToString(), typeof(T).ToString()));
                }
            }
        }
    }
}
