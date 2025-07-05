namespace AndreaFrigerio.UI.Runtime.Controls
{
    using UnityEditor;
    using UnityEditor.UI;

    /// <summary>
    /// Custom inspector that exposes the <c>segments</c> field on
    /// <see cref="AFSlider"/> while preserving the default Slider UI.
    /// </summary>
    [CustomEditor(typeof(AFSlider), true)]
    public sealed class AFSliderEditor : SliderEditor
    {
        private SerializedProperty m_segments;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_segments = this.serializedObject.FindProperty("m_segments");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_segments);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
