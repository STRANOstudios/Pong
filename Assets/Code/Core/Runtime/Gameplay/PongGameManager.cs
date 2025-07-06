namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    using System;
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Locator;
    using AndreaFrigerio.UI.Runtime.HUD;
    using PsychoGarden.TriggerEvents;

    /// <summary>
    /// Server-authoritative match controller.<br/>
    /// • Tracks score via <see cref="IScoreService"/>.<br/>
    /// • Spawns / resets the ball.<br/>
    /// • Declares victory and broadcasts UI events.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkIdentity))]
    public sealed class PongGameManager : NetworkBehaviour, IScoreService
    {
        #region Inspector

        [BoxGroup("Settings")]
        [Tooltip("Ball prefab spawned by the server."), AssetsOnly]
        [SerializeField] 
        private BallController m_ballPrefab;

        [BoxGroup("Settings")]
        [Tooltip("Half height of the playing field (UU)."), MinValue(0f)]
        [SerializeField] 
        private float m_halfFieldHeight = 5f;

        [BoxGroup("Settings")]
        [Tooltip("Points required to win the match."), MinValue(1)]
        [SerializeField] 
        private int m_scoreToWin = 11;

        [BoxGroup("Settings")]
        [Tooltip("Event raised on every victory.")]
        [SerializeField] 
        private TriggerEvent m_onWin;

        [BoxGroup("References")]
        [Tooltip("Scene-level HUD component."), Required, SceneObjectsOnly]
        [SerializeField] 
        private ScoreHUD m_hud;

        #endregion

        #region SyncVars

        [SyncVar(hook = nameof(OnScoreChanged))] private int m_scoreLeft;
        [SyncVar(hook = nameof(OnScoreChanged))] private int m_scoreRight;

        #endregion

        #region Private state

        private BallController m_ballInstance;

        #endregion

        #region Events

        /// <summary>
        /// Raised client-side when a goal is scored.
        /// </summary>
        public static event Action OnGoal;

        #endregion

        #region IScoreService

        /// <inheritdoc/>
        [Server]
        public void ServerAddScore(PlayerSide scorer)
        {
            if (scorer == PlayerSide.Left)
            {
                this.m_scoreLeft++;
            }
            else
            {
                this.m_scoreRight++;
            }

            bool hasWinner = this.m_scoreLeft >= this.m_scoreToWin ||
                             this.m_scoreRight >= this.m_scoreToWin;

            RpcOnGoalScored(scorer);

            if (hasWinner)
            {
                PlayerSide winner = this.m_scoreLeft >= this.m_scoreToWin
                                        ? PlayerSide.Left
                                        : PlayerSide.Right;
                EndMatch(winner);
            }
            else
            {
                ResetBall(scorer == PlayerSide.Left ? Vector2.right : Vector2.left);
            }
        }

        #endregion

        #region Unity life-cycle

        /// <summary>
        /// Registers the score service and starts the first rally.
        /// </summary>
        public override void OnStartServer()
        {
            ServiceLocator.Register<IScoreService>(this);
            SpawnAndLaunchBall();
            UpdateHud();
        }

        /// <summary>
        /// Synchronises HUD for late-join clients.
        /// </summary>
        public override void OnStartClient() => UpdateHud();

        /// <inheritdoc/>
        public override void OnStopServer() =>
            ServiceLocator.Unregister<IScoreService>(this);

        #endregion

        #region Ball management

        /// <summary>
        /// Instantiates the ball once and launches the first serve.
        /// </summary>
        [Server]
        private void SpawnAndLaunchBall()
        {
            this.m_ballInstance = Instantiate(this.m_ballPrefab);
            NetworkServer.Spawn(this.m_ballInstance.gameObject);

            Vector2 dir = UnityEngine.Random.value < .5f ? Vector2.right : Vector2.left;
            ResetBall(dir);
        }

        /// <summary>
        /// Moves the ball to centre-line random Y and launches it in
        /// <paramref name="dir"/>. Called after each goal.
        /// </summary>
        /// <param name="dir">Normalized serve direction.</param>
        [Server]
        private void ResetBall(Vector2 dir)
        {
            float randY = UnityEngine.Random.Range(-this.m_halfFieldHeight,
                                                    this.m_halfFieldHeight);

            this.m_ballInstance.transform.position = new Vector2(0f, randY);
            this.m_ballInstance.ResetForNewRound(dir);
        }

        #endregion

        #region Victory & replay

        /// <summary>
        /// Handles end-of-match: hides ball and notifies clients.
        /// </summary>
        /// <param name="winner">Side that reached <see cref="m_scoreToWin"/>.</param>
        [Server]
        private void EndMatch(PlayerSide winner)
        {
            this.m_ballInstance.gameObject.SetActive(false);
            RpcGameWon(winner, this.m_scoreLeft, this.m_scoreRight);
        }

        /// <summary>
        /// Client Rpc that shows the victory panel.
        /// </summary>
        [ClientRpc]
        private void RpcGameWon(PlayerSide winner, int left, int right)
        {
            this.m_onWin?.Invoke(this.transform); 
            VictoryPanel.ShowGlobal(winner, left, right);
        }

        /// <summary>
        /// Called by <see cref="VictoryPanel"/> via Cmd. Resets scores and
        /// restarts a fresh rally.
        /// </summary>
        [Server]
        public void ServerRequestReplay()
        {
            this.m_scoreLeft = 0;
            this.m_scoreRight = 0;
            UpdateHud();

            this.m_ballInstance.gameObject.SetActive(true);
            ResetBall(UnityEngine.Random.value < .5f ? Vector2.right : Vector2.left);
        }

        #endregion

        #region HUD & SyncVar hook

        /// <summary>Updates the <see cref="ScoreHUD"/> on host and clients.</summary>
        private void UpdateHud() =>
            this.m_hud?.UpdateScores(this.m_scoreLeft, this.m_scoreRight);

        /// <summary>Mirror hook invoked when either score changes.</summary>
        private void OnScoreChanged(int _, int __) => UpdateHud();

        #endregion

        #region Client Rpcs

        [ClientRpc]
        private void RpcOnGoalScored(PlayerSide _) => OnGoal?.Invoke();

        #endregion

        #region Gizmo

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 size = new(0f, this.m_halfFieldHeight, 0f);
            Gizmos.DrawLine(this.transform.position - size, this.transform.position + size);
        }

        #endregion
    }
}
