﻿using GraphicTest.TransformData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.RenderData
{
    public struct Color : IEquatable<Color>
    {
        private float r;
        private float g;
        private float b;

        public Color(float r, float g, float b)
        {
            this.r = Mathf.Clamp(r, 0, 1);
            this.g = Mathf.Clamp(g, 0, 1);
            this.b = Mathf.Clamp(g, 0, 1);
        }

        public float R
        {
            get => r;
            set => r = Mathf.Clamp(value, 0, 1);
        }

        public float G
        {
            get => g;
            set => g = Mathf.Clamp(value,0,1);
        }

        public float B
        {
            get => b;
            set => b = Mathf.Clamp(value,0,1);
        }

        public Color(System.Drawing.Color c)
        {
            r = Mathf.Clamp((float)c.R / 255, 0, 1);
            g = Mathf.Clamp((float)c.G / 255, 0, 1);
            b = Mathf.Clamp((float)c.B / 255, 0 ,1);
        }

        public static Color operator *(Color c1,Color c2)
        {
            Color c = new Color
            {
                r = c1.r * c2.r,
                g = c1.g * c2.g,
                b = c1.b * c2.b
            };
            return c;
        }

        public static Color operator *(Color c1 ,float num)
        {
            Color c = new Color
            {
                r = c1.r * num,
                g = c1.g * num,
                b = c1.b * num
            };
            return c;
        }

        public static Color operator +(Color c1,Color c2)
        {
            return new Color { r = c1.r + c2.r, g = c1.g + c2.g, b = c1.b + c2.b };
        }

        public static bool operator ==(Color c1,Color c2)
        {
            return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
        }

        public static bool operator !=(Color c1,Color c2)
        {
            return !(c1 == c2);
        }


        public System.Drawing.Color TransToSystemColor()
        {
            float r = Mathf.Clamp(this.r * 255, 0, 255);
          
            float g = Mathf.Clamp(this.g * 255, 0, 255);
            float b = Mathf.Clamp(this.b * 255, 0, 255);
            return System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
        }

        public void Reset()
        {
            this.r = 1;
            this.g = 1;
            this.b = 1;
        }
        public bool Equals(Color other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = r.GetHashCode();
                hashCode = (hashCode * 397) ^ g.GetHashCode();
                hashCode = (hashCode * 397) ^ b.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"R={r} G={g} B={b}";
        }
    }
}
