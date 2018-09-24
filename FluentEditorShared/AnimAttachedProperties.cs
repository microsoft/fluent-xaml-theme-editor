// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace FluentEditorShared
{
    public static class AnimAttachedProperties
    {
        #region TranslateCubicProperty

        // duration,delay,controlPoint1X,controlPoint1Y,controlPoint2X,controlPoint2Y
        public static DependencyProperty TranslateCubicProperty = DependencyProperty.RegisterAttached("TranslateCubic", typeof(string), typeof(AnimAttachedProperties), new PropertyMetadata(null, new PropertyChangedCallback(OnTranslateCubicPropertyChanged)));

        public static string GetTranslateCubic(DependencyObject d)
        {
            return d.GetValue(TranslateCubicProperty) as string;
        }

        public static void SetTranslateCubic(DependencyObject d, string value)
        {
            d.SetValue(TranslateCubicProperty, value);
        }

        private static void OnTranslateCubicPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Visual targetVisual = null;
            if (target is UIElement targetElement)
            {
                targetVisual = ElementCompositionPreview.GetElementVisual(targetElement);
            }
            if (targetVisual == null)
            {
                return;
            }

            bool valid = false;
            Vector2 point1 = Vector2.Zero;
            Vector2 point2 = Vector2.Zero;
            TimeSpan duration = TimeSpan.Zero;
            TimeSpan? delay = null;

            if (e.NewValue == null || (e.NewValue as string) == string.Empty)
            {
                // Remove existing implicit anim on offset if any
                if (targetVisual.ImplicitAnimations != null && targetVisual.ImplicitAnimations.ContainsKey("Offset"))
                {
                    targetVisual.ImplicitAnimations.Remove("Offset");
                }
                return;
            }
            if (e.NewValue is string raw)
            {
                string[] rawPoints = raw.Split(",");
                if (rawPoints != null && rawPoints.Length == 6)
                {
                    if (rawPoints[1] != string.Empty && TimeSpan.TryParse(rawPoints[1], out TimeSpan de))
                    {
                        delay = de;
                    }

                    if (TimeSpan.TryParse(rawPoints[0], out duration))
                    {
                        if (float.TryParse(rawPoints[2], out float a))
                        {
                            if (float.TryParse(rawPoints[3], out float b))
                            {
                                if (float.TryParse(rawPoints[4], out float c))
                                {
                                    if (float.TryParse(rawPoints[5], out float d))
                                    {
                                        valid = true;
                                        point1 = new Vector2(a, b);
                                        point2 = new Vector2(c, d);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!valid)
            {
                return;
            }

            var compositor = Window.Current.Compositor;
            var easing = compositor.CreateCubicBezierEasingFunction(point1, point2);
            var anim = compositor.CreateVector3KeyFrameAnimation();
            if (delay.HasValue)
            {
                anim.DelayTime = delay.Value;
            }
            anim.Duration = duration;
            anim.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easing);

            if (targetVisual.ImplicitAnimations == null)
            {
                targetVisual.ImplicitAnimations = compositor.CreateImplicitAnimationCollection();
            }
            if (!targetVisual.ImplicitAnimations.ContainsKey("Offset"))
            {
                targetVisual.ImplicitAnimations.Add("Offset", anim);
            }
            else
            {
                targetVisual.ImplicitAnimations["Offset"] = anim;
            }
        }

        #endregion
    }
}
