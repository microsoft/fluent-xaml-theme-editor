// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace FluentEditorShared.ViewCommon
{
    public delegate IView ViewFactoryDelegate(object viewModel);

    public interface IView
    {
        // It is desirable to have strong typing from View to ViewModel to allow for x:Bind
        // but type erasure can also be useful here. The usual implementation of AttachViewModel is just this.ViewModel = viewModel as MyVMType.
        void AttachViewModel(object viewModel);
        void DetachViewModel();

        ViewFactoryDelegate ViewFactory { get; set; }
    }
}
