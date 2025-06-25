namespace NeonBlack.Systems.LocalizationManager
{
    public interface IDataSource
    {
        string GetRawData(string language);
    }
}
