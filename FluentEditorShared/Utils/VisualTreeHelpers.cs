// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace FluentEditorShared.Utils
{
    public static class VisualTreeHelpers
    {
        // This should only be called on the dispatcher thread associated with parent
        public static T FindElementByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null || name == null)
            {
                return null;
            }
            if (parent is T d)
            {
                if (d.Name == name)
                {
                    return d;
                }
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var retVal = FindElementByName<T>(VisualTreeHelper.GetChild(parent, i), name);
                if (retVal != null)
                {
                    return retVal;
                }
            }
            return null;
        }
    }
}
