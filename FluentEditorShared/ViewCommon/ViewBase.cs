// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ViewCommon
{
    public abstract class ViewBase<T> : UserControl, IView where T : class
    {
        // The viewmodel can be attached via the weakly typed AttachViewModel but still want to have a strongly typed dependency property to x:Bind to
        public void AttachViewModel(object viewModel)
        {
            ViewModel = viewModel as T;
        }

        public void DetachViewModel()
        {
            ViewModel = null;
        }

        #region ViewModel property

        // The viewmodel can be attached via the weakly typed AttachViewModel but still want to have a strongly typed dependency property to x:Bind to
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(T), typeof(ViewBase<T>), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelPropertyChanged)));

        public T ViewModel
        {
            get { return GetValue(ViewModelProperty) as T; }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ViewBase<T> target)
            {
                target.OnViewModelChanged(e.OldValue as T, e.NewValue as T);
            }
        }

        protected virtual void OnViewModelChanged(T oldViewModel, T newViewModel)
        {
            // derived classes might need to do something here
        }

        #endregion

        #region ViewFactory property

        public static readonly DependencyProperty ViewFactoryProperty = DependencyProperty.Register("ViewFactory", typeof(ViewFactoryDelegate), typeof(ViewBase<T>), new PropertyMetadata(null, new PropertyChangedCallback(OnViewFactoryPropertyChanged)));

        public ViewFactoryDelegate ViewFactory
        {
            get { return GetValue(ViewFactoryProperty) as ViewFactoryDelegate; }
            set { SetValue(ViewFactoryProperty, value); }
        }

        private static void OnViewFactoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ViewBase<T> target)
            {
                target.OnViewFactoryChanged(e.OldValue as ViewFactoryDelegate, e.NewValue as ViewFactoryDelegate);
            }
        }

        protected virtual void OnViewFactoryChanged(ViewFactoryDelegate oldViewFactory, ViewFactoryDelegate newViewFactory)
        {
            // derived classes might need to do something here
        }

        #endregion
    }
}
