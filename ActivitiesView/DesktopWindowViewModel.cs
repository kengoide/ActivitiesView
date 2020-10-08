using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActivitiesView {
    class DesktopWindowViewModel {
        private readonly DesktopWindow _window;
        private readonly StatelessCommand _selectCommand;
        private readonly StatelessCommand _closeCommand;

        public IntPtr Hwnd { get => _window.Hwnd; }
        public string Title { get => _window.WindowText; }
        public StatelessCommand SelectCommand { get => _selectCommand; }
        public StatelessCommand CloseCommand { get => _closeCommand; }

        public DesktopWindowViewModel(DesktopWindow window) {
            _window = window;
            _selectCommand = new StatelessCommand(() => {
                _window.BringToForeground();
                Application.Current.MainWindow.Close();
            });
            _closeCommand = new StatelessCommand(() => _window.Close());
        }
    }
}
