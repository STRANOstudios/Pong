namespace AndreaFrigerio.Network.Runtime
{
    using UnityEngine;
    using TMPro;
    using Mirror;
    using kcp2k;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Simple UI-driven wrapper around <see cref="NetworkManager"/> that
    /// hosts or joins a Mirror session based on the IP address (and
    /// optional port) typed into a <see cref="TMP_InputField"/>.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Networking/Online Connection Manager")]
    public sealed class MirrorConnectionController : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Reference")]
        [Tooltip("The input field where the IP address is typed."), Required]
        [SerializeField]
        private TMP_InputField m_ipInputField = null;

        [BoxGroup("Reference")]
        [Tooltip("The transport to use for the Mirror session."), Required]
        [SerializeField]
        private Transport m_transport = null;

        #endregion

        #region Public API

        /// <summary>
        /// Starts a listen-server and joins it locally.
        /// </summary>
        public void HostGame()
        {
            ParseAndSetAddress();

            NetworkManager.singleton.StartHost();
        }

        /// <summary>
        /// Connects as a client to the address in the input field.
        /// </summary>
        public void JoinGame()
        {
            ParseAndSetAddress();

            NetworkManager.singleton.StartClient();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads “IP[:Port]” from the input field and configures
        /// <see cref="NetworkManager"/> and the underlying
        /// <see cref="Transport"/>.
        /// </summary>
        private void ParseAndSetAddress()
        {
            if (this.m_ipInputField == null
                || this.m_transport == null)
            {
                return;
            }

            string input = this.m_ipInputField.text.Trim();
            string[] parts = input.Split(':');

            string ip = parts[0];
            ushort port = 7777; // Mirror default

            if (parts.Length > 1 &&
                ushort.TryParse(parts[1], out ushort parsed))
            {
                port = parsed;
            }

            NetworkManager.singleton.networkAddress = ip;

            if (this.m_transport is KcpTransport kcp)
            {
                kcp.Port = port;
            }
        }

        #endregion
    }
}
