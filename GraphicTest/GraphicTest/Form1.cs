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
        private RenderMode rendMode; //渲染模式
        private bool isOpenLight; //光照模式
        private bool isOpenTexture; //纹理采样
        private bool isCull = true;
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
            rendMode = RenderMode.Textured;
            isOpenLight = true;
            isOpenTexture = true;
            ambientColor = new RenderData.Color(0.1f, 0.1f, 0.1f);
            mesh = new Mesh(CubeTestData.PointList, CubeTestData.Indexs, CubeTestData.UVs, CubeTestData.VertColors,
               CubeTestData.Normals, QuadTestData.Mat);
            camera = new Camera(new Vector4(0, 4, 1, 1), new Vector4(0, 1, 0, 0), new Vector4(0, 4, 6, 1),
               (float)Math.PI / 3, width / (float)height, 3, 30);
            light = new Light(new Vector3(0, 10, 0), new RenderData.Color(1, 1, 1));
            thread = new Thread(new ThreadStart(Tick));
            thread.Start();
            pictureBox1.Hide();
        }

        private void Tick()
        {
            while (true)
            {
                lock (frameBuff)
                {
                    ClearBuff();
                    Matrix4x4 worldMatrix = Matrix4x4.Translate(new Vector3(-2, 3, 12)) * Matrix4x4.RotateY(0) *
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

        private void RenderBtn_Click(object sender, EventArgs e)
        {
            switch (rendMode)
            {
                case RenderMode.Wireframe:
                    rendMode = RenderMode.Textured;
                    renderBtn.Text = "贴图";
                    break;
                case RenderMode.Textured:
                    rendMode = RenderMode.Wireframe;
                    renderBtn.Text = "线框";
                    break;
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

            if (isCull)
            {
                List<Triangle> outValue;
                CubeClip(new Triangle(v1, v2, v3), out outValue);
                for (int i = 0; i < outValue.Count; i++)
                {
                    Rasterization(outValue[i][0], outValue[i][1], outValue[i][2]);
                }
                //Rasterization(p1, p2, p3);
            }
            else
            {
                Rasterization(v1, v2, v3);
            }

            //TriangleRasterization(v1,v2,v3);
        }

        void Rasterization(Vertex p1, Vertex p2, Vertex p3)
        {
            if (rendMode == RenderMode.Wireframe)
            {
                BresenhamDrawLine(p1, p2);
                BresenhamDrawLine(p2, p3);
                BresenhamDrawLine(p3, p1);
            }
            else
            {
                TriangleRasterization(p1, p2, p3);
            }
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
            int curX = startX, curY = startY;
            float disX = endX - startX;
            float disY = endY - startY;
            int stepX = Math.Sign(disX);
            int stepY = Math.Sign(disY);
            float e = 0.0f;
            float k = 0;
            float t = 0;
            if (Math.Abs(disX) > Math.Abs(disY))
            {
                if (disX == 0)
                {
                    disX = 999;
                }

                k = Math.Abs(disY / disX);
                disX = 1 / disX;
                while (true)
                {
                    
                    //if(startX >= 0 && startY >= 0 && startX < width && startY < height)
                    //    frameBuff.SetPixel(startX, startY, finalColor.TransToSystemColor());
                    t = (startX - startY) * disY;

                    MixColor(v1,v2,t,curX,curY);
                    e += k;
                    if (e >= 1)
                    {
                        e--;
                    }

                    if (e > 0.5)
                        curY += stepY;
                    if (curX == endX) break;
                    curX += stepX;
                }
            }
        }

        private void MixColor(Vertex v1, Vertex v2, float t, int curX, int curY)
        {
            RenderData.Color finalColor = new RenderData.Color(1, 1, 1);
            if (rendMode == RenderMode.Textured)
            {
                if (v1.depth == 0)
                    Console.WriteLine();
                float w = Mathf.Lerp(v1.depth, v2.depth, t);
                w = 1 / w;
                if (isOpenLight)
                {
                    Mathf.Lerp(ref finalColor, v1.lightingColor, v2.lightingColor, t);
                    finalColor *= w;
                }
                if (!isOpenTexture)
                {
                    //颜色和光照混合
                    RenderData.Color temp = new RenderData.Color();
                    Mathf.Lerp(ref temp, v1.pointColor, v2.pointColor, t);
                    finalColor = temp * w * finalColor;
                }
                else
                {
                    //uv坐标
                    int u = (int)(Mathf.Lerp(v1.u, v2.u, t) * w * (imgWidth - 1));
                    int v = (int)(Mathf.Lerp(v1.v, v2.v, t) * w * (imgHeight - 1));

                    //纹理颜色
                    finalColor = new RenderData.Color(Tex(u, v)) * finalColor;
                    if (!(finalColor.R >= 0 && finalColor.R <= 255))
                        Console.WriteLine();
                }
            }
            if (curX >= 0 && curY >= 0 && curX < width && curY < height)
            {
                frameBuff.SetPixel(curX, curY, finalColor.TransToSystemColor());
            }
        }

        private System.Drawing.Color Tex(int i, int j)
        {
            if (i < 0 || i > imgWidth - 1 || j < 0 || j > imgHeight - 1)
            {
                return System.Drawing.Color.Black;
            }

            return textureArray[i, imgHeight - 1 - j];
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
                float t = (y1 - startY) / (float)(y1 - y3);
                Mathf.Lerp(ref vl, v1, v3, t);
                Mathf.Lerp(ref vR, v1, v2, t);

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
                float t = (starY - y1) * 1.0f / (y2 - y1);
                Mathf.Lerp(ref vl, v1, v2, t);
                Mathf.Lerp(ref vR, v1, v3, t);
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

        List<Vector3> normalList = new List<Vector3>()
        {
               new Vector3(0,0,1),
               new Vector3(0,0,-1),
               new Vector3(1,0,0),
               new Vector3(-1,0,0),
               new Vector3(0,-1,0),
               new Vector3(0,1,0),
        };

        int[] disArr = new int[] { -1, -1, 1, -798, -2, 598 };

        List<Func<float, int, bool>> funcs = new List<Func<float, int, bool>>()
        {
            (view,dis) => view>dis,
            (view,dis) => view>dis,
            (view,dis) => view>dis,
            (view,dis) => view>dis,
            (view,dis) => view<dis,
            (view,dis) => view<dis,

        };

        private Queue<Triangle> clipQueue = new Queue<Triangle>();

        private void CubeClip(Triangle triangle, out List<Triangle> outValue)
        {
            outValue = new List<Triangle>();
            clipQueue.Enqueue(triangle);
            bool isClip = false;
            Triangle temp;
            while (clipQueue.Count > 0)
            {
                temp = clipQueue.Dequeue();
                for (int i = temp.startIndex; i < normalList.Count; i++)
                {
                    if (!isClip)
                    {
                        isClip = CubeClip(temp[0], temp[1], temp[2], normalList[i],disArr[i], funcs[i],i);
                    }
                    else
                    {
                        break;
                    }
                }
                if (!isClip)
                {
                    outValue.Add(temp);
                }
                isClip = false;
            }

        }

        bool CubeClip(Vertex v1, Vertex v2, Vertex v3, Vector3 v4 , int nDis, Func<float, int, bool> nFunc, int startIndex)
        {
            Vector3 normal = v4;
            int dis = nDis;
            Func<float, int, bool> checkIsIn = nFunc;
            //点在法线上的投影
            float projectV1 = Vector3.Dot(normal, v1.point);
            float projectV2 = Vector3.Dot(normal, v2.point);
            float projectV3 = Vector3.Dot(normal, v3.point);
            //点与点之间的距离
            float dv1v2 = Math.Abs(projectV1 - projectV2);
            float dv1v3 = Math.Abs(projectV1 - projectV3);
            float dv2v3 = Math.Abs(projectV2 - projectV3);
            //颠倒平面的距离
            float pv1 = Math.Abs(projectV1 - dis);
            float pv2 = Math.Abs(projectV2 - dis);
            float pv3 = Math.Abs(projectV3 - dis);
            //插值
            float t = 0;

            if (checkIsIn(projectV1, dis) && checkIsIn(projectV2, dis) && checkIsIn(projectV3, dis))
            {
                //都在里面 
                return false;
            }
            if (!checkIsIn(projectV1, dis) && checkIsIn(projectV2, dis) && checkIsIn(projectV3, dis))//v1在外面
            {
                Vertex temp12 = new Vertex();
                t = pv2 / dv1v2;
                temp12.point.x = Mathf.Lerp(v2.point.x, v1.point.x, t);
                temp12.point.y = Mathf.Lerp(v2.point.y, v1.point.y, t);
                temp12.point.z = dis;
                temp12.point.w = 1;
                Mathf.Lerp(ref temp12, v2, v1, t);

                Vertex temp13 = new Vertex();
                t = pv3 / dv1v3;
                temp13.point.x = Mathf.Lerp(v3.point.x, v1.point.x, t);
                temp13.point.y = Mathf.Lerp(v3.point.y, v1.point.y, t);
                temp13.point.z = dis;
                temp13.point.w = 1;
                Mathf.Lerp(ref temp13, v3, v1, t);

                clipQueue.Enqueue(new Triangle(temp13, temp12, v2, startIndex + 1));
                clipQueue.Enqueue(new Triangle(temp13, v2, v3, startIndex + 1));
                return true;
            }
            if (checkIsIn(projectV1, dis) && !checkIsIn(projectV2, dis) && checkIsIn(projectV3, dis))//v2在外面
            {
                Vertex temp12 = new Vertex();
                t = pv1 / dv1v2;
                temp12.point.x = Mathf.Lerp(v1.point.x, v2.point.x, t);
                temp12.point.y = Mathf.Lerp(v1.point.y, v2.point.y, t);
                temp12.point.z = Mathf.Lerp(v1.point.z, v2.point.z, t);
                Mathf.Lerp(ref temp12, v1, v2, t);


                Vertex temp23 = new Vertex();
                t = pv3 / dv2v3;
                temp23.point.x = Mathf.Lerp(v3.point.x, v2.point.x, t);
                temp23.point.y = Mathf.Lerp(v3.point.y, v2.point.y, t);
                temp23.point.z = Mathf.Lerp(v3.point.z, v2.point.z, t);
                Mathf.Lerp(ref temp23, v3, v2, t);

                clipQueue.Enqueue(new Triangle(temp12, temp23, v3, startIndex + 1));
                clipQueue.Enqueue(new Triangle(temp12, v3, v1, startIndex + 1));
                return true;
            }
            if (checkIsIn(projectV1, dis) && checkIsIn(projectV2, dis) && !checkIsIn(projectV3, dis))//v3在外面
            {
                Vertex temp23 = new Vertex();
                t = pv2 / dv2v3;
                temp23.point.x = Mathf.Lerp(v2.point.x, v3.point.x, t);
                temp23.point.y = Mathf.Lerp(v2.point.y, v3.point.y, t);
                temp23.point.z = Mathf.Lerp(v2.point.z, v3.point.z, t);
                Mathf.Lerp(ref temp23, v2, v3, t);

                Vertex temp13 = new Vertex();
                t = pv1 / dv1v3;
                temp13.point.x = Mathf.Lerp(v1.point.x, v3.point.x, t);
                temp13.point.y = Mathf.Lerp(v1.point.y, v3.point.y, t);
                temp13.point.z = Mathf.Lerp(v1.point.z, v3.point.z, t);
                Mathf.Lerp(ref temp13, v1, v3, t);

                clipQueue.Enqueue(new Triangle(temp23, temp13, v1, startIndex + 1));
                clipQueue.Enqueue(new Triangle(temp23, v1, v2, startIndex + 1));
                return true;
            }
            if (!checkIsIn(projectV1, dis) && !checkIsIn(projectV2, dis) && checkIsIn(projectV3, dis))//v1 v2在外面
            {
                Vertex temp13 = new Vertex();
                t = pv3 / dv1v3;
                temp13.point.x = Mathf.Lerp(v3.point.x, v1.point.x, t);
                temp13.point.y = Mathf.Lerp(v3.point.y, v1.point.y, t);
                temp13.point.z = Mathf.Lerp(v3.point.z, v1.point.z, t);
                Mathf.Lerp(ref temp13, v3, v1, t);

                Vertex temp23 = new Vertex();
                t = pv3 / dv2v3;
                temp23.point.x = Mathf.Lerp(v3.point.x, v2.point.x, t);
                temp23.point.y = Mathf.Lerp(v3.point.y, v2.point.y, t);
                temp23.point.z = Mathf.Lerp(v3.point.z, v2.point.z, t);
                Mathf.Lerp(ref temp23, v3, v2, t);

                clipQueue.Enqueue(new Triangle(temp13, temp23, v3, startIndex + 1));
                return true;
            }
            if (!checkIsIn(projectV1, dis) && checkIsIn(projectV2, dis) && !checkIsIn(projectV3, dis))//v1 v3在外面
            {
                Vertex temp23 = new Vertex();
                t = pv2 / dv2v3;
                temp23.point.x = Mathf.Lerp(v2.point.x, v3.point.x, t);
                temp23.point.y = Mathf.Lerp(v2.point.y, v3.point.y, t);
                temp23.point.z = Mathf.Lerp(v2.point.z, v3.point.z, t);
                Mathf.Lerp(ref temp23, v2, v3, t);

                Vertex temp12 = new Vertex();
                t = pv2 / dv1v2;
                temp12.point.x = Mathf.Lerp(v2.point.x, v1.point.x, t);
                temp12.point.y = Mathf.Lerp(v2.point.y, v1.point.y, t);
                temp12.point.z = Mathf.Lerp(v2.point.z, v1.point.z, t);
                Mathf.Lerp(ref temp12, v2, v1, t);

                clipQueue.Enqueue(new Triangle(temp23, temp12, v2, startIndex + 1));
                return true;
            }
            if (checkIsIn(projectV1, dis) && !checkIsIn(projectV2, dis) && !checkIsIn(projectV3, dis))//v2 v3在外面
            {
                Vertex temp12 = new Vertex();
                t = pv1 / dv1v2;
                temp12.point.x = Mathf.Lerp(v1.point.x, v2.point.x, t);
                temp12.point.y = Mathf.Lerp(v1.point.y, v2.point.y, t);
                temp12.point.z = Mathf.Lerp(v1.point.z, v2.point.z, t);
                Mathf.Lerp(ref temp12, v1, v2, t);

                Vertex temp13 = new Vertex();
                t = pv1 / dv1v3;
                temp13.point.x = Mathf.Lerp(v1.point.x, v3.point.x, t);
                temp13.point.y = Mathf.Lerp(v1.point.y, v3.point.y, t);
                temp13.point.z = Mathf.Lerp(v1.point.z, v3.point.z, t);
                Mathf.Lerp(ref temp13, v1, v3, t);

                clipQueue.Enqueue(new Triangle(temp12, temp13, v1, startIndex + 1));
                return true;
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textureBtn_Click(object sender, EventArgs e)
        {
            isOpenTexture = !isOpenTexture;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void cullingBtn_Click(object sender, EventArgs e)
        {
            //isCull = !isCull;
        }
    }
}
