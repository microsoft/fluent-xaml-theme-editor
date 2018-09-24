// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public static class BitmapUtils
    {
        public static (double min, double average, double max)[] ComputeContrastRatioForImage(IPixelBlob pixels, Color[] testColors)
        {
            (double min, double average, double max)[] retVal = new(double min, double average, double max)[testColors.Length];
            for (int i = 0; i < testColors.Length; i++)
            {
                retVal[i].min = double.MaxValue;
            }
            double count = 0;
            pixels.PerPixelAction((c, x, y) =>
            {
                count++;
                for (int i = 0; i < testColors.Length; i++)
                {
                    var ratio = ColorUtils.ContrastRatio(c, testColors[i], false);
                    retVal[i].average += ratio;
                    retVal[i].min = Math.Min(retVal[i].min, ratio);
                    retVal[i].max = Math.Max(retVal[i].max, ratio);
                }
            });
            for (int i = 0; i < testColors.Length; i++)
            {
                retVal[i].average /= count;
            }
            return retVal;
        }
    }
}
