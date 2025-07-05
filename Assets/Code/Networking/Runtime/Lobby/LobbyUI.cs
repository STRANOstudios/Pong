namespace AndreaFrigerio.Network.Runtime.Lobby
{
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Handles the single “Leave Lobby” button.  
    /// If running as host it shuts down both server and client;
    /// otherwise it simply disconnects the local client.
    /// </summary>
    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/Networking/Lobby UI")]
    public sealed class LobbyUI : MonoBehaviour
    {
        /// <summary>
        /// Disconnects from the current Mirror session.
        /// </summary>
        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();   // host -> stop server & client
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient(); // pure client
            }
        }
    }
}
