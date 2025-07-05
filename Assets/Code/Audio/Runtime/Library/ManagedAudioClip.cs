namespace AndreaFrigerio.Audio.Runtime.Library
{
    using UnityEngine;
    using UnityEngine.Audio;

    /// <summary>
    /// Audio clip managed by the <see cref="AudioManager"/>.
    /// </summary>
    [System.Serializable]
    public sealed class ManagedAudioClip
    {
        [Tooltip("Identifier used to play this clip.")]
        public string Name;

        [Tooltip("Audio clip asset.")]
        public AudioClip Clip;

        [Tooltip("Output mixer group.")]
        public AudioMixerGroup Group;
    }
}
