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

        [BoxGroup("References")]
        [Tooltip("Target for graphics.")]
        [SerializeField, Required]
        private GameObject m_graphicsTarget;

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
        }

        /// <summary>
        /// Hides the panel on every client. Called by <c>PongGameManager</c>.
        /// </summary>
        public static void HideGlobal() => s_instance?.Hide();

        #endregion

        #region Unity

        private void Awake()
        {
            s_instance = this;
            Hide();
        }

        #endregion

        #region UI display

        /// <summary>
        /// Fills texts and enables the panel.
        /// </summary>
        private void Show(PlayerSide winner, int left, int right)
        {
            Debug.Log($"VictoryPanel.Show({winner}, {left}, {right})");
            this.m_graphicsTarget.SetActive(true);

            string playerName = winner == PlayerSide.Left ? "PLAYER 1" : "PLAYER 2";
            this.m_winnerText.SetText($"{playerName} WINS");
            this.m_scoreText.SetText($"{left} – {right}");
        }

        /// <summary>
        /// Hides the panel.
        /// </summary>
        public void Hide() => this.m_graphicsTarget.SetActive(false);

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
                Hide();
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
