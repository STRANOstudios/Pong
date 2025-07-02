using UnityEngine;
using TMPro;
using kcp2k;
using Mirror;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Network
{
    [HideMonoScript]
    public class OnlineConnectionManager : MonoBehaviour
    {
        [SerializeField] 
        private TMP_InputField ipInputField;

        [SerializeField]
        private Transport transport;

        public void HostGame()
        {
            ParseAndSetAddress();

            NetworkManager.singleton.StartHost();
        }

        public void JoinGame()
        {
            ParseAndSetAddress();

            NetworkManager.singleton.StartClient();
        }

        private void ParseAndSetAddress()
        {
            string input = ipInputField.text.Trim();

            string[] parts = input.Split(':');

            string ip = parts[0];
            ushort port = 7777; // default

            if (parts.Length > 1 && ushort.TryParse(parts[1], out ushort parsedPort))
            {
                port = parsedPort;
            }

            NetworkManager.singleton.networkAddress = ip;

            if (transport is KcpTransport kcp)
            {
                kcp.Port = port;
            }
        }
    }
}
