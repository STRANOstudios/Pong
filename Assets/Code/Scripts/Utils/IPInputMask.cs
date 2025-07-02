using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Menu.Scripts
{
    [HideMonoScript]
    public class IpInputMask : MonoBehaviour
    {
        [BoxGroup("Reference")]
        [Tooltip("The input field to apply the mask to.")]
        [SerializeField]
        private TMP_InputField inputField;

        private string lastValidText = "";

        // Regex for validation IPv4 + port
        private readonly Regex ipv4PortRegex = new Regex(
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(:[0-9]{1,5})?$");

        // Regex for validation partial input
        private readonly Regex partialInputRegex = new Regex(
            @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){0,3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])?(:[0-9]{0,5})?$");

        private void Awake()
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnValueChanged(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                lastValidText = "";
                return;
            }

            // Check if the input is valid
            if (!partialInputRegex.IsMatch(newValue))
            {
                // Reset the input to the last valid text
                inputField.text = lastValidText;
                return;
            }

            // Automatically add separators
            string processedText = ProcessAutomaticSeparators(newValue);

            if (processedText != newValue)
            {
                inputField.text = processedText;
                return;
            }

            lastValidText = newValue;
        }

        private void OnEndEdit(string finalValue)
        {
            if (!ipv4PortRegex.IsMatch(finalValue))
            {
                Debug.LogWarning("Invalid IP or port format.");
                inputField.text = lastValidText;
            }
            else
            {
                lastValidText = finalValue;
            }
        }

        private string ProcessAutomaticSeparators(string input)
        {
            if (input.Length == 3 && !input.Contains(".") && !input.Contains(":"))
            {
                if (int.TryParse(input, out int num) && num <= 255)
                {
                    return input + ".";
                }
            }

            if (input.EndsWith(".") && input.Split('.').Length < 5)
            {
                return input;
            }

            string[] parts = input.Split('.');
            if (parts.Length > 0 && parts.Length < 4)
            {
                string lastPart = parts[parts.Length - 1];
                if (lastPart.Length == 3 && !lastPart.Contains(":"))
                {
                    if (int.TryParse(lastPart, out int octet) && octet <= 255)
                    {
                        if (input.Length >= 3 && !input.Contains(":"))
                        {
                            return input + ".";
                        }
                    }
                }
            }

            if (!input.Contains(":") && input.Split('.').Length == 4)
            {
                string ipPart = input;
                if (ipPart.Length > 0 && ipPart[^1] != '.')
                {
                    string[] octets = ipPart.Split('.');
                    if (octets.Length == 4 && octets[3].Length > 0)
                    {
                        if (int.TryParse(octets[3], out int lastOctet) && lastOctet <= 255)
                        {
                            return input + ":";
                        }
                    }
                }
            }

            return input;
        }

        public bool TryGetIPAndPort(out string ip, out ushort port)
        {
            ip = "";
            port = 0;

            if (!ipv4PortRegex.IsMatch(lastValidText))
                return false;

            string[] mainParts = lastValidText.Split(':');
            if (mainParts.Length != 2)
                return false;

            ip = mainParts[0];
            return ushort.TryParse(mainParts[1], out port);
        }
    }
}
