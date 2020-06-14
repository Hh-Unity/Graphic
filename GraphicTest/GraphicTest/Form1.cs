using GraphicTest.RenderData;
using GraphicTest.TransformData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicTest
{
    public partial class Form1 : Form
    {
        private Bitmap texture;
        private Thread thread;
        private Bitmap frameBuff;
        private Graphics frameG;
        private int width = 800;
        private int height = 600;
        private int imgWidth = 512;
        private int imgHeight = 512;
        private System.Drawing.Color[,] textureArray; //纹理颜色值
        Graphics g;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Image img = Image.FromFile(Environment.CurrentDirectory + @"\..\..\Texture\texture.jpg");
            Control.CheckForIllegalCrossThreadCalls = false;
            texture = new Bitmap(img, imgWidth, imgHeight);
            InitTexture();
            g = CreateGraphics();
            frameBuff = new Bitmap(width, height);
            frameG = Graphics.FromImage(frameBuff);

            thread = new Thread(new ThreadStart(Tick));
            thread.Start();
        }

        private void Tick()
        {
            while (true)
            {
                lock (frameBuff)
                {
                    ClearBuff();
                    BresenhamDrawLine(100, 100, 500, 200);
                    if (frameBuff != null)
                        g.DrawImage(frameBuff, 0, 0);
                   
                }
            }
        }

        private void InitTexture()
        {
            textureArray = new System.Drawing.Color[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    textureArray[i, j] = texture.GetPixel(i, j);
                }
            }
        }

        private void ClearBuff()
        {
            frameG.Clear(System.Drawing.Color.Black);
        }

        private void DrawTriangle(Vertex v1,Vertex v2,Vertex v3,Matrix4x4 m,Matrix4x4 v,Matrix4x4 p)
        {
            SetModelToWorld(m, ref v1);
            SetModelToWorld(m, ref v2);
            SetModelToWorld(m, ref v3);

            SetWorldToCamera(v, ref v1);
            SetWorldToCamera(v, ref v2);
            SetWorldToCamera(v, ref v3);

            //在相机空间进行背面消隐
            if (Camera.BackFaceCulling(v1, v2, v3) == false)
            {
                return;
            }

            //超出屏幕外剔除
            if (Exclude(v1) == false && Exclude(v2) == false && Exclude(v3) == false)
            {
                return;
            }

            SetProjectionTransform(p, ref v1);
            SetProjectionTransform(p, ref v2);
            SetProjectionTransform(p, ref v3);

           


        }

        private void BresenhamDrawLine(int x,int y,int x1,int y1)
        {
            int startX = x;
            int startY = y;
            int endX = x1;
            int endY = y1;
            float disX = endX - startX;
            float disY = endY - startY;
            int stepX = Math.Sign(disX);
            int stepY = Math.Sign(disY);
            float e = 0.0f;
            float k = 0;
            if (Math.Abs(disX) > Math.Abs(disY))
            {
                if (disX == 0)
                {
                    disX = 999;
                }

                k = Math.Abs(disY / disX);
                while (true)
                {
                    e += k;
                    frameBuff.SetPixel(startX, startY, System.Drawing.Color.White);
                    if (e >= 1)
                    {
                        e--;
                    }

                    if (e > 0.5)
                        startY += stepY;
                    if (startX == endX) break;
                    startX += stepX;
                }
            }
        }
        /// <summary>
        /// v1上顶点
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        private void FillTopFlatTriangle(Vertex v1,Vertex v2,Vertex v3)
        {
            int x1 = (int)Math.Ceiling(v1.point.x);
            int x2 = (int)Math.Ceiling(v2.point.x);
            int x3 = (int)Math.Ceiling(v3.point.x);
            int y1 = (int)Math.Ceiling(v1.point.y);
            int y2 = (int)Math.Ceiling(v2.point.y);
            int y3 = (int)Math.Ceiling(v3.point.y);

            float invslopeL = (x3 - x1)*1.0f/(y1 - y3);
            float invslopeR = (x2 - x1) * 1.0f / (y1 - y2);

            int curxL = 0;
            int curxR = 0;

            for (int startY = y2;startY<=y1;startY++)
            {
                curxL = (int)Math.Ceiling(x1 + (y1 - startY) * invslopeL);
                curxR = (int)Math.Ceiling(x1 + (y1 - startY) * invslopeR);

                Vertex vl = new Vertex
                {
                    point = new Vector4(curxL, startY, 0, 0)
                };
                Vertex vr = new Vertex
                {
                    point = new Vector4(curxR, startY, 0, 0)
                };
                
            }
        }
        /// <summary>
        /// v1下顶点
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        private void FillBottomFlatTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            int x1 = (int)Math.Ceiling(v1.point.x);
            int x2 = (int)Math.Ceiling(v2.point.x);
            int x3 = (int)Math.Ceiling(v3.point.x);
            int y1 = (int)Math.Ceiling(v1.point.y);
            int y2 = (int)Math.Ceiling(v2.point.y);
            int y3 = (int)Math.Ceiling(v3.point.y);

            float invslopeL = (x2 - x1) * 1.0f / (y2 - y1);
            float invslopeR = (x3 - x1) * 1.0f / (y3 - y1);

            int curL = 0;
            int curR = 0;

            for (int starY = y1;starY<=y2;starY++)
            {
                curL = (int)Math.Ceiling(x1 + (starY - y1) * invslopeL);
                curR = (int)Math.Ceiling(x1 + (starY - y1) * invslopeR);

                Vertex vl = new Vertex
                {
                    point = new Vector4(curL, starY, 0, 0)
                };

                Vertex vR = new Vertex
                {
                    point = new Vector4(curR, starY, 0, 0)
                };
            }
        }

        /// <summary>
        /// 世界空间到相机空间
        /// </summary>
        private void SetWorldToCamera(Matrix4x4 v, ref Vertex vertex)
        {
            vertex.point = v * vertex.point;
        }

        /// <summary>
        /// 模型空间到世界空间
        /// </summary>
        private void SetModelToWorld(Matrix4x4 m, ref Vertex vertex)
        {
            vertex.point = m * vertex.point;
        }

        /// <summary>
        /// 投影变换，从相机空间到齐次剪裁空间
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vertex"></param>
        private void SetProjectionTransform(Matrix4x4 p, ref Vertex vertex)
        {
            vertex.point = p * vertex.point;
        }

        private bool Exclude(Vertex v)
        {
            if (v.point.x >= -v.point.w && v.point.x <= v.point.w
                && v.point.y >= -v.point.w && v.point.y <= v.point.w)
            {
                return true;
            }
            return false;
        }
    }
}
