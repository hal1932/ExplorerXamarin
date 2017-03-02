using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerXamarin.WPF
{
    class MainWindowViewModel : Bindable
    {
        #region GoBackwardCommand
        private DelegateCommand _GoBackwardCommand;
        public DelegateCommand GoBackwardCommand
        {
            get
            {
                return _GoBackwardCommand ?? (_GoBackwardCommand = new DelegateCommand(GoBackward, EnableGoBackward));
            }
        }
        #endregion

        #region GoForwardCommand
        private DelegateCommand _GoForwardCommand;
        public DelegateCommand GoForwardCommand
        {
            get { return _GoForwardCommand ?? (_GoForwardCommand = new DelegateCommand(GoForward, EnableGoForward)); }
        }
        #endregion

        #region GoUpCommand
        private DelegateCommand _GoUpCommand;
        public DelegateCommand GoUpCommand
        {
            get { return _GoUpCommand ?? (_GoUpCommand = new DelegateCommand(GoUp, EnableGoUp)); }
        }
        #endregion

        #region SwitchViewCommand
        private DelegateCommand<string> _SwitchViewCommand;
        public DelegateCommand<string> SwitchViewCommand
        {
            get { return _SwitchViewCommand ?? (_SwitchViewCommand = new DelegateCommand<string>(arg => SwitchView(arg))); }
        }
        #endregion

        #region DirectoryPath
        private string _DirectoryPath;
        public string DirectoryPath
        {
            get { return _DirectoryPath; }
            set
            {
                if (SetProperty(ref _DirectoryPath, value))
                {
                    UpdateDirectoryPath(value);
                }
            }
        }
        #endregion

        #region SearchFilter
        private string _SearchFilter;
        public string SearchFilter
        {
            get { return _SearchFilter; }
            set
            {
                if (SetProperty(ref _SearchFilter, value))
                {
                    UpdateFilter(value);
                }
            }
        }
        #endregion

        private void GoBackward()
        {
            Console.WriteLine("back");
            UpdateHistoryTransitionStatus();
        }

        private bool EnableGoBackward()
        {
            return true;
        }

        private void GoForward()
        {
            Console.WriteLine("forward");
            UpdateHistoryTransitionStatus();
        }

        private bool EnableGoForward()
        {
            return true;
        }

        private void GoUp()
        {
            Console.WriteLine("up");
            UpdateHistoryTransitionStatus();
        }

        private bool EnableGoUp()
        {
            return true;
        }

        private void SwitchView(string view)
        {
            Console.WriteLine(view);
        }

        private void UpdateHistoryTransitionStatus()
        {
            GoBackwardCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
            GoUpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateDirectoryPath(string path)
        {
            UpdateHistoryTransitionStatus();
        }

        private void UpdateFilter(string filter)
        {
        }
    }
}
