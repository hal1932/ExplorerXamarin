using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
                    _history.SetNext(new DirectoryInfo(value));
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

        #region RootDirectoryNodes
        private DirectoryNode[] _RootDirectoryNodes;
        public DirectoryNode[] RootDirectoryNodes
        {
            get { return _RootDirectoryNodes; }
            set { SetProperty(ref _RootDirectoryNodes, value); }
        }
        #endregion

        #region FileViewType
        private string _FileViewType;
        public string FileViewType
        {
            get { return _FileViewType; }
            set { SetProperty(ref _FileViewType, value); }
        }
        #endregion

        #region FileViewModel
        private FileViewModel _FileViewModel;
        public FileViewModel FileViewModel
        {
            get { return _FileViewModel; }
            set { SetProperty(ref _FileViewModel, value); }
        }
        #endregion


        public MainWindowViewModel()
        {
            RootDirectoryNodes = DriveInfo.GetDrives()
                .Where(x => x.IsReady)
                .Select(x => new DirectoryNode(x.Name, node =>
                {
                    _history.SetNext(new DirectoryInfo(node.FullName));
                    UpdateDirectoryPath(node.FullName, true);
                }))
                .ToArray();

            FileViewModel = new FileViewModel();
            FileViewModel.DirectoryChanged += (s, e) => DirectoryPath = e;

            SwitchView("list");
        }

        private void GoBackward()
        {
            var next = _history.GoBack();
            UpdateDirectoryPath(next.FullName, true);
        }

        private bool EnableGoBackward()
        {
            return _history.EnableGoingBack;
        }

        private void GoForward()
        {
            var next = _history.GoForward();
            UpdateDirectoryPath(next.FullName, true);
        }

        private bool EnableGoForward()
        {
            return _history.EnableGoingForward;
        }

        private void GoUp()
        {
            var next = _history.GoUp();
            UpdateDirectoryPath(next.FullName, true);
        }

        private bool EnableGoUp()
        {
            return _history.EnableGoingUp;
        }

        private void SwitchView(string view)
        {
            FileViewType = view;
        }

        private void UpdateHistoryTransitionStatus()
        {
            GoBackwardCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
            GoUpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateDirectoryPath(string path, bool updateProperty = false)
        {
            FileViewModel.ChangeDirectory(path);
            UpdateHistoryTransitionStatus();

            if (updateProperty)
            {
                _DirectoryPath = path;
                OnPropertyChanged(nameof(DirectoryPath));
            }
        }

        private void UpdateFilter(string filter)
        {
            FileViewModel.ApplyFilter(filter);
        }

        private DirectoryHistory _history = new DirectoryHistory();
    }
}
