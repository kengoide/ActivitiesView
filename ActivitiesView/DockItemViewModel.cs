using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ActivitiesView {
    class DockItemViewModel : DependencyObject {
        private readonly DockItem _item;
        private readonly StatelessCommand _launchCommand;

        private static readonly DependencyPropertyKey ImagePropertyKey =
            DependencyProperty.RegisterReadOnly("Image", typeof(BitmapSource), typeof(DockItemViewModel), new PropertyMetadata());
        public static readonly DependencyProperty ImageProperty = ImagePropertyKey.DependencyProperty;

        public StatelessCommand LaunchCommand { get => _launchCommand; }

        public string Name { get => _item.DisplayName; }
        public string Description { get => _item.Description; }
        public BitmapSource Image { get => (BitmapSource)GetValue(ImageProperty); }

        public DockItemViewModel(DockItem item) : base() {
            _item = item;
            _launchCommand = new StatelessCommand(() => {
                _item.Launch();
                Application.Current.MainWindow.Close();
            });
            LoadImage();
        }

        private async void LoadImage() {
            BitmapSource bitmap = await _item.LoadImageAsync();
            SetValue(ImagePropertyKey, bitmap);
        }
    }
}
