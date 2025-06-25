using System;
using System.Collections.Generic;

namespace NeonBlack.Systems.LocalizationManager
{
    public class CsvFormatParser : IFormatParser
    {
        public Dictionary<string, string> Parse(string rawData)
        {
            var dict = new Dictionary<string, string>();

            var buffer = "";
            var valueIndex = -1;
            for (var i = 0; i < rawData.Length; i++)
            {
                switch (rawData[i])
                {
                    case ';' when valueIndex == -1:
                        valueIndex = buffer.Length;
                        break;
                    case '\r':
                    case '\n':
                    {
                        if (buffer.Length == 0 || valueIndex == -1)
                        {
                            throw new FormatException($"Invalid CSV format in line {dict.Count + 1}: {buffer}");
                        }

                        dict.Add(buffer[..valueIndex], buffer[valueIndex..]);
                        buffer = "";
                        valueIndex = -1;

                        if (i < rawData.Length - 1 && rawData[i + 1] == '\n')
                        {
                            i++;
                        }

                        break;
                    }
                    default:
                        buffer += rawData[i];
                        break;
                }
            }

            if (buffer.Length > 0)
            {
                dict.TryAdd(buffer[..valueIndex], buffer[valueIndex..]);
            }

            return dict;
        }
    }
}
