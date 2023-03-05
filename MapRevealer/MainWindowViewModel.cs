using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MapRevealer
{
    public class MainWindowViewModel : NotifiableBase
    {
        private static readonly Lazy<MainWindowViewModel> instance = new Lazy<MainWindowViewModel>(() => new MainWindowViewModel());
        public static MainWindowViewModel Instance { get { return instance.Value; } }
        private MainWindowViewModel()
        {
            DisplayImage = new BitmapImage(new Uri("C:\\Users\\evan.f\\Documents\\Personal\\welding amp guide.png"));
        }

        private BitmapSource _displayImage;
        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set{SetAndNotify(ref _displayImage, value);}
        }

        private double _transformX;
        public double TransformX
        {
            get => _transformX;
            set { SetAndNotify(ref _transformX, value); }
        }

        private double _transformY;
        public double TransformY
        {
            get => _transformY;
            set { SetAndNotify(ref _transformY, value); }
        }

        private double _zoomPercentage;
        public double ZoomPercentage
        {
            get => _zoomPercentage;
            set { SetAndNotify(ref _zoomPercentage, value); }
        }

        private bool _isDrawing;
        public bool IsDrawing
        {
            get => _isDrawing;
            set { SetAndNotify(ref _isDrawing, value); }
        }
    }
}
