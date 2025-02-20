namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class SignalrJobRetrieverSettings
    {
        public string Name { get; set; } = String.Empty;
        required public string PrintHub { get; set; } = string.Empty;
        public List<string> PrintQueues { get; set; } = new List<string>();
        required public string ThereforeTenant { get; set; } = string.Empty;
        required public string ThereforeBaseUrl { get; set; } = string.Empty;
        required public string ThereforeAuth { get; set; } = string.Empty;
        public string DestinationDirectory { get; set; } = string.Empty;
    }

}