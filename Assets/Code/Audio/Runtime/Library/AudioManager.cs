namespace AndreaFrigerio.Audio.Runtime.Library
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Audio;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Locator;

    /// <summary>
    /// Plays named audio clips through a single <see cref="AudioSource"/>.
    /// Clips are configured in the Inspector with an optional
    /// <see cref="AudioMixerGroup"/>.
    /// </summary>
    [HideMonoScript]
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Andrea Frigerio/Audio/Audio Manager")]
    public sealed class AudioManager : MonoBehaviour
    {
        #region Fields

        [BoxGroup("References")]
        [Tooltip("Audio mixer that provides the output groups.")]
        [SerializeField] private AudioMixer m_audioMixer;

        [BoxGroup("Settings")]
        [SerializeField] private List<ManagedAudioClip> m_managedClips = new();

        private AudioSource m_audioSource;

        #endregion

#if UNITY_EDITOR
        private void OnValidate() => this.RefreshAvailableGroups();
#endif

        #region Unity Callbacks

        private void Awake()
        {
            this.m_audioSource = GetComponent<AudioSource>()
                              ?? this.gameObject.AddComponent<AudioSource>();

            this.m_audioSource.playOnAwake = false;
            this.m_audioSource.loop = false;
        }

        private void OnEnable() => ServiceLocator.Register(this);
        private void OnDisable() => ServiceLocator.Unregister(this);

        #endregion

        #region Public API

        /// <summary>Plays a managed audio clip by name.</summary>
        /// <param name="clipName">Identifier set in the Inspector list.</param>
        public void Play(string clipName)
        {
            Debug.Log($"[AudioManager] Play '{clipName}'");

            ManagedAudioClip entry =
                this.m_managedClips.Find(x => x.Name == clipName);

            if (entry != null && entry.Clip != null)
            {
                this.m_audioSource.outputAudioMixerGroup = entry.Group;
                this.m_audioSource.clip = entry.Clip;
                this.m_audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Clip '{clipName}' not found.");
            }
        }

        #endregion

#if UNITY_EDITOR
        [Button("Refresh Mixer Groups")]
        private void RefreshAvailableGroups()
        {
            if (this.m_audioMixer == null) return;

            AudioMixerGroup[] groups =
                this.m_audioMixer.FindMatchingGroups(string.Empty);

            var existing = new HashSet<AudioMixerGroup>(
                this.m_managedClips.Where(c => c.Group != null)
                                   .Select(c => c.Group));

            foreach (AudioMixerGroup g in groups)
            {
                if (!existing.Contains(g))
                {
                    this.m_managedClips.Add(new ManagedAudioClip
                    {
                        Name = g.name,
                        Clip = null,
                        Group = g
                    });
                }
            }
        }
#endif
    }
}
