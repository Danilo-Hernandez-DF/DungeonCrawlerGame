using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Localisation
{
    public static class LocalisationSystem
    {
        public static UnityAction Localise = delegate { };

        public enum LocalisedLanguage { English, Spanish }
        public static LocalisedLanguage language;

        private static Dictionary<string, string> localised;

        public static bool isInit;
        public static void Init()
        {
            language = LocalisedLanguage.Spanish;

            CSVLoader cSVLoader = new CSVLoader();
            cSVLoader.LoadCSV(Resources.Load<TextAsset>("Localisation/" + language.ToString()));
            //Debug.Log("Loaded: " + language.ToString());
            localised = cSVLoader.GetDictionaryValues();
            isInit = true;
        }

        public static string GetLocalisedValue(string key)
        {
            if (!isInit) Init();
            localised.TryGetValue(key, out string value);
            return value;
        }

        public static void SetLanguage(LocalisedLanguage language)
        {
            LocalisationSystem.language = language;
            Localise.Invoke();
        }
    }
}