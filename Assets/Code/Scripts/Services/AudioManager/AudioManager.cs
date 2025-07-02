using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;

namespace AndreaFrigerio.Service.Audio
{
    [HideMonoScript]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [BoxGroup("References")]
        [Tooltip("The audio mixer to use")]
        [SerializeField]
        private AudioMixer m_audioMixer;

        [BoxGroup("Settings")]
        [SerializeField]
        private List<ManagedAudioClip> m_managedClips = new();

        private AudioSource m_audioSource;

#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshAvailableGroups();
        }
#endif

        private void Awake()
        {
            this.m_audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            this.m_audioSource.playOnAwake = false;
            this.m_audioSource.loop = false;
        }

        private void OnEnable()
        {
            Locator.ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            Locator.ServiceLocator.Unregister(this);
        }

        /// <summary>
        /// Plays an audio clip
        /// </summary>
        /// <param name="clipName">The name of the audio clip</param>
        public void Play(string clipName)
        {
            ManagedAudioClip entry = this.m_managedClips.Find(x => x.Name == clipName);
            if (entry != null && entry.Clip != null)
            {
                this.m_audioSource.outputAudioMixerGroup = entry.Group;
                this.m_audioSource.clip = entry.Clip;
                this.m_audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Clip '{clipName}' not found in managed list.");
            }
        }

#if UNITY_EDITOR
        [Button("Refresh Mixer Groups")]
        private void RefreshAvailableGroups()
        {
            if (this.m_audioMixer == null)
            {
                return;
            }

            AudioMixerGroup[] groups = this.m_audioMixer.FindMatchingGroups(string.Empty);
            HashSet<AudioMixerGroup> existingGroups = new();

            foreach (ManagedAudioClip clip in from ManagedAudioClip clip in this.m_managedClips
                                 where clip.Group != null
                                 select clip)
            {
                existingGroups.Add(clip.Group);
            }

            if (groups != null && groups.Length > 0)
            {
                foreach (AudioMixerGroup group in groups)
                {
                    if (!existingGroups.Contains(group))
                    {
                        this.m_managedClips.Add(new ManagedAudioClip
                        {
                            Name = group.name,
                            Clip = null,
                            Group = group
                        });
                    }
                }
            }
        }
#endif
    }

    [System.Serializable]
    public class ManagedAudioClip
    {
        [Tooltip("The name of the audio clip")]
        public string Name;

        [Tooltip("The audio clip")]
        public AudioClip Clip;

        [Tooltip("The audio mixer group")]
        public AudioMixerGroup Group;
    }

}
