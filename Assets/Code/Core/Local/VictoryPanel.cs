namespace AndreaFrigerio.Pong.Core.Local
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using TMPro;
    using Sirenix.OdinInspector;
    using PsychoGarden.TriggerEvents;
    using AndreaFrigerio.Core.Runtime.Gameplay;
    using AndreaFrigerio.Core.Runtime.Locator;

    /// <summary>
    /// Pop-up shown at match end. Displays winner & scores and lets the
    /// local player request a replay.
    /// </summary>
    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/Local/Victory Panel")]
    public class VictoryPanel : MonoBehaviour
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

        private PongGameManager m_gameManager;

        #region Private Methods

        private void Start()
        {
            this.m_gameManager= ServiceLocator.Get<PongGameManager>();
        }

        private void OnEnable()
        {
            if (!this.m_gameManager.HowToWin().HasValue)
            {
                return;
            }

            this.gameObject.SetActive(true);

            this.m_winnerText.SetText(this.m_gameManager.HowToWin().Value == PlayerSide.Left ? "PLAYER 1 WINS" : "PLAYER 2 WINS");
            this.m_scoreText.SetText($"{this.m_gameManager.LeftScore} – {this.m_gameManager.RightScore}");
        }

        #endregion
    }
}
