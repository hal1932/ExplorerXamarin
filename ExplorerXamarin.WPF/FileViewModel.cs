using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExplorerXamarin.WPF
{
    class FileItem
    {
        public FileSystemInfo Info { get; private set; }
        public BitmapSource IconSource { get; set; }

        public FileItem(FileViewModel parent, FileSystemInfo info, BitmapSource icon)
        {
            Info = info;
            IconSource = icon;
            _parent = parent;
        }

        public void Open()
        {
            if ((Info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                _parent.RaiseDirectoryChanged(Info.FullName);
            }
        }

        private FileViewModel _parent;
    }

    class FileViewModel : Bindable
    {
        public event EventHandler<string> DirectoryChanged;

        #region FileItems
        private ObservableCollection<FileItem> _FileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return _FileItems; }
            set { SetProperty(ref _FileItems, value); }
        }
        #endregion

        public void RaiseDirectoryChanged(string path)
        {
            DirectoryChanged?.Invoke(this, path);
        }

        public void ChangeDirectory(string path)
        {
            ClearItems();

            var entries = new List<FileSystemInfo>();

            try
            {
                entries.AddRange(Directory.EnumerateDirectories(path).Select(x => new DirectoryInfo(x)));
                entries.AddRange(Directory.EnumerateFiles(path).Select(x => new FileInfo(x)));
            }
            catch(UnauthorizedAccessException)
            {
                return;
            }

            _baseItems = entries.Select(x => new FileItem(this, x, null)).ToArray();
            FileItems = new ObservableCollection<FileItem>(_baseItems);

            if (_loadTask != null)
            {
                _loadCancelTokenSource.Cancel();
                try
                {
                    _loadTask.Wait();
                }
                catch (AggregateException e)
                { }
            }

            _loadCancelTokenSource = new CancellationTokenSource();

            _loadTask = Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < FileItems.Count; ++i)
                {
                    var item = _baseItems[i];

                    if (_imageExts.Contains(Path.GetExtension(item.Info.Name)))
                    {
                        var itemIndex = i;
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var thumbnail = new BitmapImage(new Uri(item.Info.FullName));
                            thumbnail.Freeze();
                            _baseItems[itemIndex] = new FileItem(this, item.Info, thumbnail);
                            FileItems[itemIndex] = _baseItems[itemIndex];
                        }));
                    }
                    else
                    {
                        var icon = IconHelper.CreateIcon(item.Info);

                        var itemIndex = i;
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var iconSource = IconHelper.CreateBitmapSourceFromIcon(icon);
                            _baseItems[itemIndex] = new FileItem(this, item.Info, iconSource);
                            FileItems[itemIndex] = _baseItems[itemIndex];
                        }));
                    }
                }
            },
            _loadCancelTokenSource.Token);
        }

        public void ApplyFilter(string filter)
        {
            if (_baseItems == null)
            {
                return;
            }

            IEnumerable<FileItem> newItems;
            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                newItems = _baseItems.Where(x => x.Info.Name.ToLower().Contains(filter));
            }
            else
            {
                newItems = _baseItems;
            }
            FileItems = new ObservableCollection<FileItem>(newItems);
        }

        private void ClearItems()
        {
            _baseItems = null;

            if (FileItems != null)
            {
                FileItems.Clear();
            }

            GC.Collect();
        }

        private FileItem[] _baseItems;

        private Task _loadTask;
        private CancellationTokenSource _loadCancelTokenSource;

        private static string[] _imageExts = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
    }
}
