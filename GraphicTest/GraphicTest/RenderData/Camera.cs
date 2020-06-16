using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicTest.TransformData;
namespace GraphicTest.RenderData
{
    class Camera
    {
        public Vector4 eyePosition;
        public Vector4 up;
        public Vector4 lookAtPos;

        /// <summary>
        /// 观察角，弧度
        /// </summary>
        public float fov;

        /// <summary>
        /// 纵宽比 长宽比
        /// </summary>
        public float aspect;
        
        /// <summary>
        /// 进裁平面到原点距离
        /// </summary>
        public float nearOrigin;

        /// <summary>
        /// 远裁平面到原点距离
        /// </summary>
        public float farOrigin;

        public Camera(Vector4 eyePos,Vector4 nUp,Vector4 lookPos,float nFov,float nAspect,float nearLength,float farLength)
        {
            this.eyePosition = eyePos;
            this.up = nUp;
            this.lookAtPos = lookPos;
            this.fov = nFov;
            this.aspect = nAspect;
            this.nearOrigin = nearLength;
            this.farOrigin = farLength;
        }

        /// <summary>
        /// 创建视图矩阵
        /// 视图矩阵为右手坐标系，我们之前使用的都是左手坐标系
        /// 所以需要先转成右手坐标系
        /// </summary>
        /// <param name="eyePos"></param>
        /// <param name="nUp"></param>
        /// <param name="lookAtPos"></param>
        /// <returns></returns>
        public static Matrix4x4 BuildViewMatrix(Vector4 eyePos,Vector4 nUp,Vector4 lookAtPos)
        {
            Vector4 forward = lookAtPos - eyePos;
            forward.Normalize();
            Vector4 side = Vector4.Corss(nUp, forward);
            side.Normalize();
            Vector4 up = Vector4.Corss(forward, side);
            up.Normalize();

            Matrix4x4 m1 = new Matrix4x4
            (
                1, 0, 0, -eyePos.x,
                0, 1, 0, -eyePos.y,
                0, 0, 1, -eyePos.z,
                0, 0, 0, 1
            );

            Matrix4x4 m2 = new Matrix4x4
            (
                side.x, side.y, side.z, 0,
                up.x, up.y, up.z, 0,
                forward.x, forward.y, forward.z, 0,
                0, 0, 0, 1
            );

            Matrix4x4 m3 = Matrix4x4.identityMatrix;
            m3[2, 2] = -1;
            return m3 * m2 * m1;
        }


        /// <summary>
        ///  求投影矩阵
        /// </summary>
        /// <param name="fov"></param>
        /// <param name="aspect">纵横比</param>
        /// <param name="zn">近裁切面到原点的距离</param>
        /// <param name="zf">远裁切面到原点的距离</param>
        /// <returns></returns>
        public static Matrix4x4 BuildProjectionMatrix(float fov, float aspect, float zn, float zf)
        {
            Matrix4x4 proj = new Matrix4x4
            {
                [0, 0] = (float)(1 / (Math.Tan(fov * 0.5f) * aspect)),
                [1, 1] = (float)(1 / Math.Tan(fov * 0.5f)),
                [2, 2] = -(zf + zn) / (zf - zn),
                [3, 2] = -1.0f,
                [2, 3] = (2 * zn * zf) / (zn - zf)
            };

            return proj;
        }

        public static bool BackFaceCulling(Vertex p1, Vertex p2, Vertex p3)
        {
            //其中p1 P2 p3必定严格按照逆时针或者顺时针的顺序存储
            //而且p1 p2 p3的point必须是视图空间的坐标
            Vector3 v1 = p2.point - p1.point;
            Vector3 v2 = p3.point - p2.point;
            Vector3 normal = Vector3.Cross(v1, v2); //计算法线
            //由于在视空间中，所以相机点就是（0,0,0）
            Vector3 viewDir = p1.point - Vector4.ZeroVector4;
            return Vector3.Dot(normal, viewDir) > 0;
        }

    }
}
