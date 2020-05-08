using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.TransformData
{
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {

        public static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vector4(0.0f , 0.0f , 0.0f , 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
                                                                    new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

        public static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vector4(1.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                                                                        new Vector4(0.0f, 0.0f, 1.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

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

        public Matrix4x4(float a1 , float b1 , float c1 , float d1,
                         float a2 , float b2 , float c2 , float d2,
                         float a3 , float b3 , float c3 , float d3,
                         float a4 , float b4 , float c4 , float d4)
        {
            m00 = a1;
            m01 = b1;
            m02 = c1;
            m03 = d1;
            m10 = a2;
            m11 = b2;
            m12 = c2;
            m13 = d2;
            m20 = a3;
            m21 = b3;
            m22 = c3;
            m23 = d3;
            m30 = a4;
            m31 = b4;
            m32 = c4;
            m33 = d4;
        }

        public void Set(float[ , ] arr)
        {
            if (arr.GetLength(0) != 4 || arr.GetLength(1) != 4) return;
            Vector4[] vecs = new Vector4[4];
            for (int i = 0 ; i < arr.GetLength(0) ; i++)
            {
                vecs[i] = new Vector4(arr[i , 0] , arr[i , 1] , arr[i, 2] , arr[i, 3]);
            }
            Set(vecs[0] , vecs[1] , vecs[2] , vecs[3]);
        }

        public void Set(Vector4 column0 , Vector4 column1 , Vector4 column2 , Vector4 column3)
        {
            this = new Matrix4x4(column0 , column1 , column2 , column3);
        }

        public float this[int row, int column]
        {
            get => this[row + column * 4];
            set => this[row + column * 4] = value;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.m00;
                    case 1:
                        return this.m10;
                    case 2:
                        return this.m20;
                    case 3:
                        return this.m30;
                    case 4:
                        return this.m01;
                    case 5:
                        return this.m11;
                    case 6:
                        return this.m21;
                    case 7:
                        return this.m31;
                    case 8:
                        return this.m02;
                    case 9:
                        return this.m12;
                    case 10:
                        return this.m22;
                    case 11:
                        return this.m32;
                    case 12:
                        return this.m03;
                    case 13:
                        return this.m13;
                    case 14:
                        return this.m23;
                    case 15:
                        return this.m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value;
                        break;
                    case 1:
                        this.m10 = value;
                        break;
                    case 2:
                        this.m20 = value;
                        break;
                    case 3:
                        this.m30 = value;
                        break;
                    case 4:
                        this.m01 = value;
                        break;
                    case 5:
                        this.m11 = value;
                        break;
                    case 6:
                        this.m21 = value;
                        break;
                    case 7:
                        this.m31 = value;
                        break;
                    case 8:
                        this.m02 = value;
                        break;
                    case 9:
                        this.m12 = value;
                        break;
                    case 10:
                        this.m22 = value;
                        break;
                    case 11:
                        this.m32 = value;
                        break;
                    case 12:
                        this.m03 = value;
                        break;
                    case 13:
                        this.m13 = value;
                        break;
                    case 14:
                        this.m23 = value;
                        break;
                    case 15:
                        this.m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }

            }
        }
        public bool Equals(Matrix4x4 other)
        {
            return this.GetColumn(0).Equals(other.GetColumn(0)) && this.GetColumn(1).Equals(other.GetColumn(1)) &&
                   this.GetColumn(2).Equals(other.GetColumn(2)) && this.GetColumn(3).Equals(other.GetColumn(3));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Matrix4x4 other && Equals(other);
        }

        /// <summary>
        /// 获得某一列
        /// </summary>
        /// <param name="index">0 - 3</param>
        /// <returns></returns>
        public Vector4 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector4(m00 , m10 , m20 , m30);
                case 1:
                    return new Vector4(m01 , m11 , m21 , m31);
                case 2:
                    return new Vector4(m02 , m12 , m22 , m32);
                case 3:
                    return new Vector4(m03 , m13 , m23 , m33);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        /// <summary>
        /// 获得某一行
        /// </summary>
        /// <param name="index">0 - 3</param>
        /// <returns></returns>
        public Vector4 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector4(m00 , m01 , m02 , m03);
                case 1:
                    return new Vector4(m10 , m11 , m12 , m13);
                case 2:
                    return new Vector4(m20 , m21 , m22 , m23);
                case 3:
                    return new Vector4(m30 , m31 , m32 , m33);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        public static Matrix4x4 Rotate(Vector3 euler)
        {
            return RotateY(euler.y) * RotateX(euler.x) * RotateZ(euler.z);
        }

        /// <summary>
        /// 绕y轴旋转
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static Matrix4x4 RotateY(float rad)
        {
            float sin = Mathf.Sin(rad);
            float cos = (float)Math.Cos(rad);
            Matrix4x4 mat = identityMatrix;
            mat[0,0] = cos;
            mat[0,2] = sin;
            mat[2,0] = -sin;
            mat[2,2] = cos;
            return mat;
        }
        /// <summary>
        /// 绕x轴旋转
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static Matrix4x4 RotateX(float rad)
        {
            float sin = Mathf.Sin(rad);
            float cos = (float)Math.Cos(rad);
            Matrix4x4 mat = identityMatrix;
            mat[1, 1] = cos;
            mat[1, 2] = -sin;
            mat[2, 1] = sin;
            mat[2, 2] = cos;
            return mat;
        }
        /// <summary>
        /// 绕z轴旋转
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static Matrix4x4 RotateZ(float rad)
        {
            float sin = Mathf.Sin(rad);
            float cos = (float)Math.Cos(rad);
            Matrix4x4 mat = identityMatrix;
            mat[0, 0] = cos;
            mat[0, 1] = -sin;
            mat[1, 0] = sin;
            mat[1, 1] = cos;
            return mat;
        }

    }
}
