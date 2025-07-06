namespace AndreaFrigerio.Core.Editor.Gameplay
{
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using AndreaFrigerio.Core.Runtime.Gameplay;

    /// <summary>
    /// Custom editor for the <see cref="PongGameManager"/> class.
    /// </summary>
    [CustomEditor(typeof(PongGameManager), true)]
    public class PongGameManagerEditor : OdinEditor { }
}
