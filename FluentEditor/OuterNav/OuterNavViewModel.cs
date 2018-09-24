// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditorShared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentEditor.OuterNav
{
    public class OuterNavViewModel : INotifyPropertyChanged
    {
        public OuterNavViewModel(IReadOnlyList<INavItem> navItems, INavItem selectedNavItem = null)
        {
            if (navItems == null)
            {
                throw new ArgumentNullException("navItems");
            }
            _navItems = new ObservableList<INavItem>(navItems);

            if (selectedNavItem != null)
            {
                if (!_navItems.Contains(selectedNavItem))
                {
                    throw new Exception("selectedNavItem is not contained in navItems");
                }
                _selectedNavItem = selectedNavItem;
            }
        }

        private ObservableList<INavItem> _navItems;
        public ObservableList<INavItem> NavItems
        {
            get { return _navItems; }
        }

        private INavItem _selectedNavItem;
        public INavItem SelectedNavItem
        {
            get { return _selectedNavItem; }
            set
            {
                if (_selectedNavItem != value)
                {
                    _selectedNavItem = value;
                    RaisePropertyChangedFromSource();

                    NavigateToItem?.Invoke(this, _selectedNavItem);
                }
            }
        }

        public event Action<OuterNavViewModel, INavItem> NavigateToItem;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
