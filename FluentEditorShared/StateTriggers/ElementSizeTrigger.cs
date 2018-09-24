// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml;

namespace FluentEditorShared.StateTriggers
{
    public enum ElementSizeTriggerAxis { Width, Height };
    public enum ElementSizeTriggerMode { LessThan, GreaterThan, Equals };

    public class ElementSizeTrigger : StateTriggerBase
    {
        private FrameworkElement _targetElement = null;
        public FrameworkElement TargetElement
        {
            get { return _targetElement; }
            set
            {
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged -= _targetElement_SizeChanged;
                }
                _targetElement = value;
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged += _targetElement_SizeChanged;
                }

                EvaluateTrigger();
            }
        }

        private void _targetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EvaluateTrigger();
        }

        private ElementSizeTriggerAxis _axis = ElementSizeTriggerAxis.Width;
        public ElementSizeTriggerAxis Axis
        {
            get { return _axis; }
            set
            {
                _axis = value;
                EvaluateTrigger();
            }
        }

        private ElementSizeTriggerMode _mode = ElementSizeTriggerMode.LessThan;
        public ElementSizeTriggerMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                EvaluateTrigger();
            }
        }

        private double _targetValue = 0.0;
        public double TargetValue
        {
            get { return _targetValue; }
            set
            {
                _targetValue = value;
                EvaluateTrigger();
            }
        }

        private int _targetValuePrecision = 2;
        public int TargetValuePrecision
        {
            get { return _targetValuePrecision; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("TargetValuePrecision must be a nonnegative integer");
                }
                _targetValuePrecision = value;
                EvaluateTrigger();
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
        }

        private void EvaluateTrigger()
        {
            bool newActive = false;

            if (_targetElement != null)
            {
                if (_axis == ElementSizeTriggerAxis.Width)
                {
                    switch (_mode)
                    {
                        case ElementSizeTriggerMode.Equals:
                            newActive = Math.Round(_targetElement.ActualWidth, _targetValuePrecision) == Math.Round(_targetValue, _targetValuePrecision);
                            break;
                        case ElementSizeTriggerMode.GreaterThan:
                            newActive = Math.Round(_targetElement.ActualWidth, _targetValuePrecision) > Math.Round(_targetValue, _targetValuePrecision);
                            break;
                        case ElementSizeTriggerMode.LessThan:
                            newActive = Math.Round(_targetElement.ActualWidth, _targetValuePrecision) < Math.Round(_targetValue, _targetValuePrecision);
                            break;
                    }
                }
                else
                {
                    switch (_mode)
                    {
                        case ElementSizeTriggerMode.Equals:
                            newActive = Math.Round(_targetElement.ActualHeight, _targetValuePrecision) == Math.Round(_targetValue, _targetValuePrecision);
                            break;
                        case ElementSizeTriggerMode.GreaterThan:
                            newActive = Math.Round(_targetElement.ActualHeight, _targetValuePrecision) > Math.Round(_targetValue, _targetValuePrecision);
                            break;
                        case ElementSizeTriggerMode.LessThan:
                            newActive = Math.Round(_targetElement.ActualHeight, _targetValuePrecision) < Math.Round(_targetValue, _targetValuePrecision);
                            break;
                    }
                }
            }

            if (newActive != _isActive)
            {
                _isActive = newActive;
                SetActive(_isActive);
            }
        }
    }
}
