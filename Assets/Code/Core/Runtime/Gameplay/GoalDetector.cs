namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Locator;

    /// <summary>
    /// Trigger placed behind each paddle. When the server detects that the
    /// ball crossed it, a point is awarded to the opposing side.
    /// </summary>
    [HideMonoScript]
    [RequireComponent(typeof(BoxCollider2D), typeof(NetworkIdentity))]
    [AddComponentMenu("Andrea Frigerio/Game/Goal Detector")]
    public sealed class GoalDetector : NetworkBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Which side of the field this goal belongs to.")]
        [SerializeField] 
        private PlayerSide m_side;

        private IScoreService m_score;

        /// <summary>
        /// Caches the scoring service from the <see cref="ServiceLocator"/>.
        /// </summary>
        public override void OnStartServer() =>
            this.m_score = ServiceLocator.Get<IScoreService>();

        /// <summary>
        /// Server-side trigger callback that awards the point.
        /// </summary>
        /// <param name="col">Collider that entered the goal area.</param>
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Ball"))
            {
                return;
            }

            PlayerSide scorer =
                this.m_side == PlayerSide.Left ? PlayerSide.Right : PlayerSide.Left;

            this.m_score.ServerAddScore(scorer);
        }
    }
}
