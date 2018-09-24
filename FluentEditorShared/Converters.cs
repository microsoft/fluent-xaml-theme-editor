// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace FluentEditorShared
{
    public class ColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is Color c)
            {
                return c.ToString();
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
                if(boxedColor is Color c)
                {
                    return c;
                }
                else
                {
                    return Colors.White;
                }
            }
            catch
            {
                return Colors.White;
            }
        }
    }

    public class NullableColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Nullable<Color>)
            {
                var c = (Color?)value;
                if(c.HasValue)
                {
                    return c.Value.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
                if (boxedColor is Color c)
                {
                    return new Color?(c);
                }
                else
                {
                    return new Color?(Colors.White);
                }
            }
            catch
            {
                return new Color?(Colors.White);
            }
        }
    }
}
