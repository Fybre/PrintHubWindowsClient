using Microsoft.Extensions.Logging;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using PrintHubWindowsClient.Services.PrintProcessors;
using PrintHubWindowsClient.Services;

namespace PrintHubWindowsClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSettings _settings;
        private readonly PrintQueue _queue;

        public Worker(AppSettings appSettings, PrintQueue queue, ILogger<Worker> logger)
        {
            _logger = logger;
            _settings = appSettings;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting Job Retrievers");
            List<IPrintJobRetriever> jobRetrievers = _settings.CreateNewJobRetrieverList();

            // start up the print retrievers
            foreach (IPrintJobRetriever jobRetriever in jobRetrievers)
            {
                _logger.LogInformation($"Starting {jobRetriever}");
                jobRetriever.PrintJobAvailable += PrintJobAvailable;
                jobRetriever.SetLogger(_logger);
                jobRetriever.Start();
            }
            _logger.LogInformation("Ready for job processing.");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void PrintJobAvailable(object? sender, PrintJobEventArgs e)
        {
            _logger.LogInformation($"PrintJobAvailable Notification: {e.PrintJob.DocumentName}");
            _queue.AddJob(e.PrintJob);
        }
    }
}
