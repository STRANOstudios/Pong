namespace AndreaFrigerio.Core.Editor.Gameplay
{
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using AndreaFrigerio.Core.Runtime.Gameplay;

    /// <summary>
    /// Custom editor for the <see cref="GoalDetector"/> class.
    /// </summary>
    [CustomEditor(typeof(GoalDetector), true)]
    public class GoalDetectorEditor : OdinEditor { }
}
