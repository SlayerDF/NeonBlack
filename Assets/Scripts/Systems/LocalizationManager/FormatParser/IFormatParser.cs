using System.Collections.Generic;

namespace NeonBlack.Systems.LocalizationManager
{
    public interface IFormatParser
    {
        Dictionary<string, string> Parse(string rawData);
    }
}
