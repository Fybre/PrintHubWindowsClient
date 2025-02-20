namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class FolderJobRetrieverSettings
    {
        public string Name { get; set; } = String.Empty;
        required public string SourceDirectory { get; set; } = string.Empty;
        public string DestinationDirectory { get; set; } = string.Empty;
        public string PrintQueue { get; set; } = string.Empty;
        public int TimerInterval { get; set; } = 10000;
        public string FileFilter { get; set; } = "*.*";
    }
}