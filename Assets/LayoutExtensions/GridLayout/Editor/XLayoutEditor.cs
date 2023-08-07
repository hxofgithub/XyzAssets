
using UnityEditor;
using UnityEngine.ILayoutExtensions;

[CustomEditor(typeof(XGridLayoutGroup))]
public class XGridLayoutGroupEditor : Editor
{
    SerializedProperty m_Padding;
    SerializedProperty m_CellSize;
    SerializedProperty m_Spacing;
    SerializedProperty m_StartAxis;
    SerializedProperty m_ChildAlignment;
    SerializedProperty m_Constraint;
    SerializedProperty m_ConstraintCount;
    SerializedProperty m_ScrollRect;
    XGridLayoutGroup xLayout;
    private void OnEnable()
    {
        m_Padding = serializedObject.FindProperty("m_Padding");
        m_CellSize = serializedObject.FindProperty("m_CellSize");
        m_Spacing = serializedObject.FindProperty("m_Spacing");
        m_StartAxis = serializedObject.FindProperty("m_StartAxis");
        m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
        m_Constraint = serializedObject.FindProperty("m_Constraint");
        m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
        m_ScrollRect = serializedObject.FindProperty("m_ScrollRect");        
        xLayout = target as XGridLayoutGroup;

        if(xLayout.scrollRect == null)
        {
            m_ScrollRect.objectReferenceValue = xLayout.GetComponentInParent<UnityEngine.UI.ScrollRect>();
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();        
        EditorGUILayout.PropertyField(m_Padding, true);
        EditorGUILayout.PropertyField(m_CellSize, true);
        EditorGUILayout.PropertyField(m_Spacing, true);
        EditorGUILayout.PropertyField(m_StartAxis, true);
        EditorGUILayout.PropertyField(m_ChildAlignment, true);
        EditorGUILayout.PropertyField(m_Constraint, true);        
        if (m_Constraint.enumValueIndex > 0)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_ConstraintCount, true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (m_ScrollRect.objectReferenceValue == null)
            EditorGUILayout.HelpBox("This component require ScrollRect.", MessageType.Error);
        EditorGUILayout.PropertyField(m_ScrollRect, true);        
        serializedObject.ApplyModifiedProperties();
        xLayout.Reposition();
    }
}
