using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonBlack.Systems.LocalizationManager
{
    public class CsvFormatParser : IFormatParser
    {
        public Dictionary<string, string> Parse(string rawData)
        {
            return rawData
                .Replace("\r", "")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var separatorIndex = line.IndexOf(';');
                    if (separatorIndex == -1)
                    {
                        throw new FormatException($"Invalid CSV format in line: {line}");
                    }

                    return (line[..separatorIndex], line[(separatorIndex + 1)..]);
                })
                .ToDictionary(x => x.Item1, x => x.Item2);
        }
    }
}
