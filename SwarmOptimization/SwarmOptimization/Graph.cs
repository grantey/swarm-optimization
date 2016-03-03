using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SwarmOptimization
{
    class Graph
    {
        Point[] Points;
        PictureBox DesignerPanel;

        public Graph(PictureBox dp, double[] values)
        {
            DesignerPanel = dp;
            Points = new Point[50];

            int t;
            for (int i = 0; i < 50; i++)
            {
                for (t = -9; t < 3; t++) if (values[i] < Math.Pow(10, t)) break;
                if (t == -9) Points[i].Y = 365;
                else if (t == 2) Points[i].Y = 10;
                else Points[i].Y = 45 - 35 * t + (int)((Math.Pow(10, t) - values[i]) / (Math.Pow(10, t) - Math.Pow(10, t - 1)) * 35);
                Points[i].X = 40 + i*10;
            }
                
            Draw();
        }

        private void Draw()
        {
            Graphics gr = DesignerPanel.CreateGraphics();
            Random c = new Random();
            int r = c.Next(255);
            int g = c.Next(255);
            int b = c.Next(255);
            Pen pen = new Pen(Color.FromArgb(r, g, b), 1);
            gr.DrawCurve(pen,Points,0.0F);            
        }
    }
}
