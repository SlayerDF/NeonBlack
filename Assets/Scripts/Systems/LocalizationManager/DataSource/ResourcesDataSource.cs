using UnityEngine;

namespace NeonBlack.Systems.LocalizationManager
{
    public class ResourcesDataSource : IDataSource
    {
        private readonly string localizationResourcesPath;

        public ResourcesDataSource(string localizationResourcesPath)
        {
            this.localizationResourcesPath = localizationResourcesPath;
        }

        public string GetRawData(string language)
        {
            return Resources.Load<TextAsset>($"{localizationResourcesPath}/{language}").text;
        }
    }
}
