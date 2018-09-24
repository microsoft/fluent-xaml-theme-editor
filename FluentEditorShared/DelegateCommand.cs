// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Windows.Input;

namespace FluentEditorShared
{
    // Not thread safe, but allows changing the delegate
    public class DelegateCommand : ICommand
    {
        public DelegateCommand()
        { }

        public DelegateCommand(Action<object> del)
        {
            _delegate = del;
        }

        private Action<object> _delegate = null;
        public Action<object> Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                if (_delegate != null)
                {
                    _delegate = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        #region ICommand

        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return _delegate != null;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _delegate(parameter);
            }
        }

        #endregion
    }

    // Immutable, this is thread safe though the delegate it points to might not be
    public class ImmutableDelegateCommand : ICommand
    {
        public ImmutableDelegateCommand(Action<object> del)
        {
            if (del == null)
            {
                throw new ArgumentNullException("del");
            }
            _delegate = del;
        }

        private readonly Action<object> _delegate = null;
        public Action<object> Delegate
        {
            get { return _delegate; }
        }

        #region ICommand

        // This must be included for the ICommand interface but is never used in an ImmutableDelegateCommand
        #pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _delegate(parameter);
        }

        #endregion
    }
}

