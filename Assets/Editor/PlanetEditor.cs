using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class NewBehaviourScript : Editor
{
    Planet m_planet;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSettingsEditor(m_planet.ShapeSettings, m_planet.OnShapeSettingsUpdated);
    }

    void DrawSettingsEditor(Object _settings, System.Action _onSettingsChanged)
    {
        using (var check = new EditorGUI.ChangeCheckScope()) 
        {
            EditorGUILayout.InspectorTitlebar(true, _settings); //ajouter une barre de titre dépliante

            Editor editor = CreateEditor(_settings);
            editor.OnInspectorGUI();
            if (check.changed)
            {
                _onSettingsChanged();
            }
        }
    }

    private void OnEnable()
    {
        m_planet = (Planet)target;
    }
}
