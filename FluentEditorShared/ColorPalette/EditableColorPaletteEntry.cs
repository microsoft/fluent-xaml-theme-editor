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
    public class EditableColorPaletteEntry : IColorPaletteEntry
    {
        public static EditableColorPaletteEntry Parse(JsonObject data, IColorPaletteEntry sourceColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            Color customColor;
            if (data.ContainsKey("CustomColor"))
            {
                customColor = data["CustomColor"].GetColor();
            }
            else
            {
                customColor = default(Color);
            }
            bool useCustomColor = false;
            if (data.ContainsKey("UseCustomColor"))
            {
                useCustomColor = data["UseCustomColor"].GetBoolean();
            }

            FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat = FluentEditorShared.Utils.ColorStringFormat.PoundRGB;
            if (data.ContainsKey("ActiveColorStringFormat"))
            {
                activeColorStringFormat = data.GetEnum<FluentEditorShared.Utils.ColorStringFormat>();
            }

            return new EditableColorPaletteEntry(sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
        }

        public EditableColorPaletteEntry(IColorPaletteEntry sourceColor, Color customColor, bool useCustomColor, string title, string description, FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            _sourceColor = sourceColor;
            _customColor = customColor;
            _useCustomColor = useCustomColor;
            _title = title;
            _description = description;
            _activeColorStringFormat = activeColorStringFormat;

            if (_useCustomColor || _sourceColor == null)
            {
                _activeColor = _customColor;
            }
            else
            {
                _activeColor = _sourceColor.ActiveColor;
            }

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

        private IColorPaletteEntry _sourceColor;
        public IColorPaletteEntry SourceColor
        {
            get { return _sourceColor; }
            set
            {
                if (_sourceColor != null)
                {
                    _sourceColor.ActiveColorChanged -= _sourceColor_ActiveColorChanged;
                }

                _sourceColor = value;

                if (_sourceColor != null)
                {
                    _sourceColor.ActiveColorChanged += _sourceColor_ActiveColorChanged;
                }

                CheckActiveColor();
            }
        }

        private void _sourceColor_ActiveColorChanged(IColorPaletteEntry obj)
        {
            CheckActiveColor();
        }

        private Color _customColor;
        public Color CustomColor
        {
            get { return _customColor; }
            set
            {
                if (_customColor != value)
                {
                    _customColor = value;
                    CustomColorChanged?.Invoke(this);

                    CheckActiveColor();
                }
            }
        }

        public event Action<IColorPaletteEntry> CustomColorChanged;

        private bool _useCustomColor;
        public bool UseCustomColor
        {
            get { return _useCustomColor; }
            set
            {
                if (_useCustomColor != value)
                {
                    _useCustomColor = value;

                    CheckActiveColor();
                }
            }
        }

        private void CheckActiveColor()
        {
            Color newVal;

            if (_useCustomColor || _sourceColor == null)
            {
                newVal = _customColor;
            }
            else
            {
                newVal = _sourceColor.ActiveColor;
            }

            if (newVal != _activeColor)
            {
                _activeColor = newVal;
                ActiveColorChanged?.Invoke(this);

                UpdateContrastColor();
            }
        }

        private Color _activeColor;
        public Color ActiveColor
        {
            get { return _activeColor; }
            set
            {
                CustomColor = value;
                if (_sourceColor != null)
                {
                    UseCustomColor = _customColor != _sourceColor.ActiveColor;
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
