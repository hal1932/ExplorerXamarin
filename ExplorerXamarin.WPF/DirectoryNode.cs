using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerXamarin.WPF
{
    class DirectoryNode : Bindable
    {
        public string Name { get; private set; }

        #region FullName
        private string _FullName;
        public string FullName
        {
            get { return _FullName; }
            set
            {
                if (SetProperty(ref _FullName, value))
                {
                    ApplyPathUpdate(value);
                }
            }
        }
        #endregion

        #region Children
        private DirectoryNode[] _Children;
        public DirectoryNode[] Children
        {
            get { return _Children; }
            set { SetProperty(ref _Children, value); }
        }
        #endregion


        #region IsExpanded
        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (SetProperty(ref _IsExpanded, value))
                {
                    Expand();
                }
            }
        }
        #endregion

        #region IsSelected
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value))
                {
                    if (value)
                    {
                        _onSelected(this);
                    }
                }
            }
        }
        #endregion

        public DirectoryNode(string path, Action<DirectoryNode> onSelected)
        {
            if (path == null)
            {
                return;
            }

            FullName = path;
            ApplyPathUpdate(path);
            _onSelected = onSelected;

            try
            {
                if (Directory.EnumerateDirectories(path).Any())
                {
                    Children = new DirectoryNode[] { new DirectoryNode(null, null) };
                }
            }
            catch (UnauthorizedAccessException)
            { }
        }

        private void ApplyPathUpdate(string path)
        {
            var name = Path.GetFileName(path);
            if (name == string.Empty)
            {
                name = path;
            }
            Name = name;
        }

        private void Expand()
        {
            Children = Directory.EnumerateDirectories(FullName)
                .Select(x => new DirectoryNode(x, _onSelected))
                .ToArray();
        }

        private Action<DirectoryNode> _onSelected;
    }
}
