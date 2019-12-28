using Newtonsoft.Json;

namespace ScadaArchiveAdapter
{
    public class ScadaArchMeasurement
    {
        [JsonProperty("measType")]
        public string MeasType { get; set; }
        [JsonProperty("measTag")]
        public string MeasId { get; set; }
        [JsonProperty("description")]
        public string MeasTag { get; set; }
        public ScadaArchMeasurement Clone()
        {
            return new ScadaArchMeasurement
            {
                MeasType = MeasType,
                MeasId = MeasId,
                MeasTag = MeasTag
            };
        }
    }
}
