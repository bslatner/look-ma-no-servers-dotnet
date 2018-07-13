using Newtonsoft.Json;

namespace PreTalkSurvey
{
    public class ReportItem
    {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("percentage")]
        public double Percentage { get; set; }
    }
}