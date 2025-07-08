namespace AndreaFrigerio.Networking.Runtime.Mirror
{
    using System.Collections.Generic;
    using UnityEngine;
    using global::Mirror;
    using Sirenix.OdinInspector;
    using UnityEngine.Serialization;

    /// <summary>
    /// Custom NetworkManager to handle player connections and scene-based spawning.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Networking/Custom Network Manager")]
    public sealed class CustomNetworkManager : NetworkManager
    {
        #region Fields

        [BoxGroup("Spawn Points")]
        [Tooltip("Transform where PLAYER 1 (left) appears.")]
        [SerializeField, Required]
        private Vector3 m_leftSpawn;

        [BoxGroup("Spawn Points")]
        [Tooltip("Transform where PLAYER 2 (right) appears.")]
        [SerializeField, Required]
        private Vector3 m_rightSpawn;

        [BoxGroup("Scenes")]
        [Scene]
        [FormerlySerializedAs("m_GameplayScene")]
        [Tooltip("Scene that Mirror will switch to when the server is started.")]
        public string gameplayScene = "";

        private readonly List<NetworkConnectionToClient> m_readyPlayers = new();
        private bool m_gameStarted = false;

        #endregion

        #region Server

        /// <summary>
        /// Called on the server when a client connects.
        /// </summary>
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            if (this.m_gameStarted)
            {
                conn.Disconnect();
                return;
            }

            this.m_readyPlayers.Add(conn);

            Debug.Log("<color=green>Player " + conn.connectionId + " connected.</color>");

            if (this.m_readyPlayers.Count == 2)
            {
                Debug.Log("<color=green>Game started!</color>");
                this.m_gameStarted = true;
                ServerChangeScene(gameplayScene);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("<color=blue>[Client] Server Connected!</color>");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("<color=red>[Client] Server Disconnected!</color>");
        }

        /// <summary>
        /// Called on the server when the scene changes.
        /// Players are spawned manually here once the gameplay scene is loaded.
        /// </summary>
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            if (sceneName == gameplayScene)
            {
                for (int i = 0; i < this.m_readyPlayers.Count; i++)
                {
                    NetworkConnectionToClient conn = this.m_readyPlayers[i];
                    Vector3 spawn = i == 0 ? this.m_leftSpawn : this.m_rightSpawn;

                    GameObject player = Instantiate(playerPrefab, spawn, Quaternion.identity);
                    NetworkServer.AddPlayerForConnection(conn, player);
                }
            }
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// </summary>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            this.m_readyPlayers.Remove(conn);
            base.OnServerDisconnect(conn);

            if (this.m_gameStarted && NetworkServer.connections.Count < 2)
            {
                ServerChangeScene(offlineScene);
                this.m_gameStarted = false;
            }
        }

        /// <summary>
        /// Called when the server stops.
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();
            this.m_readyPlayers.Clear();
            this.m_gameStarted = false;
        }

        /// <summary>
        /// Resets all player positions to Y = 0.
        /// </summary>
        public void ResetPlayersPosition()
        {
            foreach (NetworkConnectionToClient conn in this.m_readyPlayers)
            {
                if (conn.identity != null)
                {
                    Vector3 pos = conn.identity.transform.position;
                    pos.y = 0;
                    conn.identity.transform.position = pos;
                }
            }
        }

        #endregion
    }
}
