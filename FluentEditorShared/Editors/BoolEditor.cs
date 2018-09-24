// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.Editors
{
    public class BoolEditor : Control
    {
        public BoolEditor()
        {
            this.DefaultStyleKey = typeof(BoolEditor);
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

        #region ValueProperty

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(BoolEditor), new PropertyMetadata(false, new PropertyChangedCallback(OnValuePropertyChanged)));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BoolEditor target = d as BoolEditor;
            if (target != null)
            {
                target.OnValueChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private void OnValueChanged(bool oldValue, bool newValue)
        {
            DisplayValue = newValue.ToString();
        }

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region DisplayValueProperty

        public static DependencyProperty DisplayValueProperty = DependencyProperty.Register("DisplayValue", typeof(string), typeof(BoolEditor), new PropertyMetadata("False"));

        public string DisplayValue
        {
            get { return GetValue(DisplayValueProperty) as string; }
            set { SetValue(DisplayValueProperty, value); }
        }

        #endregion

        #region HeaderProperty

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(BoolEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderPropertyChanged)));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BoolEditor target)
            {
                target.OnHeaderPropertyChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnHeaderPropertyChanged(object oldVal, object newVal)
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

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(BoolEditor), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        #endregion
    }
}
