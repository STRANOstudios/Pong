namespace AndreaFrigerio.Pong.Core.Local
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using TMPro;
    using AndreaFrigerio.Core.Runtime.Locator;

    /// <summary>
    /// Displays the score of the local player.
    /// </summary>
    [HideMonoScript]
    [DefaultExecutionOrder(100)]
	[AddComponentMenu("Andrea Frigerio/Local/Score HUD")]
    public class ScoreHUD : MonoBehaviour
	{
        #region Inspector

        [BoxGroup("References")]
        [Tooltip("TMP element for the left player’s score.")]
        [SerializeField, Required]
        private TMP_Text m_leftScore = null;

        [BoxGroup("References")]
        [Tooltip("TMP element for the right player’s score.")]
        [SerializeField, Required]
        private TMP_Text m_rightScore = null;

        #endregion

        private PongGameManager m_gameManager;

        #region Unity Lifecycle

        private void Start()
        {
            this.m_gameManager = ServiceLocator.Get<PongGameManager>();
        }

        private void OnEnable()
        {
            PongGameManager.OnGoal += UpdateScores;
            PongGameManager.OnRestart += UpdateScores;
        }

        private void OnDisable()
        {
            PongGameManager.OnGoal -= UpdateScores;
            PongGameManager.OnRestart -= UpdateScores;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Updates the HUD texts. Safe to call from host or pure client,
        /// because it modifies local UI only.
        /// </summary>
        /// <param name="left">Left-side score.</param>
        /// <param name="right">Right-side score.</param>
        public void UpdateScores()
        {
            this.m_leftScore.SetText(this.m_gameManager.LeftScore.ToString());
            this.m_rightScore.SetText(this.m_gameManager.RightScore.ToString());
        }

        #endregion
    }
}