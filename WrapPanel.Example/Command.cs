using System;
using System.Windows.Input;

namespace WrapPanel.Example
{
    public class Command : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        public Command(Predicate<object> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }
        public Command(Action<object> execute) : this((o) => true, execute) { }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) =>
            canExecute(parameter);

        public void Execute(object parameter) =>
            execute(parameter);
    }
}
