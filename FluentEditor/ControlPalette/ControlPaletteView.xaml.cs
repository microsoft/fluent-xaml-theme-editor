// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FluentEditor.ControlPalette
{
    public sealed partial class ControlPaletteView : Page
    {
        public ControlPaletteView()
        {
            this.InitializeComponent();

            //MainContentAreaShadow.Receivers.Add(ShadowCatcher);
            //MainContentArea.Translation = new System.Numerics.Vector3(0f, 0f, 8f);
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ControlPaletteViewModel), typeof(ControlPaletteView), new PropertyMetadata(null));

        public ControlPaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as ControlPaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel = e.Parameter as ControlPaletteViewModel;
        }
    }
}
