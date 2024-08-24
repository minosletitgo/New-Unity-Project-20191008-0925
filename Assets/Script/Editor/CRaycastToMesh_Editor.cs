using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CRaycastToMesh_Monobehaviour))]
public class CRaycastToMesh_Editor : Editor
{
    static string m_strLocation;


    private void OnSceneGUI()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("EarthRayTest"))))
        {
            Debug.Log(GameCommon.PrintVector3(hit.point));
            m_strLocation = GameCommon.PrintVector3(hit.point);

            Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("Location: " + m_strLocation);
    }
}
