// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.Editors
{
    public class DoubleEditor : Control
    {
        public DoubleEditor()
        {
            this.DefaultStyleKey = typeof(DoubleEditor);
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

        private double AdjustValue(double i)
        {
            int p = Precision;
            if (p > 0)
            {
                i = Math.Round(i, p);
            }
            double min = MinValue;
            double max = MaxValue;
            if (i < min)
            {
                i = min;
            }
            if (i > max)
            {
                i = max;
            }

            return i;
        }

        #region ValueProperty

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleEditor), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValuePropertyChanged)));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleEditor target)
            {
                target.OnValueChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnValueChanged(double oldValue, double newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                newValue = AdjustValue(newValue);
                Value = newValue;
                ValueString = newValue.ToString();
                ValueInt = (int)Math.Round(newValue);

                _internalValueChanging = false;
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region ValueStringProperty

        public static readonly DependencyProperty ValueStringProperty = DependencyProperty.Register("ValueString", typeof(string), typeof(DoubleEditor), new PropertyMetadata("0", new PropertyChangedCallback(OnValueStringPropertyChanged)));

        private static void OnValueStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleEditor target)
            {
                target.OnValueStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnValueStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }
                double newVal;
                if (!double.TryParse(newValue, out newVal))
                {
                    return;
                }
                newVal = AdjustValue(newVal);

                Value = newVal;
                ValueString = newVal.ToString();
                ValueInt = (int)Math.Round(newVal);

                _internalValueChanging = false;
            }
        }

        public string ValueString
        {
            get { return GetValue(ValueStringProperty) as string; }
            set { SetValue(ValueStringProperty, value); }
        }


        #endregion

        #region ValueIntProperty

        public static readonly DependencyProperty ValueIntProperty = DependencyProperty.Register("ValueInt", typeof(int), typeof(DoubleEditor), new PropertyMetadata((int)0, new PropertyChangedCallback(OnValueIntPropertyChanged)));

        private static void OnValueIntPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleEditor target)
            {
                target.OnValueIntChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        private void OnValueIntChanged(int oldValue, int newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                double newVal = AdjustValue(newValue);
                Value = newVal;
                ValueString = newVal.ToString();
                ValueInt = (int)Math.Round(newVal);

                _internalValueChanging = false;
            }
        }

        public int ValueInt
        {
            get { return (int)GetValue(ValueIntProperty); }
            set { SetValue(ValueIntProperty, value); }
        }

        #endregion

        #region PrecisionProperty

        public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register("Precision", typeof(int), typeof(DoubleEditor), new PropertyMetadata((int)4, new PropertyChangedCallback(OnPrecisionPropertyChanged)));

        private static void OnPrecisionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleEditor target)
            {
                target.OnPrecisionChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        private void OnPrecisionChanged(int oldValue, int newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                double newVal = AdjustValue(Value);

                Value = newVal;
                ValueString = newVal.ToString();
                ValueInt = (int)Math.Round(newVal);

                _internalValueChanging = false;
            }
        }

        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        #endregion

        #region MinValueProperty

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata(double.MinValue));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region MaxValueProperty

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata(double.MaxValue));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region SmallStepValueProperty

        public static readonly DependencyProperty SmallStepValueProperty = DependencyProperty.Register("SmallStepValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata((double)1));

        public double SmallStepValue
        {
            get { return (double)GetValue(SmallStepValueProperty); }
            set { SetValue(SmallStepValueProperty, value); }
        }

        #endregion

        #region LargeStepValueProperty

        public static readonly DependencyProperty LargeStepValueProperty = DependencyProperty.Register("LargeStepValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata((double)10));

        public double LargeStepValue
        {
            get { return (double)GetValue(LargeStepValueProperty); }
            set { SetValue(LargeStepValueProperty, value); }
        }

        #endregion

        #region HeaderProperty

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(DoubleEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderPropertyChanged)));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleEditor target)
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

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DoubleEditor), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        #endregion
    }
}
