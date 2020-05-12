using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.RenderData
{
    public struct Triangle
    {

        public Vertex[] Vertices { get; }
        public int startIndex;//从哪个平面开始裁剪，默认从近平面


        public Triangle(Vertex v1,Vertex v2,Vertex v3)
        {
            Vertices = new Vertex[3];
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
            startIndex = 0;
        }

        public Triangle(Vertex v1, Vertex v2, Vertex v3, int startIndex)
        {
            Vertices = new Vertex[3];
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
            this.startIndex = startIndex;
        }

        public Vertex this[int index]
        {
            get
            {
                if (index < Vertices.Length)
                {
                    return Vertices[index];
                }
                throw new IndexOutOfRangeException();
            }

            set
            {
                if (index < Vertices.Length)
                {
                    Vertices[index] = value;
                }
                throw new IndexOutOfRangeException();
            }
        }
    }
}
