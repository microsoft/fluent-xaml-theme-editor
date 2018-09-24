// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ViewCommon
{
    public class ViewContainer : Control
    {
        public ViewContainer()
        {
            this.DefaultStyleKey = typeof(ViewContainer);
        }

        private Border _outerContainer = null;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _outerContainer = GetTemplateChild("OuterContainer") as Border;

            if (ViewModel != null)
            {
                BuildView();
            }
        }

        private void BuildView()
        {
            var factory = ViewFactory;
            if (factory != null)
            {
                IView view = factory(ViewModel);
                if (view != null)
                {
                    view.ViewFactory = ViewFactory;
                    view.AttachViewModel(ViewModel);
                }
                _outerContainer.Child = view as UIElement;
            }
        }        

        #region ViewFactory property

        public static readonly DependencyProperty ViewFactoryProperty = DependencyProperty.Register("ViewFactory", typeof(ViewFactoryDelegate), typeof(ViewContainer), null);

        public ViewFactoryDelegate ViewFactory
        {
            get { return GetValue(ViewFactoryProperty) as ViewFactoryDelegate; }
            set { SetValue(ViewFactoryProperty, value); }
        }

        #endregion

        #region ViewModel property

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewContainer), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelPropertyChanged)));

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ViewContainer source)
            {
                source.OnViewModelChanged(d, e);
            }
        }

        private void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // _outerContainer == null is a legit case between the ctor completing and the template finishing load
            if (_outerContainer != null)
            {
                if (_outerContainer.Child != null)
                {
                    var oldView = _outerContainer.Child as IView;
                    if (oldView != null)
                    {
                        oldView.DetachViewModel();
                        oldView.ViewFactory = null;
                    }

                    _outerContainer.Child = null;
                }

                BuildView();
            }
        }

        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

    }
}
