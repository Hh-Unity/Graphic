using GraphicTest.RenderData;
using GraphicTest.Test;
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
        private Light light;    
        private Camera camera;
        private RenderData.Color ambientColor; //全局环境光颜色 
        private System.Drawing.Color[,] textureArray; //纹理颜色值
        Graphics g;
        private Mesh mesh;
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
            ambientColor = new RenderData.Color(0.1f, 0.1f, 0.1f);
            mesh = new Mesh(CubeTestData.PointList, CubeTestData.Indexs, CubeTestData.UVs, CubeTestData.VertColors,
               CubeTestData.Normals, QuadTestData.Mat);
            camera = new Camera(new Vector4(0, 4, 1, 1), new Vector4(0, 1, 0, 0), new Vector4(0, 4, 6, 1),
               (float)Math.PI / 3, width / (float)height, 3, 30);
            light = new Light(new Vector3(0, 10, 0), new RenderData.Color(1, 1, 1));
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
                    Matrix4x4 worldMatrix = Matrix4x4.Translate(new Vector3(0, 3, 12)) * Matrix4x4.RotateY(0) *
                                           Matrix4x4.RotateX(-0.2f) * Matrix4x4.RotateZ(0);
                    Matrix4x4 viewMatrix = Camera.BuildViewMatrix(camera.eyePosition, camera.up, camera.lookAtPos);
                    Matrix4x4 projectionMatrix =
                        Camera.BuildProjectionMatrix(camera.fov, camera.aspect, camera.nearOrigin, camera.farOrigin);
                    Draw(worldMatrix, viewMatrix, projectionMatrix);
                    //BresenhamDrawLine(100, 100, 500, 200);
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

        private void Draw(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
        {
            for (int i = 0; i + 2 < mesh.vertices.Length; i += 3)
            {
                DrawTriangle(mesh.vertices[i], mesh.vertices[i + 1], mesh.vertices[i + 2], m, v, p);
            }
        }

        private void DrawTriangle(Vertex v1,Vertex v2,Vertex v3,Matrix4x4 m,Matrix4x4 v,Matrix4x4 p)
        {
            SetModelToWorld(m, ref v1);
            SetModelToWorld(m, ref v2);
            SetModelToWorld(m, ref v3);

            Light.BaseLight(m, light, mesh, camera.eyePosition, ambientColor, ref v1);
            Light.BaseLight(m, light, mesh, camera.eyePosition, ambientColor, ref v2);
            Light.BaseLight(m, light, mesh, camera.eyePosition, ambientColor, ref v3);

            SetWorldToCamera(v, ref v1);
            SetWorldToCamera(v, ref v2);
            SetWorldToCamera(v, ref v3);

            //在相机空间进行背面消隐
            if (Camera.BackFaceCulling(v1, v2, v3) == false)
            {
                return;
            }
            SetProjectionTransform(p, ref v1);
            SetProjectionTransform(p, ref v2);
            SetProjectionTransform(p, ref v3);
            //超出屏幕外剔除
            if (Exclude(v1) == false && Exclude(v2) == false && Exclude(v3) == false)
            {
                return;
            }

            TransformToScreen(ref v1);
            TransformToScreen(ref v2);
            TransformToScreen(ref v3);

            TriangleRasterization(v1,v2,v3);
        }

        private void TriangleRasterization(Vertex v1,Vertex v2,Vertex v3)
        {
            if (v1.point.y == v2.point.y)
            {
                if (v1.point.y < v3.point.y)
                    FillTopFlatTriangle(v3, v2, v1);
                else
                    FillBottomFlatTriangle(v3,v2,v1);
            }
            else if (v1.point.y == v3.point.y)
            {
                if (v1.point.y < v2.point.y)
                    FillTopFlatTriangle(v2, v1, v3);
                else
                    FillBottomFlatTriangle(v2,v1,v3);
            }
            else if (v2.point.y == v3.point.y)
            {
                if (v2.point.y < v1.point.y)
                    FillTopFlatTriangle(v1, v3, v2);
                else
                    FillBottomFlatTriangle(v1,v3,v2);
            }
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



        private void BresenhamDrawLine(Vertex v1, Vertex v2)
        {
            int startX = (int)Math.Ceiling(v1.point.x);
            int startY = (int)Math.Ceiling(v1.point.y);
            int endX = (int)Math.Ceiling(v2.point.x);
            int endY = (int)Math.Ceiling(v2.point.y);
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
                    if(startX >= 0 && startY >= 0 && startX < width && startY < height)
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
                Vertex vR = new Vertex
                {
                    point = new Vector4(curxR, startY, 0, 0)
                };
                BresenhamDrawLine(vl, vR);
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

                BresenhamDrawLine(vl, vR);
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


        /// <summary>
        /// 从齐次剪裁坐标系转到屏幕坐标
        /// </summary>
        private void TransformToScreen(ref Vertex v)
        {
            if (v.point.w != 0)
            {
                //插值矫正系数
                v.depth = 1 / v.point.w;
                //先进行透视除法，转到cvv
                v.point.x *= v.depth;
                v.point.y *= v.depth;
                v.point.z *= v.depth;
                v.point.w = 1;
                v.onePerZ = (v.point.z + 1) / 2;
                //cvv到屏幕坐标
                v.point.x = (v.point.x + 1) * 0.5f * width;
                v.point.y = (1 - v.point.y) * 0.5f * height;
                v.u *= v.depth;
                v.v *= v.depth;
                v.pointColor *= v.depth;
                v.lightingColor *= v.depth;
            }
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
