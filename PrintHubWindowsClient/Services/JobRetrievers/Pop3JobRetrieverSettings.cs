namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class Pop3JobRetrieverSettings
    {
        public string Name { get; set; } = String.Empty;
        required public string Pop3Server { get; set; } = string.Empty;
        public int Pop3Port { get; set; } = 110;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int TimerInterval { get; set; } = 20000;
        public string SubjectFilter { get; set; } = string.Empty;
        public string SubjectQueueRegex { get; set; } = string.Empty;
        public string AttachmentTypeFilter { get; set; } = ".pdf";
        public string DestinationDirectory { get; set; } = string.Empty;
        public string DefaultOutputQueue { get; set; } = string.Empty;
        public bool UseSSL { get; set; } = false;
        public bool DeleteOnFiltered { get; set; } = false;
    }
}