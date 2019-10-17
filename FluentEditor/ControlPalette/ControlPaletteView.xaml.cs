// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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

        private void ShapeHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab(ShapeTabGrid, ColorHyperlinkButton, ShapePanel);

            HideTab(ColorTabGrid, ColorHyperlinkButton, ColorPanel);
            HideTab(FontTabGrid, FontHyperlinkButton, FontPanel);
        }

        private void ColorHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab(ColorTabGrid, ColorHyperlinkButton, ColorPanel);

            HideTab(ShapeTabGrid, ShapeHyperlinkButton, ShapePanel);
            HideTab(FontTabGrid, FontHyperlinkButton, FontPanel);
        }

        private void FontHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab(FontTabGrid, FontHyperlinkButton, FontPanel);

            HideTab(ShapeTabGrid, ShapeHyperlinkButton, ShapePanel);
            HideTab(ColorTabGrid, ColorHyperlinkButton, ColorPanel);
        }

        private void ShowTab(Grid tabGrid, Control tabButton, StackPanel tabPanel)
        {
            tabGrid.Background = (SolidColorBrush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            tabButton.Foreground = (SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"];
            tabButton.Opacity = 1.0;
            tabPanel.Visibility = Visibility.Visible;
        }

        private void HideTab(Grid tabGrid, Control tabButton, StackPanel tabPanel)
        {
            tabGrid.Background = (SolidColorBrush)App.Current.Resources["AppBackgroundBrush"];
            tabButton.Foreground = (SolidColorBrush)App.Current.Resources["ButtonForegroundThemeBrush"];
            tabButton.Opacity = 0.6;
            tabPanel.Visibility = Visibility.Collapsed;
        }
    }
}
