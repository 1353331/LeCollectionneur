using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeCollectionneur.Outils
{
    class Commande : ICommand
    {
        readonly Action<object> actionAExecuter;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            actionAExecuter(parameter);
        }
        public Commande(Action<object> execute) : this(execute, null) { }
        public Commande(Action<object> execute, Predicate<object> canExecute)
        {
            actionAExecuter = execute;
        }
    }
}
