// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentEditorShared.Editors
{
    // Sometimes it is easier to handle enums on the ViewModel side of things rather than using EnumEditor as a control. Depends on the situation.
    public class EnumSelectionAdapter : INotifyPropertyChanged
    {
        public EnumSelectionAdapter(Type enumType)
        {
            _enumType = enumType;
            _availableStrings = Enum.GetNames(_enumType);
            _selectedString = _availableStrings[0];
        }

        private string _caption = null;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private readonly Type _enumType = null;
        public Type EnumType
        {
            get { return _enumType; }
        }

        private readonly string[] _availableStrings = null;
        public string[] AvailableStrings
        {
            get { return _availableStrings; }
        }

        private string _selectedString = null;
        public string SelectedString
        {
            get { return _selectedString; }
            set
            {
                if (_selectedString != value)
                {
                    _selectedString = value;
                    RaisePropertyChangedFromSource();

                    SelectionChanged?.Invoke(this);
                }
            }
        }

        public event Action<EnumSelectionAdapter> SelectionChanged;

        public T GetSelectedValue<T>() where T : struct
        {
            T output;
            if (Enum.TryParse<T>(_selectedString, out output))
            {
                return output;
            }
            return default(T);
        }

        public void SetSelectedValue<T>(T value) where T : struct
        {
            if (typeof(T) != _enumType)
            {
                return;
            }
            SelectedString = value.ToString();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(name));
            }
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            RaisePropertyChanged(name);
        }

        #endregion
    }
}
