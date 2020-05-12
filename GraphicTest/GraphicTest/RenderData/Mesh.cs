using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest.RenderData
{
    class Mesh
    {

        public Vertex[] vertices { get; private set; }
        public Material material { get; private set; }

        public Mesh(Vector4[] pointList,int[] indexs,Vector2[] UVs,Vector3[] vertColors, Vector4[] normals, Material mat)
        {
            vertices = new Vertex[indexs.Length];
            for (int i = 0;i<indexs.Length;i++)
            {
                int pointIndex = indexs[i];
                Vector4 point = pointList[pointIndex];
                vertices[i] = new Vertex(point,normals[i],UVs[i].x,UVs[i].y,vertColors[i].x,vertColors[i].y,vertColors[i].z);
            }
            material = mat;
        }
    }
}
