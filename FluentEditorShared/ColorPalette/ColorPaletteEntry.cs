// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.Data.Json;
using Windows.UI;
using FluentEditorShared.Utils;
using System.Collections.Generic;

namespace FluentEditorShared.ColorPalette
{
    // These classes are not intended to be viewmodels.
    // They deal with the data about an editable palette and are passed to special purpose controls for editing
    public class ColorPaletteEntry : IColorPaletteEntry
    {
        public static ColorPaletteEntry Parse(JsonObject data, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            Color color;
            if (data.ContainsKey("Color"))
            {
                color = data["Color"].GetColor();
            }
            else
            {
                color = default(Color);
            }

            FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat = FluentEditorShared.Utils.ColorStringFormat.PoundRGB;
            if (data.ContainsKey("ActiveColorStringFormat"))
            {
                activeColorStringFormat = data.GetEnum<FluentEditorShared.Utils.ColorStringFormat>();
            }

            return new ColorPaletteEntry(color, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
        }

        public ColorPaletteEntry(Color color, string title, string description, FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            _activeColor = color;
            _title = title;
            _description = description;
            _activeColorStringFormat = activeColorStringFormat;

            ContrastColors = contrastColors;
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private Color _activeColor;
        public Color ActiveColor
        {
            get { return _activeColor; }
            set
            {
                if (_activeColor != value)
                {
                    _activeColor = value;
                    ActiveColorChanged?.Invoke(this);

                    UpdateContrastColor();
                }
            }
        }

        public string ActiveColorString
        {
            get
            {
                return FluentEditorShared.Utils.ColorUtils.FormatColorString(_activeColor, _activeColorStringFormat);
            }
        }

        private FluentEditorShared.Utils.ColorStringFormat _activeColorStringFormat = FluentEditorShared.Utils.ColorStringFormat.PoundRGB;
        public FluentEditorShared.Utils.ColorStringFormat ActiveColorStringFormat
        {
            get { return _activeColorStringFormat; }
        }

        public event Action<IColorPaletteEntry> ActiveColorChanged;

        private IReadOnlyList<ContrastColorWrapper> _contrastColors;
        public IReadOnlyList<ContrastColorWrapper> ContrastColors
        {
            get { return _contrastColors; }
            set
            {
                if (_contrastColors != value)
                {
                    if (_contrastColors != null)
                    {
                        foreach (var c in _contrastColors)
                        {
                            c.Color.ActiveColorChanged -= ContrastColor_ActiveColorChanged;
                        }
                    }

                    _contrastColors = value;

                    if (_contrastColors != null)
                    {
                        foreach (var c in _contrastColors)
                        {
                            c.Color.ActiveColorChanged += ContrastColor_ActiveColorChanged;
                        }
                    }

                    UpdateContrastColor();
                }
            }
        }

        private void ContrastColor_ActiveColorChanged(IColorPaletteEntry obj)
        {
            UpdateContrastColor();
        }

        private ContrastColorWrapper _bestContrastColor;
        public ContrastColorWrapper BestContrastColor
        {
            get { return _bestContrastColor; }
        }

        public double BestContrastValue
        {
            get
            {
                if (_bestContrastColor == null)
                {
                    return 0;
                }
                return ColorUtils.ContrastRatio(ActiveColor, _bestContrastColor.Color.ActiveColor);
            }
        }

        private void UpdateContrastColor()
        {
            ContrastColorWrapper newContrastColor = null;

            if (_contrastColors != null && _contrastColors.Count > 0)
            {
                double maxContrast = -1;
                foreach (var c in _contrastColors)
                {
                    double contrast = ColorUtils.ContrastRatio(ActiveColor, c.Color.ActiveColor);
                    if (contrast > maxContrast)
                    {
                        maxContrast = contrast;
                        newContrastColor = c;
                    }
                }
            }

            if (_bestContrastColor != newContrastColor)
            {
                _bestContrastColor = newContrastColor;
                ContrastColorChanged?.Invoke(this);
            }
        }

        public event Action<IColorPaletteEntry> ContrastColorChanged;
    }
}

