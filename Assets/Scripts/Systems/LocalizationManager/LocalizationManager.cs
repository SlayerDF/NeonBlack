using System;
using System.Collections.Generic;

namespace NeonBlack.Systems.LocalizationManager
{
    public class LocalizationManager : SceneSingleton<LocalizationManager>
    {
        private const string DefaultLanguage = "en";
        private const string LocalizationResourcesPath = "Localization";

        private readonly IDataSource dataSource = new ResourcesDataSource(LocalizationResourcesPath);
        private readonly IFormatParser formatParser = new CsvFormatParser();
        private readonly LookupTable<TranslationKey, string> translationCache = new(Translate);

        private Dictionary<string, string> translation;

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();
            ChangeLanguage(DefaultLanguage);
        }

        #endregion

        public static event Action LanguageChanged;

        public static string GetTranslation(string id, params object[] args)
        {
            return Instance ? Instance.GetTranslationInternal(id, args) : "";
        }

        public static void ChangeLanguage(string language)
        {
            if (Instance)
            {
                Instance.ChangeLanguageInternal(language);
            }
        }

        private string GetTranslationInternal(string id, params object[] args)
        {
            return translationCache.Calculate(new TranslationKey(translation[id], args));
        }

        private void ChangeLanguageInternal(string language)
        {
            var rawData = dataSource.GetRawData(language);
            translation = formatParser.Parse(rawData);

            LanguageChanged?.Invoke();
        }

        private static string Translate(TranslationKey key)
        {
            var text = key.RawText.Replace("\\n", "\n");

            return string.Format(text, key.Args);
        }
    }
}
