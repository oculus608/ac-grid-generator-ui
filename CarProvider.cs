using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AcGridGeneratorUi
{
    public class CarProvider
    {
        public static Car[] GetInstalledCars()
        {
            var carsList = new List<Car>();
            string acRootPath = AppSettings.AssettoCorsaRoot;
            string carsDirectory = Path.Combine(acRootPath, "content", "cars");

            if (!Directory.Exists(carsDirectory))
            {
                return Array.Empty<Car>();
            }

            foreach (string carDir in Directory.GetDirectories(carsDirectory))
            {
                string acdFile = Path.Combine(carDir, "data.acd");
                string dataFolder = Path.Combine(carDir, "data");
                if (!File.Exists(acdFile) && !Directory.Exists(dataFolder))
                {
                    continue;
                }

                string folderName = Path.GetFileName(carDir);
                string uiJsonPath = Path.Combine(carDir, "ui", "ui_car.json");
                string carName = null;
                string carYear = null;

                if (File.Exists(uiJsonPath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(uiJsonPath);
                        var (extractedName, extractedYear) = ExtractCarMetadataRobustly(jsonContent);
                        carName = extractedName;
                        carYear = extractedYear;
                    }
                    catch (Exception) { /* Fall through */ }
                }

                if (string.IsNullOrWhiteSpace(carName))
                {
                    carName = FormatFolderNameToName(folderName);
                }

                // If a year is present, format it as a two-digit suffix (e.g., "1964" -> " '64")
                if (!string.IsNullOrWhiteSpace(carYear) && carYear.Length >= 2)
                {
                    string shortYear = carYear.Substring(carYear.Length - 2);
                    carName = $"{carName} '{shortYear}";
                }

                carsList.Add(new Car(folderName, carName));
            }

            // De-duplicate items by name for clean UI list picking
            return carsList
                .GroupBy(car => car.Name, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToArray();
        }

        private static string FormatFolderNameToName(string folderName)
        {
            string workingName = folderName.StartsWith("ks_", StringComparison.OrdinalIgnoreCase) ? folderName.Substring(3) : folderName;
            string[] words = workingName.Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    string upper = words[i].ToUpper();
                    if (upper == "V6" || upper == "TI" || upper == "GTA" || upper == "QV" || upper == "DTM" || upper == "AMG")
                        words[i] = upper;
                    else
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }

        private static (string Name, string Year) ExtractCarMetadataRobustly(string jsonContent)
        {
            try
            {
                var carData = JsonSerializer.Deserialize<AcCarUiMetadata>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                });

                if (carData != null)
                {
                    // Convert object/numeric year safely to string
                    string yearStr = carData.Year?.ToString();
                    return (carData.Name, yearStr);
                }
            }
            catch (JsonException)
            {
                // Fallback raw text parsers for broken mod comma patterns
                string name = ExtractJsonValueByKey(jsonContent, "name");
                string year = ExtractJsonValueByKey(jsonContent, "year");
                return (name, year);
            }
            return (null, null);
        }

        private static string ExtractJsonValueByKey(string jsonContent, string key)
        {
            int keyIndex = jsonContent.IndexOf($"\"{key}\"", StringComparison.OrdinalIgnoreCase);
            if (keyIndex != -1)
            {
                int colonIndex = jsonContent.IndexOf(":", keyIndex);
                if (colonIndex != -1)
                {
                    int startQuote = jsonContent.IndexOf("\"", colonIndex);
                    int endQuote = jsonContent.IndexOf("\"", startQuote + 1);
                    if (startQuote != -1 && endQuote != -1)
                    {
                        return jsonContent.Substring(startQuote + 1, endQuote - startQuote - 1).Trim();
                    }

                    // If it's a numeric value without quotes (e.g. "year": 1964)
                    int commaIndex = jsonContent.IndexOf(",", colonIndex);
                    if (commaIndex == -1) commaIndex = jsonContent.IndexOf("}", colonIndex);
                    if (commaIndex != -1)
                    {
                        return jsonContent.Substring(colonIndex + 1, commaIndex - colonIndex - 1).Replace("\"", "").Trim();
                    }
                }
            }
            return null;
        }
    }

    public class AcCarUiMetadata
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("year")]
        public object Year { get; set; } // Map as object to handle both 1964 and "1964" formats
    }
}
