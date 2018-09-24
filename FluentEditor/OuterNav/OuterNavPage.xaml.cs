// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml.Controls;

namespace FluentEditor.OuterNav
{
    public sealed partial class OuterNavPage : Page
    {
        public OuterNavPage(OuterNavViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            _viewModel = viewModel;
            _viewModel.NavigateToItem += _viewModel_NavigateToItem;

            this.InitializeComponent();

            NavigateToViewModel(_viewModel.SelectedNavItem);
        }

        private readonly OuterNavViewModel _viewModel;
        public OuterNavViewModel ViewModel { get { return _viewModel; } }

        private void _viewModel_NavigateToItem(OuterNavViewModel source, INavItem navItem)
        {
            NavigateToViewModel(navItem);
        }

        private void NavigateToViewModel(object viewModel)
        {
            Type pageType = null;

            switch (viewModel)
            {
                case FluentEditor.ControlPalette.ControlPaletteViewModel controlPalette:
                    pageType = typeof(FluentEditor.ControlPalette.ControlPaletteView);
                    break;
            }

            if(pageType == null)
            {
                return;
            }

            NavigationFrame.Navigate(pageType, viewModel);
        }
    }
}
