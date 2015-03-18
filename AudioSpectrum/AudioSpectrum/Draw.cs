using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;

namespace AudioSpectrum
{
    public class DrawingElement : FrameworkElement
    {
        static readonly TranslateTransform tt_cache = new TranslateTransform();

        private DrawingImage di;
        private Rect bounds;
        private DrawingContext dcc;
        public DrawingElement()
        {
            di = new DrawingImage();
            bounds = new Rect(0, 0, this.Width, this.Height);
            this.drawing = new ImageDrawing(di, bounds);
           
        }
        readonly Drawing drawing;
        private int x = 0;
        private int cellw = 3;
        private int cellh = 3;
        private Brush bb;
        private System.Windows.Media.Color c;
        private byte[] bbb, ggg, rrr;
        //public void DrawData(byte[] r, byte[] g, byte[] b)
        //{
        //    bbb = b;
        //    ggg = g;
        //    rrr = r;
            
        //}
        TranslateTransform get_transform()
        {
            if (Margin.Left == 0 && Margin.Top == 0)
                return null;
            tt_cache.X = Margin.Left;
            tt_cache.Y = Margin.Top;
            return tt_cache;
        }
        protected override Size MeasureOverride(Size _)
        {
            var sz = drawing.Bounds.Size;
            return new Size
            {
                Width = sz.Width + Margin.Left + Margin.Right,
                Height = sz.Height + Margin.Top + Margin.Bottom,
            };
        }
        protected override void OnRender(DrawingContext dc)
        {
            var tt = get_transform();
            if (tt != null)
                dc.PushTransform(tt);
            dc.DrawDrawing(drawing);
            //dc.DrawRectangle(Brushes.Black , new System.Windows.Media.Pen(), new Rect(0, 0, this.Width , this.Height));

            if (rrr!=null)
            {
                cellh = (int)Math.Ceiling(this.Height / rrr.Length);
                for (int i = 0; i < rrr.Length; i++)
                {
                    c = System.Windows.Media.Color.FromRgb(rrr[i], ggg[i], bbb[i]);
                    bb = new SolidColorBrush(c);
                    dc.DrawRectangle(bb, new Pen(), new Rect(x, i * cellh, cellw, cellh));
                }
                if (x < this.Width)
                {
                    x += cellw;
                }
            }
            dcc = dc;
            
            if (tt != null)
                dc.Pop();
        }
    };
}
