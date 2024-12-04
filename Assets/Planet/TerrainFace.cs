using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator m_shapeGenerator;

    Mesh m_mesh;
    int m_resolution;

    Vector3 m_localUp;
    Vector3 m_axisA;
    Vector3 m_axisB;



    public TerrainFace(ShapeGenerator _shapeGenerator, Mesh _mesh, int _resolution, Vector3 _localUp)
    {
        m_shapeGenerator = _shapeGenerator;
        m_mesh = _mesh;
        m_resolution = _resolution;
        m_localUp = _localUp;

        m_axisA = new Vector3(m_localUp.y, m_localUp.z, m_localUp.x);
        m_axisB = Vector3.Cross(m_localUp, m_axisA);
    }

    public void BuildMesh()
    {
        Vector3[] vertices = new Vector3[m_resolution * m_resolution];
        int[] triangles = new int[(m_resolution-1)*(m_resolution-1) * 6];

        int i = 0;
        int tri = 0;
        for (int y = 0; y < m_resolution; y++)
        {
            for (int x = 0; x < m_resolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / (m_resolution-1);
                Vector3 pointOnUnitCube = m_localUp + (percent.x - 0.5f) * 2 * m_axisA + (percent.y - 0.5f) * 2 * m_axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = m_shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != m_resolution - 1 && y != m_resolution - 1)
                {
                    triangles[tri] = i;
                    triangles[tri + 1] = i + m_resolution + 1;
                    triangles[tri + 2] = i + m_resolution;

                    triangles[tri + 3] = i;
                    triangles[tri + 4] = i + 1;
                    triangles[tri + 5] = i + m_resolution + 1;
                    tri += 6;
                }

                i++;
            }
        }
        m_mesh.Clear();
        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        GenerateUVs(vertices);
        m_mesh.RecalculateNormals();
    }

    private void GenerateUVs(Vector3[] _vertices)
    {
        Vector2[] uvs = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector3 pointOnSphere = _vertices[i].normalized;

            // Calculer les coordonnées UV
            float u = 0.5f + Mathf.Atan2(pointOnSphere.z, pointOnSphere.x) / (2 * Mathf.PI);
            float v = 0.5f - Mathf.Asin(pointOnSphere.y) / Mathf.PI;

            uvs[i] = new Vector2(u, v);
        }
        m_mesh.uv = uvs;
    }
}
