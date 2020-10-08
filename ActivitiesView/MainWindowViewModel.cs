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
        private readonly ObservableCollection<DockItemViewModel> _dockItemViewModels;
        private readonly DesktopWindowTracker _windowTracker;
        private readonly StatelessCommand _closeViewCommand;
        private readonly StatelessCommand _selectWindowCommand;
        private readonly StatelessCommand _closeWindowCommand;

        public MainWindowViewModel() {
            _dockItemViewModels = new ObservableCollection<DockItemViewModel>(
                DockItemsLoader.Load().Select((item) => new DockItemViewModel(item)));
            _windowTracker = new DesktopWindowTracker();
            _closeViewCommand = new StatelessCommand(() => Application.Current.MainWindow.Close());
            _selectWindowCommand = new StatelessCommand((o) => {
                ((DesktopWindow)o).BringToForeground();
                Application.Current.MainWindow.Close();
            });
            _closeWindowCommand = new StatelessCommand((o) => ((DesktopWindow)o).Close());
        }

        public ObservableCollection<DockItemViewModel> DockItems { get => _dockItemViewModels; }
        public DesktopWindowTracker WindowTracker { get => _windowTracker; }
        public StatelessCommand CloseViewCommand { get => _closeViewCommand; }
        public StatelessCommand SelectWindowCommand { get => _selectWindowCommand; }
        public StatelessCommand CloseWindowCommand { get => _closeWindowCommand; }
    }
}
