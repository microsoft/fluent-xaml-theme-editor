// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public struct ColorPaletteConstraint
    {
        public static readonly ColorPaletteConstraint DefaultVibrant = new ColorPaletteConstraint("Vibrant", "Vibrant", 0.5, 0.3, 0.7, 1.0, 0.35, 1.0);
        public static readonly ColorPaletteConstraint DefaultLightVibrant = new ColorPaletteConstraint("LightVibrant", "Light Vibrant", 0.74, 0.55, 1.0, 1.0, 0.35, 1.0);
        public static readonly ColorPaletteConstraint DefaultDarkVibrant = new ColorPaletteConstraint("DarkVibrant", "Dark Vibrant", 0.26, 0.0, 0.45, 1.0, 0.35, 1.0);
        public static readonly ColorPaletteConstraint DefaultMuted = new ColorPaletteConstraint("Muted", "Muted", 0.5, 0.3, 0.7, 0.3, 0.0, 0.4);
        public static readonly ColorPaletteConstraint DefaultLightMuted = new ColorPaletteConstraint("LightMuted", "Light Muted", 0.74, 0.55, 1.0, 0.3, 0.0, 0.4);
        public static readonly ColorPaletteConstraint DefaultDarkMuted = new ColorPaletteConstraint("DarkMuted", "Dark Muted", 0.26, 0.0, 0.45, 0.3, 0.0, 0.4);

        //public static readonly Dictionary<string, ColorPaletteConstraint> DefaultPaletteConstraints;
        public static readonly ColorPaletteConstraint[] DefaultPaletteConstraints;

        static ColorPaletteConstraint()
        {
            DefaultPaletteConstraints = new ColorPaletteConstraint[6];
            DefaultPaletteConstraints[0] = DefaultVibrant;
            DefaultPaletteConstraints[1] = DefaultLightVibrant;
            DefaultPaletteConstraints[2] = DefaultDarkVibrant;
            DefaultPaletteConstraints[3] = DefaultMuted;
            DefaultPaletteConstraints[4] = DefaultLightMuted;
            DefaultPaletteConstraints[5] = DefaultDarkMuted;
        }

        public ColorPaletteConstraint(string id, string caption, double targetLuminosity, double minLuminosity, double maxLuminosity, double targetSaturation, double minSaturation, double maxSaturation)
        {
            Id = id;
            Caption = caption;
            TargetLuminosity = targetLuminosity;
            MinLuminosity = minLuminosity;
            MaxLuminosity = maxLuminosity;
            TargetSaturation = targetSaturation;
            MinSaturation = minSaturation;
            MaxSaturation = maxSaturation;
        }

        public readonly string Id;
        public readonly string Caption;
        public readonly double TargetLuminosity;
        public readonly double MinLuminosity;
        public readonly double MaxLuminosity;
        public readonly double TargetSaturation;
        public readonly double MinSaturation;
        public readonly double MaxSaturation;
    }

    public static class PaletteExtraction
    {
        public const double DefaultLuminosityWeight = 3;
        public const double DefaultSaturationWeight = 6;
        public const double DefaultPopulationWeight = 1;

        public static (bool found, Color color, ColorPaletteConstraint constraint)[] ExtractColorPalette(IPixelBlob pixels, ColorPaletteConstraint[] constraints, int quantizedPaletteSize = 64, int pixelSkipping = 5, byte minAlpha = 125, byte maxWhiteLevel = 250, int signifigantBits = 5, int minPopulation = 0, double saturationWeight = DefaultSaturationWeight, double luminosityWeight = DefaultLuminosityWeight, double populationWeight = DefaultPopulationWeight)
        {
            if (constraints == null || constraints.Length == 0)
            {
                return null;
            }

            var quantizedImage = ColorQuantization.Quantize(pixels, quantizedPaletteSize, 0, pixelSkipping, minAlpha, maxWhiteLevel, signifigantBits, minPopulation);
            uint totalPopulation = 0;
            for (int i = 0; i < quantizedImage.Length; i++)
            {
                totalPopulation += quantizedImage[i].population;
            }

            double[] bestFitValues = new double[constraints.Length];
            Color[] colors = new Color[constraints.Length];
            bool[] foundEntry = new bool[constraints.Length];
            for (int i = 0; i < foundEntry.Length; i++)
            {
                foundEntry[i] = false;
                colors[i] = Colors.Black;
                bestFitValues[i] = 0;
            }

            for (int i = quantizedImage.Length - 1; i >= 0; i--)
            {
                // Ignore pixels that are mostly transparent or nearly white
                if (quantizedImage[i].color.A < minAlpha || (quantizedImage[i].color.R >= maxWhiteLevel && quantizedImage[i].color.G >= maxWhiteLevel && quantizedImage[i].color.B >= maxWhiteLevel))
                {
                    continue;
                }

                var pixelColorHSL = ColorUtils.RGBToHSL(quantizedImage[i].color);

                for (int j = 0; j < constraints.Length; j++)
                {
                    if (/* min / max luminosity */ pixelColorHSL.L >= constraints[j].MinLuminosity && pixelColorHSL.L <= constraints[j].MaxLuminosity &&
                        /* min / max saturation */ pixelColorHSL.S >= constraints[j].MinSaturation && pixelColorHSL.S <= constraints[j].MaxSaturation)
                    {
                        double pop = (double)quantizedImage[i].population / (double)totalPopulation;
                        var matchValue = ComputeColorMatchValue(pixelColorHSL, pop, constraints[j].TargetSaturation, constraints[j].TargetLuminosity, saturationWeight, luminosityWeight, populationWeight);
                        if (matchValue > bestFitValues[j])
                        {
                            bool found = false;
                            for (int k = 0; k < j; k++)
                            {
                                if (bestFitValues[k] >= 0 && quantizedImage[i].color == colors[k])
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                foundEntry[j] = true;
                                bestFitValues[j] = matchValue;
                                colors[j] = (quantizedImage[i].color);
                            }
                        }
                    }
                }
            }

            (bool found, Color color, ColorPaletteConstraint constraint)[] retVal = new(bool found, Color color, ColorPaletteConstraint constraint)[constraints.Length];
            for (int i = 0; i < constraints.Length; i++)
            {
                retVal[i] = (foundEntry[i], colors[i], constraints[i]);
            }
            return retVal;
        }

        private static double ComputeColorMatchValue(HSL hsl, double population, double targetSaturation, double targetLuminosity, double saturationWeight = DefaultSaturationWeight, double luminosityWeight = DefaultLuminosityWeight, double populationWeight = DefaultPopulationWeight)
        {
            var satVal = 1.0 - Math.Abs(hsl.S - targetSaturation);
            var lumVal = 1.0 - Math.Abs(hsl.L - targetLuminosity);
            return MathUtils.ComputeMean(new double[] { satVal, lumVal, population }, new double[] { saturationWeight, luminosityWeight, populationWeight });
        }

        // This duplicates the bugs in vibrant.js so as to return an identical palette result
        public static (bool found, Color color, ColorPaletteConstraint constraint)[] AlternateExtractColorPalette(IPixelBlob pixels, ColorPaletteConstraint[] constraints)
        {
            if (constraints == null || constraints.Length == 0)
            {
                return null;
            }

            // Duplicate bug in quantize.js which stops the second iteration of quantization too early
            var quantizedImage = ColorQuantization.Quantize(pixels, 64, 1);
            Color[] colors = new Color[constraints.Length];
            bool[] foundEntry = new bool[constraints.Length];
            for (int i = 0; i < foundEntry.Length; i++)
            {
                foundEntry[i] = false;
                colors[i] = Colors.Black;
            }

            // Duplicate the bug in vibrant.js that ignores the color comparison value and just takes the first color that matches the min / max
            for (int j = 0; j < constraints.Length; j++)
            {
                for (int i = quantizedImage.Length - 1; i >= 0; i--)
                {
                    var pixelColorHSL = ColorUtils.RGBToHSL(quantizedImage[i].color);
                    if (/* min / max luminosity */ pixelColorHSL.L >= constraints[j].MinLuminosity && pixelColorHSL.L <= constraints[j].MaxLuminosity &&
                        /* min / max saturation */ pixelColorHSL.S >= constraints[j].MinSaturation && pixelColorHSL.S <= constraints[j].MaxSaturation)
                    {
                        bool found = false;
                        for (int k = 0; k < j; k++)
                        {
                            if (quantizedImage[i].color == colors[k])
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            foundEntry[j] = true;
                            colors[j] = (quantizedImage[i].color);
                            break;
                        }
                    }
                }
            }

            (bool found, Color color, ColorPaletteConstraint constraint)[] retVal = new(bool found, Color color, ColorPaletteConstraint constraint)[constraints.Length];
            for (int i = 0; i < constraints.Length; i++)
            {
                retVal[i] = (foundEntry[i], colors[i], constraints[i]);
            }
            return retVal;
        }
    }
}
