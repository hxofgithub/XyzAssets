using System.Collections.Generic;
using UnityEngine;
namespace XyzLocalization.Runtime
{
    public class LocalizationManager : MonoBehaviour
    {


        public static event System.Action OnLanguageChanged;

        public static string Language
        {
            get
            {
                return m_CurrentLanguage;
            }
            set
            {
                if (m_CurrentLanguage == value) return;
                if (string.IsNullOrEmpty(value)) return;
                m_CurrentLanguage = value;
                NotifyLanguageChanged();
                OnLanguageChanged?.Invoke();
            }
        }
        private static string m_CurrentLanguage;

        public static void ReadData(byte[] buffer)
        {

        }

        public static void SetLocalizationData(string languge, Dictionary<string, string> datas)
        {
            if (m_LocalizationValues == null)
                m_LocalizationValues = new Dictionary<string, Dictionary<string, string>>();
            m_LocalizationValues[languge] = datas;
        }
        public static string GetValue(string key)
        {
            if (m_LocalizationValues == null || !m_LocalizationValues.ContainsKey(Language))
                return key;
            if (!m_LocalizationValues[Language].TryGetValue(key, out string value))
                return key;
            return value;
        }
        internal static void AddItem(BaseLocalizationItem localization)
        {
            m_CachedItems.Add(localization);
        }

        internal static void RemoveItem(BaseLocalizationItem localization)
        {
            m_CachedItems.Remove(localization);
        }

        private static void NotifyLanguageChanged()
        {
            if (m_CachedItems == null) return;
            for (int i = 0; i < m_CachedItems.Count; i++)
            {
                m_CachedItems[i].OnLocalization();
            }
        }

        internal static Font GetFont()
        {
            return null;
        }

        private static List<BaseLocalizationItem> m_CachedItems;
        private static Dictionary<string, Dictionary<string, string>> m_LocalizationValues;
    }
}
