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
        private readonly ObservableCollection<DesktopWindowViewModel> _desktopWindowViewModels;
        private readonly DesktopWindowTracker _windowTracker;
        private readonly StatelessCommand _closeViewCommand;

        public ObservableCollection<DockItemViewModel> DockItems { get => _dockItemViewModels; }
        public ObservableCollection<DesktopWindowViewModel> Windows { get => _desktopWindowViewModels; }
        public StatelessCommand CloseViewCommand { get => _closeViewCommand; }

        public MainWindowViewModel() {
            _dockItemViewModels = new ObservableCollection<DockItemViewModel>(
                DockItemsLoader.Load().Select((item) => new DockItemViewModel(item)));
            _windowTracker = new DesktopWindowTracker();
            _desktopWindowViewModels = new ObservableCollection<DesktopWindowViewModel>(
                _windowTracker.Windows.Select((window) => new DesktopWindowViewModel(window)));
            _closeViewCommand = new StatelessCommand(() => Application.Current.MainWindow.Close());
        }
    }
}
