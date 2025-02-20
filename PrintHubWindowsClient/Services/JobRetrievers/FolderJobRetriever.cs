using PrintHubWindowsClient.Enums;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using System.Timers;

namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class FolderJobRetriever : IPrintJobRetriever
    {
        public event EventHandler<PrintJobEventArgs>? PrintJobAvailable;

        private readonly System.Timers.Timer _timer;
        private readonly FolderJobRetrieverSettings _settings;
        private ILogger? _logger;

        public FolderJobRetriever(FolderJobRetrieverSettings settings)
        {
            _settings = settings;
            _timer = new System.Timers.Timer(_settings.TimerInterval);
            _timer.Elapsed += Timer_Elapsed;

            if (string.IsNullOrEmpty(_settings.DestinationDirectory) || !Path.Exists(_settings.DestinationDirectory)) _settings.DestinationDirectory = Path.GetTempPath();
        }

        public override string ToString()
        {
            return _settings.Name;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            if (Directory.Exists(_settings.SourceDirectory) && Directory.Exists(_settings.DestinationDirectory))
            {
                DirectoryInfo di = new DirectoryInfo(_settings.SourceDirectory);

                foreach (FileInfo fi in di.GetFiles(_settings.FileFilter))
                {
                    _logger?.LogInformation($"Processing file: {fi.Name}");
                    var destPath = Utils.GetUniqueFileName(Path.Combine(_settings.DestinationDirectory, fi.Name));
                    if (Utils.FileCanOpenAsync(fi.FullName, 5000).Result == true)
                    {
                        fi.MoveTo(destPath);
                        PrintJob job = new PrintJob()
                        {
                            DocumentName = fi.Name,
                            FilePath = destPath,
                            PrintQueue = _settings.PrintQueue,
                            JobStatus = PrintJobStatus.queued
                        };
                        OnJobAvailable(new PrintJobEventArgs(job));
                    }
                }
            }
            else
            {
                //invalid source/dest dir
            }
            _timer.Start();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        protected void OnJobAvailable(PrintJobEventArgs e)
        {
            _logger?.LogInformation($"Invoking PrintJobAvailable: {e.PrintJob.DocumentName} - {e.PrintJob.PrintQueue}");
            PrintJobAvailable?.Invoke(this, e);
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}