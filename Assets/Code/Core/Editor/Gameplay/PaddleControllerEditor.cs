namespace AndreaFrigerio.Core.Editor.Gameplay
{
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using AndreaFrigerio.Core.Runtime.Gameplay;

    /// <summary>
    /// Custom editor for the <see cref="PaddleController"/> class.
    /// </summary>
    [CustomEditor(typeof(PaddleController), true)]
    public class PaddleControllerEditor : OdinEditor { }
}
