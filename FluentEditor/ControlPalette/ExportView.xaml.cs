// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml.Controls;

namespace FluentEditor.ControlPalette.Export
{
    public sealed partial class ExportView : Page
    {
        public ExportView(ExportViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            _viewModel = viewModel;
            this.InitializeComponent();
        }

        private readonly ExportViewModel _viewModel;
        public ExportViewModel ViewModel { get { return _viewModel; } }
    }
}
