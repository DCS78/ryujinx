using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Ryujinx.BuildValidationTasks
{
    public class LocalesValidationTask : IValidationTask
    {
        static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            NewLine = "\n",
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            // Allow parsing of files that contain comments or trailing commas
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public LocalesValidationTask() { }

        public bool Execute(string projectPath, bool isGitRunner)
        {
            Console.WriteLine("Running Locale Validation Task...");

            string path = projectPath + "assets/locales.json";
            string data;

            using (StreamReader sr = new(path))
            {
                data = sr.ReadToEnd();
            }

            LocalesJson json;

            if (isGitRunner && data.Contains("\r\n"))
                throw new FormatException("locales.json is using CRLF line endings! It should be using LF line endings, rebuild locally to fix...");

            try
            {
                json = JsonSerializer.Deserialize<LocalesJson>(data, _jsonOptions);

            }
            catch (JsonException e)
            {
                throw new JsonException(e.Message); //shorter and easier stacktrace
            }

            bool encounteredIssue = false;

            for (int i = 0; i < json.Locales.Count; i++)
            {
                LocalesEntry locale = json.Locales[i];

                foreach (string langCode in json.Languages.Where(lang => !locale.Translations.ContainsKey(lang)))
                {
                    encounteredIssue = true;

                    if (!isGitRunner)
                    {
                        locale.Translations.Add(langCode, string.Empty);
                        Console.WriteLine($"Added '{langCode}' to Locale '{locale.ID}'");
                    }
                    else
                    {
                        Console.WriteLine($"Missing '{langCode}' in Locale '{locale.ID}'!");
                    }
                }

                foreach (string langCode in json.Languages.Where(lang => locale.Translations.ContainsKey(lang) && lang != "en_US" && locale.Translations[lang] == locale.Translations["en_US"]))
                {
                    encounteredIssue = true;

                    if (!isGitRunner)
                    {
                        locale.Translations[langCode] = string.Empty;
                        Console.WriteLine($"Lanugage '{langCode}' is a duplicate of en_US in Locale '{locale.ID}'! Resetting it...");
                    }
                    else
                    {
                        Console.WriteLine($"Lanugage '{langCode}' is a duplicate of en_US in Locale '{locale.ID}'!");
                    }
                }

                locale.Translations = locale.Translations.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
                json.Locales[i] = locale;
            }

            if (isGitRunner && encounteredIssue)
                throw new JsonException("1 or more locales are invalid! Rebuild locally to fix...");

            string jsonString = JsonSerializer.Serialize(json, _jsonOptions);

            using (StreamWriter sw = new(path))
            {
                sw.Write(jsonString);
            }

            Console.WriteLine("Finished Locale Validation Task!");

            return true;
        }

        struct LocalesJson
        {
            public Dictionary<string, string> Info { get; set; }
            public List<string> Languages { get; set; }
            public List<LocalesEntry> Locales { get; set; }
        }

        struct LocalesEntry
        {
            public string ID { get; set; }
            public Dictionary<string, string> Translations { get; set; }
        }
    }
}
