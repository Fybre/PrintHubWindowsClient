namespace PrintHubWindowsClient.Models
{
    public class PrintJobEventArgs : EventArgs
    {
        public PrintJob PrintJob { get; }
        public PrintJobEventArgs(PrintJob printJob)
        { this.PrintJob = printJob; }
    }
}