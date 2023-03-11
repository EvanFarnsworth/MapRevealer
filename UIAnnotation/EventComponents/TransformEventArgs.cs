using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UIAnnotation
{
    public delegate void TransformEventHandler(object sender, TransformEventArgs e);
    public class TransformEventArgs : EventArgs
    {
        public TransformEventArgs(ScaleTransform st, TranslateTransform tt)
        {
            Translation = tt;
            Scale = st;
        }

        public TranslateTransform Translation { get; private set; }
        public ScaleTransform Scale { get; private set; }
    }
}
