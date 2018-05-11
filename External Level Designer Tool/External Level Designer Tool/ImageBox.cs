using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace External_Level_Designer_Tool
{
    public class ImageBox : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }
    }

    public class VertButton : Button
    { 
        public string VerticalText { get; set; }

        StringFormat fmt = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            PointF spot = new PointF();
            pe.Graphics.TranslateTransform(Width, 0);
            pe.Graphics.RotateTransform(90);
            pe.Graphics.DrawString(VerticalText, DefaultFont, System.Drawing.Brushes.Black, spot);
        }
    }
}