namespace AndreaFrigerio.UI.Runtime.HUD
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using TMPro;
    using Mirror;
    using Sirenix.OdinInspector;
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
        private GameObject m_root;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private InputActionReference m_escapeKey;

        #endregion

        #region Static

        private static VictoryPanel s_self;

        /// <summary>
        /// Shows the panel on every client. Called by <c>PongGameManager</c>.
        /// </summary>
        /// <param name="winner">Winner side.</param>
        /// <param name="left">Left score.</param>
        /// <param name="right">Right score.</param>
        public static void ShowGlobal(PlayerSide winner, int left, int right)
        {
            s_self?.Show(winner, left, right);
        }

        /// <summary>
        /// Hides the panel on every client. Called by <c>PongGameManager</c>.
        /// </summary>
        public static void HideGlobal() => s_self?.Hide();

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            s_self = this;
            Hide();
        }

        private void OnEnable()
        {
            if (this.m_escapeKey != null)
            {
                this.m_escapeKey.action.performed += OnEscapePerformed;
            }
        }

        private void OnDisable()
        {
            if (this.m_escapeKey != null)
            {
                this.m_escapeKey.action.performed -= OnEscapePerformed;
            }
        }

        #endregion

        #region UI

        private void Show(PlayerSide w, int l, int r)
        {
            this.m_root.SetActive(true);
            this.m_winnerText.SetText(w == PlayerSide.Left ? "PLAYER 1 WINS" : "PLAYER 2 WINS");
            this.m_scoreText.SetText($"{l} – {r}");
        }

        private void Hide() => this.m_root.SetActive(false);

        #endregion

        #region BUTTONS

        [Button("Restart")]
        public void OnRestart()
        {
            CmdRestart();
        }

        [Button("Quit")]
        public void OnQuit()
        {
            CmdQuit();
        }

        private void OnEscapePerformed(InputAction.CallbackContext ctx)
        {
            CmdQuit();
        }

        #endregion

        #region COMMANDS

        [Command(requiresAuthority = false)]
        private void CmdRestart(NetworkConnectionToClient sender = null)
        {
            if (ServiceLocator.TryGet(out PongGameManager gm))
            {
                Debug.Log("Vote");
                gm.VoteRestart(sender.connectionId);
            }
            else
            {
                Debug.Log("<color=red>Voting doesn't possible, no game manager ");
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdQuit(NetworkConnectionToClient sender = null)
        {
            if (ServiceLocator.TryGet(out PongGameManager gm))
            {
                Debug.Log("Quit");
                gm.VoteQuit();
            }
            else
            {
                Debug.Log("<color=red>Qutting doesn't possible, no game manager ");
            }
        }

        #endregion
    }
}
