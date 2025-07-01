using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AFSlider), true)]
public class AFSliderEditor : SliderEditor
{
    private SerializedProperty m_segments;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_segments = serializedObject.FindProperty("segments");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_segments);

        serializedObject.ApplyModifiedProperties();
    }
}
