using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeCollectionneur.Outils
{
    class Commande :ICommand
    {
        readonly Action<object> actionAExecuter;
        private Func<bool> peutEtreExecute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {

            if (peutEtreExecute == null)
            {
                return true;
            }
            else
            {
                
                return peutEtreExecute.Invoke();
            }
        }
        public void Execute(object parameter)
        {
            actionAExecuter(parameter);
        }
        public Commande(Action<object> execute)
        {
            actionAExecuter = execute;
        }
        public Commande(Action<object> execute, Predicate<object> canExecute)
        {
            actionAExecuter = execute;
        }

        public Commande(Action<object> execute, Func<bool> canExecute)
        {
            actionAExecuter = execute;
            peutEtreExecute = canExecute;
        }
    }
}

