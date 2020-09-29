using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ActivitiesView
{
    class MainWindowViewModel
    {
        private ObservableCollection<DockItem> _dockItems;
        private DesktopWindowTracker _windowTracker;
        private readonly StatelessCommand _closeViewCommand;
        private readonly StatelessCommand _launchCommand;
        private readonly StatelessCommand _selectWindowCommand;

        public MainWindowViewModel()
        {
            _dockItems = new ObservableCollection<DockItem>();
            _windowTracker = new DesktopWindowTracker();
            _closeViewCommand = new StatelessCommand(() => Application.Current.MainWindow.Close());
            _launchCommand = new StatelessCommand((o) =>
            {
                ((DockItem)o).Launch();
                Application.Current.MainWindow.Close();
            });
            _selectWindowCommand = new StatelessCommand((o) =>
            {
                ((DesktopWindow)o).BringToForeground();
                Application.Current.MainWindow.Close();
            });

            StringBuilder buffer = new StringBuilder(Win32.MAX_PATH);
            foreach (string fileName in Directory.EnumerateFiles("DockShortcuts")
                                                 .Where(name => name.EndsWith(".lnk")))
            {
                string absoluteFilePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                Win32.IShellLinkW shellLink = (Win32.IShellLinkW)new Win32.ShellLink();
                Win32.IPersistFile persistFile = (Win32.IPersistFile)shellLink;
                persistFile.Load(absoluteFilePath, Win32.STGM_READ);
                shellLink.Resolve(IntPtr.Zero, Win32.SLR_NO_UI);
                shellLink.GetPath(buffer, buffer.Capacity, IntPtr.Zero, 0);
                _dockItems.Add(new DockItem(buffer.ToString()));
            }
        }

        public ObservableCollection<DockItem> DockItems { get => _dockItems; }
        public DesktopWindowTracker WindowTracker { get => _windowTracker; }
        public StatelessCommand CloseViewCommand { get => _closeViewCommand; }
        public StatelessCommand LaunchCommand { get => _launchCommand; }
        public StatelessCommand SelectWindowCommand { get => _selectWindowCommand; }
    }
}
