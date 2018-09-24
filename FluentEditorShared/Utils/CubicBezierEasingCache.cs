// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.Foundation;

namespace FluentEditorShared.Utils
{
    public class CubicBezierEasingCache
    {
        public CubicBezierEasingCache(Point control1, Point control2)
        {
            _control1 = control1;
            _control2 = control2;

            RebuildCache();
        }

        public CubicBezierEasingCache(double x1, double y1, double x2, double y2)
        {
            _control1 = new Point(x1, y1);
            _control2 = new Point(x2, y2);

            RebuildCache();
        }

        private Point _control1;
        public Point Control1
        {
            get { return _control1; }
            set
            {
                if (_control1 != value)
                {
                    _control1 = value;
                    RebuildCache();
                }
            }
        }

        private Point _control2;
        public Point Control2
        {
            get { return _control2; }
            set
            {
                if (_control2 != value)
                {
                    _control2 = value;
                    RebuildCache();
                }
            }
        }

        public double GetValueImmediate(double x)
        {
            return MathUtils.CubicBezierEasing(x, _control1, _control2);
        }

        public double GetValue(double x)
        {
            if (x < _cacheMin || x > _cacheMax)
            {
                return GetValueImmediate(x);
            }

            int index = (int)Math.Round(((x - _cacheMin) / _cacheWidth) * (double)(_cacheResolution - 1));
            return _cache[index];
        }

        private void RebuildCache()
        {
            _cacheWidth = Math.Abs(_cacheMax - _cacheMin);

            _cache = new double[_cacheResolution];
            for (int i = 0; i < _cacheResolution; i++)
            {
                _cache[i] = GetValueImmediate(((double)i / (double)(_cacheResolution - 1)) * _cacheWidth + _cacheMin);
            }
        }

        private double[] _cache = null;
        private double _cacheWidth = 1;

        private int _cacheResolution = 100;
        public int CacheResolution
        {
            get { return _cacheResolution; }
            set
            {
                if (_cacheResolution != value)
                {
                    _cacheResolution = value;
                    RebuildCache();
                }
            }
        }

        private double _cacheMin = 0;
        public double CacheMin
        {
            get { return _cacheMin; }
            set
            {
                if (_cacheMin != value)
                {
                    _cacheMin = value;
                    RebuildCache();
                }
            }
        }

        private double _cacheMax = 1;
        public double CacheMax
        {
            get { return _cacheMax; }
            set
            {
                if (_cacheMax != value)
                {
                    _cacheMax = value;
                    RebuildCache();
                }
            }
        }
    }
}
