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

namespace ActivitiesView {
    class MainWindowViewModel {
        private readonly ObservableCollection<DockItem> _dockItems;
        private readonly DesktopWindowTracker _windowTracker;
        private readonly StatelessCommand _closeViewCommand;
        private readonly StatelessCommand _launchCommand;
        private readonly StatelessCommand _selectWindowCommand;
        private readonly StatelessCommand _closeWindowCommand;

        public MainWindowViewModel() {
            _dockItems = new ObservableCollection<DockItem>();
            _windowTracker = new DesktopWindowTracker();
            _closeViewCommand = new StatelessCommand(() => Application.Current.MainWindow.Close());
            _launchCommand = new StatelessCommand((o) => {
                ((DockItem)o).Launch();
                Application.Current.MainWindow.Close();
            });
            _selectWindowCommand = new StatelessCommand((o) => {
                ((DesktopWindow)o).BringToForeground();
                Application.Current.MainWindow.Close();
            });
            _closeWindowCommand = new StatelessCommand((o) => ((DesktopWindow)o).Close());

            StringBuilder buffer = new StringBuilder(Win32.MAX_PATH);
            IEnumerable<string> e;
            try {
                e = Directory.EnumerateFiles("DockShortcuts");
            }
            catch (DirectoryNotFoundException) {
                return;
            }
            foreach (string fileName in e.Where(name => name.EndsWith(".lnk"))) {
                string linkFilePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                Win32.IShellLinkW shellLink = (Win32.IShellLinkW)new Win32.ShellLink();
                Win32.IPersistFile persistFile = (Win32.IPersistFile)shellLink;
                persistFile.Load(linkFilePath, Win32.STGM_READ);
                shellLink.Resolve(IntPtr.Zero, Win32.SLR_NO_UI);
                shellLink.GetPath(buffer, buffer.Capacity, IntPtr.Zero, 0);
                string path = buffer.ToString();
                shellLink.GetArguments(buffer, buffer.Capacity);
                string arguments = buffer.ToString();
                shellLink.GetWorkingDirectory(buffer, buffer.Capacity);
                string workingDirectory = buffer.ToString();
                shellLink.GetDescription(buffer, buffer.Capacity);
                string description = buffer.ToString();
                int showCommand = shellLink.GetShowCmd();
                string displayName = Path.GetFileNameWithoutExtension(linkFilePath);
                _dockItems.Add(new DockItem(path, arguments, workingDirectory, displayName, description, showCommand));
            }
        }

        public ObservableCollection<DockItem> DockItems { get => _dockItems; }
        public DesktopWindowTracker WindowTracker { get => _windowTracker; }
        public StatelessCommand CloseViewCommand { get => _closeViewCommand; }
        public StatelessCommand LaunchCommand { get => _launchCommand; }
        public StatelessCommand SelectWindowCommand { get => _selectWindowCommand; }
        public StatelessCommand CloseWindowCommand { get => _closeWindowCommand; }
    }
}
