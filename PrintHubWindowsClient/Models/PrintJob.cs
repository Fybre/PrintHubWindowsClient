using PrintHubWindowsClient.Enums;

namespace PrintHubWindowsClient.Models
{
    public class PrintJob
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = "";
        public string DocumentName { get; set; } = "";
        public string PrintQueue { get; set; } = "";
        public PrintJobStatus JobStatus { get; set; }
    }
}