using GraphicTest.RenderData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.TransformData
{
    class Mathf
    {

        public static float Sin(float f)
        {
            return (float)Math.Sin((double)f);
        }

        public static float Cos(float f)
        {
            return (float)Math.Cos((double)f);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value > max)
                return max;
            return value < min ? min : value;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Lerp(float a, float b, float lerp)
        {
            lerp = Clamp(lerp, 0, 1);
            return a + (b - a) * lerp;
        }
        public static void Lerp(ref Color c, Color c1, Color c2, float t)
        {
            if (t < 0)
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }
            c.R = c1.R + (c2.R - c1.R) * t;
            c.G = c1.G + (c2.G - c1.G) * t;
            c.B = c1.B + (c2.B - c1.B) * t;
            //c.R = t * c2.R + (1 - t) * c1.R;
            //c.G = t * c2.G + (1 - t) * c1.G;
            //c.B = t * c2.B + (1 - t) * c1.B;
        }

        public static void Lerp(ref Vertex v, Vertex v1, Vertex v2, float t)
        {
            //颜色插值
            Lerp(ref v.pointColor, v1.pointColor, v2.pointColor, t);
            //uv插值
            v.u = Lerp(v1.u, v2.u, t);
            v.v = Lerp(v1.v, v2.v, t);
            //光照颜色插值
            Lerp(ref v.lightingColor, v1.lightingColor, v2.lightingColor, t);
            //插值矫正系数
            v.onePerZ = Lerp(v1.onePerZ, v2.onePerZ, t);
            v.depth = Lerp(v1.depth, v2.depth, t);
        }
    }
}
