namespace AndreaFrigerio.UI.Editor.HUD
{
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using AndreaFrigerio.UI.Runtime.HUD;

    /// <summary>
    /// Custom editor for the <see cref="VictoryPanel"/> class.
    /// </summary>
    [CustomEditor(typeof(VictoryPanel), true)]
    public class VictoryPanelEditor : OdinEditor { }
}
