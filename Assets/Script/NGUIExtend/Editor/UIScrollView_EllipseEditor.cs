using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIScrollView_Ellipse), true)]
public class UIScrollView_EllipseEditor : UIWidgetContainerEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        NGUIEditorTools.DrawProperty("goCenter", serializedObject, "m_goCenter");

        NGUIEditorTools.DrawProperty("AxisA", serializedObject, "m_fAxisA");
        NGUIEditorTools.DrawProperty("AxisB", serializedObject, "m_fAxisB");
        NGUIEditorTools.DrawProperty("DegreePointPosOffset", serializedObject, "m_fDegreePointPosOffset");
        NGUIEditorTools.DrawProperty("DegreeRotateOffsetX", serializedObject, "m_fDegreeRotateOffsetX");
        NGUIEditorTools.DrawProperty("DegreeRotateOffsetY", serializedObject, "m_fDegreeRotateOffsetY");
        NGUIEditorTools.DrawProperty("DegreeRotateOffsetZ", serializedObject, "m_fDegreeRotateOffsetZ");

        NGUIEditorTools.DrawProperty("ChildRoot", serializedObject, "m_trRoot");
        NGUIEditorTools.DrawProperty("ScrollBar", serializedObject, "m_sbValue");
        NGUIEditorTools.DrawProperty("hideInactive", serializedObject, "hideInactive");

        SerializedProperty spMovement = NGUIEditorTools.DrawProperty("Movement", serializedObject, "movement");
        if (((UIScrollView_Ellipse.Movement)spMovement.intValue) == UIScrollView_Ellipse.Movement.Custom)
        {
            NGUIEditorTools.SetLabelWidth(17f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(120f);
            NGUIEditorTools.DrawProperty("X", serializedObject, "customMovement.x");
            NGUIEditorTools.DrawProperty("Y", serializedObject, "customMovement.y");
            GUILayout.EndHorizontal();
        }

        NGUIEditorTools.SetLabelWidth(130f);

        NGUIEditorTools.DrawProperty("momentumAmount", serializedObject, "momentumAmount");
        NGUIEditorTools.DrawProperty("dampenStrength", serializedObject, "dampenStrength");

        NGUIEditorTools.DrawProperty("BoxCross", serializedObject, "mBoxCross");

        serializedObject.ApplyModifiedProperties();
    }
}