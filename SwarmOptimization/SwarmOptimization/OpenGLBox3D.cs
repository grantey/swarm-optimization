using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SwarmOptimization
{
    public class OpenGLBox3D : OpenGL.OpenGLControl
    {
		private System.ComponentModel.IContainer components = null;

        public OpenGLBox3D()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// UserControl2
			// 
            this.Name = "OpenGLBox";
			this.Size = new System.Drawing.Size(208, 208);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OpenGLBox_Paint);

		}
		#endregion
        
		#region Overridables
		protected override void OnInitScene()
		{
            glDepthMask(1);

            glShadeModel(GL_SMOOTH);

            glLineWidth(1);
            glHint(GL_LINE_SMOOTH_HINT, GL_NICEST);
            glHint(GL_POINT_SMOOTH_HINT, GL_NICEST);

            BuildFont();

            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		}
		#endregion

        #region Loading Textures

        private const int FC_TextureCount = 1;
        private uint[] Textures = new uint[FC_TextureCount];
        private uint FontBase;		
        private void LoadGLTextures()                                    // Load Bitmaps And Convert To Textures
        {            
            glGenTextures(FC_TextureCount, Textures);
            glBindTexture(GL_TEXTURE_2D, Textures[0]);
            Bitmap bitmap = new Bitmap("font.bmp", true);
            System.Drawing.Imaging.BitmapData data;
            Rectangle Rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            data = bitmap.LockBits(Rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_REPLACE);
            glTexImage2D(GL_TEXTURE_2D, 0,
                            4, data.Width, data.Height, 0,
                            GL_RGBA, GL_UNSIGNED_BYTE, data.Scan0);
            bitmap.UnlockBits(data);
        }

        private void BuildFont()
        {
            LoadGLTextures();

            float cx, cy;
            glBindTexture(GL_TEXTURE_2D, Textures[0]);

            FontBase = glGenLists(256);
            for (int k = 0; k < 256; k++)
            {
                cx = (float)(k % 16) / 16.0f;
                cy = (float)(k / 16) / 16.0f;
                glNewList((uint)(FontBase + k), GL_COMPILE);                
                glBegin(GL_QUADS);
                    glTexCoord2f(cx, cy + 0.0625f);
                    glVertex3d(0, 0, 0);
                    glTexCoord2f(cx + 0.0625f, cy + 0.0625f);
                    glVertex3d(0.2, 0, 0);
                    glTexCoord2f(cx + 0.0625f, cy);
                    glVertex3d(0.2, 0.2, 0);
                    glTexCoord2f(cx, cy);
                    glVertex3d(0, 0.2, 0);
                glEnd();
                glTranslated(0.2, 0, 0);
                glEndList();
            }
        }

        private void glPrint(float x, float y, float z, string s)
        {
            char[] text = new char[s.Length];
            text = s.ToCharArray(0,s.Length);

	        glPushMatrix();			
            glTranslated(x,y,z);
            glRotated(-_fAngleX, 1, 0, 0);
            glRotated(-_fAngleY, 0, 1, 0);

	        glListBase(FontBase-32);
	        glCallLists(s.Length,GL_BYTE,text);		
   
	        glPopMatrix();		
        }
        #endregion

        #region Drawing

        float[, ,] points = new float[50, 50, 50];		
		private Point _ptCursorPos = new Point();
        private float _fTransX = 0;
        private float _fTransY = -1;
		private float _fTransZ = -10;
		private float _fAngleX = 30;
        private float _fAngleY = 30;

		public float AngleX
		{
			get {return _fAngleX;}
			set {
				_fAngleX = value;
				Invalidate();
			}
		}
		
		public float AngleY
		{
			get {return _fAngleY;}
			set 
			{
				_fAngleY = value;
				Invalidate();
			}
		}

        private void DrawSpace()
        {
            glEnable(GL_POINT_SMOOTH);
            Random r = new Random();
            glColor3ub(255, 0, 0);

            glPointSize(5.0F);
            for (int k = 0; k < Form2.I; k++)
            {
                glBegin(GL_POINTS);
                glVertex3f((float)Form2.X[k, 0], (float)Form2.X[k, 1], (float)Form2.X[k, 2]);
                glEnd();
            }
        }

		private void DrawCoordinateSystem()
		{
            glEnable(GL_BLEND);
            glEnable(GL_LINE_SMOOTH);
            glEnable(GL_POINT_SMOOTH);

            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            glColor4f(0.0F, 0.0F, 0.0F, 1.0F);

            int dist;
            if (_fTransZ < -15) dist = 5 - (int)_fTransZ / 5;
            else dist = 5;
            
            glBegin(GL_LINES);
			// X
			    glVertex3d(-dist, 0, 0);
                glVertex3d(dist, 0, 0);
			// Y
                glVertex3d(0, -dist, 0);
                glVertex3d(0, dist, 0);
			// Z
                glVertex3d(0, 0, -dist);
                glVertex3d(0, 0, dist);
			glEnd();

            for (int i = -dist + 1; i < dist; i++)
            {
                glBegin(GL_POINTS);
                glVertex3f(i, 0, 0);
                glVertex3f(0, i, 0);
                glVertex3f(0, 0, i);
                glEnd();
            }

            glBlendFunc(GL_SRC_ALPHA, GL_ONE);
            glEnable(GL_TEXTURE_2D);

            glColor4f(0.0F, 0.0F, 1.0F, 1.0F);

            for (int i = -dist + 1; i < dist; i++)
            {
                glPrint(i, -0.2F, 0, i.ToString());
                if (i != 0) glPrint(0, -0.2F, i, i.ToString());
                glBegin(GL_POINTS);
                glVertex3f(i, 0, 0);
                glEnd();
            }

            glDisable(GL_TEXTURE_2D);
            glDisable(GL_BLEND);
          
		}
		#endregion

		#region Event handlers
        private void OpenGLBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			ActivateContext();

			glClearColor(0.98F, 0.98F, 1.0F, 1.0F);

			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			glMatrixMode(GL_MODELVIEW);

			glLoadIdentity();

            glTranslated(_fTransX, _fTransY, _fTransZ);
            glRotated(_fAngleX, 1, 0, 0);
            glRotated(_fAngleY, 0, 1, 0);
            

			DrawCoordinateSystem();
            DrawSpace();

			SwapBuffers();

			DeactivateContext();
		}

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			
			_ptCursorPos.X = e.X;
			_ptCursorPos.Y = e.Y;
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
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
            Invalidate();
		}

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0) _fTransZ += 0.4F;
            else _fTransZ -= 0.4F;
            Invalidate();
        }

		#endregion
	}
}
