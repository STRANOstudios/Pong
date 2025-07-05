namespace AndreaFrigerio.UI.Runtime.Input
{
    using System.Text.RegularExpressions;
    using UnityEngine;
    using TMPro;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Input-field helper that validates and auto-formats an IPv4 address
    /// with optional port while the user types.
    /// </summary>
    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/UI/IP Input Mask")]
    public sealed class IpInputMask : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Reference")]
        [Tooltip("The input field to apply the mask to.")]
        [SerializeField]
        private TMP_InputField m_inputField = null;

        private string m_lastValidText = "";

        // IPv4: 0–255.0–255.0–255.0–255 with optional :port
        private readonly Regex m_ipv4PortRegex = new(
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}"
          + @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(:[0-9]{1,5})?$");

        // Permissive pattern while typing
        private readonly Regex m_partialRegex = new(
            @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){0,3}"
          + @"([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])?(:[0-9]{0,5})?$");

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            this.m_inputField.onValueChanged.AddListener(this.OnValueChanged);
            this.m_inputField.onEndEdit.AddListener(this.OnEndEdit);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the input is valid and updates the last valid text.
        /// </summary>
        /// <param name="newValue">The input string.</param>
        private void OnValueChanged(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                this.m_lastValidText = "";
                return;
            }

            if (!this.m_partialRegex.IsMatch(newValue))
            {
                this.m_inputField.text = this.m_lastValidText;
                return;
            }

            string processed = this.ProcessAutomaticSeparators(newValue);
            if (processed != newValue)
            {
                this.m_inputField.text = processed;
                return;
            }

            this.m_lastValidText = newValue;
        }

        /// <summary>
        /// Checks if the input is valid and updates the last valid text.
        /// </summary>
        /// <param name="finalValue">The input string.</param>
        private void OnEndEdit(string finalValue)
        {
            if (!this.m_ipv4PortRegex.IsMatch(finalValue))
            {
                Debug.LogWarning("[IpInputMask] Invalid IP or port.");
                this.m_inputField.text = this.m_lastValidText;
            }
            else
            {
                this.m_lastValidText = finalValue;
            }
        }

        /// <summary>
        /// Auto-append '.' and ':' to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The processed string.</returns>
        private string ProcessAutomaticSeparators(string input)
        {
            // Auto-append dot after 3 digits
            if (input.Length == 3 && !input.Contains('.') && !input.Contains(':'))
            {
                if (int.TryParse(input, out int n) && n <= 255)
                {
                    return input + ".";
                }
            }

            if (input.EndsWith(".") && input.Split('.').Length < 5) return input;

            string[] parts = input.Split('.');
            if (parts.Length is > 0 and < 4)
            {
                string last = parts[^1];
                if (last.Length == 3 && !last.Contains(":") &&
                    int.TryParse(last, out int oct) && oct <= 255)
                {
                    return input + ".";
                }
            }

            // Append ':' after full IPv4
            if (!input.Contains(":") && input.Split('.').Length == 4)
            {
                string[] octets = input.Split('.');
                if (octets[^1].Length > 0 &&
                    int.TryParse(octets[^1], out int lastOctet) && lastOctet <= 255)
                {
                    return input + ":";
                }
            }

            return input;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Extracts IP and port from the last valid text.
        /// </summary>
        /// <returns><c>true</c> if both values are valid.</returns>
        public bool TryGetIPAndPort(out string ip, out ushort port)
        {
            ip = "";
            port = 0;

            if (!this.m_ipv4PortRegex.IsMatch(this.m_lastValidText))
            {
                return false;
            }

            string[] parts = this.m_lastValidText.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            ip = parts[0];
            return ushort.TryParse(parts[1], out port);
        }

        #endregion
    }
}
