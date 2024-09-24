using Newtonsoft.Json;

namespace KironTest.Models
{
    public class BankHolidayModel
    {
        [JsonProperty("england-and-wales")]
        public RegionModel EnglandAndWales { get; set; }

        [JsonProperty("scotland")]
        public RegionModel Scotland { get; set; }

        [JsonProperty("northern-ireland")]
        public RegionModel NorthernIreland { get; set; }

        public List<RegionModel> Regions => new List<RegionModel> { EnglandAndWales, Scotland, NorthernIreland };
    }
}
