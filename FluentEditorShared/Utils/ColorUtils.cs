using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public static class ColorUtils
    {
        public const int DefaultRoundingPrecision = 5;

        public static string FormatColorString(Color input, ColorStringFormat format, int precision = 4)
        {
            switch (format)
            {
                case ColorStringFormat.RGB:
                    return string.Format("{0}{1}{2}", input.R.ToString("x2"), input.G.ToString("x2"), input.B.ToString("x2"));
                case ColorStringFormat.PoundRGB:
                    return string.Format("#{0}{1}{2}", input.R.ToString("X2"), input.G.ToString("X2"), input.B.ToString("X2"));
                case ColorStringFormat.ARGB:
                    return string.Format("{0}{1}{2}{3}", input.A.ToString("x2"), input.R.ToString("x2"), input.G.ToString("x2"), input.B.ToString("x2"));
                case ColorStringFormat.NormalizedRGB:
                    var normalizedRGB = new NormalizedRGB(input, true, precision);
                    return string.Format("{0},{1},{2}", normalizedRGB.R, normalizedRGB.G, normalizedRGB.B);
                case ColorStringFormat.HSL:
                    var hsl = RGBToHSL(input, true, precision);
                    return string.Format("{0},{1},{2}", hsl.H, hsl.S, hsl.L);
                case ColorStringFormat.HSV:
                    var hsv = RGBToHSV(input, true, precision);
                    return string.Format("{0},{1},{2}", hsv.H, hsv.S, hsv.V);
                case ColorStringFormat.LAB:
                    var lab = RGBToLAB(input, true, precision);
                    return string.Format("{0},{1},{2}", lab.L, lab.A, lab.B);
                case ColorStringFormat.LCH:
                    var lch = RGBToLCH(input, true, precision);
                    return string.Format("{0},{1},{2}", lch.L, lch.C, lch.H);
                case ColorStringFormat.XYZ:
                    var xyz = RGBToXYZ(input, true, precision);
                    return string.Format("{0},{1},{2}", xyz.X, xyz.Y, xyz.Z);
                case ColorStringFormat.Luminance:
                    var lum = RGBToLuminance(input, true, precision);
                    return lum.ToString();
                case ColorStringFormat.Temperature:
                    var temp = RGBToTemperature(input, true, 0);
                    return temp.ToString();
            }
            return null;
        }

        public static bool TryParseColorString(string input, out Color retVal)
        {
            if (input.StartsWith("#"))
            {
                UInt32 raw;
                if (UInt32.TryParse(input.Substring(1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out raw))
                {
                    if (input.Length == 7)
                    {
                        raw |= 0xFF000000;
                    }

                    retVal = Color.FromArgb(
                        (byte)((raw & 0xFF000000) >> 24),
                        (byte)((raw & 0x00FF0000) >> 16),
                        (byte)((raw & 0x0000FF00) >> 8),
                        (byte)(raw & 0x000000FF));
                    return true;
                }
                else
                {
                    retVal = default(Color);
                    return false;
                }
            }
            else
            {
                string inputLower = input.ToLowerInvariant();
                switch (inputLower)
                {
                    case "aliceblue":
                        retVal = Colors.AliceBlue;
                        return true;
                    case "antiquewhite":
                        retVal = Colors.AntiqueWhite;
                        return true;
                    case "aqua":
                        retVal = Colors.Aqua;
                        return true;
                    case "aquamarine":
                        retVal = Colors.Aquamarine;
                        return true;
                    case "azure":
                        retVal = Colors.Azure;
                        return true;
                    case "beige":
                        retVal = Colors.Beige;
                        return true;
                    case "bisque":
                        retVal = Colors.Bisque;
                        return true;
                    case "black":
                        retVal = Colors.Black;
                        return true;
                    case "blanchedalmond":
                        retVal = Colors.BlanchedAlmond;
                        return true;
                    case "blue":
                        retVal = Colors.Blue;
                        return true;
                    case "blueviolet":
                        retVal = Colors.BlueViolet;
                        return true;
                    case "brown":
                        retVal = Colors.Brown;
                        return true;
                    case "burlywood":
                        retVal = Colors.BurlyWood;
                        return true;
                    case "cadetblue":
                        retVal = Colors.CadetBlue;
                        return true;
                    case "chartreuse":
                        retVal = Colors.Chartreuse;
                        return true;
                    case "chocolate":
                        retVal = Colors.Chocolate;
                        return true;
                    case "coral":
                        retVal = Colors.Coral;
                        return true;
                    case "cornflowerblue":
                        retVal = Colors.CornflowerBlue;
                        return true;
                    case "cornsilk":
                        retVal = Colors.Cornsilk;
                        return true;
                    case "crimson":
                        retVal = Colors.Crimson;
                        return true;
                    case "cyan":
                        retVal = Colors.Cyan;
                        return true;
                    case "darkblue":
                        retVal = Colors.DarkBlue;
                        return true;
                    case "darkcyan":
                        retVal = Colors.DarkCyan;
                        return true;
                    case "darkgoldenrod":
                        retVal = Colors.DarkGoldenrod;
                        return true;
                    case "darkgray":
                        retVal = Colors.DarkGray;
                        return true;
                    case "darkgreen":
                        retVal = Colors.DarkGreen;
                        return true;
                    case "darkkhaki":
                        retVal = Colors.DarkKhaki;
                        return true;
                    case "darkmagenta":
                        retVal = Colors.DarkMagenta;
                        return true;
                    case "darkolivegreen":
                        retVal = Colors.DarkOliveGreen;
                        return true;
                    case "darkorange":
                        retVal = Colors.DarkOrange;
                        return true;
                    case "darkorchid":
                        retVal = Colors.DarkOrchid;
                        return true;
                    case "darkred":
                        retVal = Colors.DarkRed;
                        return true;
                    case "darksalmon":
                        retVal = Colors.DarkSalmon;
                        return true;
                    case "darkseagreen":
                        retVal = Colors.DarkSeaGreen;
                        return true;
                    case "darkslateblue":
                        retVal = Colors.DarkSlateBlue;
                        return true;
                    case "darkslategray":
                        retVal = Colors.DarkSlateGray;
                        return true;
                    case "darkturquoise":
                        retVal = Colors.DarkTurquoise;
                        return true;
                    case "darkviolet":
                        retVal = Colors.DarkViolet;
                        return true;
                    case "deeppink":
                        retVal = Colors.DeepPink;
                        return true;
                    case "deepskyblue":
                        retVal = Colors.DeepSkyBlue;
                        return true;
                    case "dimgray":
                        retVal = Colors.DimGray;
                        return true;
                    case "dodgerblue":
                        retVal = Colors.DodgerBlue;
                        return true;
                    case "firebrick":
                        retVal = Colors.Firebrick;
                        return true;
                    case "floralwhite":
                        retVal = Colors.FloralWhite;
                        return true;
                    case "forestgreen":
                        retVal = Colors.ForestGreen;
                        return true;
                    case "fuchsia":
                        retVal = Colors.Fuchsia;
                        return true;
                    case "gainsboro":
                        retVal = Colors.Gainsboro;
                        return true;
                    case "ghostwhite":
                        retVal = Colors.GhostWhite;
                        return true;
                    case "gold":
                        retVal = Colors.Gold;
                        return true;
                    case "goldenrod":
                        retVal = Colors.Goldenrod;
                        return true;
                    case "gray":
                        retVal = Colors.Gray;
                        return true;
                    case "green":
                        retVal = Colors.Green;
                        return true;
                    case "greenyellow":
                        retVal = Colors.GreenYellow;
                        return true;
                    case "honeydew":
                        retVal = Colors.Honeydew;
                        return true;
                    case "hotpink":
                        retVal = Colors.HotPink;
                        return true;
                    case "indianred":
                        retVal = Colors.IndianRed;
                        return true;
                    case "indigo":
                        retVal = Colors.Indigo;
                        return true;
                    case "ivory":
                        retVal = Colors.Ivory;
                        return true;
                    case "khaki":
                        retVal = Colors.Khaki;
                        return true;
                    case "lavender":
                        retVal = Colors.Lavender;
                        return true;
                    case "lavenderblush":
                        retVal = Colors.LavenderBlush;
                        return true;
                    case "lawngreen":
                        retVal = Colors.LawnGreen;
                        return true;
                    case "lemonchiffon":
                        retVal = Colors.LemonChiffon;
                        return true;
                    case "lightblue":
                        retVal = Colors.LightBlue;
                        return true;
                    case "lightcoral":
                        retVal = Colors.LightCoral;
                        return true;
                    case "lightcyan":
                        retVal = Colors.LightCyan;
                        return true;
                    case "lightgoldenrodyellow":
                        retVal = Colors.LightGoldenrodYellow;
                        return true;
                    case "lightgray":
                        retVal = Colors.LightGray;
                        return true;
                    case "lightgreen":
                        retVal = Colors.LightGreen;
                        return true;
                    case "lightpink":
                        retVal = Colors.LightPink;
                        return true;
                    case "lightsalmon":
                        retVal = Colors.LightSalmon;
                        return true;
                    case "lightseagreen":
                        retVal = Colors.LightSeaGreen;
                        return true;
                    case "lightskyblue":
                        retVal = Colors.LightSkyBlue;
                        return true;
                    case "lightslategray":
                        retVal = Colors.LightSlateGray;
                        return true;
                    case "lightsteelblue":
                        retVal = Colors.LightSteelBlue;
                        return true;
                    case "lightyellow":
                        retVal = Colors.LightYellow;
                        return true;
                    case "lime":
                        retVal = Colors.Lime;
                        return true;
                    case "limegreen":
                        retVal = Colors.LimeGreen;
                        return true;
                    case "linen":
                        retVal = Colors.Linen;
                        return true;
                    case "magenta":
                        retVal = Colors.Magenta;
                        return true;
                    case "maroon":
                        retVal = Colors.Maroon;
                        return true;
                    case "mediumaquamarine":
                        retVal = Colors.MediumAquamarine;
                        return true;
                    case "mediumblue":
                        retVal = Colors.MediumBlue;
                        return true;
                    case "mediumorchid":
                        retVal = Colors.MediumOrchid;
                        return true;
                    case "mediumpurple":
                        retVal = Colors.MediumPurple;
                        return true;
                    case "mediumseagreen":
                        retVal = Colors.MediumSeaGreen;
                        return true;
                    case "mediumslateblue":
                        retVal = Colors.MediumSlateBlue;
                        return true;
                    case "mediumspringgreen":
                        retVal = Colors.MediumSpringGreen;
                        return true;
                    case "mediumturquoise":
                        retVal = Colors.MediumTurquoise;
                        return true;
                    case "mediumvioletred":
                        retVal = Colors.MediumVioletRed;
                        return true;
                    case "midnightblue":
                        retVal = Colors.MidnightBlue;
                        return true;
                    case "mintcream":
                        retVal = Colors.MintCream;
                        return true;
                    case "mistyrose":
                        retVal = Colors.MistyRose;
                        return true;
                    case "moccasin":
                        retVal = Colors.Moccasin;
                        return true;
                    case "navajowhite":
                        retVal = Colors.NavajoWhite;
                        return true;
                    case "navy":
                        retVal = Colors.Navy;
                        return true;
                    case "oldlace":
                        retVal = Colors.OldLace;
                        return true;
                    case "olive":
                        retVal = Colors.Olive;
                        return true;
                    case "olivedrab":
                        retVal = Colors.OliveDrab;
                        return true;
                    case "orange":
                        retVal = Colors.Orange;
                        return true;
                    case "orangered":
                        retVal = Colors.OrangeRed;
                        return true;
                    case "orchid":
                        retVal = Colors.Orchid;
                        return true;
                    case "palegoldenrod":
                        retVal = Colors.PaleGoldenrod;
                        return true;
                    case "palegreen":
                        retVal = Colors.PaleGreen;
                        return true;
                    case "paleturquoise":
                        retVal = Colors.PaleTurquoise;
                        return true;
                    case "palevioletred":
                        retVal = Colors.PaleVioletRed;
                        return true;
                    case "papayawhip":
                        retVal = Colors.PapayaWhip;
                        return true;
                    case "peachpuff":
                        retVal = Colors.PeachPuff;
                        return true;
                    case "peru":
                        retVal = Colors.Peru;
                        return true;
                    case "pink":
                        retVal = Colors.Pink;
                        return true;
                    case "plum":
                        retVal = Colors.Plum;
                        return true;
                    case "powderblue":
                        retVal = Colors.PowderBlue;
                        return true;
                    case "purple":
                        retVal = Colors.Purple;
                        return true;
                    case "red":
                        retVal = Colors.Red;
                        return true;
                    case "rosybrown":
                        retVal = Colors.RosyBrown;
                        return true;
                    case "royalblue":
                        retVal = Colors.RoyalBlue;
                        return true;
                    case "saddlebrown":
                        retVal = Colors.SaddleBrown;
                        return true;
                    case "salmon":
                        retVal = Colors.Salmon;
                        return true;
                    case "sandybrown":
                        retVal = Colors.SandyBrown;
                        return true;
                    case "seagreen":
                        retVal = Colors.SeaGreen;
                        return true;
                    case "seashell":
                        retVal = Colors.SeaShell;
                        return true;
                    case "sienna":
                        retVal = Colors.Sienna;
                        return true;
                    case "silver":
                        retVal = Colors.Silver;
                        return true;
                    case "skyblue":
                        retVal = Colors.SkyBlue;
                        return true;
                    case "slateblue":
                        retVal = Colors.SlateBlue;
                        return true;
                    case "slategray":
                        retVal = Colors.SlateGray;
                        return true;
                    case "snow":
                        retVal = Colors.Snow;
                        return true;
                    case "springgreen":
                        retVal = Colors.SpringGreen;
                        return true;
                    case "steelblue":
                        retVal = Colors.SteelBlue;
                        return true;
                    case "tan":
                        retVal = Colors.Tan;
                        return true;
                    case "teal":
                        retVal = Colors.Teal;
                        return true;
                    case "thistle":
                        retVal = Colors.Thistle;
                        return true;
                    case "tomato":
                        retVal = Colors.Tomato;
                        return true;
                    case "transparent":
                        retVal = Colors.Transparent;
                        return true;
                    case "turquoise":
                        retVal = Colors.Turquoise;
                        return true;
                    case "violet":
                        retVal = Colors.Violet;
                        return true;
                    case "wheat":
                        retVal = Colors.Wheat;
                        return true;
                    case "white":
                        retVal = Colors.White;
                        return true;
                    case "whitesmoke":
                        retVal = Colors.WhiteSmoke;
                        return true;
                    case "yellow":
                        retVal = Colors.Yellow;
                        return true;
                    case "yellowgreen":
                        retVal = Colors.YellowGreen;
                        return true;
                }
                retVal = default(Color);
                return false;
            }
        }

        // This ignores the Alpha channel of the input color
        public static HSL RGBToHSL(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToHSL(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static HSL RGBToHSL(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            double min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            double delta = max - min;

            double hue = 0;
            if (delta == 0)
            {
                hue = 0;
            }
            else if (max == rgb.R)
            {
                hue = 60 * (((rgb.G - rgb.B) / delta) % 6);
            }
            else if (max == rgb.G)
            {
                hue = 60 * ((rgb.B - rgb.R) / delta + 2);
            }
            else
            {
                hue = 60 * ((rgb.R - rgb.G) / delta + 4);
            }
            if (hue < 0)
            {
                hue += 360;
            }

            double lit = (max + min) / 2;

            double sat = 0;
            if (delta != 0)
            {
                sat = delta / (1 - Math.Abs(2 * lit - 1));
            }

            return new HSL(hue, sat, lit, round, roundingPrecision);
        }

        public static NormalizedRGB HSLToRGB(in HSL hsl, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double c = (1 - Math.Abs(2 * hsl.L - 1)) * hsl.S;
            double x = c * (1 - Math.Abs((hsl.H / 60 % 2) - 1));
            double m = hsl.L - c / 2;

            double r = 0, g = 0, b = 0;
            if (hsl.H < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (hsl.H < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (hsl.H < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (hsl.H < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (hsl.H < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (hsl.H < 360)
            {
                r = c;
                g = 0;
                b = x;
            }

            return new NormalizedRGB(r + m, g + m, b + m, round, roundingPrecision);
        }

        // This ignores the Alpha channel of the input color
        public static HSV RGBToHSV(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToHSV(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static HSV RGBToHSV(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            double min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            double delta = max - min;

            double hue = 0;
            if (delta == 0)
            {
                hue = 0;
            }
            else if (max == rgb.R)
            {
                hue = 60 * ((rgb.G - rgb.B) / delta % 6);
            }
            else if (max == rgb.G)
            {
                hue = 60 * ((rgb.B - rgb.R) / delta + 2);
            }
            else
            {
                hue = 60 * ((rgb.R - rgb.G) / delta + 4);
            }
            if (hue < 0)
            {
                hue += 360;
            }

            double sat = 0;
            if (max != 0)
            {
                sat = delta / max;
            }

            double val = max;

            return new HSV(hue, sat, val, round, roundingPrecision);
        }

        public static NormalizedRGB HSVToRGB(in HSV hsv, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double c = hsv.S * hsv.V;
            double x = c * (1 - Math.Abs((hsv.H / 60 % 2) - 1));
            double m = hsv.V - c;

            double r = 0, g = 0, b = 0;
            if (hsv.H < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (hsv.H < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (hsv.H < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (hsv.H < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (hsv.H < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (hsv.H < 360)
            {
                r = c;
                g = 0;
                b = x;
            }

            return new NormalizedRGB(r + m, g + m, b + m, round, roundingPrecision);
        }

        public static LAB LCHToLAB(in LCH lch, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            // LCH lit == LAB lit
            double a = 0;
            double b = 0;
            // In chroma.js this case is handled by having the rgb -> lch conversion special case h == 0. In that case it changes h to NaN. Which then requires some NaN checks elsewhere.
            // it seems preferable to handle the case of h = 0 here
            if (lch.H != 0)
            {
                a = Math.Cos(MathUtils.DegreesToRadians(lch.H)) * lch.C;
                b = Math.Sin(MathUtils.DegreesToRadians(lch.H)) * lch.C;
            }

            return new LAB(lch.L, a, b, round, roundingPrecision);
        }

        // This discontinuity in the C parameter at 0 means that floating point errors will often result in values near 0 giving unpredictable results. 
        // EG: 0.0000001 gives a very different result than -0.0000001
        public static LCH LABToLCH(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            // LCH lit == LAB lit
            double h = (MathUtils.RadiansToDegrees(Math.Atan2(lab.B, lab.A)) + 360) % 360;
            double c = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);

            return new LCH(lab.L, c, h, round, roundingPrecision);
        }

        // This conversion uses the D65 constants for 2 degrees. That determines the constants used for the pure white point of the XYZ space of 0.95047, 1.0, 1.08883
        public static XYZ LABToXYZ(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double y = (lab.L + 16.0) / 116.0;
            double x = y + (lab.A / 500.0);
            double z = y - (lab.B / 200.0);

            double LABToXYZHelper(double i)
            {
                if (i > 0.206896552)
                {
                    return Math.Pow(i, 3);
                }
                else
                {
                    return 0.12841855 * (i - 0.137931034);
                }
            }

            x = 0.95047 * LABToXYZHelper(x);
            y = LABToXYZHelper(y);
            z = 1.08883 * LABToXYZHelper(z);

            return new XYZ(x, y, z, round, roundingPrecision);
        }

        // This conversion uses the D65 constants for 2 degrees. That determines the constants used for the pure white point of the XYZ space of 0.95047, 1.0, 1.08883
        public static LAB XYZToLAB(in XYZ xyz, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double XYZToLABHelper(double i)
            {
                if (i > 0.008856452)
                {
                    return Math.Pow(i, 1.0 / 3.0);
                }
                else
                {
                    return i / 0.12841855 + 0.137931034;
                }
            }

            double x = XYZToLABHelper(xyz.X / 0.95047);
            double y = XYZToLABHelper(xyz.Y);
            double z = XYZToLABHelper(xyz.Z / 1.08883);

            double l = (116.0 * y) - 16.0;
            double a = 500.0 * (x - y);
            double b = -200.0 * (z - y);

            return new LAB(l, a, b, round, roundingPrecision);
        }

        // This ignores the Alpha channel of the input color
        public static XYZ RGBToXYZ(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToXYZ(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static XYZ RGBToXYZ(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double RGBToXYZHelper(double i)
            {
                if (i <= 0.04045)
                {
                    return i / 12.92;
                }
                else
                {
                    return Math.Pow((i + 0.055) / 1.055, 2.4);
                }
            }

            double r = RGBToXYZHelper(rgb.R);
            double g = RGBToXYZHelper(rgb.G);
            double b = RGBToXYZHelper(rgb.B);

            double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
            double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
            double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

            return new XYZ(x, y, z, round, roundingPrecision);
        }

        public static NormalizedRGB XYZToRGB(in XYZ xyz, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double XYZToRGBHelper(double i)
            {
                if (i <= 0.0031308)
                {
                    return i * 12.92;
                }
                else
                {
                    return 1.055 * Math.Pow(i, 1 / 2.4) - 0.055;
                }
            }

            double r = XYZToRGBHelper(xyz.X * 3.2404542 - xyz.Y * 1.5371385 - xyz.Z * 0.4985314);
            double g = XYZToRGBHelper(xyz.X * -0.9692660 + xyz.Y * 1.8760108 + xyz.Z * 0.0415560);
            double b = XYZToRGBHelper(xyz.X * 0.0556434 - xyz.Y * 0.2040259 + xyz.Z * 1.0572252);

            return new NormalizedRGB(MathUtils.ClampToUnit(r), MathUtils.ClampToUnit(g), MathUtils.ClampToUnit(b), round, roundingPrecision);
        }

        // This ignores the Alpha channel of the input color
        public static LAB RGBToLAB(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToLAB(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static LAB RGBToLAB(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            XYZ xyz = RGBToXYZ(rgb, false);
            return XYZToLAB(xyz, round, roundingPrecision);
        }

        public static NormalizedRGB LABToRGB(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            XYZ xyz = LABToXYZ(lab, false);
            return XYZToRGB(xyz, round, roundingPrecision);
        }

        // This ignores the Alpha channel of the input color
        public static LCH RGBToLCH(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToLCH(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static LCH RGBToLCH(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            // The discontinuity near 0 in LABToLCH means we should round here even if the bound param is false
            LAB lab = RGBToLAB(rgb, true, 4);

            // This appears redundant but is actually nescessary in order to prevent floating point rounding errors from throwing off the Atan2 computation in LABToLCH
            // https://msdn.microsoft.com/en-us/library/system.math.atan2(v=vs.110).aspx
            // For the RGB value 255,255,255 what happens is the a value appears to be rounded to 0 but is still treated as negative by Atan2 which then returns PI instead of 0

            double l = lab.L == 0 ? 0 : lab.L;
            double a = lab.A == 0 ? 0 : lab.A;
            double b = lab.B == 0 ? 0 : lab.B;

            return LABToLCH(new LAB(l, a, b, false), round, roundingPrecision);
        }

        public static NormalizedRGB LCHToRGB(in LCH lch, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            LAB lab = LCHToLAB(lch, false);
            return LABToRGB(lab, round, roundingPrecision);
        }

        public static NormalizedRGB TemperatureToRGB(double tempKelvin, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            // The constants I could find assumed a decimal range of [0,255] for each channel. Just going to put a /255.0 at the end
            double r = 0.0, g = 0.0, b = 0.0;

            if (tempKelvin < 6600.0)
            {
                r = 255.0;

                g = tempKelvin / 100.0 - 2.0;
                g = -155.25485562709179 - 0.44596950469579133 * g + 104.49216199393888 * Math.Log(g);
            }
            else
            {
                r = tempKelvin / 100.0 - 55.0;
                r = 351.97690566805693 + 0.114206453784165 * r - 40.25366309332127 * Math.Log(r);

                g = tempKelvin / 100.0 - 50.0;
                g = 325.4494125711974 + 0.07943456536662342 * g - 28.0852963507957 * Math.Log(g);
            }

            if (tempKelvin >= 6600.0)
            {
                b = 255.0;
            }
            else if (tempKelvin < 2000.0)
            {
                b = 0.0;
            }
            else
            {
                b = tempKelvin / 100.0 - 10;
                b = -254.76935184120902 + 0.8274096064007395 * b + 115.67994401066147 * Math.Log(b);
            }

            return new NormalizedRGB(r / 255.0, g / 255.0, b / 255.0, round, roundingPrecision);
        }

        // This ignores the Alpha channel of the input color
        public static double RGBToTemperature(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToTemperature(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static double RGBToTemperature(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double t = 0;
            double min = 1000.0;
            double max = 40000.0;
            while (max - min > 0.4)
            {
                t = (max + min) / 2.0;
                NormalizedRGB testColor = TemperatureToRGB(t, false);
                if (testColor.B / testColor.R >= rgb.B / rgb.R)
                {
                    max = t;
                }
                else
                {
                    min = t;
                }
            }
            if (round)
            {
                return Math.Round(t, roundingPrecision);
            }
            else
            {
                return t;
            }
        }

        // This ignores the Alpha channel of the input color
        public static double RGBToLuminance(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToLuminance(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static double RGBToLuminance(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double LuminanceHelper(double i)
            {
                if (i <= 0.03928)
                {
                    return i / 12.92;
                }
                else
                {
                    return Math.Pow((i + 0.055) / 1.055, 2.4);
                }
            }
            double r = LuminanceHelper(rgb.R);
            double g = LuminanceHelper(rgb.G);
            double b = LuminanceHelper(rgb.B);

            // More accurate constants would be helpful here
            double l = r * 0.2126 + g * 0.7152 + b * 0.0722;

            if (round)
            {
                return Math.Round(l, roundingPrecision);
            }
            else
            {
                return l;
            }
        }

        // This ignores the Alpha channel of both input colors
        public static double ContrastRatio(Color a, Color b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return ContrastRatio(new NormalizedRGB(a, false), new NormalizedRGB(b, false), round, roundingPrecision);
        }

        public static double ContrastRatio(in NormalizedRGB a, in NormalizedRGB b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double la = RGBToLuminance(a, false);
            double lb = RGBToLuminance(b, false);
            double retVal = 0;
            if (la > lb)
            {
                retVal = (la + 0.05) / (lb + 0.05);
            }
            else
            {
                retVal = (lb + 0.05) / (la + 0.05);
            }

            if (round)
            {
                return Math.Round(retVal, roundingPrecision);
            }
            else
            {
                return retVal;
            }
        }

        // Returns the Color from colorOptions which has the best contrast ratio with background
        // in the case of ties the first color to appear in the list will be chosen
        // alpha channels are ignored
        public static Color ChooseColorForContrast(IEnumerable<Color> colorOptions, Color background)
        {
            if (colorOptions == null)
            {
                throw new ArgumentNullException("colorOptions");
            }
            Color bestColor = default(Color);
            double bestRatio = 0;
            foreach(var c in colorOptions)
            {
                double ratio = ContrastRatio(c, background, false);
                if (ratio > bestRatio)
                {
                    bestColor = c;
                    bestRatio = ratio;
                }
            }

            return bestColor;
        }

        public static Color InterpolateRGB(Color left, Color right, double position)
        {
            if (position <= 0)
            {
                return left;
            }
            if (position >= 1)
            {
                return right;
            }
            return Color.FromArgb(MathUtils.Lerp(left.A, right.A, position), MathUtils.Lerp(left.R, right.R, position), MathUtils.Lerp(left.G, right.G, position), MathUtils.Lerp(left.B, right.B, position));
        }

        public static NormalizedRGB InterpolateRGB(in NormalizedRGB left, in NormalizedRGB right, double position)
        {
            if (position <= 0)
            {
                return left;
            }
            if (position >= 1)
            {
                return right;
            }
            return new NormalizedRGB(MathUtils.Lerp(left.R, right.R, position), MathUtils.Lerp(left.G, right.G, position), MathUtils.Lerp(left.B, right.B, position), false);
        }

        // Generally looks better than RGB for interpolation
        public static LAB InterpolateLAB(in LAB left, in LAB right, double position)
        {
            if (position <= 0)
            {
                return left;
            }
            if (position >= 1)
            {
                return right;
            }
            return new LAB(MathUtils.Lerp(left.L, right.L, position), MathUtils.Lerp(left.A, right.A, position), MathUtils.Lerp(left.B, right.B, position), false);
        }

        // Possibly a better choice than LAB for very dark colors
        public static XYZ InterpolateXYZ(in XYZ left, in XYZ right, double position)
        {
            if (position <= 0)
            {
                return left;
            }
            if (position >= 1)
            {
                return right;
            }
            return new XYZ(MathUtils.Lerp(left.X, right.X, position), MathUtils.Lerp(left.Y, right.Y, position), MathUtils.Lerp(left.Z, right.Z, position), false);
        }

        public static NormalizedRGB InterpolateColor(in NormalizedRGB left, in NormalizedRGB right, double position, ColorScaleInterpolationMode mode)
        {
            switch (mode)
            {
                case ColorScaleInterpolationMode.LAB:
                    var leftLAB = ColorUtils.RGBToLAB(left, false);
                    var rightLAB = ColorUtils.RGBToLAB(right, false);
                    return LABToRGB(InterpolateLAB(leftLAB, rightLAB, position));
                case ColorScaleInterpolationMode.XYZ:
                    var leftXYZ = RGBToXYZ(left, false);
                    var rightXYZ = RGBToXYZ(right, false);
                    return XYZToRGB(InterpolateXYZ(leftXYZ, rightXYZ, position));
                default:
                    return InterpolateRGB(left, right, position);
            }
        }
    }
}
