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
            _dockItems = new ObservableCollection<DockItem>(DockItemsLoader.Load());
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
        }

        public ObservableCollection<DockItem> DockItems { get => _dockItems; }
        public DesktopWindowTracker WindowTracker { get => _windowTracker; }
        public StatelessCommand CloseViewCommand { get => _closeViewCommand; }
        public StatelessCommand LaunchCommand { get => _launchCommand; }
        public StatelessCommand SelectWindowCommand { get => _selectWindowCommand; }
        public StatelessCommand CloseWindowCommand { get => _closeWindowCommand; }
    }
}
