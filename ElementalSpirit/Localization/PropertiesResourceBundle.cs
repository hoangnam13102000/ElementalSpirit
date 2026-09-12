using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ElementalSpirit.Localization
{
    /// <summary>
    /// Đọc file .properties dạng "key=value" giống java.util.Properties.load(),
    /// hỗ trợ comment bằng '#' hoặc ';', encoding UTF-8 để giữ dấu tiếng Việt,
    /// và unescape \n, \t, \\ trong value giống hành vi thật của Java Properties.
    /// </summary>
    public sealed class PropertiesResourceBundle : IResourceBundle
    {
        private readonly Dictionary<string, string> _entries = new();
        private readonly string _sourcePath;

        public PropertiesResourceBundle(string filePath)
        {
            _sourcePath = filePath;
            Load(filePath);
        }

        private void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"[PropertiesResourceBundle] NOT FOUND: {filePath}");
                return;
            }

            foreach (var rawLine in File.ReadAllLines(filePath, Encoding.UTF8))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                string key = line[..separatorIndex].Trim();
                string rawValue = line[(separatorIndex + 1)..].Trim();
                _entries[key] = Unescape(rawValue);
            }
        }

        /// <summary>
        /// Chuyển \n, \t, \r, \\ trong value thành ký tự thật,
        /// giống cách java.util.Properties xử lý escape khi load file.
        /// </summary>
        private static string Unescape(string value)
        {
            if (!value.Contains('\\'))
                return value;

            var sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '\\' && i + 1 < value.Length)
                {
                    char next = value[i + 1];
                    switch (next)
                    {
                        case 'n': sb.Append('\n'); i++; break;
                        case 't': sb.Append('\t'); i++; break;
                        case 'r': sb.Append('\r'); i++; break;
                        case '\\': sb.Append('\\'); i++; break;
                        default: sb.Append(c); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string GetString(string key)
        {
            if (_entries.TryGetValue(key, out var value))
                return value;

            Debug.WriteLine($"[PropertiesResourceBundle] MISSING KEY '{key}' in '{_sourcePath}'");
            return $"!{key}!";
        }

        public bool ContainsKey(string key) => _entries.ContainsKey(key);
    }
}