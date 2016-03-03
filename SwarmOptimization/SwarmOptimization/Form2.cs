using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace SwarmOptimization
{
    public partial class Form2 : Form
    {
        public Form2(SO swarm, int n, int d, int i)
        {
            InitializeComponent();
            OpenGLBox.InitializeContexts();
            Swarm = swarm;
            this.I = i;
            this.N = n;
            this.D = d;
        }

        private SO Swarm;
        private int N;
        private int D;
        private int I;
        private int k = 0;

        private int Dim = 2;
        private Point _ptCursorPos = new Point();
        private float _fTransX = 6.5F;
        private float _fTransY = 3.5F;
        private float _fTransZ = 20;
        private float _fAngleX;
        private float _fAngleY;

        private void timer1_Tick(object sender, EventArgs e)
        {
            Swarm.Swap();
            Swarm.Move();

            if (Dim == 3) Scene3D();
            else Scene2D();

            if (k++ > N) timer1.Stop();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (D == 2) radioButton2.Enabled = false;
          
            //Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Gl.glClearColor(0.98F, 0.98F, 1.0F, 1.0F);
            Gl.glViewport(0, 0, OpenGLBox.Width, OpenGLBox.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            if (Dim == 2) Glu.gluOrtho2D(0.0, 30.0 * (float)OpenGLBox.Width / (float)OpenGLBox.Height, 0.0, 30.0);
            else Glu.gluPerspective(45, (float)OpenGLBox.Width / (float)OpenGLBox.Height, 1, 200);

            Gl.glDepthMask(1);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glLineWidth(1);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);

            Gl.glPointSize(3.0F);
            //Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            label2.Text = I + "  частиц, " + N + "  шагов";
            BuildFont();
            Scene2D();
        }

        private void Scene2D()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glLoadIdentity();

            Gl.glTranslated(_fTransX*2, _fTransY*2, 0);
            Gl.glScaled(_fTransZ / 10, _fTransZ / 10, _fTransZ / 10);

            DrawCoordinateSystem2D();
            DrawSpace();

            Gl.glFlush();
            OpenGLBox.Invalidate();
        }

        private void Scene3D()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glLoadIdentity();

            Gl.glTranslated(_fTransX, _fTransY, _fTransZ);

            Gl.glRotated(_fAngleX, 1, 0, 0);
            Gl.glRotated(_fAngleY, 0, 1, 0);

            DrawCoordinateSystem3D();
            DrawSpace();

            Gl.glFlush();
            OpenGLBox.Invalidate();
        }

        private void DrawSpace()
        {
          //  Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glColor3ub(255, 0, 0);

            if (Dim == 3)
                for (int k = 0; k < I; k++)
                {
                    Gl.glBegin(Gl.GL_POINTS);
                        Gl.glVertex3f((float)Swarm.currX[k, 0], (float)Swarm.currX[k, 1], (float)Swarm.currX[k, 2]);
                    Gl.glEnd();
                }
            else
                for (int k = 0; k < I; k++)
                {
                    Gl.glBegin(Gl.GL_POINTS);
                    Gl.glVertex2f((float)Swarm.currX[k, 0], (float)Swarm.currX[k, 1]);
                    Gl.glEnd();
                }
        }

        private void DrawCoordinateSystem3D()
        {
            Gl.glColor4f(0.0F, 0.0F, 0.0F, 1.0F);

            int dist;
            if (_fTransZ < -15) dist = 5 - (int)_fTransZ / 5;
            else dist = 5;

            Gl.glBegin(Gl.GL_LINES);
                // X
                Gl.glVertex3d(-dist, 0, 0);
                Gl.glVertex3d(dist, 0, 0);
                // Y
                Gl.glVertex3d(0, -dist, 0);
                Gl.glVertex3d(0, dist, 0);                
                // Z
                if (Dim == 3)
                {
                    Gl.glVertex3d(0, 0, -dist);
                    Gl.glVertex3d(0, 0, dist);
                }
            Gl.glEnd();

            if (dist > 10) return;
            for (int i = -dist + 1; i < dist; i++)
            {
                Gl.glBegin(Gl.GL_POINTS);
                    Gl.glVertex3f(i, 0, 0);
                    Gl.glVertex3f(0, i, 0);
                    if (Dim == 3) Gl.glVertex3f(0, 0, i);
                Gl.glEnd();
            }

            //Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
            //Gl.glEnable(Gl.GL_TEXTURE_2D);

            //Gl.glColor4f(0.0F, 0.0F, 1.0F, 1.0F);

            //for (int i = -dist + 1; i < dist; i++)
            //{
            //    glPrint(i, -0.2F, 0, i.ToString());
            //    if (i != 0) glPrint(0, -0.2F, i, i.ToString());
            //    Gl.glBegin(Gl.GL_POINTS);
            //    Gl.glVertex3f(i, 0, 0);
            //    Gl.glEnd();
            //}

            //Gl.glDisable(Gl.GL_TEXTURE_2D);
            //Gl.glDisable(Gl.GL_BLEND);

        }

        private void DrawCoordinateSystem2D()
        {
            Gl.glColor4f(0.0F, 0.0F, 0.0F, 1.0F);

            Gl.glBegin(Gl.GL_LINES);
            // X
            Gl.glVertex2d(-100, 0);
            Gl.glVertex2d(100, 0);
            // Y
            Gl.glVertex2d(0, -100);
            Gl.glVertex2d(0, 100);
            Gl.glEnd();

            if (_fTransZ < -15) return;
            for (int i = -20; i < 21; i++)
            {
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glVertex2d(i, 0);
                Gl.glVertex2d(0, i);
                Gl.glEnd();
            }
        }

        private void OpenGLBox_MouseDown(object sender, MouseEventArgs e)
        {
            _ptCursorPos.X = e.X;
            _ptCursorPos.Y = e.Y;
        }

        private void OpenGLBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Right)
            {
                float _fShiftDY = (float)(e.Y - _ptCursorPos.Y) / 4f;
                float _fShiftDX = (float)(e.X - _ptCursorPos.X) / 4f;

                _fAngleX += _fShiftDY;
                _fAngleY += _fShiftDX;
                if (_fAngleX > 360) _fAngleX = 0;
                if (_fAngleY > 360) _fAngleY = 0;
            }
            if (MouseButtons == MouseButtons.Left)
            {
                float _fShiftDY = (float)(e.Y - _ptCursorPos.Y) / 60f;
                float _fShiftDX = (float)(e.X - _ptCursorPos.X) / 60f;

                _fTransX += _fShiftDX;
                _fTransY -= _fShiftDY;
            }
            _ptCursorPos.X = e.X;
            _ptCursorPos.Y = e.Y;

            if (Dim == 3) Scene3D();
            else Scene2D();
        }

        private void OpenGLBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) _fTransZ += 0.4F;
            else _fTransZ -= 0.4F;
            if (Dim == 3) Scene3D();
            else Scene2D();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Gl.glFinish();
        }

        private const int FC_TextureCount = 1;
        private uint[] Textures = new uint[FC_TextureCount];
        private int FontBase;

        #region Print
        private void LoadGLTextures()                                    // Load Bitmaps And Convert To Textures
        {
            Gl.glGenTextures(FC_TextureCount, Textures);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Textures[0]);
            Bitmap bitmap = new Bitmap("font.bmp", true);
            System.Drawing.Imaging.BitmapData data;
            Rectangle Rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            data = bitmap.LockBits(Rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0,
                            4, data.Width, data.Height, 0,
                            Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data.Scan0);
            bitmap.UnlockBits(data);
        }

        private void BuildFont()
        {
            LoadGLTextures();

            float cx, cy;
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Textures[0]);

            FontBase = Gl.glGenLists(256);
            for (int k = 0; k < 256; k++)
            {
                cx = (float)(k % 16) / 16.0f;
                cy = (float)(k / 16) / 16.0f;
                Gl.glNewList((uint)(FontBase + k), Gl.GL_COMPILE);
                Gl.glBegin(Gl.GL_QUADS);
                    Gl.glTexCoord2f(cx, cy + 0.0625f);
                    Gl.glVertex3d(0, 0, 0);
                    Gl.glTexCoord2f(cx + 0.0625f, cy + 0.0625f);
                    Gl.glVertex3d(0.2, 0, 0);
                    Gl.glTexCoord2f(cx + 0.0625f, cy);
                    Gl.glVertex3d(0.2, 0.2, 0);
                    Gl.glTexCoord2f(cx, cy);
                    Gl.glVertex3d(0, 0.2, 0);
                Gl.glEnd();
                Gl.glTranslated(0.2, 0, 0);
                Gl.glEndList();
            }
        }

        private void glPrint(float x, float y, float z, string s)
        {
            char[] text = new char[s.Length];
            text = s.ToCharArray(0, s.Length);

            Gl.glPushMatrix();
            Gl.glTranslated(x, y, z);
            Gl.glRotated(-_fAngleX, 1, 0, 0);
            Gl.glRotated(-_fAngleY, 0, 1, 0);

            Gl.glListBase(FontBase - 32);
            Gl.glCallLists(s.Length, Gl.GL_BYTE, text);

            Gl.glPopMatrix();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Stop();
            else 
            {
                timer1.Start();                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Stop();
            Random r = new Random();

            for (int i = 0; i < I; i++)
                for (int j = 0; j < D; j++) Swarm.SetX(r.NextDouble() * 5 + 5, r.NextDouble() * 2 - 1, i, j);

            for (int j = 0; j < D; j++) Swarm.bestX[0, j] = Swarm.currX[0, j];
            Swarm.bestF = 1E+6;

            if (Dim == 3) Scene3D();
            else Scene2D();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = 95 - (int)0.9*trackBar1.Value;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Dim = 2;
                k = 0;
                _fTransX = 6.5F;
                _fTransY = 3.5F;
                _fTransZ = 20;
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                Glu.gluOrtho2D(0.0, 30.0 * (float)OpenGLBox.Width / (float)OpenGLBox.Height, 0.0, 30.0);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Scene2D();
            }
            else
            {
                Dim = 3;
                k = 0;
                _fTransX = 0;
                _fTransY = -1;
                _fTransZ = -25;
                _fAngleX = 30;
                _fAngleY = -30;
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                Glu.gluPerspective(45, (float)OpenGLBox.Width / (float)OpenGLBox.Height, 1, 200);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Scene3D();
            }            
        }

        private void OpenGLBox_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
