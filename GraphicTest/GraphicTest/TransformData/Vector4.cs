using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest
{
    public struct Vector4 : IEquatable<Vector4>
    {
        private static readonly Vector4 zeroVector4 = new Vector4(0.0f,0.0f,0.0f,0.0f);
        private static readonly Vector4 oneVector4 = new Vector4(1,1,1,1);

        public float x;
        public float y;
        public float z;
        public float w;
	 public float eee;
        public Vector4 ZeroVector4
        {
            get { return zeroVector4; }
        }

        public Vector4 OneVector4
        {
            get { return oneVector4; }
        }

        public Vector4(float x = 0.0f,float y = 0.0f,float z = 0.0f,float w = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float magnitude => (float)Math.Sqrt(Dot(this,this));

        public static float Dot(Vector4 v1,Vector4 v2)
        {
            return (float)(v1.x * v2.x + v1.y *v2.y + v1.z * v2.z + v1.w * v2.w);
        }

        public static Vector4 Corss(Vector4 v1,Vector4 v2)
        {
            float x = v1.y * v2.z - v1.z * v2.y;
            float y = v1.z * v2.x - v1.x * v2.z;
            float z = v1.x * v2.y - v1.y * v2.x;
            return new Vector4(x,y,z);
        }


        public static Vector4 operator +(Vector4 v1,Vector4 v2)
        {
            return new Vector4(v1.x + v2.x , v1.y + v2.y , v1.z + v2.z , v1.w + v2.w);
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        public static Vector4 operator *(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }

        public static Vector4 operator /(Vector4 v1, Vector4 v2)
        {
            if (v2.x != 0 && v2.y != 0 && v2.z != 0 && v2.w != 0)
                return new Vector4(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z, v1.w / v2.w);
            return new Vector4();
        }

        public static Vector4 operator *(Vector4 v1, float num)
        {
            return new Vector4(v1.x * num ,v1.y * num, v1.z * num, v1.w * num);
        }

        public static Vector4 operator /(Vector4 v1, float num)
        {
            if (num != 0)
                return new Vector4(v1.x / num, v1.y / num, v1.z / num, v1.w / num);
            return new Vector4();
        }

        public static Vector4 operator -(Vector4 v1)
        {
            return new Vector4(-v1.x, -v1.y, -v1.z, -v1.w);
        }

        public static bool operator ==(Vector4 v1,Vector4 v2)
        {
            return (v1 - v2).magnitude < 1e-7;
        }

        public static bool operator !=(Vector4 v1, Vector4 v2)
        {
            return !(v1 == v2);
        }

        public static implicit operator Vector4(Vector2 v2)
        {
            Vector4 v = new Vector4(v2.x,v2.y,0,0);
            return v;
        }

        public static implicit operator Vector4(Vector3 v3)
        {
            Vector4 v = new Vector4(v3.x, v3.y, v3.z, 0);
            return v;
        }

        public bool Equals(Vector4 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                hashCode = (hashCode * 397) ^ w.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"x={x} y={y} z={z} w={w}";
        }

        public Vector4 Normalize()
        {
            if (magnitude > 0)
                this /= magnitude;
            else
                this = ZeroVector4;
            return this;
        }

        /// <summary>
        /// 取反
        /// </summary>
        public static Vector4 opposite(Vector4 v)
        {
            return new Vector4(-v.x, -v.y, -v.z, v.w);
        }
    }
}
