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
    }
}
