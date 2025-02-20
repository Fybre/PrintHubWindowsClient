namespace PrintHubWindowsClient.Models
{
    using Newtonsoft.Json;

    public class StatusHubMessage
    {
        public DateTime MessageTimeStamp { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public int MessageTimeout { get; set; }

        public static StatusHubMessage FromJson(string message)
        {
            return JsonConvert.DeserializeObject<StatusHubMessage>(message) ?? new StatusHubMessage();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}