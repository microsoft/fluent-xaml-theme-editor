// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditorShared.ColorPalette
{
    // These classes are not intended to be viewmodels.
    // They deal with the data about an editable palette and are passed to special purpose controls for editing
    public class ColorPalette : IColorPalette
    {
        public static ColorPalette Parse(JsonObject data, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            IColorPaletteEntry baseColor = null;
            if (data.ContainsKey("EditableBaseColor"))
            {
                baseColor = EditableColorPaletteEntry.Parse(data["EditableBaseColor"].GetObject(), null, contrastColors);
            }
            else if (data.ContainsKey("BaseColor"))
            {
                baseColor = ColorPaletteEntry.Parse(data["BaseColor"].GetObject(), contrastColors);
            }

            int steps = data["Steps"].GetInt();

            return new ColorPalette(steps, baseColor, contrastColors);
        }

        public ColorPalette(int steps, Color baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : this(steps, new ColorPaletteEntry(baseColor, null, null, ColorStringFormat.PoundRGB, null), contrastColors)
        { }

        public ColorPalette(int steps, IColorPaletteEntry baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            if (baseColor == null)
            {
                throw new ArgumentNullException("baseColor");
            }
            if (_steps <= 0)
            {
                throw new ArgumentException("steps must > 0");
            }
            _contrastColors = contrastColors;
            _steps = steps;
            _baseColor = baseColor;
            _baseColor.ActiveColorChanged += BaseColor_ActiveColorChanged;

            _palette = new List<EditableColorPaletteEntry>(_steps);
            for (int i = 0; i < _steps; i++)
            {
                string title = baseColor.Title + " " + (i * 100).ToString("000");
                _palette.Add(new EditableColorPaletteEntry(null, default(Color), false, title, baseColor.Description, ColorStringFormat.PoundRGB, _contrastColors));
            }

            UpdatePaletteColors();
        }

        private void BaseColor_ActiveColorChanged(IColorPaletteEntry obj)
        {
            UpdatePaletteColors();
        }

        protected readonly IColorPaletteEntry _baseColor;
        public IColorPaletteEntry BaseColor
        {
            get { return _baseColor; }
        }

        protected readonly int _steps = 11;
        public int Steps
        {
            get { return _steps; }
        }

        protected readonly List<EditableColorPaletteEntry> _palette;
        public IReadOnlyList<EditableColorPaletteEntry> Palette
        {
            get { return _palette; }
        }

        protected ColorScaleInterpolationMode _interpolationMode = ColorScaleInterpolationMode.RGB;
        public ColorScaleInterpolationMode InterpolationMode
        {
            get { return _interpolationMode; }
            set
            {
                if (_interpolationMode != value)
                {
                    _interpolationMode = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected Color _scaleColorLight = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        public Color ScaleColorLight
        {
            get { return _scaleColorLight; }
            set
            {
                if (_scaleColorLight != value)
                {
                    _scaleColorLight = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected Color _scaleColorDark = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
        public Color ScaleColorDark
        {
            get { return _scaleColorDark; }
            set
            {
                if (_scaleColorDark != value)
                {
                    _scaleColorDark = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _clipLight = 0.185;
        public double ClipLight
        {
            get { return _clipLight; }
            set
            {
                if (_clipLight != value)
                {
                    _clipLight = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _clipDark = 0.160;
        public double ClipDark
        {
            get { return _clipDark; }
            set
            {
                if (_clipDark != value)
                {
                    _clipDark = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _saturationAdjustmentCutoff = 0.05;
        public double SaturationAdjustmentCutoff
        {
            get { return _saturationAdjustmentCutoff; }
            set
            {
                if (_saturationAdjustmentCutoff != value)
                {
                    _saturationAdjustmentCutoff = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _saturationLight = 0.35;
        public double SaturationLight
        {
            get { return _saturationLight; }
            set
            {
                if (_saturationLight != value)
                {
                    _saturationLight = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _saturationDark = 1.25;
        public double SaturationDark
        {
            get { return _saturationDark; }
            set
            {
                if (_saturationDark != value)
                {
                    _saturationDark = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _overlayLight = 0.0;
        public double OverlayLight
        {
            get { return _overlayLight; }
            set
            {
                if (_overlayLight != value)
                {
                    _overlayLight = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _overlayDark = 0.25;
        public double OverlayDark
        {
            get { return _overlayDark; }
            set
            {
                if (_overlayDark != value)
                {
                    _overlayDark = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _multiplyLight = 0.0;
        public double MultiplyLight
        {
            get { return _multiplyLight; }
            set
            {
                if (_multiplyLight != value)
                {
                    _multiplyLight = value;
                    UpdatePaletteColors();
                }
            }
        }

        protected double _multiplyDark = 0.0;
        public double MultiplyDark
        {
            get { return _multiplyDark; }
            set
            {
                if (_multiplyDark != value)
                {
                    _multiplyDark = value;
                    UpdatePaletteColors();
                }
            }
        }

        public void UpdatePaletteGenerationValues(ColorScaleInterpolationMode? interpolationMode, Color? scaleColorLight, Color? scaleColorDark, double? clipLight, double? clipDark, double? saturationAdjustmentCutoff, double? saturationLight, double? saturationDark, double? overlayLight, double? overlayDark, double? multiplyLight, double? multiplyDark)
        {
            if (interpolationMode.HasValue)
            {
                _interpolationMode = interpolationMode.Value;
            }
            if (scaleColorLight.HasValue)
            {
                _scaleColorLight = scaleColorLight.Value;
            }
            if (scaleColorDark.HasValue)
            {
                _scaleColorDark = scaleColorDark.Value;
            }
            if (clipLight.HasValue)
            {
                _clipLight = clipLight.Value;
            }
            if (clipDark.HasValue)
            {
                _clipDark = clipDark.Value;
            }
            if (saturationAdjustmentCutoff.HasValue)
            {
                _saturationAdjustmentCutoff = saturationAdjustmentCutoff.Value;
            }
            if (saturationLight.HasValue)
            {
                _saturationLight = saturationLight.Value;
            }
            if (saturationDark.HasValue)
            {
                _saturationDark = saturationDark.Value;
            }
            if (overlayLight.HasValue)
            {
                _overlayLight = overlayLight.Value;
            }
            if (overlayDark.HasValue)
            {
                _overlayDark = overlayDark.Value;
            }
            if (multiplyLight.HasValue)
            {
                _multiplyLight = multiplyLight.Value;
            }
            if (multiplyDark.HasValue)
            {
                _multiplyDark = multiplyDark.Value;
            }
            UpdatePaletteColors();
        }

        public ColorScale GetPaletteScale()
        {
            var baseColorRGB = _baseColor.ActiveColor;
            var baseColorHSL = ColorUtils.RGBToHSL(baseColorRGB);
            var baseColorNormalized = new NormalizedRGB(baseColorRGB);

            var baseScale = new ColorScale(new Color[] { _scaleColorLight, baseColorRGB, _scaleColorDark, });

            var trimmedScale = baseScale.Trim(_clipLight, 1.0 - _clipDark);
            var trimmedLight = new NormalizedRGB(trimmedScale.GetColor(0, _interpolationMode));
            var trimmedDark = new NormalizedRGB(trimmedScale.GetColor(1, _interpolationMode));

            var adjustedLight = trimmedLight;
            var adjustedDark = trimmedDark;

            if (baseColorHSL.S >= _saturationAdjustmentCutoff)
            {

                adjustedLight = ColorBlending.SaturateViaLCH(adjustedLight, _saturationLight);
                adjustedDark = ColorBlending.SaturateViaLCH(adjustedDark, _saturationDark);
            }

            if (_multiplyLight != 0)
            {
                var multiply = ColorBlending.Blend(baseColorNormalized, adjustedLight, ColorBlendMode.Multiply);
                adjustedLight = ColorUtils.InterpolateColor(adjustedLight, multiply, _multiplyLight, _interpolationMode);
            }

            if (_multiplyDark != 0)
            {
                var multiply = ColorBlending.Blend(baseColorNormalized, adjustedDark, ColorBlendMode.Multiply);
                adjustedDark = ColorUtils.InterpolateColor(adjustedDark, multiply, _multiplyDark, _interpolationMode);
            }

            if (_overlayLight != 0)
            {
                var overlay = ColorBlending.Blend(baseColorNormalized, adjustedLight, ColorBlendMode.Overlay);
                adjustedLight = ColorUtils.InterpolateColor(adjustedLight, overlay, _overlayLight, _interpolationMode);
            }

            if (_overlayDark != 0)
            {
                var overlay = ColorBlending.Blend(baseColorNormalized, adjustedDark, ColorBlendMode.Overlay);
                adjustedDark = ColorUtils.InterpolateColor(adjustedDark, overlay, _overlayDark, _interpolationMode);
            }

            var finalScale = new ColorScale(new Color[] { adjustedLight.Denormalize(), baseColorRGB, adjustedDark.Denormalize() });
            return finalScale;
        }

        protected virtual void UpdatePaletteColors()
        {
            var scale = GetPaletteScale();

            for (int i = 0; i < _steps; i++)
            {
                var c = scale.GetColor((double)i / (double)(_steps - 1), _interpolationMode);
                _palette[i].SourceColor = new ColorPaletteEntry(c, null, null, ColorStringFormat.PoundRGB, null);
            }
        }

        private IReadOnlyList<ContrastColorWrapper> _contrastColors;
        public IReadOnlyList<ContrastColorWrapper> ContrastColors
        {
            get { return _contrastColors; }
            set
            {
                if(_contrastColors != value)
                {
                    _contrastColors = value;
                    if(_baseColor != null)
                    {
                        _baseColor.ContrastColors = _contrastColors;
                    }
                    if(_palette != null && _palette.Count > 0)
                    {
                        foreach(var p in _palette)
                        {
                            p.ContrastColors = _contrastColors;
                        }
                    }
                }
            }
        }
    }
}
