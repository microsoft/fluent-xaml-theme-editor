// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public class ClippedPixelBlob : IPixelBlob
    {
        #region Helper for loading from a stream

        public static async Task<ClippedPixelBlob> LoadPixelsFromStream(IRandomAccessStream source)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);

            if (decoder.BitmapPixelFormat != BitmapPixelFormat.Rgba8 && decoder.BitmapPixelFormat != BitmapPixelFormat.Bgra8)
            {
                throw new Exception("Unsupported pixel format");
            }

            var pixelData = await decoder.GetPixelDataAsync();
            var pixelBytes = pixelData.DetachPixelData();

            return new ClippedPixelBlob(pixelBytes, decoder.BitmapPixelFormat, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight);
        }

        #endregion

        // The pixelBytes array is never changed by this class
        public ClippedPixelBlob(byte[] pixelBytes, BitmapPixelFormat pixelFormat, uint width, uint height)
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
            _imageWidth = width;
            _imageHeight = height;
            _imagePixels = _imageWidth * _imageHeight;

            SetClip(0, 0, _imageWidth, _imageHeight);
        }

        public ClippedPixelBlob(ClippedPixelBlob source)
        {
            _imageWidth = source._imageWidth;
            _imageHeight = source._imageHeight;
            _imagePixels = source._imagePixels;
            _clipLeft = source._clipLeft;
            _clipTop = source._clipTop;
            _clipWidth = source._clipWidth;
            _clipHeight = source._clipHeight;
            _clipPixels = source._clipPixels;
            _pixelBytes = source._pixelBytes;
            _pixelFormat = source._pixelFormat;
        }

        private readonly uint _imageWidth = 0;
        public uint ImageWidth
        {
            get { return _imageWidth; }
        }

        private readonly uint _imageHeight = 0;
        public uint ImageHeight
        {
            get { return _imageHeight; }
        }

        private readonly uint _imagePixels = 0;
        public uint ImagePixels
        {
            get { return _imagePixels; }
        }

        private uint _clipLeft = 0;
        private uint _clipTop = 0;
        private uint _clipWidth = 0;
        private uint _clipHeight = 0;
        private uint _clipPixels = 0;

        public void SetClip(uint left, uint top, uint width, uint height)
        {
            if (left > _imageWidth)
            {
                left = _imageWidth;
            }
            if (left + width > _imageWidth)
            {
                width = _imageWidth - left;
            }

            if (top > _imageHeight)
            {
                top = _imageHeight;
            }
            if (top + height > _imageHeight)
            {
                height = _imageHeight - top;
            }

            _clipLeft = left;
            _clipTop = top;
            _clipWidth = width;
            _clipHeight = height;

            _clipPixels = _clipWidth * _clipHeight;
        }

        private readonly byte[] _pixelBytes = null;

        private readonly BitmapPixelFormat _pixelFormat = BitmapPixelFormat.Rgba8;
        public BitmapPixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        public uint Width
        {
            get { return _clipWidth; }
        }

        public uint Height
        {
            get { return _clipHeight; }
        }

        public uint TotalPixels
        {
            get { return _clipPixels; }
        }

        public Color GetPixel(uint x, uint y)
        {
            x += _clipLeft;
            y += _clipTop;
            if (x >= _clipLeft + _clipWidth || y >= _clipTop + _clipHeight)
            {
                throw new Exception("Pixel requested outside of bounds");
            }
            uint offset = (y * (_imageWidth << 2)) + (x << 2);
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
            for (uint x = 0; x < _clipWidth; x++)
            {
                for (uint y = 0; y < _clipHeight; y++)
                {
                    action(GetPixel(x, y), x, y);
                }
            }
        }
    }
}
