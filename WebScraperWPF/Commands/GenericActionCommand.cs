using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebScraperWPF.Commands
{
    public class GenericActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action Action
        {
            private set;
            get;
        }
        public GenericActionCommand(Action actionToExecute)
        {
            Action = actionToExecute ?? throw new Exception("Action cannot be null");
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action();
        }
    }

    public class GenericActionCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<T> Action
        {
            private set;
            get;
        }
        public GenericActionCommand(Action<T> actionToExecute)
        {
            Action = actionToExecute ?? throw new Exception("Action cannot be null");
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action((T) parameter);
        }
    }
}
