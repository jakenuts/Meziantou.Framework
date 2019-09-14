﻿using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Meziantou.Framework.WPF
{
    internal sealed class AsyncDelegateCommand : IDelegateCommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Dispatcher _dispatcher;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged;

        public AsyncDelegateCommand(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object parameter)
        {
            if (_isExecuting)
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute?.Invoke(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }

        }

        public void RaiseCanExecuteChanged()
        {
            if (_dispatcher != null)
            {
                _dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}