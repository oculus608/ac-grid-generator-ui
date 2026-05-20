using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcGridGeneratorUi
{
    public class GridConfig
    {
        [JsonPropertyName("AcRootPath")]
        public string AcRootPath { get; set; } = string.Empty;

        [JsonPropertyName("PresetFolder")]
        public string PresetFolder { get; set; } = string.Empty;

        [JsonPropertyName("PresetName")]
        public string PresetName { get; set; } = string.Empty;

        [JsonPropertyName("GridStrategy")]
        public string GridStrategy { get; set; } = "Fixed";

        [JsonPropertyName("BaseSkill")]
        public int BaseSkill { get; set; } = 85;

        [JsonPropertyName("BaseAggression")]
        public int BaseAggression { get; set; } = 50;

        [JsonPropertyName("CarAllocations")]
        public List<CarAllocation> CarAllocations { get; set; } = new List<CarAllocation>();

        [JsonPropertyName("_Documentation")]
        public Dictionary<string, string> Documentation { get; set; } = new Dictionary<string, string>
        {
            { "Lottery", "Picks randomly from the full driver pool in random order. Results in random cars and skins." },
            { "Franchise", "Picks drivers sequentially. The same subset of drivers are picked each session but assigned to cars randomly." },
            { "Fixed", "Like Lottery, but assigns drivers to identical cars consistently. Guarantees an idempotent, reproducible grid layout." }
        };
    }

    public class CarAllocation
    {
        [JsonPropertyName("CarId")]
        public string CarId { get; set; } = string.Empty;

        [JsonPropertyName("Count")]
        public int Count { get; set; } = 1;

        [JsonPropertyName("Ballast")]
        public int Ballast { get; set; } = 0;

        [JsonPropertyName("Restrictor")]
        public int Restrictor { get; set; } = 0;
    }
}
