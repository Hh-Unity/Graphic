using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.TransformData
{
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {

        private float m00;
        private float m10;
        private float m20;
        private float m30;
        private float m01;
        private float m11;
        private float m21;
        private float m31;
        private float m02;
        private float m12;
        private float m22;
        private float m32;
        private float m03;
        private float m13;
        private float m23;
        private float m33;


        public Matrix4x4(Vector4 column0,Vector4 column1,Vector4 column2,Vector4 column3)
        {
            m00 = column0.x;
            m01 = column1.x;
            m02 = column2.x;
            m03 = column3.x;
            m10 = column0.y;
            m11 = column1.y;
            m12 = column2.y;
            m13 = column3.y;
            m20 = column0.z;
            m21 = column1.z;
            m22 = column2.z;
            m23 = column3.z;
            m30 = column0.w;
            m31 = column1.w;
            m32 = column2.w;
            m33 = column3.w;
        }
        public bool Equals(Matrix4x4 other)
        {
            throw new NotImplementedException();
        }
    }
}
