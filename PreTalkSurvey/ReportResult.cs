using Newtonsoft.Json;

namespace PreTalkSurvey
{
    public class ReportResult
    {
        [JsonProperty("question")]
        public string Question { get; set; }
        
        [JsonProperty("items")]
        public ReportItem[] Items { get; set; }
    }
}