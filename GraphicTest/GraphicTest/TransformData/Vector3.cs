using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest
{
    public struct Vector3 : IEquatable<Vector3>
    {
        private static readonly Vector3 zeroVector3 = new Vector3(0.0f,0.0f,0.0f);
        private static readonly Vector3 oneVector3 = new Vector3(1.0f,1.0f,1.0f);
        private static readonly Vector3 upVector3 = new Vector3(0.0f,1.0f,0.0f);
        private static readonly Vector3 downVector3 = new Vector3(0.0f, -1.0f, 0.0f);
        private static readonly Vector3 leftVector3 = new Vector3(1.0f, 0.0f, 0.0f);
        private static readonly Vector3 rightVector3 = new Vector3(-1.0f, 0.0f, 0.0f);
        private static readonly Vector3 forwardVector3 = new Vector3(0.0f, 0.0f, 1.0f);
        private static readonly Vector3 backVector3 = new Vector3(0.0f, 0.0f, -1.0f);

        public float x;
        public float y;
        public float z;

        public Vector3(float x = 0.0f,float y = 0.0f,float z = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ZeroVector3
        {
            get { return zeroVector3; }
        }

        public Vector3 OneVector3
        {
            get { return oneVector3; }
        }

        public Vector3 UpVector3
        {
            get { return upVector3; }
        }

        public Vector3 DownVector3
        {
            get { return downVector3; }
        }

        public Vector3 LeftVector3
        {
            get { return leftVector3; }
        }

        public Vector3 RightVector3
        {
            get { return rightVector3; }
        }

        public Vector3 ForwardVector3
        {
            get { return forwardVector3; }
        }

        public Vector3 BackVector3
        {
            get { return backVector3; }
        }

        public float magnitude => (float)Math.Sqrt(Dot(this,this));

        public static float Dot(Vector3 v1,Vector3 v2)
        {
            return (float)(v1.x * v2.x + v1.y * v2.y + v1.z * v2.z);
        }
        //叉乘
        public static Vector3 Cross(Vector3 v1, Vector3 r2)
        {
            return new Vector3((float)(v1.y * (double)r2.z - v1.z * (double)r2.y),
                (float)(v1.z * (double)r2.x - v1.x * (double)r2.z),
                (float)(v1.x * (double)r2.y - v1.y * (double)r2.x));
        }

        public static Vector3 operator +(Vector3 v1,Vector3 v2)
        {
            return new Vector3(v1.x + v2.x , v1.y + v2.y , v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator *(Vector3 v1,Vector3 v2)
        {
            return new Vector3(v1.x * v2.x , v1.y * v2.y ,v1.z * v2.z);
        }

        public static Vector3 operator /(Vector3 v1,Vector3 v2)
        {
            if (v2.x != 0 && v2.y != 0 && v2.z != 0)
                return new Vector3(v1.x / v2.x , v1.y / v2.y , v1.z / v2.z);
            return new Vector3();
        }

        public static Vector3 operator *(Vector3 v1,float num)
        {
            return new Vector3(v1.x * num , v1.y * num ,v1.z*num);
        }

        public static Vector3 operator /(Vector3 v1,float num)
        {
            if (num != 0)
                return new Vector3(v1.x / num , v1.y / num , v1.z / num);
            return new Vector3();
        }

        public static Vector3 operator -(Vector3 v1)
        {
            return new Vector3(-v1.x , -v1.y , -v1.z);
        }

        public static bool operator ==(Vector3 v1,Vector3 v2)
        {
            return (v1 - v2).magnitude < 1e-7;
        }

        public static bool operator !=(Vector3 v1,Vector3 v2)
        {
            return !(v1 == v2);
        }

        public static implicit operator Vector3(Vector2 v2)
        {
            Vector3 v1 = new Vector3(v2.x , v2.y,0.0f);
            return v1;
        }

        public static implicit operator Vector3(Vector4 v4)
        {
            Vector3 v1 = new Vector3(v4.x,v4.y,v4.z);
            return v1;
        }



        public bool Equals(Vector3 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public Vector3 Normalize()
        {
            if (magnitude > 0)
                this /= magnitude;
            else
                this = ZeroVector3;
            return this;
        }

        public override string ToString()
        {
            return $"x={x} y={y} z={z}";
        }
    }
}
