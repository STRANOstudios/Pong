namespace AndreaFrigerio.Audio.Runtime.Mixer
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the volume of an audio mixer
    /// </summary>
    [System.Serializable]
    public sealed class MixerVolumeData
    {
        public List<VolumeParam> Parameters = new();

        public float? GetValue(string key) =>
            this.Parameters.Find(p => p.Key == key)?.Value;

        public void SetValue(string key, float value)
        {
            VolumeParam entry = this.Parameters.Find(p => p.Key == key);
            if (entry != null) entry.Value = value;
            else this.Parameters.Add(new VolumeParam { Key = key, Value = value });
        }
    }
}
