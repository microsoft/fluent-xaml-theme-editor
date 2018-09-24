// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public enum ColorBlendMode { Burn, Darken, Dodge, Lighten, Multiply, Overlay, Screen };

    public static class ColorBlending
    {
        public const double DefaultSaturationConstant = 18.0;
        public const double DefaultDarkenConstant = 18.0;

        public static NormalizedRGB SaturateViaLCH(in NormalizedRGB input, double saturation, double saturationConstant = DefaultSaturationConstant)
        {
            LCH lch = ColorUtils.RGBToLCH(input, false);
            double saturated = lch.C + saturation * saturationConstant;
            if (saturated < 0)
            {
                saturated = 0;
            }
            return ColorUtils.LCHToRGB(new LCH(lch.L, saturated, lch.H, false), false);
        }

        public static NormalizedRGB DesaturateViaLCH(in NormalizedRGB input, double saturation, double saturationConstant = DefaultSaturationConstant)
        {
            return SaturateViaLCH(input, -1.0 * saturation, saturationConstant);
        }

        public static NormalizedRGB DarkenViaLAB(in NormalizedRGB input, double amount, double darkenConstant = DefaultDarkenConstant)
        {
            LAB lab = ColorUtils.RGBToLAB(input, false);
            double darkened = lab.L - amount * darkenConstant;
            return ColorUtils.LABToRGB(new LAB(darkened, lab.A, lab.B), false);
        }

        public static NormalizedRGB LightenViaLAB(in NormalizedRGB input, double amount, double darkenConstant = DefaultDarkenConstant)
        {
            return DarkenViaLAB(input, -1.0 * amount, darkenConstant);
        }

        public static NormalizedRGB Blend(in NormalizedRGB bottom, in NormalizedRGB top, ColorBlendMode mode)
        {
            switch (mode)
            {
                case ColorBlendMode.Burn:
                    return BlendBurn(bottom, top);
                case ColorBlendMode.Darken:
                    return BlendDarken(bottom, top);
                case ColorBlendMode.Dodge:
                    return BlendDodge(bottom, top);
                case ColorBlendMode.Lighten:
                    return BlendLighten(bottom, top);
                case ColorBlendMode.Multiply:
                    return BlendMultiply(bottom, top);
                case ColorBlendMode.Overlay:
                    return BlendOverlay(bottom, top);
                case ColorBlendMode.Screen:
                    return BlendScreen(bottom, top);
                default:
                    throw new ArgumentException("Unknown blend mode", "mode");
            }
        }

        public static NormalizedRGB BlendBurn(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendBurn(bottom.R, top.R), BlendBurn(bottom.G, top.G), BlendBurn(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendBurn(double bottom, double top)
        {
            if (top == 0.0)
            {
                // Despite the discontinuity, other sources seem to use 0.0 here instead of 1
                return 0.0;
            }
            return 1.0 - (1.0 - bottom) / top;
        }

        public static NormalizedRGB BlendDarken(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendDarken(bottom.R, top.R), BlendDarken(bottom.G, top.G), BlendDarken(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendDarken(double bottom, double top)
        {
            return Math.Min(bottom, top);
        }

        public static NormalizedRGB BlendDodge(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendDodge(bottom.R, top.R), BlendDodge(bottom.G, top.G), BlendDodge(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendDodge(double bottom, double top)
        {
            if (top >= 1.0)
            {
                return 1.0;
            }
            double retVal = bottom / (1.0 - top);
            if (retVal >= 1.0)
            {
                return 1.0;
            }
            return retVal;
        }

        public static NormalizedRGB BlendLighten(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendLighten(bottom.R, top.R), BlendLighten(bottom.G, top.G), BlendLighten(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendLighten(double bottom, double top)
        {
            return Math.Max(bottom, top);
        }

        public static NormalizedRGB BlendMultiply(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendMultiply(bottom.R, top.R), BlendMultiply(bottom.G, top.G), BlendMultiply(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendMultiply(double bottom, double top)
        {
            return bottom * top;
        }

        public static NormalizedRGB BlendOverlay(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendOverlay(bottom.R, top.R), BlendOverlay(bottom.G, top.G), BlendOverlay(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendOverlay(double bottom, double top)
        {
            if (bottom < 0.5)
            {
                return MathUtils.ClampToUnit(2.0 * top * bottom);
            }
            else
            {
                return MathUtils.ClampToUnit(1.0 - 2.0 * (1.0 - top) * (1.0 - bottom));
            }
        }

        public static NormalizedRGB BlendScreen(in NormalizedRGB bottom, in NormalizedRGB top)
        {
            return new NormalizedRGB(BlendScreen(bottom.R, top.R), BlendScreen(bottom.G, top.G), BlendScreen(bottom.B, top.B), false);
        }

        // single channel in the range [0.0,1.0]
        public static double BlendScreen(double bottom, double top)
        {
            return 1.0 - (1.0 - top) * (1.0 - bottom);
        }


        // Alpha channel of background is ignored
        // The returned color always has an alpha channel of 0xff
        // Different programs (eg: paint.net, photoshop) will give different answers than this occasionally but within +/- 1 in each channel
        // just depends on the details of how they round off decimals
        public static Color ComputeAlphaBlend(Color foreground, Color background)
        {
            if (foreground.A == 255)
            {
                return foreground;
            }
            if (foreground.A == 0)
            {
                return Color.FromArgb(255, background.R, background.G, background.B);
            }

            double fr = (double)foreground.R / 255.0;
            double fg = (double)foreground.G / 255.0;
            double fb = (double)foreground.B / 255.0;
            double fa = (double)foreground.A / 255.0;

            double br = (double)background.R / 255.0;
            double bg = (double)background.G / 255.0;
            double bb = (double)background.B / 255.0;

            double finalr = fa * fr + (1.0 - fa) * br;
            double finalg = fa * fg + (1.0 - fa) * bg;
            double finalb = fa * fb + (1.0 - fa) * bb;

            return Color.FromArgb(255, (byte)Math.Round(255.0 * finalr), (byte)Math.Round(255.0 * finalg), (byte)Math.Round(255.0 * finalb));
        }
    }
}
