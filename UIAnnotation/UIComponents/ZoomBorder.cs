using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace UIAnnotation
{
    /// <summary>
    /// Class that extends the WPF to allow panning and zooming
    /// https://stackoverflow.com/a/6782715/6552757
    /// </summary>
    public class ZoomBorder : Border, IDisposable
    {
        private UIElement? child = null;
        private Point origin;
        private Point start;
        private bool hasTranslated = false;

        public event TransformEventHandler? TransformChanged;

        public void Initialize(UIElement element)
        {
            child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += Child_MouseWheel;
                MouseLeftButtonDown += Child_MouseLeftButtonDown;
                MouseLeftButtonUp += Child_MouseLeftButtonUp;
                MouseMove += Child_MouseMove;
                PreviewMouseRightButtonDown += Child_PreviewMouseRightButtonDown;
            }
            ClipToBounds = true; //there is no reason for this to ever be false
            ResetZoomCommand = new RelayCommand(_ => Reset(), _ => ZoomPercentage != 100 || hasTranslated, nameof(ResetZoomCommand));
        }

        public void Dispose()
        {
            //EventHelper.RemoveAllEventHandlers(this);
            Child = null;
            MouseWheel -= Child_MouseWheel;
            MouseLeftButtonDown -= Child_MouseLeftButtonDown;
            MouseLeftButtonUp -= Child_MouseLeftButtonUp;
            MouseMove -= Child_MouseMove;
            PreviewMouseRightButtonDown -= Child_PreviewMouseRightButtonDown;
        }

        public int ZoomPercentage 
        { 
            get { return (int)GetValue(ZoomPercentageProperty); }
            set 
            { 
                SetValue(ZoomPercentageProperty, value);
                CommandManager.InvalidateRequerySuggested();
            } 
        }

        public static readonly DependencyProperty ZoomPercentageProperty = DependencyProperty.Register("ZoomPercentage", 
                                                                                                       typeof(int), 
                                                                                                       typeof(ZoomBorder), 
                                                                                                       new PropertyMetadata(100));
        public int ZoomMinimum
        {
            get { return (int)GetValue(ZoomMinimumProperty); }
            set { SetValue(ZoomMinimumProperty, value); }
        }

        public static readonly DependencyProperty ZoomMinimumProperty = DependencyProperty.Register("ZoomMinimum",
                                                                                                       typeof(int),
                                                                                                       typeof(ZoomBorder),
                                                                                                       new PropertyMetadata(100));
        public int ZoomMaximum
        {
            get { return (int)GetValue(ZoomMaximumProperty); }
            set { SetValue(ZoomMaximumProperty, value); }
        }

        public static readonly DependencyProperty ZoomMaximumProperty = DependencyProperty.Register("ZoomMaximum",
                                                                                                       typeof(int),
                                                                                                       typeof(ZoomBorder),
                                                                                                       new PropertyMetadata(1000));

        public bool IsDisableZoomingBorder
        {
            get { return (bool)GetValue(IsDisableZoomingBorderProperty); }
            set { SetValue(IsDisableZoomingBorderProperty, value); }
        }

        public static readonly DependencyProperty IsDisableZoomingBorderProperty = DependencyProperty.Register("IsDisableZoomingBorder",
                                                                                                       typeof(bool),
                                                                                                       typeof(ZoomBorder),
                                                                                                       new PropertyMetadata(false));
        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement? Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;
                ZoomPercentage = 100;
                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
                hasTranslated = false;
                TransformChanged?.Invoke(this, new TransformEventArgs(st, tt));
            }
        }

        public RelayCommand? ResetZoomCommand { get; set; }
        #region Child Events

        private void Child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null && !IsDisableZoomingBorder)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (e.Delta < 0 && (st.ScaleX + zoom < ZoomMinimum / 100.0 || st.ScaleY + zoom < ZoomMinimum / 100.0))
                    return;
                if (e.Delta > 0 && (st.ScaleX + zoom > ZoomMaximum / 100.0 || st.ScaleY + zoom > ZoomMaximum / 100.0))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;
                st.ScaleX = Math.Round(st.ScaleX, 2);
                st.ScaleY = Math.Round(st.ScaleY, 2);
                ZoomPercentage = (int)(st.ScaleX * 100.0);

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
                TransformChanged?.Invoke(this, new TransformEventArgs(st, tt));
            }
        }

        private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null && !IsDisableZoomingBorder)
            {
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                child.CaptureMouse();
            }
        }

        private void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null && !IsDisableZoomingBorder)
            {
                child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
                hasTranslated = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void Child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!IsDisableZoomingBorder)
                Reset();
        }

        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null && !IsDisableZoomingBorder)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                    TransformChanged?.Invoke(this, new TransformEventArgs(GetScaleTransform(child), tt));
                }
            }
        }
        #endregion
    }
}
