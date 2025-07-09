namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    using System;
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Server‑authoritative ball controller that emulates the original Atari Pong
    /// (1972) three‑step speed timing:
    /// <list type="bullet">
    /// <item>Slow  ≈ 254 µs horizontal delay (reference ratio 1.0)</item>
    /// <item>Medium ≈ 190 µs (≈ 1.3368 × faster)</item>
    /// <item>Fast  ≈ 127 µs (≈ 2 × faster)</item>
    /// </list>
    ///
    /// The ball accelerates <b>only</b> when it hits a paddle; bounces against the
    /// top/bottom walls keep the current speed. After every goal the speed and
    /// hit counter are reset to the slow level, exactly like on the original
    /// cabinet. All physics are executed on the server; clients merely receive
    /// state replication via <see cref="NetworkRigidbody2D"/>.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    [AddComponentMenu("Andrea Frigerio/Game/Ball Controller")]
    public sealed class BallController : NetworkBehaviour
    {
        #region Inspector

        [BoxGroup("Settings")]
        [Tooltip("Base speed (slow level) in Unity units per second."), SuffixLabel("uu/s", true)] // Unity Units per Second
        [SerializeField, MinValue(0f)]
        private float m_baseSpeed = 20f;

        [BoxGroup("Settings")]
        [Tooltip("Number of paddle hits required to switch from slow → medium.")]
        [SerializeField, MinValue(1)]
        private int m_hitsToMedium = 4;

        [BoxGroup("Settings")]
        [Tooltip("Number of paddle hits required to switch from medium → fast.")]
        [SerializeField, MinValue("@(m_hitsToMedium + 1)")]
        private int m_hitsToFast = 12;

        #endregion

        #region Constants

        // Speed multipliers derived from 254 µs : 190 µs : 127 µs → 1 : 1.3368 : 2
        private static readonly float[] k_SpeedMul = { 1f, 1.3368f, 2f };

        #endregion

        #region Private fields

        private Rigidbody2D m_rb;
        private float m_currentSpeed;
        private int m_paddleHitCount;
        private byte m_speedLevel; // 0 = slow, 1 = medium, 2 = fast

        public static event Action OnPaddleBounce;
        public static event Action OnWallBounce;

        #endregion

        #region Server life‑cycle

        /// <inheritdoc/>
        public override void OnStartServer()
        {
            this.m_rb = GetComponent<Rigidbody2D>();
            this.m_rb.simulated = true; // physics solely on the server
            ResetForNewRound(UnityEngine.Random.value < 0.5f ? Vector2.right : Vector2.left);
        }

        /// <summary>
        /// Resets the speed level, hit counter and position, then launches the
        /// ball towards <paramref name="launchDir"/>. This must be called by the
        /// match controller after every goal.
        /// </summary>
        /// <param name="launchDir">Initial launch direction (normalized).</param>
        [Server]
        public void ResetForNewRound(Vector2 launchDir)
        {
            this.m_paddleHitCount = 0;
            this.m_speedLevel = 0; // slow
            this.m_currentSpeed = this.m_baseSpeed;
            this.m_rb.position = Vector2.zero;
            LaunchTowards(launchDir);
        }

        /// <summary>
        /// Applies the current speed in the given <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Normalized direction vector.</param>
        [Server]
        private void LaunchTowards(Vector2 dir) =>
            this.m_rb.linearVelocity = dir.normalized * this.m_currentSpeed;

        #endregion

        #region Server – collision handling

        /// <summary>
        /// Central collision handler: paddles trigger speed progression, walls
        /// simply reflect the current velocity.
        /// </summary>
        /// <param name="col">Collision information from Unity.</param>
        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.CompareTag("Paddle"))
            {
                Vector2 dir = CalculateBounceDirection(col);
                ApplyPaddleHitSpeed();
                this.m_rb.linearVelocity = dir * this.m_currentSpeed;
                RpcHandleBounce(BounceType.Paddle);
                return;
            }

            if (col.transform.CompareTag("Wall"))
            {
                // No acceleration on wall hits; preserve current magnitude.
                this.m_rb.linearVelocity = this.m_rb.linearVelocity.normalized * this.m_currentSpeed;
                RpcHandleBounce(BounceType.Wall);
            }
        }

        /// <summary>
        /// Calculates the outgoing direction based on where the ball hits the
        /// paddle, reproducing the classic "8‑zone" angle logic.
        /// </summary>
        /// <param name="col">Collision data.</param>
        /// <returns>Normalized bounce direction.</returns>
        [Server]
        private Vector2 CalculateBounceDirection(Collision2D col)
        {
            float yFactor = HitFactor(transform.position,
                                      col.transform.position,
                                      col.collider.bounds.size.y);
            float xDir = col.relativeVelocity.x > 0 ? 1f : -1f;
            return new Vector2(xDir, yFactor).normalized;
        }

        /// <summary>
        /// Increments the paddle‑hit counter and, if the configured thresholds are
        /// met, advances the speed level (slow → medium → fast).
        /// </summary>
        [Server]
        private void ApplyPaddleHitSpeed()
        {
            this.m_paddleHitCount++;

            if (this.m_speedLevel == 0 && this.m_paddleHitCount >= this.m_hitsToMedium)
            {
                this.m_speedLevel = 1;
            }
            else if (this.m_speedLevel == 1 && this.m_paddleHitCount >= this.m_hitsToFast)
            {
                this.m_speedLevel = 2;
            }

            this.m_currentSpeed = this.m_baseSpeed * k_SpeedMul[this.m_speedLevel];
        }

        /// <summary>
        /// Returns a value in the range [‑1, 1] that indicates where the ball hit
        /// the paddle: ‑1 = bottom edge, 0 = centre, 1 = top edge.
        /// </summary>
        /// <param name="ballPos">Ball world position.</param>
        /// <param name="racketPos">Paddle world position.</param>
        /// <param name="racketHeight">Total height of the paddle collider.</param>
        [Server]
        private static float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight) =>
            (ballPos.y - racketPos.y) / racketHeight;

        #endregion

        #region Server

        /// <summary>
        /// Sets the visibility of the ball and its collider.
        /// </summary>
        /// <param name="value">The new visibility state.</param>
        public void SetVisible(bool value)
        {
            this.GetComponentInChildren<SpriteRenderer>().enabled = value;
            this.GetComponent<Collider2D>().enabled = value;
        }

        #endregion

        #region Client – bounce feedback

        private enum BounceType : byte { Paddle = 0, Wall = 1 }

        /// <summary>
        /// Client‑side bounce notification used to trigger sound effects or VFX.
        /// </summary>
        /// <param name="type">Type of surface that was hit.</param>
        [ClientRpc]
        private void RpcHandleBounce(BounceType type)
        {
            switch (type)
            {
                case BounceType.Paddle:
                    OnPaddleBounce?.Invoke();
                    break;
                case BounceType.Wall:
                    OnWallBounce?.Invoke();
                    break;
            }
        }

        #endregion
    }
}
