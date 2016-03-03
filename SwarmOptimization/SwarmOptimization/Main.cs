using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SwarmOptimization
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private int N;
        private int D;
        private int I;
        private Graph bestFGraph;
        private int _func = 1;

        private SO Init()
        {
            SO Swarm = null;
            double[] args;

            try
            {
                I = Convert.ToInt32(textBox5.Text);
                N = Convert.ToInt32(textBox2.Text);
                D = Convert.ToInt32(textBox6.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show(this, "Неверные входные данные");
                return null;
            }

            if (radioButton1.Checked) _func = 1;
            if (radioButton2.Checked) _func = 2;
            if (radioButton3.Checked) _func = 3;

            try
            {
                if (radioButton4.Checked)
                {
                    args = new double[] { Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text) };
                    Swarm = new Classic(I, D, _func, args);                    
                }
                if (radioButton5.Checked)
                {
                    args = new double[] { Convert.ToDouble(textBox8.Text), Convert.ToDouble(textBox7.Text) };
                    Swarm = new FIPS(I, D, _func, args);                    
                }
                if (radioButton6.Checked)
                {
                    args = new double[] { Convert.ToDouble(textBox10.Text), Convert.ToDouble(textBox9.Text), Convert.ToDouble(textBox11.Text), Convert.ToDouble(textBox12.Text), Convert.ToDouble(trackBar1.Value), Convert.ToDouble(trackBar2.Value) };
                    Swarm = new DEMPS(I, D, _func, args);                  
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(this, "Неверные входные данные");
                return null;
            }

            return Swarm;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            double[] bestFvalues;
            SO Swarm;

            Swarm = Init();
            if (Swarm == null)
            {
                MessageBox.Show(this, "Ошибка инициализации");
                return;
            }

            double dN = N / 50;
            bestFvalues = new double[51];
            int _progressBarCount = 0;
            int _bestFcount = 0;
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Value = 0;            

            for (int i = 0; i < N; i++)
            {
                Swarm.Swap();
                Swarm.Move();
                if (++_progressBarCount > N / 100)
                {
                    toolStripProgressBar1.PerformStep();
                    _progressBarCount = 0;
                }
                if (i >= _bestFcount * dN) bestFvalues[_bestFcount++] = Swarm.bestF;
            }

            toolStripProgressBar1.Visible = false;

            if (toolStripMenuItem1.Checked) bestFGraph = new Graph(pictureBox1, bestFvalues);

            if (toolStripMenuItem2.Checked)
            {
                if (radioButton4.Checked) textBox1.Text += "Canonical PSO >> Function " + _func + " >> I = " + I + "  D = " + D + "  N = " + N + "  c1 = " + textBox3.Text + "  c2 = " + textBox4.Text;
                if (radioButton5.Checked) textBox1.Text += "Fully Informed PSO >> Function " + _func + " >> I = " + I + "  D = " + D + "  N = " + N + "  phi = " + textBox8.Text + "  K = " + textBox7.Text;
                if (radioButton6.Checked) textBox1.Text += "DEM PSO >> Function " + _func + " >> I = " + I + "  D = " + D + "  N = " + N + "  w = " + textBox10.Text + "  sigma = " + textBox9.Text + "  r = " + textBox11.Text + "  (p, q) = (5, 7)";

                textBox1.Text += "\r\nЛучшая точка:  ";
                for (int i = 0; i < D; i++) textBox1.Text += Convert.ToString(Math.Round(Swarm.bestX[0, i], 5)) + "    ";
                textBox1.Text += "\r\nЗначение функции:  " + Convert.ToString(Math.Round(Swarm.bestF, 5)) + "\r\n\r\n";
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox6.Text) < 2)
            {
                MessageBox.Show(this, "Размерность пространства должна быть 2 и выше");
                return;
            }

            SO Swarm = Init();
            if (Swarm == null)
            {
                MessageBox.Show(this, "Ошибка инициализации");
                return;
            }

            Visualisation Visualization = new Visualisation(Swarm, N, D, I);
            Visualization.ShowDialog();
            
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.') e.KeyChar = ',';
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.Black);
            Font font = new Font("Arial", 7);
            int N = Convert.ToInt32(textBox2.Text);
            gr.DrawLine(pen, 30, 380, 30, 10);
            gr.DrawLine(pen, 30, 380, 530, 380);
            for (int i = 0; i < 2; i++) gr.DrawString("1.E+0" + i.ToString(), font, brush, 0, 45 - i * 35);
            for (int i = 1; i < 10; i++) gr.DrawString("1.E-0" + i.ToString(), font, brush, 0, i * 35 + 45);
            for (int i = 0; i < 5; i++) gr.DrawString((i*25).ToString() + "%", font, brush, 40 + i * 100, 385);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                textBox8.Visible = textBox7.Visible = textBox10.Visible = textBox9.Visible = textBox11.Visible = textBox12.Visible = trackBar1.Visible = trackBar2.Visible = false;
                textBox3.Visible = textBox4.Visible = true;
                label7.Visible = label6.Visible = label9.Visible = label8.Visible = label10.Visible = label11.Visible = label12.Visible = label13.Visible = false;
                label3.Visible = label4.Visible = true;
            }
            if (radioButton5.Checked)
            {
                textBox3.Visible = textBox4.Visible = textBox10.Visible = textBox9.Visible = textBox11.Visible = textBox12.Visible = trackBar1.Visible = trackBar2.Visible = false;
                textBox8.Visible = textBox7.Visible = true;
                label3.Visible = label4.Visible = label9.Visible = label8.Visible = label10.Visible = label11.Visible = label12.Visible = label13.Visible = false;
                label6.Visible = label7.Visible = true;
            }
            if (radioButton6.Checked)
            {
                textBox10.Visible = textBox9.Visible = textBox11.Visible = textBox12.Visible = trackBar1.Visible = trackBar2.Visible = true;
                textBox3.Visible = textBox4.Visible = textBox8.Visible = textBox7.Visible = false;
                label9.Visible = label8.Visible = label10.Visible = label11.Visible = label12.Visible = label13.Visible = true;
                label3.Visible = label4.Visible = label7.Visible = label6.Visible = false;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripMenuItem1.Checked = !toolStripMenuItem1.Checked;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            toolStripMenuItem2.Checked = !toolStripMenuItem2.Checked;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics gr = Graphics.FromImage(bmp);
            Point p = pictureBox1.PointToScreen(new Point(0, 0));
            gr.CopyFromScreen(p.X, p.Y, 0, 0, pictureBox1.Size);

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (saveFileDialog1.FileName.IndexOf('.') == -1) saveFileDialog1.FileName += ".bmp";
                bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo(DateTime.Now.ToString("yy.MM.dd HH.mm.ss") + ".txt");
            StreamWriter sw;
            sw = fi.CreateText();
            sw.Write(textBox1.Text);
            MessageBox.Show(this, "Сохранено");
            sw.Dispose();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "PSOHelp.chm";
            proc.Start();
        }
    }
}
