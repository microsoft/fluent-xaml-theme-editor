// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public interface IPixelBlob
    {
        BitmapPixelFormat PixelFormat { get; }
        uint Width { get; }
        uint Height { get; }
        uint TotalPixels { get; }

        Color GetPixel(uint x, uint y);
        void PerPixelAction(Action<Color, uint, uint> action);
    }
}
