namespace AndreaFrigerio.Audio.Runtime.Mixer
{
    using UnityEngine;
    using UnityEngine.UI;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Binds UI sliders to exposed parameters of an
    /// </summary>
    [System.Serializable]
    public sealed class MixerParamSliderPair
    {
        [Tooltip("Exposed parameter name inside the AudioMixer.")]
        [ReadOnly] public string ParameterName;

        [Tooltip("UI slider bound to this parameter.")]
        [SceneObjectsOnly] public Slider Slider;

        [Tooltip("Default linear value (0–1) if no save exists.")]
        [Range(0f, 1f)] public float DefaultValue = 0.75f;
    }
}
