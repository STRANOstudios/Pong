namespace AndreaFrigerio.Pong.Core.Local
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Sirenix.OdinInspector;
    using PsychoGarden.TriggerEvents;
    using AndreaFrigerio.Core.Runtime.Gameplay;
    using AndreaFrigerio.Core.Runtime.Locator;

    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Local/Game Manager")]
    public class PongGameManager : MonoBehaviour
    {
        #region Exposed Members

        [BoxGroup("Settings")]
        [Tooltip("Ball prefab spawned at runtime."), AssetsOnly]
        [SerializeField]
        private GameObject m_ballPrefab;

        [BoxGroup("Settings")]
        [Tooltip("Half height of the playing field (UU)."), MinValue(0f)]
        [SerializeField]
        private float m_halfCourt = 5f;

        [BoxGroup("Settings")]
        [Tooltip("Points required to win the match."), MinValue(1)]
        [SerializeField]
        private int m_scoreToWin = 11;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private InputActionReference m_escapeKey;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, Required]
        private TriggerEvent m_endMatch;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, Required]
        private TriggerEvent m_quit;

        #endregion

        #region Private Members

        private int m_left;
        private int m_right;

        public int LeftScore => this.m_left;
        public int RightScore => this.m_right;

        private GameObject m_ball;

        public static event Action OnGoal;
        public static event Action OnRestart;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            ServiceLocator.Register(this);
            SpawnBall();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this);
        }

        #region Unity Lifecycle

        private void OnEnable()
        {
            if (this.m_escapeKey != null)
            {
                this.m_escapeKey.action.performed += OnEscapePerformed;
            }

            GoalDetector.OnGoal += AddScore;
        }

        private void OnDisable()
        {
            if (this.m_escapeKey != null)
            {
                this.m_escapeKey.action.performed -= OnEscapePerformed;
            }

            GoalDetector.OnGoal -= AddScore;
        }

        #endregion

        #endregion

        #region Game Logic

        private void AddScore(PlayerSide player)
        {
            if (player == PlayerSide.Left)
            {
                this.m_left++;
            }
            else
            {
                this.m_right++;
            }

            OnGoal?.Invoke();

            if (this.m_left >= this.m_scoreToWin || this.m_right >= this.m_scoreToWin)
            {
                EndMatch();
            }
            else
            {
                ResetBall(player == PlayerSide.Left ? Vector2.right : Vector2.left);
            }
        }

        private void EndMatch()
        {
            this.m_endMatch?.Invoke(this.transform);
            this.m_ball.gameObject.SetActive(false);
        }

        private void OnEscapePerformed(InputAction.CallbackContext ctx)
        {
            Quit();
        }

        private void SpawnBall()
        {
            this.m_ball = Instantiate(this.m_ballPrefab);
            ResetBall(UnityEngine.Random.value < .5f ? Vector2.left : Vector2.right);
        }

        private void ResetBall(Vector2 dir)
        {
            float y = UnityEngine.Random.Range(-this.m_halfCourt, this.m_halfCourt);
            this.m_ball.transform.position = new Vector2(0, y);
            this.m_ball.GetComponent<BallController>().ResetForNewRound(dir);
        }

        #endregion

        #region Public Methods

        public void Quit()
        {
            this.m_quit?.Invoke(this.transform);
        }

        public void RestartMatch()
        {
            OnRestart?.Invoke();
            this.m_left = this.m_right = 0;
            this.m_ball.gameObject.SetActive(true);
            ResetBall(UnityEngine.Random.value < .5f ? Vector2.left : Vector2.right);
        }

        public PlayerSide? HowToWin() => this.m_left >= this.m_scoreToWin ? PlayerSide.Left : this.m_right >= this.m_scoreToWin ? PlayerSide.Right : null;

        #endregion

        #region Gizmo

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 size = new(0f, this.m_halfCourt, 0f);
            Gizmos.DrawLine(transform.position - size, transform.position + size);
        }

        #endregion
    }
}
