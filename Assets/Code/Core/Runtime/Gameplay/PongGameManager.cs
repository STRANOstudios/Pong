namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    using System;
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Locator;
    using AndreaFrigerio.UI.Runtime.HUD;
    using System.Collections.Generic;
    using AndreaFrigerio.Networking.Runtime.Mirror;

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
        #region Exposed Members

        [BoxGroup("Settings")]
        [Tooltip("Ball prefab spawned by the server."), AssetsOnly]
        [SerializeField]
        private BallController m_ballPrefab;

        [BoxGroup("Settings")]
        [Tooltip("Half height of the playing field (UU)."), MinValue(0f)]
        [SerializeField]
        private float m_halfCourt = 5f;

        [BoxGroup("Settings")]
        [Tooltip("Points required to win the match."), MinValue(1)]
        [SerializeField]
        private int m_scoreToWin = 11;

        [BoxGroup("References")]
        [Tooltip("Scene-level HUD component."), Required, SceneObjectsOnly]
        [SerializeField]
        private ScoreHUD m_hud;

        #endregion

        #region Private Members

        [SyncVar(hook = nameof(HookScore))] private int m_left;
        [SyncVar(hook = nameof(HookScore))] private int m_right;

        private BallController m_ball;
        private readonly HashSet<int> m_votes = new();

        /// <summary>
        /// Raised client-side when a goal is scored.
        /// </summary>
        public static event Action OnGoal;

        #endregion

        #region SERVER

        public override void OnStartServer()
        {
            ServiceLocator.Register(this);
            ServiceLocator.Register<IScoreService>(this);
            SpawnBall();
            UpdateHud();
        }

        public override void OnStopServer()
        {
            ServiceLocator.Unregister<IScoreService>(this);
            ServiceLocator.Unregister(this);
        }

        [Server]
        public void ServerAddScore(PlayerSide scorer)
        {
            OnGoal?.Invoke();

            if (scorer == PlayerSide.Left)
            {
                this.m_left++;
            }
            else
            {
                this.m_right++;
            }

            bool win = this.m_left >= this.m_scoreToWin || this.m_right >= this.m_scoreToWin;
            if (win)
            {
                EndMatch(this.m_left > this.m_right ? PlayerSide.Left : PlayerSide.Right);
            }
            else
            {
                ResetBall(scorer == PlayerSide.Left ? Vector2.right : Vector2.left);
            }
        }

        [Server]
        private void EndMatch(PlayerSide winner)
        {
            this.m_ball.SetVisible(false);
            RpcShowVictory(winner, this.m_left, this.m_right);
            this.m_votes.Clear();
        }

        [Server]
        public void VoteRestart(int connId)
        {
            if (!NetworkServer.connections.ContainsKey(connId))
            {
                return;
            }

            this.m_votes.Add(connId);
            if (this.m_votes.Count == NetworkServer.connections.Count)
            {
                RestartMatch();
            }
        }

        [Server] 
        public void VoteQuit() => NetworkManager.singleton.StopHost();

        [Server]
        private void RestartMatch()
        {
            this.m_left = this.m_right = 0; UpdateHud();
            ((CustomNetworkManager)NetworkManager.singleton).ResetPlayersPosition();
            this.m_ball.SetVisible(true);
            ResetBall(UnityEngine.Random.value < .5f ? Vector2.left : Vector2.right);
            RpcHideVictory();
        }

        [Server]
        private void SpawnBall()
        {
            this.m_ball = Instantiate(this.m_ballPrefab);
            NetworkServer.Spawn(this.m_ball.gameObject);
            ResetBall(UnityEngine.Random.value < .5f ? Vector2.left : Vector2.right);
        }

        [Server]
        private void ResetBall(Vector2 dir)
        {
            float y = UnityEngine.Random.Range(-this.m_halfCourt, this.m_halfCourt);
            this.m_ball.transform.position = new Vector2(0, y);
            this.m_ball.ResetForNewRound(dir);
        }

        #endregion

        #region Client

        public override void OnStartClient() => UpdateHud();

        private void HookScore(int _, int __) => UpdateHud();

        private void UpdateHud() => this.m_hud?.UpdateScores(this.m_left, this.m_right);

        [ClientRpc] 
        void RpcShowVictory(PlayerSide w, int l, int r) => VictoryPanel.ShowGlobal(w, l, r);

        [ClientRpc] 
        void RpcHideVictory() => VictoryPanel.HideGlobal();

        #endregion

        #region Gizmo

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 size = new(0f, this.m_halfCourt, 0f);
            Gizmos.DrawLine(this.transform.position - size, this.transform.position + size);
        }

        #endregion
    }
}
