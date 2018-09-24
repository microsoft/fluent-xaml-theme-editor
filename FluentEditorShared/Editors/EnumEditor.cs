// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.Editors
{
    public class EnumEditor : Control
    {
        public EnumEditor()
        {
            this.DefaultStyleKey = typeof(EnumEditor);
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

        public T GetSelectedValue<T>() where T : struct
        {
            T output;
            if (Enum.TryParse<T>(SelectedString, out output))
            {
                return output;
            }
            return default(T);
        }

        public void SetSelectedValue<T>(T value) where T : struct
        {
            var e = EnumType;
            if (e == null || typeof(T) != EnumType)
            {
                return;
            }
            SelectedString = value.ToString();
        }

        #region EnumTypeProperty

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnEnumTypePropertyChanged)));

        private static void OnEnumTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnumEditor target)
            {
                target.OnEnumTypeChanged(e.OldValue as Type, e.NewValue as Type);
            }
        }

        private void OnEnumTypeChanged(Type oldValue, Type newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if (newValue == null)
                {
                    AvailableStrings = null;
                }
                else
                {
                    try
                    {
                        AvailableStrings = Enum.GetNames(newValue);
                    }
                    catch
                    {
                        AvailableStrings = null;
                    }
                }

                string s = SelectedString;
                if (s != null)
                {
                    var a = AvailableStrings;
                    if (!a.Contains(s))
                    {
                        SelectedString = null;
                        SelectedBoxedEnum = null;
                    }
                }

                _internalValueChanging = false;
            }
        }

        public Type EnumType
        {
            get { return GetValue(EnumTypeProperty) as Type; }
            set { SetValue(EnumTypeProperty, value); }
        }

        #endregion

        #region AvailableStringsProperty

        public static readonly DependencyProperty AvailableStringsProperty = DependencyProperty.Register("AvailableStrings", typeof(string[]), typeof(EnumEditor), new PropertyMetadata(null));

        public string[] AvailableStrings
        {
            get { return GetValue(AvailableStringsProperty) as string[]; }
            set { SetValue(AvailableStringsProperty, value); }
        }
        #endregion

        #region SelectedString

        public static readonly DependencyProperty SelectedStringProperty = DependencyProperty.Register("SelectedString", typeof(string), typeof(EnumEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedStringPropertyChanged)));

        private static void OnSelectedStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnumEditor target)
            {
                target.OnSelectedStringChanged(e.OldValue as string, e.NewValue as string);
            }
        }

        private void OnSelectedStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if (newValue != null)
                {
                    var a = AvailableStrings;
                    if (!a.Contains(newValue))
                    {
                        SelectedString = null;
                        SelectedBoxedEnum = null;
                    }
                    else
                    {
                        SelectedBoxedEnum = Enum.Parse(EnumType, newValue);
                    }
                }
                else
                {
                    SelectedBoxedEnum = null;
                }

                _internalValueChanging = false;
            }
        }

        public string SelectedString
        {
            get { return GetValue(SelectedStringProperty) as string; }
            set { SetValue(SelectedStringProperty, value); }
        }

        #endregion

        #region SelectedBoxedEnumProperty

        public static readonly DependencyProperty SelectedBoxedEnumProperty = DependencyProperty.Register("SelectedBoxedEnum", typeof(object), typeof(EnumEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedBoxedEnumPropertyChanged)));

        private static void OnSelectedBoxedEnumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnumEditor target)
            {
                target.OnSelectedBoxedEnumChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnSelectedBoxedEnumChanged(object oldValue, object newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var t = EnumType;
                if (newValue == null || newValue.GetType() != t)
                {
                    SelectedString = null;
                }
                else
                {
                    SelectedString = newValue.ToString();
                }

                _internalValueChanging = false;
            }
        }

        public object SelectedBoxedEnum
        {
            get { return GetValue(SelectedBoxedEnumProperty); }
            set { SetValue(SelectedBoxedEnumProperty, value); }
        }

        #endregion

        #region HeaderProperty

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(EnumEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderPropertyChanged)));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnumEditor target)
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

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(EnumEditor), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        #endregion
    }
}
