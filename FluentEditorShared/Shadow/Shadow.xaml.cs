// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace FluentEditorShared.Shadow
{
    public sealed partial class Shadow : UserControl
    {
        public Shadow()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeShadow();
            ResizeShadow();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeShadow();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeShadow();
        }

        #region ElevationProperty

        public static DependencyProperty ElevationProperty = DependencyProperty.Register("Elevation", typeof(double), typeof(Shadow), new PropertyMetadata((double)1, new PropertyChangedCallback(OnElevationPropertyChanged)));

        private static void OnElevationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Shadow target)
            {
                target.OnElevationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnElevationChanged(double oldValue, double newValue)
        {
            UpdateShadowValues();
        }

        public double Elevation
        {
            get { return (double)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        #endregion

        #region ColorProperty

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(Shadow), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnColorPropertyChanged)));

        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Shadow target)
            {
                target.OnColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            UpdateShadowValues();
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        private ContainerVisual _shadowContainer;
        private DropShadow _shadowDirectional;
        private DropShadow _shadowAmbient;
        private SpriteVisual _shadowDirectionalVisual;
        private SpriteVisual _shadowAmbientVisual;

        private void InitializeShadow()
        {
            if (_shadowDirectional != null)
            {
                DisposeShadow();
            }

            var compositor = Window.Current.Compositor;

            _shadowContainer = compositor.CreateContainerVisual();

            _shadowAmbientVisual = compositor.CreateSpriteVisual();
            _shadowAmbient = compositor.CreateDropShadow();
            _shadowAmbientVisual.Shadow = _shadowAmbient;

            _shadowDirectionalVisual = compositor.CreateSpriteVisual();
            _shadowDirectional = compositor.CreateDropShadow();
            _shadowDirectionalVisual.Shadow = _shadowDirectional;

            _shadowContainer.Children.InsertAtTop(_shadowAmbientVisual);
            _shadowContainer.Children.InsertAtTop(_shadowDirectionalVisual);
            ElementCompositionPreview.SetElementChildVisual(PlaceholderElement, _shadowContainer);

            UpdateShadowValues();
        }

        private void DisposeShadow()
        {
            if (_shadowAmbient != null)
            {
                _shadowAmbient.Dispose();
                _shadowAmbient = null;
            }
            if (_shadowAmbientVisual != null)
            {
                _shadowAmbientVisual.Dispose();
                _shadowAmbientVisual = null;
            }
            if (_shadowDirectional != null)
            {
                _shadowDirectional.Dispose();
                _shadowDirectional = null;
            }
            if (_shadowDirectionalVisual != null)
            {
                _shadowDirectionalVisual.Dispose();
                _shadowDirectionalVisual = null;
            }
        }

        private void ResizeShadow()
        {
            if (_shadowDirectionalVisual != null)
            {
                _shadowDirectionalVisual.Size = new Vector2((float)PlaceholderElement.ActualWidth, (float)PlaceholderElement.ActualHeight);
            }
            if (_shadowAmbientVisual != null)
            {
                _shadowAmbientVisual.Size = new Vector2((float)PlaceholderElement.ActualWidth, (float)PlaceholderElement.ActualHeight);
            }
        }
        private void UpdateShadowValues()
        {
            var e = (float)Elevation;
            var o = e < 24 ? 1 : 0.6f;

            if (_shadowDirectionalVisual != null)
            {
                _shadowDirectional.BlurRadius = (e * 0.9f);
                _shadowDirectional.Opacity = o * 0.22f;
                _shadowDirectional.Offset = new Vector3(0, e * 0.4f, 0);
                _shadowDirectional.Color = Color;
            }
            if (_shadowAmbientVisual != null)
            {
                _shadowAmbient.BlurRadius = (e * 0.225f);
                _shadowAmbient.Opacity = o * 0.18f;
                _shadowAmbient.Offset = new Vector3(0, e * 0.0075f, 0);
                _shadowAmbient.Color = Color;
            }
        }
    }
}

