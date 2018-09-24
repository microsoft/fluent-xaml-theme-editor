// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public enum RGBAxis { Red, Green, Blue };

    public class ColorQuantizationBox
    {
        public ColorQuantizationBox(IPixelBlob pixels, int signifigantBits = 5, int pixelSkipping = 5, byte minAlpha = 125, byte whiteCutoff = 250)
        {
            if (pixels == null)
            {
                throw new ArgumentNullException("pixels");
            }
            if (signifigantBits <= 0 || signifigantBits > 8)
            {
                throw new ArgumentException("signifigantBits must be in the range [1,8]");
            }
            SignifigantBits = signifigantBits;
            int sigShift = 8 - SignifigantBits;
            int retLength = 1 << (3 * SignifigantBits);
            Histogram = new uint[retLength];
            int pixelCount = 0;

            MinRed = 255;
            MaxRed = 0;
            MinGreen = 255;
            MaxGreen = 0;
            MinBlue = 255;
            MaxBlue = 0;

            for (uint y = 0; y < pixels.Height; y++)
            {
                for (uint x = 0; x < pixels.Width; x++)
                {
                    if (pixelCount++ % pixelSkipping != 0)
                    {
                        continue;
                    }
                    var c = pixels.GetPixel(x, y);
                    if (c.A < minAlpha || (c.R > whiteCutoff && c.G > whiteCutoff && c.B > whiteCutoff))
                    {
                        continue;
                    }
                    byte r = (byte)(c.R >> sigShift);
                    byte g = (byte)(c.G >> sigShift);
                    byte b = (byte)(c.B >> sigShift);
                    int index = GetColorIndex(r, g, b, SignifigantBits);
                    Histogram[index] += 1;

                    MinRed = Math.Min(MinRed, r);
                    MaxRed = Math.Max(MaxRed, r);
                    MinGreen = Math.Min(MinGreen, g);
                    MaxGreen = Math.Max(MaxGreen, g);
                    MinBlue = Math.Min(MinBlue, b);
                    MaxBlue = Math.Max(MaxBlue, b);
                }
            }

            HistogramTotal = 0;
            for (int i = MinRed; i <= MaxRed; i++)
            {
                for (int j = MinGreen; j <= MaxGreen; j++)
                {
                    for (int k = MinBlue; k <= MaxBlue; k++)
                    {
                        var index = GetColorIndex((byte)i, (byte)j, (byte)k, SignifigantBits);
                        HistogramTotal += Histogram[index];
                    }
                }
            }
        }

        public ColorQuantizationBox(ColorQuantizationBox source)
        {
            SignifigantBits = source.SignifigantBits;
            Histogram = source.Histogram;
            HistogramTotal = source.HistogramTotal;
            MinRed = source.MinRed;
            MaxRed = source.MaxRed;
            MinGreen = source.MinGreen;
            MaxGreen = source.MaxGreen;
            MinBlue = source.MinBlue;
            MaxBlue = source.MaxBlue;
        }

        public ColorQuantizationBox(uint[] histogram, byte minRed, byte maxRed, byte minGreen, byte maxGreen, byte minBlue, byte maxBlue, int signifigantBits = 5, int minPopulation = 0)
        {
            if (histogram == null)
            {
                throw new ArgumentNullException("histogram");
            }
            if (signifigantBits <= 0 || signifigantBits > 8)
            {
                throw new ArgumentException("signifigantBits must be in the range [1,8]");
            }
            SignifigantBits = signifigantBits;
            int sigShift = 8 - SignifigantBits;
            Histogram = histogram;
            MinRed = minRed;
            MaxRed = maxRed;
            MinGreen = minGreen;
            MaxGreen = maxGreen;
            MinBlue = minBlue;
            MaxBlue = maxBlue;

            HistogramTotal = 0;
            for (int i = MinRed; i <= MaxRed; i++)
            {
                for (int j = MinGreen; j <= MaxGreen; j++)
                {
                    for (int k = MinBlue; k <= MaxBlue; k++)
                    {
                        var index = GetColorIndex((byte)i, (byte)j, (byte)k, SignifigantBits);
                        HistogramTotal += Histogram[index];
                    }
                }
            }
        }

        private static int GetColorIndex(byte r, byte g, byte b, int signifigantBits = 5)
        {
            return (r << (2 * signifigantBits)) + (g << signifigantBits) + b;
        }

        public Color ComputeAverageColor()
        {
            int multiplier = 1 << (8 - SignifigantBits);
            double total = (double)HistogramTotal;
            double redTotal = 0;
            double greenTotal = 0;
            double blueTotal = 0;
            for (int i = MinRed; i <= MaxRed; i++)
            {
                for (int j = MinGreen; j <= MaxGreen; j++)
                {
                    for (int k = MinBlue; k <= MaxBlue; k++)
                    {
                        var index = GetColorIndex((byte)i, (byte)j, (byte)k, SignifigantBits);
                        double h = (double)Histogram[index];
                        redTotal += h * ((double)i + 0.5) * multiplier;
                        greenTotal += h * ((double)j + 0.5) * multiplier;
                        blueTotal += h * ((double)k + 0.5) * multiplier;
                    }
                }
            }
            if (total > 0)
            {
                double red = Math.Round(redTotal / total);
                double green = Math.Round(greenTotal / total);
                double blue = Math.Round(blueTotal / total);
                return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
            }
            else
            {
                double red = Math.Round(multiplier * (double)(MinRed + MaxRed + 1) / 2.0);
                double green = Math.Round(multiplier * (double)(MinGreen + MaxGreen + 1) / 2.0);
                double blue = Math.Round(multiplier * (double)(MinBlue + MaxBlue + 1) / 2.0);
                return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
            }
        }

        public Tuple<ColorQuantizationBox, ColorQuantizationBox> MedianCut()
        {
            if (this.HistogramTotal <= 1)
            {
                return new Tuple<ColorQuantizationBox, ColorQuantizationBox>(this, null);
            }

            uint total = 0;
            uint[] partialSum = null;
            RGBAxis primaryAxis = WidestAxis;
            byte primaryMin, primaryMax;

            if (primaryAxis == RGBAxis.Red)
            {
                primaryMin = MinRed;
                primaryMax = MaxRed;
                partialSum = new uint[RedRange + 1];
                for (byte i = MinRed; i <= MaxRed; i++)
                {
                    uint sum = 0;
                    for (byte j = MinGreen; j <= MaxGreen; j++)
                    {
                        for (byte k = MinBlue; k <= MaxBlue; k++)
                        {
                            var index = GetColorIndex(i, j, k, SignifigantBits);
                            sum += Histogram[index];
                        }
                    }
                    total += sum;
                    partialSum[i - primaryMin] = total;
                }
            }
            else if (primaryAxis == RGBAxis.Green)
            {
                primaryMin = MinGreen;
                primaryMax = MaxGreen;
                partialSum = new uint[GreenRange + 1];
                for (byte i = MinGreen; i <= MaxGreen; i++)
                {
                    uint sum = 0;
                    for (byte j = MinRed; j <= MaxRed; j++)
                    {
                        for (byte k = MinBlue; k <= MaxBlue; k++)
                        {
                            var index = GetColorIndex(j, i, k, SignifigantBits);
                            sum += Histogram[index];
                        }
                    }
                    total += sum;
                    partialSum[i - primaryMin] = total;
                }
            }
            else
            {
                primaryMin = MinBlue;
                primaryMax = MaxBlue;
                partialSum = new uint[BlueRange + 1];
                for (byte i = MinBlue; i <= MaxBlue; i++)
                {
                    uint sum = 0;
                    for (byte j = MinRed; j <= MaxRed; j++)
                    {
                        for (byte k = MinGreen; k <= MaxGreen; k++)
                        {
                            var index = GetColorIndex(j, k, i, SignifigantBits);
                            sum += Histogram[index];
                        }
                    }
                    total += sum;
                    partialSum[i - primaryMin] = total;
                }
            }

            if (primaryMin == primaryMax)
            {
                return new Tuple<ColorQuantizationBox, ColorQuantizationBox>(this, null);
            }

            uint[] lookAheadSum = new uint[partialSum.Length];
            for (int i = 0; i < partialSum.Length; i++)
            {
                lookAheadSum[i] = total - partialSum[i];
            }

            for (byte i = primaryMin; i <= primaryMax; i++)
            {
                var partial = partialSum[i - primaryMin];
                if (partial > total / 2)
                {
                    float left = i - primaryMin;
                    float right = primaryMax - i;
                    int cutValue = 0;
                    if (left <= right)
                    {
                        cutValue = Math.Min(primaryMax - 1, (int)Math.Floor((float)i + right / 2f));
                    }
                    else
                    {
                        cutValue = Math.Max(primaryMin, (int)Math.Floor((float)i - 1f - left / 2f));
                    }
                    while (partialSum[cutValue - primaryMin] <= 0)
                    {
                        cutValue++;
                    }
                    if (cutValue - primaryMin >= partialSum.Length)
                    {
                        cutValue = partialSum.Length - 1 + primaryMin;
                    }
                    uint lookAhead = lookAheadSum[cutValue - primaryMin];
                    while (lookAhead == 0 && cutValue > primaryMin && partialSum[cutValue - primaryMin - 1] != 0)
                    {
                        cutValue--;
                        lookAhead = lookAheadSum[cutValue - primaryMin];
                    }

                    ColorQuantizationBox leftBox = null, rightBox = null;
                    if (primaryAxis == RGBAxis.Red)
                    {
                        leftBox = new ColorQuantizationBox(Histogram, MinRed, (byte)(cutValue), MinGreen, MaxGreen, MinBlue, MaxBlue, SignifigantBits);
                        rightBox = new ColorQuantizationBox(Histogram, (byte)(cutValue + 1), MaxRed, MinGreen, MaxGreen, MinBlue, MaxBlue, SignifigantBits);
                    }
                    else if (primaryAxis == RGBAxis.Green)
                    {
                        leftBox = new ColorQuantizationBox(Histogram, MinRed, MaxRed, MinGreen, (byte)(cutValue), MinBlue, MaxBlue, SignifigantBits);
                        rightBox = new ColorQuantizationBox(Histogram, MinRed, MaxRed, (byte)(cutValue + 1), MaxGreen, MinBlue, MaxBlue, SignifigantBits);
                    }
                    else
                    {
                        leftBox = new ColorQuantizationBox(Histogram, MinRed, MaxRed, MinGreen, MaxGreen, MinBlue, (byte)(cutValue), SignifigantBits);
                        rightBox = new ColorQuantizationBox(Histogram, MinRed, MaxRed, MinGreen, MaxGreen, (byte)(cutValue + 1), MaxBlue, SignifigantBits);
                    }


                    return new Tuple<ColorQuantizationBox, ColorQuantizationBox>(leftBox, rightBox);
                }
            }

            throw new Exception("Unable to find median partial sum. This should never happen");
        }

        public readonly int SignifigantBits;
        public readonly uint[] Histogram;
        public readonly uint HistogramTotal;

        public readonly byte MinRed;
        public readonly byte MaxRed;
        public int RedRange
        {
            get { return MaxRed - MinRed; }
        }

        public readonly byte MinGreen;
        public readonly byte MaxGreen;
        public int GreenRange
        {
            get { return MaxGreen - MinGreen; }
        }

        public readonly byte MinBlue;
        public readonly byte MaxBlue;
        public int BlueRange
        {
            get { return MaxBlue - MinBlue; }
        }

        public RGBAxis WidestAxis
        {
            get
            {
                var r = RedRange;
                var g = GreenRange;
                var b = BlueRange;
                if (r >= g && r >= b)
                {
                    return RGBAxis.Red;
                }
                if (g >= r && g >= b)
                {
                    return RGBAxis.Green;
                }
                else
                {
                    return RGBAxis.Blue;
                }
            }
        }

        public int ColorVolume
        {
            get { return (RedRange + 1) * (GreenRange + 1) * (BlueRange + 1); }
        }

        public static void InsertIntoSortedList(List<ColorQuantizationBox> list, ColorQuantizationBox newItem, Func<ColorQuantizationBox, long> sortPriority)
        {
            if (list.Count == 0)
            {
                list.Add(newItem);
                return;
            }
            long newItemPriority = sortPriority(newItem);
            if (newItemPriority < sortPriority(list[0]))
            {
                list.Insert(0, newItem);
                return;
            }
            if (newItemPriority > sortPriority(list.Last()))
            {
                list.Add(newItem);
                return;
            }

            int newIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (newItemPriority <= sortPriority(list[i]))
                {
                    newIndex = i;
                    break;
                }
            }
            list.Insert(newIndex, newItem);
        }
    }

    // Based on Modified Median Cut Quantization from https://github.com/DanBloomberg/leptonica/blob/master/src/colorquant2.c
    public static class ColorQuantization
    {
        private static int CountValidColors(List<ColorQuantizationBox> queue, int minPopulation)
        {
            int retVal = 0;
            if (queue == null)
            {
                return retVal;
            }
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].HistogramTotal >= minPopulation)
                {
                    retVal++;
                }
            }
            return retVal;
        }

        public static (uint population, Color color)[] Quantize(IPixelBlob pixels, int finalPaletteMax = 64, int finalPaletteOffset = 0, int pixelSkipping = 5, byte minAlpha = 125, byte maxWhiteLevel = 250, int signifigantBits = 5, int minPopulation = 0, float fractionByPopulation = 0.75f, int maxIterations = 1000)
        {
            if (pixels == null || pixels.TotalPixels <= finalPaletteMax)
            {
                throw new ArgumentException("Invalid pixels");
            }
            if (finalPaletteMax <= 0 || finalPaletteMax > 256)
            {
                throw new ArgumentException("FinalPaletteSize must be in the range [0,256]");
            }
            if (signifigantBits <= 0 || signifigantBits > 8)
            {
                throw new ArgumentException("SignifigantBits must be in the range [1,8]");
            }
            int sigShift = 8 - signifigantBits;

            var initialVBox = new ColorQuantizationBox(pixels, signifigantBits, pixelSkipping, minAlpha, maxWhiteLevel);

            if (initialVBox.Histogram.Length <= finalPaletteMax)
            {
                Color[] smallRet = new Color[initialVBox.ColorVolume];
                int index = 0;
                for (int r = initialVBox.MinRed; r <= initialVBox.MaxRed; r++)
                {
                    for (int g = initialVBox.MinGreen; g <= initialVBox.MaxGreen; g++)
                    {
                        for (int b = initialVBox.MinBlue; b <= initialVBox.MaxBlue; b++)
                        {
                            // TODO test this
                            byte red = (byte)(r << sigShift);
                            byte green = (byte)(g << sigShift);
                            byte blue = (byte)(b << sigShift);
                            index++;
                        }
                    }
                }
            }

            List<ColorQuantizationBox> queue = new List<ColorQuantizationBox>(finalPaletteMax);
            queue.Add(initialVBox);

            int targetColors = (int)(fractionByPopulation * (float)finalPaletteMax);
            int iterationCount = 0;
            while (iterationCount <= maxIterations)
            {
                ColorQuantizationBox vbox = queue.Last();
                queue.RemoveAt(queue.Count - 1);

                var cutBoxes = vbox.MedianCut();
                ColorQuantizationBox.InsertIntoSortedList(queue, cutBoxes.Item1, (item) => item.HistogramTotal);
                if (cutBoxes.Item2 != null)
                {
                    ColorQuantizationBox.InsertIntoSortedList(queue, cutBoxes.Item2, (item) => item.HistogramTotal);
                }

                if (CountValidColors(queue, minPopulation) >= targetColors)
                {
                    break;
                }

                iterationCount++;
            }

            queue.Sort((a, b) => (a.HistogramTotal * a.ColorVolume).CompareTo(b.HistogramTotal * b.ColorVolume));

            iterationCount = 0;
            // There is a bug in vibrant.js that stops its loop one early here. Keeping the bug so as to make the final palettes match.
            targetColors = finalPaletteMax - finalPaletteOffset;
            while (iterationCount <= maxIterations)
            {
                ColorQuantizationBox vbox = queue.Last();
                queue.RemoveAt(queue.Count - 1);

                var cutBoxes = vbox.MedianCut();
                ColorQuantizationBox.InsertIntoSortedList(queue, cutBoxes.Item1, (item) => item.HistogramTotal * item.ColorVolume);
                if (cutBoxes.Item2 != null)
                {
                    ColorQuantizationBox.InsertIntoSortedList(queue, cutBoxes.Item2, (item) => item.HistogramTotal * item.ColorVolume);
                }
                if (CountValidColors(queue, minPopulation) >= targetColors)
                {
                    break;
                }

                iterationCount++;
            }

            int returnCount = CountValidColors(queue, minPopulation);
            int returnIndex = 0;
            (uint population, Color color)[] retVal = new (uint population, Color color)[returnCount];
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].HistogramTotal >= minPopulation)
                {
                    retVal[returnIndex] = (queue[i].HistogramTotal, queue[i].ComputeAverageColor());
                    returnIndex++;
                }
            }
            return retVal;
        }
    }
}
