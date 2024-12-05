using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] Material m_material;
    [SerializeField][Range(2, 768)] int m_resolution = 10;

    [SerializeField] ShapeSettings m_shapeSettings;
    public ShapeSettings ShapeSettings { get => m_shapeSettings; }

    [SerializeField, HideInInspector]
    MeshFilter[] m_meshFilters;
    TerrainFace[] m_terrainFaces;

    ShapeGenerator m_shapeGenerator;
    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
        UpdateMaterial();
    }

    void Initialize()
    {
        if(m_meshFilters == null || m_meshFilters.Length == 0)
        {
            m_meshFilters = new MeshFilter[6];
        }
        m_terrainFaces = new TerrainFace[6];

        if (m_material == null) m_material = new Material(Shader.Find("Standard"));
        m_shapeGenerator = new(m_shapeSettings);

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if(m_meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = m_material;
                m_meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                m_meshFilters[i].sharedMesh = new Mesh();
                m_meshFilters[i].sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            m_terrainFaces[i] = new TerrainFace(m_shapeGenerator, m_meshFilters[i].sharedMesh, m_resolution, directions[i]); 
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in m_terrainFaces)
        {
            face.BuildMesh();
        }
    }


    void UpdateMaterial()
    {
        m_material.SetFloat("_Radius", ShapeSettings.planetRadius);
    }


    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateMesh();
        UpdateMaterial();
    }
    
}
