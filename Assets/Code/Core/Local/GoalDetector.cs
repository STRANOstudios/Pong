namespace AndreaFrigerio.Pong.Core.Local
{
    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Gameplay;
    
    /// <summary>
    /// Trigger placed behind each paddle. When the server detects that the
    /// ball crossed it, a point is awarded to the opposing side.
    /// </summary>
    [HideMonoScript]
    [RequireComponent(typeof(BoxCollider2D))]
    [AddComponentMenu("Andrea Frigerio/Local/Goal Detector")]
    public class GoalDetector : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Which side of the field this goal belongs to.")]
        [SerializeField]
        private PlayerSide m_side;

        public static event Action<PlayerSide> OnGoal;

        /// <summary>
        /// Server-side trigger callback that awards the point.
        /// </summary>
        /// <param name="col">Collider that entered the goal area.</param>
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Ball"))
            {
                return;
            }

            PlayerSide scorer = m_side == PlayerSide.Left ? PlayerSide.Right : PlayerSide.Left;
            OnGoal?.Invoke(scorer);
        }
    }
}