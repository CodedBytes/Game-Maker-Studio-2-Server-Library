using System.Collections.Generic;
using System.IO;

namespace Game_Maker_Studio_2_Auth_Server.Core.IniReader
{
    public class IniFileReader
    {
        // Ini data dictionary
        private readonly Dictionary<string, Dictionary<string, string>> _iniData;

        /// <summary>
        /// Starts the ini reader with the path provided on the function
        /// </summary>
        /// <param name="filePath">Path where the ini file is located</param>
        public IniFileReader(string filePath)
        {
            _iniData = new Dictionary<string, Dictionary<string, string>>();
            LoadIniFile(filePath);
        }

        /// <summary>
        /// Loads the ini file from the path provided on the function
        /// </summary>
        /// <param name="filePath">Path where the ini file is located</param>
        /// <exception cref="FileNotFoundException"></exception>
        private void LoadIniFile(string filePath)
        {
            // Ini section
            string currentSection = null;

            // Exception
            if (!File.Exists(filePath)) throw new FileNotFoundException($"System > The specified INI file does not exist: {filePath}");

            // Reading the lines
            foreach (var line in File.ReadAllLines(filePath))
            {
                // Trimming the lines
                var trimmedLine = line.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";")) continue;

                // Check for section headers
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine[1..^1];
                    _iniData[currentSection] = new Dictionary<string, string>();
                }
                else if (currentSection != null)
                {
                    // Split key and value
                    var keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();
                        _iniData[currentSection][key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value from the loaded ini file
        /// </summary>
        /// <param name="section">Section inside the ini file, marked as []</param>
        /// <param name="key">The option of the given section</param>
        /// <returns>It returns the value from the given section and key</returns>
        public string GetValue(string section, string key)
        {
            if (_iniData.TryGetValue(section, out var sectionData) && sectionData.TryGetValue(key, out var value)) return value;
            return null;
        }
    }
}
