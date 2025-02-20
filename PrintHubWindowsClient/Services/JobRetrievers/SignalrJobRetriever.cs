using Microsoft.AspNetCore.SignalR.Client;
using PrintHubWindowsClient.Enums;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using System.Runtime.CompilerServices;

namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class SignalrJobRetriever : IPrintJobRetriever
    {

        public event EventHandler<PrintJobEventArgs>? PrintJobAvailable;
        private readonly SignalrJobRetrieverSettings _settings;
        private HubConnection _connection;
        private readonly object _lock = new object();
        private bool _isProcessing = false;
        private ILogger? _logger;

        public SignalrJobRetriever(SignalrJobRetrieverSettings settings)
        {
            _settings = settings;
            _connection = new HubConnectionBuilder().WithUrl(_settings.PrintHub, options => options.Headers.Add("Authorization", $"Basic xxx")).WithAutomaticReconnect(new RetryPolicyLoop()).Build();
            _connection.Reconnected += _connection_Reconnected;
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.On<string, int>(PrintHubMessages.JobNotification, HandleJobNotification);

            if (string.IsNullOrEmpty(_settings.DestinationDirectory) || !Path.Exists(_settings.DestinationDirectory)) _settings.DestinationDirectory = Path.GetTempPath();
        }

        private async void HandleJobNotification(string tenant, int jobid)
        {
            _logger?.LogInformation($"Received Job Notification. Tenant: {tenant}, JobId: {jobid}");
            await ProcessPrintHubQueues();
        }

        public override string ToString()
        {
            return _settings.Name;
        }

        private async Task ProcessPrintHubQueues()
        {
            lock (_lock)
            {
                if (_isProcessing) return;
                _isProcessing = true;
            }
            bool finished;
            do
            {
                finished = true;
                if (_settings.PrintQueues.Count == 0) _settings.PrintQueues.Add(string.Empty);
                foreach (var queue in _settings.PrintQueues)
                {
                    var job = await _connection.InvokeAsync<PrintJobEntity>(PrintHubMessages.GetNextPrintJob, _settings.ThereforeTenant, queue);
                    if (job != null)
                    {
                        _logger?.LogInformation($"Retrieving job from Therefore. Tenant: {job.Tenant}, Print Queue: {job.PrintQueue}, Id:{job.Id}");
                        finished = false;
                        ThereforeServer server = new ThereforeServer(_settings.ThereforeTenant, _settings.ThereforeBaseUrl, _settings.ThereforeAuth);
                        Conversionoptions c = new Conversionoptions() { ConvertTo = ConvertToType.SinglePDF };
                        GetConvertedDocStreamsRawRequest request = new GetConvertedDocStreamsRawRequest() { DocNo = job.DocNo, ConversionOptions = c };
                        var fileName = $"{job.Tenant}_{job.DocNo}_{job.Id}";
                        var savedFile = await server.GetConvertedDocStreamsRawAsync(request, fileName, _settings.DestinationDirectory);
                        if (!string.IsNullOrEmpty(savedFile))
                        {
                            PrintJob printJob = new PrintJob() { DocumentName = fileName, FilePath = savedFile, PrintQueue = job.PrintQueue ?? "", JobStatus = PrintJobStatus.queued };
                            OnJobAvailable(new PrintJobEventArgs(printJob));
                        }
                        else
                        {
                            _logger?.LogInformation($"Error retrieving job");
                        }
                        await _connection.InvokeAsync(PrintHubMessages.SetJobStatus, job.Id, PrintJobEntity.PrintJobStatus.Processed);
                    }
                }
            } while (!finished);
            lock (_lock)
            {
                _isProcessing = false;
            }
        }

        private Task _connection_Reconnecting(Exception? arg)
        {
            _logger?.LogInformation($"Connecting Reconnecting");
            return Task.CompletedTask;
        }

        private Task _connection_Reconnected(string? arg)
        {
            _logger?.LogInformation($"Reconnected");
            return Task.CompletedTask;
        }

        public async void Start()
        {
            bool connected = false;
            do
            {
                try
                {
                    await _connection.StartAsync();
                    _logger?.LogInformation($"Connected to Hub. ID: {_connection.ConnectionId}");
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(10000);
                }
            } while (!connected);
        }

        public async void Stop()
        {
            await _connection.StopAsync();
        }

        private void OnJobAvailable(PrintJobEventArgs e)
        {
            _logger?.LogInformation($"Invoking PrintJobAvailable: {e.PrintJob.DocumentName} - {e.PrintJob.PrintQueue}");
            PrintJobAvailable?.Invoke(this, e);
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }


    public class RetryPolicyLoop : IRetryPolicy
    {
        private const int ReconnectionWaitSeconds = 5;

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(ReconnectionWaitSeconds);
        }
    }

}