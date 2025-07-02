using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;
using AndreaFrigerio.Systems.Save;

namespace AndreaFrigerio.Systems.Volume
{
    [HideMonoScript]
    public class MixerController : MonoBehaviour
    {
        [BoxGroup("References")]
        [Tooltip("Audio mixer")]
        [SerializeField, Required]
        private AudioMixer m_audioMixer;

        [SerializeField]
        private List<MixerParamSliderPair> m_paramBindings = new();

        private Dictionary<string, Slider> m_paramDict = new();

        private const string SaveFileName = "volumeSettings";

        private void Start()
        {
            BuildRuntimeDictionary();

            MixerVolumeData volumeData = SaveSystem.Load<MixerVolumeData>(SaveFileName);

            foreach (KeyValuePair<string, Slider> pair in this.m_paramDict)
            {
                if (pair.Value != null)
                {
                    float linearValue;

                    float? savedValue = volumeData.GetValue(pair.Key);
                    if (savedValue.HasValue)
                    {
                        linearValue = savedValue.Value;
                    }
                    else
                    {
                        MixerParamSliderPair binding = this.m_paramBindings.Find(p => p.ParameterName == pair.Key);
                        linearValue = binding != null ? binding.DefaultValue : 1f;
                    }

                    pair.Value.value = linearValue;

                    float dB = Mathf.Log10(Mathf.Clamp(linearValue, 0.0001f, 1f)) * 20f;
                    this.m_audioMixer.SetFloat(pair.Key, dB);

                    string parameterName = pair.Key;

                    pair.Value.onValueChanged.AddListener(v =>
                    {
                        float newdB = Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
                        this.m_audioMixer.SetFloat(parameterName, newdB);

                        volumeData.SetValue(parameterName, v);
                        SaveSystem.Save(volumeData, SaveFileName);
                    });
                }
            }
        }

        private void OnValidate()
        {
            if (this.m_audioMixer == null) return;

            List<string> exposedParams = GetExposedParameters();

            foreach (string param in exposedParams)
            {
                bool exists = this.m_paramBindings.Exists(p => p.ParameterName == param);
                if (!exists)
                {
                    this.m_paramBindings.Add(new MixerParamSliderPair { ParameterName = param, Slider = null });
                }
            }
        }

        #region Private methods

        private void BuildRuntimeDictionary()
        {
            this.m_paramDict.Clear();
            foreach (MixerParamSliderPair pair in this.m_paramBindings)
            {
                if (!string.IsNullOrEmpty(pair.ParameterName)
                    && !this.m_paramDict.ContainsKey(pair.ParameterName))
                {
                    this.m_paramDict.Add(pair.ParameterName, pair.Slider);
                }
            }
        }

        private List<string> GetExposedParameters()
        {
#if UNITY_EDITOR
            List<string> exposedParams = new();
            UnityEditor.SerializedObject serializedMixer = new(this.m_audioMixer);
            UnityEditor.SerializedProperty parameters = serializedMixer.FindProperty("m_ExposedParameters");

            for (int i = 0; i < parameters.arraySize; i++)
            {
                var param = parameters.GetArrayElementAtIndex(i);
                var nameProp = param.FindPropertyRelative("name");
                exposedParams.Add(nameProp.stringValue);
            }

            return exposedParams;
#else
            return new List<string>();
#endif
        }

        #endregion
    }

    [System.Serializable]
    public class MixerParamSliderPair
    {
        [Tooltip("Parameter name")]
        [ReadOnly]
        public string ParameterName;

        [Tooltip("Slider to bind")]
        [SceneObjectsOnly]
        public Slider Slider;

        [Tooltip("Default value")]
        [Range(0f, 1f)]
        public float DefaultValue = 0.75f;
    }

    [System.Serializable]
    public class MixerVolumeData
    {
        public List<VolumeParam> Parameters = new();

        public float? GetValue(string key)
        {
            VolumeParam entry = Parameters.Find(p => p.Key == key);
            return entry?.Value;
        }

        public void SetValue(string key, float value)
        {
            VolumeParam entry = Parameters.Find(p => p.Key == key);
            if (entry != null)
            {
                entry.Value = value;
            }
            else
            {
                Parameters.Add(new VolumeParam { Key = key, Value = value });
            }
        }
    }

    [System.Serializable]
    public class VolumeParam
    {
        public string Key;
        public float Value;
    }
}
