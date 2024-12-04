using System;
using System.Collections;
using UnityEngine;


[ExecuteAlways]
public class MeshGenerator : MonoBehaviour
{
    #region FIELDS                                                                                                                   
    [Header("Mesh shape")]
    [SerializeField] bool m_drawGizmos = false;
    [SerializeField] int m_xSize = 64;
    [SerializeField] int m_zSize = 64;
    [SerializeField][Range(0.001f, 6)] float m_resolutionByUnit = 1;

    [Header("UVs settings")]
    [SerializeField] bool m_allowStretch = false; 
    [SerializeField] Vector2 m_uvScale = Vector2.one; 
    [SerializeField] Vector2 m_uvOffset = Vector2.zero; 

    float m_xEdgeLength;
    float m_zEdgeLength;

    int m_xFaces;
    int m_zFaces;

    [Header("animation settings")]
    [SerializeField] [Range(1, 1000)] int m_drawSpeed = 2;
    
    Mesh m_mesh;
    Vector3[] m_vertices;
    int[] m_triangles;

    MeshRenderer m_renderer;

    Coroutine m_coroutine;
    #endregion
    #region LIFECYCLE                                                                                                                   
    // Start is called before the first frame update
    void Start()
    {
        

        m_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_mesh;
        m_renderer = GetComponent<MeshRenderer>();
        m_mesh.MarkDynamic();
    }

    private void OnValidate()
    {
        m_xSize = Mathf.Max(m_xSize, 1);
        m_zSize = Mathf.Max (m_zSize, 1);

        float xDivid = Mathf.Max(1, m_resolutionByUnit * m_xSize);
        float zDivid = Mathf.Max(1, m_resolutionByUnit * m_zSize);

        m_xFaces = Mathf.FloorToInt(xDivid);
        m_zFaces = Mathf.FloorToInt(zDivid);
        m_xEdgeLength = m_xSize / xDivid;
        m_zEdgeLength = m_zSize / zDivid;

        StopCoroutine(ref m_coroutine);
        m_coroutine = StartCoroutine(CreateShape());
    }

    private void Update()
    {
        if(m_coroutine != null)
        {
            UpdateMesh();
        }
    }
    #endregion

    void StopCoroutine(ref Coroutine _coroutine)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    IEnumerator CreateShape()
    {
        // create vertices
        m_vertices = new Vector3[(m_xFaces + 1)*(m_zFaces + 1)];
        for (int i = 0, z = 0; z <= m_zFaces; z++)
        {
            for(int x = 0; x <= m_xFaces; x++)
            {
                m_vertices[i] = new Vector3(x*m_xEdgeLength, 0, z*m_zEdgeLength);
                i++;
            }
        }

        // create triangles
        m_triangles = new int[m_xFaces * m_zFaces * 6];
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < m_zFaces; z++)
        {
            for(int x = 0; x < m_xFaces; x++)
            {
                if(vert % m_drawSpeed == 0) yield return null;

                m_triangles[tris + 0] = vert;
                m_triangles[tris + 1] = vert + m_xFaces + 1;
                m_triangles[tris + 2] = vert + 1;
                m_triangles[tris + 3] = vert + 1;
                m_triangles[tris + 4] = vert + m_xFaces + 1;
                m_triangles[tris + 5] = vert + m_xFaces + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        yield return null; // wait one more frame to let time to update
        m_coroutine = null;
    }

    void UpdateMesh()
    {
        if (m_vertices == null || m_triangles == null) return;

        m_mesh.Clear();
        m_mesh.vertices = m_vertices;
        m_mesh.triangles = m_triangles;

        GenerateUVs();
        //m_mesh.RecalculateNormals();
        GenerateFlatNormals();
        m_mesh.RecalculateTangents();
    }

    private void GenerateUVs()
    {
        Vector2[] uvs = new Vector2[m_vertices.Length];
        for (int i = 0; i < m_vertices.Length; i++)
        {
            // Normalisation des coordonnées (entre 0 et 1)
            float u = m_vertices[i].x / (m_xSize * (m_allowStretch ? 1 : m_resolutionByUnit));
            float v = m_vertices[i].z / (m_zSize * (m_allowStretch ? 1 : m_resolutionByUnit));

            uvs[i] = new Vector2(u, v) * m_uvScale + m_uvOffset;
        }
        m_mesh.uv = uvs;
    }

    void GenerateFlatNormals()
    {
        Vector3[] normals = new Vector3[m_vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up; // Normale vers le haut
        }
        m_mesh.normals = normals;
    }

    private void OnDrawGizmos()
    {
        if (!m_drawGizmos) return;
        if (m_vertices == null) return;

        for (int i = 0; i < m_vertices.Length; i++)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawIcon(m_vertices[i], "sommet");
        }

        for (int i = 0; i < m_vertices.Length; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_vertices[i], m_vertices[i] + m_mesh.normals[i] * 0.5f);
        }
    }
}
