namespace PrintHubWindowsClient.Models
{

    public class PrintJobEntity
    {
        public enum PrintJobStatus { Queued, Processing, Processed, Error };
        public int Id { get; set; }
        public string? Tenant { get; set; }
        public string? PrintQueue { get; set; }
        public int DocNo { get; set; }
        public DateTime SubmitTime { get; set; }
        public PrintJobStatus Status { get; set; }
        public string PrintOptions { get; set; } = string.Empty;
    }
}