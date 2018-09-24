// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;

namespace FluentEditorShared.StateTriggers
{
    public enum ElementSizeRangeTriggerBound { Closed, Open };

    public class ElementSizeRangeTrigger : StateTriggerBase
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

        private ElementSizeRangeTriggerBound _lowerBound = ElementSizeRangeTriggerBound.Closed;
        public ElementSizeRangeTriggerBound LowerBound
        {
            get { return _lowerBound; }
            set
            {
                _lowerBound = value;
                EvaluateTrigger();
            }
        }

        private ElementSizeRangeTriggerBound _upperBound = ElementSizeRangeTriggerBound.Closed;
        public ElementSizeRangeTriggerBound UpperBound
        {
            get { return _upperBound; }
            set
            {
                _upperBound = value;
                EvaluateTrigger();
            }
        }

        private double _lowerValue = 0.0;
        public double LowerValue
        {
            get { return _lowerValue; }
            set
            {
                _lowerValue = value;
                EvaluateTrigger();
            }
        }

        private double _upperValue = 100.0;
        public double UpperValue
        {
            get { return _upperValue; }
            set
            {
                _upperValue = value;
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
            if (_lowerValue > _upperValue)
            {
                return;
            }

            bool newActive = true;

            if (_targetElement != null)
            {
                double currentValue = 0.0;
                if (_axis == ElementSizeTriggerAxis.Width)
                {
                    currentValue = _targetElement.ActualWidth;
                }
                else
                {
                    currentValue = _targetElement.ActualHeight;
                }
                if (double.IsNaN(currentValue))
                {
                    return;
                }

                if (_lowerBound == ElementSizeRangeTriggerBound.Closed)
                {
                    if (currentValue < _lowerValue)
                    {
                        newActive = false;
                    }
                }
                else
                {
                    if (currentValue <= _lowerValue)
                    {
                        newActive = false;
                    }
                }
                if (_upperBound == ElementSizeRangeTriggerBound.Closed)
                {
                    if (currentValue > _upperValue)
                    {
                        newActive = false;
                    }
                }
                else
                {
                    if (currentValue >= _upperValue)
                    {
                        newActive = false;
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
