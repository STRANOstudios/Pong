namespace AndreaFrigerio.Core.Editor.Gameplay
{
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using AndreaFrigerio.Core.Runtime.Gameplay;

    /// <summary>
    /// Custom editor for the <see cref="BallController"/> class.
    /// </summary>
    [CustomEditor(typeof(BallController), true)]
    public class BallControllerEditor : OdinEditor { }
}
