// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace FluentEditorShared.Editors
{
    public class ColorEditor : Control
    {
        public ColorEditor()
        {
            this.DefaultStyleKey = typeof(ColorEditor);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var h = Header;
            if (h != null)
            {
                var headerContentPresenter = GetTemplateChild("HeaderContentPresenter") as UIElement;
                if (headerContentPresenter != null)
                {
                    headerContentPresenter.Visibility = h == null ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        // Dependency properties can safely be assumed to be single threaded so this is sufficient for dealing with loopbacks on property changed
        private bool _internalValueChanging = false;

        private void UpdateProperties(Color color)
        {
            ValueString = Utils.ColorUtils.FormatColorString(color, ColorStringFormat, Precision);
            var b = ValueBrush;
            if (b == null)
            {
                ValueBrush = new SolidColorBrush(color);
            }
            else
            {
                b.Color = color;
            }
            ValueRed = color.R;
            ValueRedString = color.R.ToString();
            ValueGreen = color.G;
            ValueGreenString = color.G.ToString();
            ValueBlue = color.B;
            ValueBlueString = color.B.ToString();
            if (UseAlpha)
            {
                ValueAlpha = color.A;
                ValueAlphaString = color.A.ToString();
            }
            else
            {
                ValueAlpha = 255;
                ValueAlphaString = "255";
            }
        }

        #region ValueColor

        public static readonly DependencyProperty ValueColorProperty = DependencyProperty.Register("ValueColor", typeof(Color), typeof(ColorEditor), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnValueColorPropertyChanged)));

        private static void OnValueColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnValueColorChanged(Color oldValue, Color newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                UpdateProperties(newValue);

                _internalValueChanging = false;
            }
        }

        public Color ValueColor
        {
            get { return (Color)GetValue(ValueColorProperty); }
            set { SetValue(ValueColorProperty, value); }
        }

        #endregion

        #region ValueStringProperty

        public static readonly DependencyProperty ValueStringProperty = DependencyProperty.Register("ValueString", typeof(string), typeof(ColorEditor), new PropertyMetadata("000000", new PropertyChangedCallback(OnValueStringPropertyChanged)));

        private static void OnValueStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                Color c;
                try
                {
                    var boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), newValue);
                    if (boxedColor == null)
                    {
                        return;
                    }
                    c = (Color)boxedColor;
                }
                catch
                {
                    return;
                }

                _internalValueChanging = true;

                UpdateProperties(c);

                _internalValueChanging = false;
            }
        }

        public string ValueString
        {
            get { return GetValue(ValueStringProperty) as string; }
            set { SetValue(ValueStringProperty, value); }
        }

        #endregion

        #region ValueBrushProperty

        public static DependencyProperty ValueBrushProperty = DependencyProperty.Register("ValueBrush", typeof(SolidColorBrush), typeof(ColorEditor), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnValueBrushPropertyChanged)));

        private static void OnValueBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueBrushChanged(e.OldValue as SolidColorBrush, e.NewValue as SolidColorBrush);
            }
        }

        private void OnValueBrushChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;
                if (newValue == null)
                {
                    ValueBrush = new SolidColorBrush(ValueColor);
                }
                else
                {
                    var b = ValueBrush;
                    if (b != null)
                    {
                        UpdateProperties(b.Color);
                    }
                }

                _internalValueChanging = false;
            }
        }

        public SolidColorBrush ValueBrush
        {
            get { return GetValue(ValueBrushProperty) as SolidColorBrush; }
            set { SetValue(ValueBrushProperty, value); }
        }

        #endregion

        #region ValueRed

        public static DependencyProperty ValueRedProperty = DependencyProperty.Register("ValueRed", typeof(byte), typeof(ColorEditor), new PropertyMetadata((byte)0, new PropertyChangedCallback(OnValueRedPropertyChanged)));

        private static void OnValueRedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueRedChanged((byte)e.OldValue, (byte)e.NewValue);
            }
        }

        private void OnValueRedChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, newValue, oldColor.G, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueRed
        {
            get { return (byte)GetValue(ValueRedProperty); }
            set { SetValue(ValueRedProperty, value); }
        }

        #endregion

        #region ValueRedString

        public static DependencyProperty ValueRedStringProperty = DependencyProperty.Register("ValueRedString", typeof(string), typeof(ColorEditor), new PropertyMetadata("0", new PropertyChangedCallback(OnValueRedStringPropertyChanged)));

        private static void OnValueRedStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueRedStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueRedStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, newByte, oldColor.G, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueRedString
        {
            get { return GetValue(ValueRedStringProperty) as string; }
            set { SetValue(ValueRedStringProperty, value); }
        }

        #endregion

        #region ValueGreen

        public static DependencyProperty ValueGreenProperty = DependencyProperty.Register("ValueGreen", typeof(byte), typeof(ColorEditor), new PropertyMetadata((byte)0, new PropertyChangedCallback(OnValueGreenPropertyChanged)));

        private static void OnValueGreenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueGreenChanged((byte)e.OldValue, (byte)e.NewValue);
            }
        }

        private void OnValueGreenChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, newValue, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueGreen
        {
            get { return (byte)GetValue(ValueGreenProperty); }
            set { SetValue(ValueGreenProperty, value); }
        }

        #endregion

        #region ValueGreenString

        public static DependencyProperty ValueGreenStringProperty = DependencyProperty.Register("ValueGreenString", typeof(string), typeof(ColorEditor), new PropertyMetadata("0", new PropertyChangedCallback(OnValueGreenStringPropertyChanged)));

        private static void OnValueGreenStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueGreenStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueGreenStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, newByte, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueGreenString
        {
            get { return GetValue(ValueGreenStringProperty) as string; }
            set { SetValue(ValueGreenStringProperty, value); }
        }

        #endregion

        #region ValueBlue

        public static DependencyProperty ValueBlueProperty = DependencyProperty.Register("ValueBlue", typeof(byte), typeof(ColorEditor), new PropertyMetadata((byte)0, new PropertyChangedCallback(OnValueBluePropertyChanged)));

        private static void OnValueBluePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueBlueChanged((byte)e.OldValue, (byte)e.NewValue);
            }
        }

        private void OnValueBlueChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, newValue));

                _internalValueChanging = false;
            }
        }

        public byte ValueBlue
        {
            get { return (byte)GetValue(ValueBlueProperty); }
            set { SetValue(ValueBlueProperty, value); }
        }

        #endregion

        #region ValueBlueString

        public static DependencyProperty ValueBlueStringProperty = DependencyProperty.Register("ValueBlueString", typeof(string), typeof(ColorEditor), new PropertyMetadata("0", new PropertyChangedCallback(OnValueBlueStringPropertyChanged)));

        private static void OnValueBlueStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueBlueStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueBlueStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, newByte));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueBlueString
        {
            get { return GetValue(ValueBlueStringProperty) as string; }
            set { SetValue(ValueBlueStringProperty, value); }
        }

        #endregion

        #region ValueAlpha

        public static DependencyProperty ValueAlphaProperty = DependencyProperty.Register("ValueAlpha", typeof(byte), typeof(ColorEditor), new PropertyMetadata((byte)255, new PropertyChangedCallback(OnValueAlphaPropertyChanged)));

        private static void OnValueAlphaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueAlphaChanged((byte)e.OldValue, (byte)e.NewValue);
            }
        }

        private void OnValueAlphaChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(newValue, oldColor.R, oldColor.G, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueAlpha
        {
            get { return (byte)GetValue(ValueAlphaProperty); }
            set { SetValue(ValueAlphaProperty, value); }
        }

        #endregion

        #region ValueAlphaString

        public static DependencyProperty ValueAlphaStringProperty = DependencyProperty.Register("ValueAlphaString", typeof(string), typeof(ColorEditor), new PropertyMetadata("255", new PropertyChangedCallback(OnValueAlphaStringPropertyChanged)));

        private static void OnValueAlphaStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnValueAlphaStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueAlphaStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(newByte, oldColor.R, oldColor.G, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueAlphaString
        {
            get { return GetValue(ValueAlphaStringProperty) as string; }
            set { SetValue(ValueAlphaStringProperty, value); }
        }

        #endregion

        // could easily add more properties like this for HSL or whatever color space is needed

        #region UseAlphaProperty

        public static readonly DependencyProperty UseAlphaProperty = DependencyProperty.Register("UseAlpha", typeof(bool), typeof(ColorEditor), new PropertyMetadata(true, new PropertyChangedCallback(OnUseAlphaPropertyChanged)));

        private static void OnUseAlphaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnUseAlphaChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private void OnUseAlphaChanged(bool oldValue, bool newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if(!newValue)
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(255, oldColor.R, oldColor.G, oldColor.B));
                }

                var alphaInputBox = GetTemplateChild("AlphaInputBox") as UIElement;
                if (alphaInputBox != null)
                {
                    alphaInputBox.Visibility = newValue == false ? Visibility.Collapsed : Visibility.Visible;
                }

                _internalValueChanging = false;
            }
        }

        public bool UseAlpha
        {
            get { return (bool)GetValue(UseAlphaProperty); }
            set { SetValue(UseAlphaProperty, value); }
        }

        #endregion

        #region ColorStringFormatProperty

        public static readonly DependencyProperty ColorStringFormatProperty = DependencyProperty.Register("ColorStringFormat", typeof(FluentEditorShared.Utils.ColorStringFormat), typeof(ColorEditor), new PropertyMetadata(FluentEditorShared.Utils.ColorStringFormat.PoundRGB, new PropertyChangedCallback(OnColorStringFormatPropertyChanged)));

        private static void OnColorStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnColorStringFormatChanged((FluentEditorShared.Utils.ColorStringFormat)e.OldValue, (FluentEditorShared.Utils.ColorStringFormat)e.NewValue);
            }
        }

        private void OnColorStringFormatChanged(FluentEditorShared.Utils.ColorStringFormat oldValue, FluentEditorShared.Utils.ColorStringFormat newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                ValueString = Utils.ColorUtils.FormatColorString(ValueColor, newValue, Precision);

                _internalValueChanging = false;
            }
        }

        public FluentEditorShared.Utils.ColorStringFormat ColorStringFormat
        {
            get { return (FluentEditorShared.Utils.ColorStringFormat)GetValue(ColorStringFormatProperty); }
            set { SetValue(ColorStringFormatProperty, value); }
        }

        #endregion

        #region PrecisionProperty

        public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register("Precision", typeof(int), typeof(ColorEditor), new PropertyMetadata((int)4, new PropertyChangedCallback(OnPrecisionPropertyChanged)));

        private static void OnPrecisionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnPrecisionChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        private void OnPrecisionChanged(int oldValue, int newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                ValueString = Utils.ColorUtils.FormatColorString(ValueColor, ColorStringFormat, newValue);

                _internalValueChanging = false;
            }
        }

        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        #endregion

        #region HeaderProperty

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ColorEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderPropertyChanged)));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorEditor target)
            {
                target.OnHeaderChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnHeaderChanged(object oldVal, object newVal)
        {
            var headerContentPresenter = GetTemplateChild("HeaderContentPresenter") as UIElement;
            if (headerContentPresenter != null)
            {
                headerContentPresenter.Visibility = newVal == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        #endregion

        #region HeaderTemplateProperty

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ColorEditor), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        #endregion
    }
}
