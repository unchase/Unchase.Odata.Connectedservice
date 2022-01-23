using System;
using System.Windows.Controls;
using System.Windows.Input;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Commands
{
    internal class StackPanelChangeVisibilityCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (CanExecuteChanged != null)
                (parameter as StackPanel)?.ChangeStackPanelVisibility();
        }

        public event EventHandler CanExecuteChanged;
    }
}
