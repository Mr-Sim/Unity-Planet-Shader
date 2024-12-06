using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet m_planet;
    private Editor m_shapeSettingsEditor; // Instance unique pour chaque Planet

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // V�rifie si ShapeSettings est d�fini avant d'appeler DrawSettingsEditor
        if (m_planet.ShapeSettings != null)
        {
            DrawSettingsEditor(m_planet.ShapeSettings, m_planet.OnShapeSettingsUpdated, ref m_shapeSettingsEditor);
        }
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsChanged, ref Editor editor)
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Dessine une barre de titre repliable
            bool foldout = EditorGUILayout.InspectorTitlebar(true, settings);
            if (foldout)
            {
                // Cr�e ou met � jour l'instance d'Editor pour les param�tres
                if (editor == null || editor.target != settings)
                {
                    DestroyImmediate(editor); // D�truit l'ancien �diteur si n�cessaire
                    editor = CreateEditor(settings);
                }

                editor.OnInspectorGUI();
                if (check.changed)
                {
                    onSettingsChanged?.Invoke();
                }
            }
        }
    }

    private void OnEnable()
    {
        m_planet = (Planet)target;
    }

    private void OnDisable()
    {
        // Nettoie l'�diteur pour �viter les fuites m�moire
        if (m_shapeSettingsEditor != null)
        {
            DestroyImmediate(m_shapeSettingsEditor);
        }
    }
}
