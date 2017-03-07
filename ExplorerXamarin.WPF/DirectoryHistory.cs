using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerXamarin.WPF
{
    class DirectoryHistory
    {
        public FileSystemInfo Current { get { return (_currentIndex > 0) ? _items[_currentIndex - 1] : null; } }

        public bool EnableGoingForward { get { return _currentIndex < _items.Count; } }
        public bool EnableGoingBack { get { return _currentIndex > 1; } }
        public bool EnableGoingUp
        {
            get
            {
                if (Current == null)
                {
                    return false;
                }
                var parent = Path.GetDirectoryName(Current.FullName);
                return (parent != null) && (parent != Current.FullName);
            }
        }

        public FileSystemInfo SetNext(FileSystemInfo info)
        {
            if (_currentIndex < _items.Count)
            {
                _items.RemoveRange(_currentIndex - 1, _items.Count - _currentIndex);
            }

            _items.Add(info);
            _currentIndex++;

            return info;
        }

        public FileSystemInfo GoForward()
        {
            _currentIndex++;
            return _items[_currentIndex - 1];
        }

        public FileSystemInfo GoBack()
        {
            _currentIndex--;
            return _items[_currentIndex - 1];
        }

        public FileSystemInfo GoUp()
        {
            var current = _items[_currentIndex - 1];
            var next = new DirectoryInfo(Path.GetDirectoryName(current.FullName));
            return SetNext(next);
        }

        private List<FileSystemInfo> _items = new List<FileSystemInfo>();
        private int _currentIndex = 0;
    }
}
