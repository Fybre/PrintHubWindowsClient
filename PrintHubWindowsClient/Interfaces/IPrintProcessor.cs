using PrintHubWindowsClient.Models;

namespace PrintHubWindowsClient.Interfaces
{
    public interface IPrintProcessor
    {
        public bool Init();
        public Task<bool> PrintJobAsync(PrintJob job);
    }
}