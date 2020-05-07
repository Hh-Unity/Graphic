using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest
{
    public struct Vector2 : IEquatable<Vector2>
    {
        private static readonly Vector2 zeroVector2 = new Vector2(0.0f,0.0f);
        private static readonly Vector2 oneVector2 = new Vector2(1.0f,1.0f);

        public static Vector2 ZeroVector2
        {
            get
            {
                return zeroVector2;
            }
        }

        public static Vector2 OneVector2
        {
            get
            {
                return oneVector2;
            }
        }

        //向量的模长
        public float magnitude => (float)Math.Sqrt(x*x+y*y);

        //点乘 
        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return (float)(v1.x * v2.x + v1.y * v2.y);
        }

        public float x;
        public float y;

        public Vector2(float x = 0.0f,float y = 0.0f)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 v1,Vector2 v2)
        {
            return new Vector2(v1.x + v2.x , v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1,Vector2 v2)
        {
            return new Vector2(v1.x - v2.x , v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1,Vector2 v2)
        {
            return new Vector2(v1.x * v2.x , v1.y * v2.y);
        }

        public static Vector2 operator /(Vector2 v1,Vector2 v2)
        {
            if (v2.x != 0.0f && v2.y != 0.0f)
                return new Vector2(v1.x / v2.x , v2.y / v2.y);
            return new Vector2();
        }

        public static Vector2 operator *(Vector2 v1,float num)
        {
            return new Vector2(v1.x * num,v1.y * num);
        }

        public static Vector2 operator /(Vector2 v1,float num)
        {
            if (num != 0.0f)
                return new Vector2(v1.x / num, v1.y / num);
            return new Vector2();
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return new Vector2(-v1.x , -v1.y);
        }

        public static bool operator ==(Vector2 v1,Vector2 v2)
        {
            return (v1 - v2).magnitude < 1e-7;
        }

        public static bool operator !=(Vector2 v1,Vector2 v2)
        {
            return !(v1 == v2);
        }

        public static implicit operator Vector2(Vector3 v3)
        {
            Vector2 v2 = new Vector2(v3.x,v3.y);
            return v2;
        }

        public static implicit operator Vector2(Vector4 v4)
        {
            Vector2 v2 = new Vector2(v4.x,v4.y);
            return v2;
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                return hashCode;
            }
        }
        //归一化，也就是把向量转换为单位向量的过程，计算公式就是向量除以模长
        public void Normalize()
        {
            if (magnitude > 0)
                this = this / magnitude;
            else
                this = ZeroVector2;
        }


    }
}
