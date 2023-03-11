using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapRevealer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DisplayWindow _displayWindow;
        public MainWindow()
        {
            InitializeComponent();
            _displayWindow = new DisplayWindow();
        }

        private void ImageZoomBorder_TransformChanged(object sender, UIAnnotation.TransformEventArgs e)
        {
            var group = new TransformGroup();
            var scale = new ScaleTransform(e.Scale.ScaleX, e.Scale.ScaleY);
            var newLeft = (e.Scale.ScaleX) + e.Translation.X;
            var newTop = (e.Scale.ScaleY) + e.Translation.Y;
            var translate = new TranslateTransform(newLeft, newTop);
            group.Children.Add(scale);
            group.Children.Add(translate);
            _displayWindow.ZoomBorderResponder.RenderTransform = group;
        }

        private void ToggleDisplay_Click(object sender, RoutedEventArgs e)
        {
            var thign = DataContext;
            _displayWindow.Show();
        }

        private void DrawCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            var thing = e.Stroke;
            _displayWindow.ResponseDrawCanvas.Strokes.Add(e.Stroke);
        }

        private void DrawCanvas_StrokeErased(object sender, RoutedEventArgs e)
        {

        }

        private void DrawCanvas_StrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {

        }
    }
}
