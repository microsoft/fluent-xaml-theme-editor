// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ColorPalette
{
    public class ColorPaletteEditor : Control
    {
        public ColorPaletteEditor()
        {
            this.DefaultStyleKey = typeof(ColorPaletteEditor);
        }

        #region ColorPaletteProperty

        public static readonly DependencyProperty ColorPaletteProperty = DependencyProperty.Register("ColorPalette", typeof(ColorPalette), typeof(ColorPaletteEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnColorPalettePropertyChanged)));

        private static void OnColorPalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteEditor target)
            {
                target.OnColorPaletteChanged(e.OldValue as ColorPalette, e.NewValue as ColorPalette);
            }
        }

        private void OnColorPaletteChanged(ColorPalette oldValue, ColorPalette newValue)
        {
            if(newValue == null)
            {
                SetValue(PaletteEntriesProperty, null);
            }
            else
            {
                SetValue(PaletteEntriesProperty, newValue.Palette);
            }
        }

        public ColorPalette ColorPalette
        {
            get { return GetValue(ColorPaletteProperty) as ColorPalette; }
            set { SetValue(ColorPaletteProperty, value); }
        }

        #endregion

        #region PaletteEntriesProperty

        public static readonly DependencyProperty PaletteEntriesProperty = DependencyProperty.Register("PaletteEntries", typeof(IReadOnlyList<IColorPaletteEntry>), typeof(ColorPaletteEditor), new PropertyMetadata(null));

        public IReadOnlyList<IColorPaletteEntry> PaletteEntries
        {
            get { return GetValue(PaletteEntriesProperty) as IReadOnlyList<IColorPaletteEntry>; }
        }

        #endregion

        #region IsExpandedProperty

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ColorPaletteEditor), new PropertyMetadata(true));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion
    }
}
