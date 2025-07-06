namespace AndreaFrigerio.UI.Runtime.HUD
{
    using UnityEngine;
    using Mirror;
    using Sirenix.OdinInspector;
    using TMPro;
    using AndreaFrigerio.Core.Runtime.Gameplay;
    using AndreaFrigerio.Core.Runtime.Locator;

    /// <summary>
    /// Pop-up shown at match end. Displays winner & scores and lets the
    /// local player request a replay.
    /// </summary>
    [HideMonoScript]
    public sealed class VictoryPanel : NetworkBehaviour
    {
        #region Inspector

        [BoxGroup("References")]
        [Tooltip("Texts to fill.")]
        [SerializeField, Required]
        private TMP_Text m_winnerText;

        [BoxGroup("References")]
        [Tooltip("Texts to fill.")]
        [SerializeField, Required]
        private TMP_Text m_scoreText;

        #endregion

        #region Static convenience

        private static VictoryPanel s_instance;

        /// <summary>
        /// Shows the panel on every client. Called by <c>PongGameManager</c>.
        /// </summary>
        /// <param name="winner">Winner side.</param>
        /// <param name="left">Left score.</param>
        /// <param name="right">Right score.</param>
        public static void ShowGlobal(PlayerSide winner, int left, int right)
        {
            s_instance?.Show(winner, left, right);
            s_instance?.gameObject.SetActive(true);
        }

        #endregion

        #region Unity

        private void Awake()
        {
            s_instance = this;
            this.gameObject.SetActive(false);
        }

        #endregion

        #region UI display

        /// <summary>
        /// Fills texts and enables the panel.
        /// </summary>
        private void Show(PlayerSide winner, int left, int right)
        {
            string playerName = winner == PlayerSide.Left ? "PLAYER 1" : "PLAYER 2";
            this.m_winnerText.SetText($"{playerName} WINS");
            this.m_scoreText.SetText($"{left} – {right}");
        }

        #endregion

        #region Replay request

        /// <summary>
        /// Bound to the “Play Again” button. Sends a replay request to server.
        /// </summary>
        [Button("Request Replay")]
        public void OnReplayPressed()
        {
            if (NetworkClient.active)
            {
                CmdRequestReplay();
                this.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Forwards replay request to the authoritative server.
        /// </summary>
        [Command]
        private void CmdRequestReplay() =>
            ServiceLocator.Get<PongGameManager>().ServerRequestReplay();

        #endregion
    }
}
