using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using System.Collections.Concurrent;
namespace PrintHubWindowsClient.Services
{
    public class PrintQueue
    {
        private readonly ConcurrentQueue<PrintJob> _queue = new ConcurrentQueue<PrintJob>();
        private readonly object _lock = new object();
        private bool _isProcessing = false;
        private readonly IPrintProcessor _printProcessor;
        private readonly ILogger<PrintQueue> _logger;

        public PrintQueue(IPrintProcessor printProcessor, ILogger<PrintQueue> logger)
        {
            _printProcessor = printProcessor;
            _logger = logger;
        }

        public void AddJob(PrintJob job)
        {
            _logger.LogDebug($"Queuing job: {job.DocumentName}");
            _queue.Enqueue(job);
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            lock (_lock)
            {
                if (_isProcessing) { return; }
                _isProcessing = true;
            }

            Task.Run(async () =>
            {
                while (_queue.TryDequeue(out var job))
                {
                    try
                    {
                        _logger.LogDebug($"Dequeued job: {job.DocumentName}");
                        var result = await _printProcessor.PrintJobAsync(job);
                        if (result)
                        {
                            //printed
                            _logger.LogDebug($"PrintJob {result}");
                            File.Delete(job.FilePath);
                        }
                        else
                        {
                            //fail
                            _logger.LogError($"Failed to print job {job.DocumentName}");
                            File.Move(job.FilePath, Path.ChangeExtension(job.FilePath, "failed"));
                        }
                    } catch (Exception ex)
                    {
                        _logger.LogError($"Exception: {ex.Message}");
                    }
                }
                lock (_lock)
                {
                    _isProcessing = false;
                }
            });
        }

    }
}