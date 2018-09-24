// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace FluentEditorShared.DependencyContainer
{
    // It is common for a dependency container to use Type as its key. In this case we want to be able to have multiple dependencies of the same type so an explicit Id is needed.
    // This implementation also does not allow for removal of dependencies once added. That is not a needed feature at this time and would require that users of such a container subscribe to events and handle cases of their dependencies being removed at runtime.
    public interface IDependencyContainer
    {
        void AddDependency<T>(string id, T dependency) where T : class;
        T GetDependency<T>(string id) where T : class;
    }
}
