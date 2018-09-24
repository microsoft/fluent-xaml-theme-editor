// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditorShared.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FluentEditorShared.ColorPalette
{
    public class ContrastListItem : INotifyPropertyChanged
    {
        public ContrastListItem() { }
        public ContrastListItem(IColorPaletteEntry baseColor, IColorPaletteEntry contrastColor, double minContrast = 4.5)
        {
            _minContrast = minContrast;
            _baseColorBrush = new SolidColorBrush();
            _contrastColorBrush = new SolidColorBrush();

            BaseColor = baseColor;
            ContrastColor = contrastColor;
        }

        private void RecalcContrast()
        {
            if (_baseColor == null || _contrastColor == null)
            {
                Contrast = 0.0;
                return;
            }

            Contrast = ColorUtils.ContrastRatio(_baseColor.ActiveColor, _contrastColor.ActiveColor);
        }

        private double _minContrast;
        private double _contrast;
        public double Contrast
        {
            get { return _contrast; }
            private set
            {
                if (_contrast != value)
                {
                    _contrast = value;
                    RaisePropertyChangedFromSource();

                    RaisePropertyChanged("ContrastError");
                }
            }
        }

        public bool ContrastError
        {
            get
            {
                if(_minContrast <= 0)
                {
                    return false;
                }
                return _contrast < _minContrast;
            }
        }

        private IColorPaletteEntry _baseColor;
        public IColorPaletteEntry BaseColor
        {
            get { return _baseColor; }
            set
            {
                if (_baseColor != value)
                {
                    if (_baseColor != null)
                    {
                        _baseColor.ActiveColorChanged -= _baseColor_ActiveColorChanged;
                    }

                    _baseColor = value;
                    RaisePropertyChangedFromSource();

                    if (_baseColor != null)
                    {
                        _baseColorBrush.Color = _baseColor.ActiveColor;
                        _baseColor.ActiveColorChanged += _baseColor_ActiveColorChanged;
                    }

                    RecalcContrast();
                }
            }
        }

        private void _baseColor_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _baseColorBrush.Color = obj.ActiveColor;
            RecalcContrast();
        }

        private SolidColorBrush _baseColorBrush;
        public SolidColorBrush BaseColorBrush
        {
            get { return _baseColorBrush; }
        }

        private IColorPaletteEntry _contrastColor;
        public IColorPaletteEntry ContrastColor
        {
            get { return _contrastColor; }
            set
            {
                if (_contrastColor != value)
                {
                    if (_contrastColor != null)
                    {
                        _contrastColor.ActiveColorChanged -= _contrastColor_ActiveColorChanged;
                    }

                    _contrastColor = value;
                    RaisePropertyChangedFromSource();

                    if (_contrastColor != null)
                    {
                        _contrastColorBrush.Color = _contrastColor.ActiveColor;
                        _contrastColor.ActiveColorChanged += _contrastColor_ActiveColorChanged;
                    }

                    RecalcContrast();
                }
            }
        }

        private void _contrastColor_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _contrastColorBrush.Color = obj.ActiveColor;
            RecalcContrast();
        }

        private SolidColorBrush _contrastColorBrush;
        public SolidColorBrush ContrastColorBrush
        {
            get { return _contrastColorBrush; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
