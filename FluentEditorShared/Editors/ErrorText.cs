// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.Editors
{
    public class ErrorText : Control
    {
        public ErrorText()
        {
            this.DefaultStyleKey = typeof(ErrorText);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateState();
        }

        private void UpdateState()
        {
            if(ShowErrorState)
            {
                VisualStateManager.GoToState(this, "ErrorStateActive", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "ErrorStateInactive", true);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ErrorText), new PropertyMetadata(null));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ShowErrorStateProperty = DependencyProperty.Register("ShowErrorState", typeof(bool), typeof(ErrorText), new PropertyMetadata(false, new PropertyChangedCallback(OnShowErrorStatePropertyChanged)));

        private static void OnShowErrorStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ErrorText target)
            {
                target.UpdateState();
            }
        }

        public bool ShowErrorState
        {
            get { return (bool)GetValue(ShowErrorStateProperty); }
            set { SetValue(ShowErrorStateProperty, value); }
        }
    }
}
