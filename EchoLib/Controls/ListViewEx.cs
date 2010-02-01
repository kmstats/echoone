using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace com.echo.Controls
{
    public class ListViewEx : ListView
    {
        private const int WM_PAINT = 0x000F;

        public ListViewEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private int _LineBefore = -1;
        public int LineBefore
        {
            get { return _LineBefore; }
            set { _LineBefore = value; }
        }

        private int _LineAfter = -1;
        public int LineAfter
        {
            get { return _LineAfter; }
            set { _LineAfter = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                if (LineBefore >= 0 && LineBefore < Items.Count)
                {
                    Rectangle rc = Items[LineBefore].GetBounds(ItemBoundsPortion.Entire);
                    DrawInsertionLine(rc.Left, rc.Top, rc.Bottom);
                }
                if (LineAfter >= 0 && LineBefore < Items.Count)
                {
                    try
                    {
                        Rectangle rc = Items[LineAfter].GetBounds(ItemBoundsPortion.Entire);
                        DrawInsertionLine(rc.Right, rc.Top, rc.Bottom);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void DrawInsertionLine(int X, int Y1, int Y2)
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.DrawLine(Pens.Red, X, Y1, X, Y2);

                Point[] TopTriangle = new Point[3] {
                            new Point(X-4, Y1),
                            new Point(X,   Y1+7),
                            new Point(X+4, Y1)
                        };
                Point[] BottomTriangle = new Point[3] {
                            new Point(X-4,     Y2),
                            new Point(X, Y2-7),
                            new Point(X+4,     Y2)
                        };
                g.FillPolygon(Brushes.Red, TopTriangle);
                g.FillPolygon(Brushes.Red, BottomTriangle);
            }
        }
    }
}
