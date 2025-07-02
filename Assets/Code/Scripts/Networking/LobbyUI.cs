using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Network
{
    [HideMonoScript]
    public class LobbyUI : MonoBehaviour
    {
        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}
