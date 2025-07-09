namespace AndreaFrigerio.Pong.Core.Local
{
    using System;
    using UnityEngine;
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
    [AddComponentMenu("Andrea Frigerio/Local/Ball Controller")]
    public class BallController : MonoBehaviour
    {
        #region Inspector

        [BoxGroup("Settings")]
        [Tooltip("Base speed (slow level) in Unity units per second.")]
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

        private static readonly float[] k_SpeedMul = { 1f, 1.3368f, 2f };

        #endregion

        #region Private Fields

        private Rigidbody2D m_rb;
        private float m_currentSpeed;
        private int m_paddleHitCount;
        private byte m_speedLevel;

        public static event Action OnPaddleBounce;
        public static event Action OnWallBounce;

        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            this.m_rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            ResetForNewRound(UnityEngine.Random.value < 0.5f ? Vector2.right : Vector2.left);
        }

        #endregion

        #region Public

        public void ResetForNewRound(Vector2 launchDir)
        {
            this.m_paddleHitCount = 0;
            this.m_speedLevel = 0;
            this.m_currentSpeed = this.m_baseSpeed;
            this.m_rb.position = Vector2.zero;
            LaunchTowards(launchDir);
        }

        #endregion

        #region Physics

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.CompareTag("Paddle"))
            {
                Vector2 dir = CalculateBounceDirection(col);
                ApplyPaddleHitSpeed();
                this.m_rb.linearVelocity = dir * this.m_currentSpeed;
                HandleBounce(BounceType.Paddle);
            }
            else if (col.transform.CompareTag("Wall"))
            {
                this.m_rb.linearVelocity = this.m_rb.linearVelocity.normalized * this.m_currentSpeed;
                HandleBounce(BounceType.Wall);
            }
        }

        #endregion

        #region Bounce Logic

        private enum BounceType : byte { Paddle = 0, Wall = 1 }

        private void HandleBounce(BounceType type)
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

        private Vector2 CalculateBounceDirection(Collision2D col)
        {
            float yFactor = HitFactor(transform.position,
                                      col.transform.position,
                                      col.collider.bounds.size.y);
            float xDir = col.relativeVelocity.x > 0 ? 1f : -1f;
            return new Vector2(xDir, yFactor).normalized;
        }

        private static float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight) =>
            (ballPos.y - racketPos.y) / racketHeight;

        private void LaunchTowards(Vector2 dir)
        {
            this.m_rb.linearVelocity = dir.normalized * this.m_currentSpeed;
        }

        #endregion
    }
}
