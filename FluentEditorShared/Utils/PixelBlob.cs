// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public class PixelBlob : IPixelBlob
    {
        #region Helper for loading from a stream

        public static async Task<PixelBlob> LoadPixelsFromStream(IRandomAccessStream source)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);

            if (decoder.BitmapPixelFormat != BitmapPixelFormat.Rgba8 && decoder.BitmapPixelFormat != BitmapPixelFormat.Bgra8)
            {
                throw new Exception("Unsupported pixel format");
            }

            var pixelData = await decoder.GetPixelDataAsync();
            var pixelBytes = pixelData.DetachPixelData();

            return new PixelBlob(pixelBytes, decoder.BitmapPixelFormat, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight);
        }

        #endregion

        // The pixelBytes array is never changed by this class
        public PixelBlob(byte[] pixelBytes, BitmapPixelFormat pixelFormat, uint width, uint height)
        {
            if (pixelFormat != BitmapPixelFormat.Rgba8 && pixelFormat != BitmapPixelFormat.Bgra8)
            {
                throw new Exception("Unsupported pixel format");
            }
            if ((long)width * (long)height * 4L >= (long)int.MaxValue)
            {
                throw new Exception("Image too large");
            }
            uint expectedSize = (width * height) * 4;
            if (pixelBytes.Length != expectedSize)
            {
                throw new Exception("Pixel array length does not match dimensions");
            }
            _pixelBytes = pixelBytes;
            _pixelFormat = pixelFormat;
            _width = width;
            _height = height;
            _totalPixels = _width * _height;
        }

        private readonly byte[] _pixelBytes = null;

        private readonly BitmapPixelFormat _pixelFormat = BitmapPixelFormat.Rgba8;
        public BitmapPixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        private readonly uint _width = 0;
        public uint Width
        {
            get { return _width; }
        }

        private readonly uint _height = 0;
        public uint Height
        {
            get { return _height; }
        }

        private readonly uint _totalPixels = 0;
        public uint TotalPixels
        {
            get { return _totalPixels; }
        }

        public Color GetPixel(uint x, uint y)
        {
            if (x >= _width || y >= _height)
            {
                throw new Exception("Pixel requested outside of bounds");
            }
            uint offset = (y * (_width << 2)) + (x << 2);
            if (_pixelFormat == BitmapPixelFormat.Bgra8)
            {
                return Color.FromArgb(_pixelBytes[offset + 3], _pixelBytes[offset + 2], _pixelBytes[offset + 1], _pixelBytes[offset]);
            }
            else
            {
                return Color.FromArgb(_pixelBytes[offset + 3], _pixelBytes[offset], _pixelBytes[offset + 1], _pixelBytes[offset + 2]);
            }
        }

        public void PerPixelAction(Action<Color, uint, uint> action)
        {
            for (uint y = 0; y < _height; y++)
            {
                for (uint x = 0; x < _height; x++)
                {
                    action(GetPixel(x, y), x, y);
                }
            }
        }
    }
}
