namespace AndreaFrigerio.UI.Runtime.HUD
{
    using UnityEngine;
    using TMPro;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Simple UI overlay that displays the current score.
    /// Called from <c>PongGameManager</c> whenever the SyncVars change.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/UI/HUD/Score HUD")]
    public sealed class ScoreHUD : MonoBehaviour
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

        #region Public API

        /// <summary>
        /// Updates the HUD texts. Safe to call from host or pure client,
        /// because it modifies local UI only.
        /// </summary>
        /// <param name="left">Left-side score.</param>
        /// <param name="right">Right-side score.</param>
        public void UpdateScores(int left, int right)
        {
            this.m_leftScore.SetText(left.ToString());
            this.m_rightScore.SetText(right.ToString());
        }

        #endregion
    }
}
