namespace AndreaFrigerio.Audio.Runtime.Mixer
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.UI;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Save;

    /// <summary>
    /// Binds UI sliders to exposed parameters of an
    /// <see cref="AudioMixer"/> and persists the values with
    /// <see cref="SaveSystem"/>.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Audio/Mixer Controller")]
    public sealed class MixerController : MonoBehaviour
    {
        #region Fields

        [BoxGroup("References")]
        [Tooltip("Audio mixer whose exposed parameters will be controlled.")]
        [SerializeField, Required]
        private AudioMixer m_audioMixer;

        [SerializeField]
        private List<MixerParamSliderPair> m_paramBindings = new();

        private readonly Dictionary<string, Slider> m_paramDict = new();

        private const string SaveFileName = "volumeSettings";

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            this.BuildRuntimeDictionary();

            MixerVolumeData volumeData =
                SaveSystem.Load<MixerVolumeData>(SaveFileName);

            foreach (KeyValuePair<string, Slider> pair in this.m_paramDict)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                float linearValue = volumeData.GetValue(pair.Key)
                                  ?? this.m_paramBindings
                                         .Find(p => p.ParameterName == pair.Key)
                                         ?.DefaultValue
                                  ?? 1f;

                pair.Value.value = linearValue;
                this.SetMixerLinear(pair.Key, linearValue);

                string pname = pair.Key;
                pair.Value.onValueChanged.AddListener(v =>
                {
                    this.SetMixerLinear(pname, v);
                    volumeData.SetValue(pname, v);
                    SaveSystem.Save(volumeData, SaveFileName);
                });
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.m_audioMixer == null)
            {
                return;
            }

            List<string> exposed = this.GetExposedParameters();

            foreach (string p in exposed)
            {
                bool exists = this.m_paramBindings.Exists(b => b.ParameterName == p);
                if (!exists)
                {
                    this.m_paramBindings.Add(new MixerParamSliderPair
                    {
                        ParameterName = p,
                        Slider = null
                    });
                }
            }
        }
#endif

        #endregion

        #region Private Methods

        private void BuildRuntimeDictionary()
        {
            this.m_paramDict.Clear();
            foreach (MixerParamSliderPair pair in this.m_paramBindings)
            {
                if (!string.IsNullOrEmpty(pair.ParameterName) &&
                    !this.m_paramDict.ContainsKey(pair.ParameterName))
                {
                    this.m_paramDict.Add(pair.ParameterName, pair.Slider);
                }
            }
        }

        private void SetMixerLinear(string parameter, float linear)
        {
            float dB = Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
            this.m_audioMixer.SetFloat(parameter, dB);
        }

#if UNITY_EDITOR
        private List<string> GetExposedParameters()
        {
            var list = new List<string>();
            var so = new UnityEditor.SerializedObject(this.m_audioMixer);
            var arr = so.FindProperty("m_ExposedParameters");

            for (int i = 0; i < arr.arraySize; i++)
            {
                UnityEditor.SerializedProperty p = arr.GetArrayElementAtIndex(i);
                list.Add(p.FindPropertyRelative("name").stringValue);
            }
            return list;
        }
#endif

        #endregion
    }
}
