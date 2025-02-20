using PrintHubWindowsClient.Models;

namespace PrintHubWindowsClient.Interfaces
{
    public interface IPrintJobRetriever
    {
        public void SetLogger(ILogger logger);
        public event EventHandler<PrintJobEventArgs>? PrintJobAvailable;
        public void Start();
        public void Stop();

    }
}