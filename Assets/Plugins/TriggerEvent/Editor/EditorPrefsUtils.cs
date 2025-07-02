using UnityEditor;

namespace PsychoGarden.TriggerEvents.Editor
{
    public static class EditorPrefsUtils
    {
        public static void SetInt(string key, int value) => EditorPrefs.SetInt(key, value);
        public static int GetInt(string key, int defaultValue = 0) => EditorPrefs.GetInt(key, defaultValue);

        public static void SetFloat(string key, float value) => EditorPrefs.SetFloat(key, value);
        public static float GetFloat(string key, float defaultValue = 0f) => EditorPrefs.GetFloat(key, defaultValue);

        public static void SetString(string key, string value) => EditorPrefs.SetString(key, value);
        public static string GetString(string key, string defaultValue = "") => EditorPrefs.GetString(key, defaultValue);

        public static void SetBool(string key, bool value) => EditorPrefs.SetBool(key, value);
        public static bool GetBool(string key, bool defaultValue = false) => EditorPrefs.GetBool(key, defaultValue);
    }
}
