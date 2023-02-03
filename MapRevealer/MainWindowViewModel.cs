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
        public MainWindowViewModel()
        {
            DisplayImage = new BitmapImage(new Uri("C:\\Users\\evan.f\\Documents\\Personal\\welding amp guide.png"));
        }

        private BitmapSource _displayImage;
        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set{SetAndNotify(ref _displayImage, value);}
        }
    }
}
