using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings m_settings;

    public ShapeGenerator(ShapeSettings _settings)
    {
        m_settings = _settings;
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        return pointOnUnitSphere * m_settings.planetRadius;
    }
}

