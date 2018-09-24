// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ColorPalette
{
    public sealed partial class ExpandedColorPaletteEntryEditor : UserControl
    {
        public ExpandedColorPaletteEntryEditor()
        {
            this.InitializeComponent();
        }

        #region ColorPaletteEntryProperty

        public static readonly DependencyProperty ColorPaletteEntryProperty = DependencyProperty.Register("ColorPaletteEntry", typeof(IColorPaletteEntry), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnColorPaletteEntryPropertyChanged)));

        private static void OnColorPaletteEntryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExpandedColorPaletteEntryEditor target)
            {
                target.OnColorPaletteEntryChanged(e.OldValue as IColorPaletteEntry, e.NewValue as IColorPaletteEntry);
            }
        }

        private void OnColorPaletteEntryChanged(IColorPaletteEntry oldValue, IColorPaletteEntry newValue)
        {
            if (oldValue != null)
            {
                oldValue.ActiveColorChanged -= ColorPaletteEntry_ActiveColorChanged;
            }

            if (newValue != null)
            {
                ColorPicker.Color = newValue.ActiveColor;
                newValue.ActiveColorChanged += ColorPaletteEntry_ActiveColorChanged;

                if (string.IsNullOrEmpty(newValue.Title))
                {
                    TitleField.Visibility = Visibility.Collapsed;
                    TitleField.Text = string.Empty;
                }
                else
                {
                    TitleField.Visibility = Visibility.Visible;
                    TitleField.Text = newValue.Title;
                }
                if (string.IsNullOrEmpty(newValue.Description))
                {
                    DescriptionField.Visibility = Visibility.Collapsed;
                    DescriptionField.Text = string.Empty;
                }
                else
                {
                    DescriptionField.Visibility = Visibility.Visible;
                    DescriptionField.Text = newValue.Description;
                }
                if (newValue is EditableColorPaletteEntry editableNewValue)
                {
                    RevertButton.Visibility = Visibility.Visible;
                    RevertButton.IsEnabled = editableNewValue.UseCustomColor;
                }
                else
                {
                    RevertButton.Visibility = Visibility.Collapsed;
                }

                if (newValue.ContrastColors != null)
                {
                    List<ContrastListItem> contrastList = new List<ContrastListItem>();

                    foreach (var c in newValue.ContrastColors)
                    {
                        if (c.ShowInContrastList)
                        {
                            if (c.ShowContrastErrors)
                            {
                                contrastList.Add(new ContrastListItem(newValue, c.Color));
                            }
                            else
                            {
                                contrastList.Add(new ContrastListItem(newValue, c.Color, 0));
                            }

                        }
                    }

                    SetValue(ContrastListProperty, contrastList);
                }
                else
                {
                    SetValue(ContrastListProperty, null);
                }
            }
            else
            {
                ColorPicker.Color = default(Color);
                SetValue(ContrastListProperty, null);
            }
        }

        public IColorPaletteEntry ColorPaletteEntry
        {
            get { return GetValue(ColorPaletteEntryProperty) as IColorPaletteEntry; }
            set { SetValue(ColorPaletteEntryProperty, value); }
        }

        #endregion

        #region ContrastListProperty

        public static readonly DependencyProperty ContrastListProperty = DependencyProperty.Register("ContrastList", typeof(List<ContrastListItem>), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(null));

        public List<ContrastListItem> ContrastList
        {
            get { return GetValue(ContrastListProperty) as List<ContrastListItem>; }
        }

        #endregion

        private void ColorPaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            if (obj is EditableColorPaletteEntry editablePaletteEntry)
            {
                RevertButton.IsEnabled = editablePaletteEntry.UseCustomColor;
            }
            if (obj != null)
            {
                ColorPicker.Color = obj.ActiveColor;
            }
        }

        private void ColorPicker_ColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
        {
            // In this case it is easier to deal with an event than the loopbacks that the ColorPicker creates with a two way binding
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry != null)
            {
                paletteEntry.ActiveColor = args.NewColor;
            }
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry is EditableColorPaletteEntry editablePaletteEntry)
            {
                editablePaletteEntry.UseCustomColor = false;
            }
        }
    }
}
