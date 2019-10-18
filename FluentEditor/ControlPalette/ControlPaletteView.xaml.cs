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

        private void UpdateBorderThickness(Thickness value)
        {
            App.Current.Resources["ComboBoxBorderThemeThickness"] = value;
            App.Current.Resources["DatePickerBorderThemeThickness"] = value;
            App.Current.Resources["TimePickerBorderThemeThickness"] = value;
            App.Current.Resources["TextControlBorderThemeThickness"] = value;

            App.Current.Resources["ToggleSwitchOuterBorderStrokeThickness"] = EqualizeThicknessValue(value);
            App.Current.Resources["CheckBoxBorderThemeThickness"] = EqualizeThicknessValue(value);
            App.Current.Resources["RadioButtonBorderThemeThickness"] = EqualizeThicknessValue(value);
        }

        private Thickness EqualizeThicknessValue(Thickness before)
        {
            Thickness after = new Thickness(1);
            double finalEqualValue = Math.Max(Math.Max(Math.Max(before.Left, before.Right), before.Top), before.Bottom);

            if (finalEqualValue > 0)
                after = new Thickness(finalEqualValue);

            return after;
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

        private void TextBoxBorder_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                UpdateBorderThickness(new Thickness(Double.Parse(LeftBorderTB.Text), Double.Parse(TopBorderTB.Text),
                Double.Parse(RightBorderTB.Text), Double.Parse(BottomBorderTB.Text)));

                RefreshControls();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateBorderThickness(new Thickness(Double.Parse(LeftBorderTB.Text), Double.Parse(TopBorderTB.Text),
                Double.Parse(RightBorderTB.Text), Double.Parse(BottomBorderTB.Text)));
            UpdateCorners();
            UpdateOverlayCorners();

            RefreshControls();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (ControlBorderThicknessSlider == null)
                return;

            if (comboBox.SelectedIndex == 1)
            {
                ControlBorderThicknessSlider.Value = 2;
                ControlRoundSlider.Value = 0;
                ControlRoundSliderOverlay.Value = 0;
            }
            else
            {
                ControlBorderThicknessSlider.Value = 1;
                ControlRoundSlider.Value = 2;
                ControlRoundSliderOverlay.Value = 4;
                
            }

            UpdateBorderThickness(new Thickness(Double.Parse(LeftBorderTB.Text), Double.Parse(TopBorderTB.Text),
                Double.Parse(RightBorderTB.Text), Double.Parse(BottomBorderTB.Text)));
            UpdateCorners();
            UpdateOverlayCorners();
            RefreshControls();
        }
    }
}
