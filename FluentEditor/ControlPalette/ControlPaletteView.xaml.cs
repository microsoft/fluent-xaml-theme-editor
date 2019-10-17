// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using System;
using System.Threading;

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

        private void ControlRoundSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            CornerRadius cornerRadius = (CornerRadius)App.Current.Resources["ControlCornerRadius"];
            App.Current.Resources["ControlCornerRadius"] = new CornerRadius(ControlRoundSlider.Value);

            RefreshControls();
        }

        private void ControlRoundSliderOverlay_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            CornerRadius cornerRadius = (CornerRadius)App.Current.Resources["OverlayCornerRadius"];
            App.Current.Resources["OverlayCornerRadius"] = new CornerRadius(ControlRoundSliderOverlay.Value);
        }

        private void RefreshControls()
        {
            LightTestContent.RequestedTheme = ElementTheme.Dark;
            LightTestContent.RequestedTheme = ElementTheme.Light;

            DarkTestContent.RequestedTheme = ElementTheme.Light;
            DarkTestContent.RequestedTheme = ElementTheme.Dark;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateCorners();
        }

        private void UpdateCorners()
        {
            App.Current.Resources["ControlCornerRadius"] =
                new CornerRadius(Double.Parse(TopLeftTB.Text), Double.Parse(TopRightTB.Text), 
                Double.Parse(BottomRightTB.Text), Double.Parse(BottomLeftTB.Text));
            RefreshControls();
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                UpdateCorners();
            }
        }

        private void ControlBorderThicknessSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            UpdateBorderThickness(new Thickness(ControlBorderThicknessSlider.Value));
            RefreshControls();
        }

        private void UpdateBorderThickness(Thickness value)
        {
            App.Current.Resources["ComboBoxBorderThemeThickness"] = value;
            App.Current.Resources["DatePickerBorderThemeThickness"] = value;
            App.Current.Resources["TimePickerBorderThemeThickness"] = value;
            App.Current.Resources["TextControlBorderThemeThickness"] = value;
            App.Current.Resources["CheckBoxBorderThemeThickness"] = value;
            App.Current.Resources["ToggleSwitchOuterBorderStrokeThickness"] = value;
            App.Current.Resources["RadioButtonBorderThemeThickness"] = value;
        }

        private void TextBoxOverlay_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateOverlayCorners();
        }

        private void UpdateOverlayCorners()
        {
            App.Current.Resources["OverlayCornerRadius"] =
                new CornerRadius(Double.Parse(TopLeftTBOverlay.Text), Double.Parse(TopRightTBOverlay.Text),
                Double.Parse(BottomRightTBOverlay.Text), Double.Parse(BottomLeftTBOverlay.Text));
        }

        private void TextBoxOverlay_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                UpdateOverlayCorners();
            }
        }

        private void TextBoxBorder_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateBorderThickness(new Thickness(Double.Parse(LeftBorderTB.Text), Double.Parse(TopBorderTB.Text),
                Double.Parse(RightBorderTB.Text), Double.Parse(BottomBorderTB.Text)));

            RefreshControls();
        }
    }
}
