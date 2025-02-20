using MailKit.Net.Pop3;
using MimeKit;
using PrintHubWindowsClient.Enums;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using System.Text.RegularExpressions;

namespace PrintHubWindowsClient.Services.JobRetrievers
{
    public class Pop3JobRetriever : IPrintJobRetriever
    {
        const string PRINTER_REGEX_MATCHING_GROUP = "\\[(.*)\\]";
        public event EventHandler<PrintJobEventArgs>? PrintJobAvailable;
        private readonly System.Timers.Timer _timer;
        private Pop3JobRetrieverSettings _settings;
        private ILogger? _logger;

        public Pop3JobRetriever(Pop3JobRetrieverSettings settings)
        {
            _settings = settings;
            _timer = new System.Timers.Timer();
            _timer.Interval = _settings.TimerInterval;
            _timer.Elapsed += Timer_Elapsed;
            if (string.IsNullOrEmpty(_settings.DestinationDirectory) || !Path.Exists(_settings.DestinationDirectory)) _settings.DestinationDirectory = Path.GetTempPath();
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            await Processmail();
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

        public override string ToString()
        {
            return _settings.Name;
        }

        private async Task Processmail()
        {
            using (var client = new Pop3Client())
            {
                await client.ConnectAsync(_settings.Pop3Server, _settings.Pop3Port, _settings.UseSSL);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                if (client.Count > 0) { _logger?.LogInformation($"Processing {client.Count} inbox messages."); }
                for (int i = 0; i < client.Count; i++)
                {
                    bool markForDelete = false;
                    var msg = await client.GetMessageAsync(i);

                    // Subject filter check
                    if (!string.IsNullOrEmpty(_settings.SubjectFilter) && !msg.Subject.Contains(_settings.SubjectFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger?.LogInformation($"Skipping by Subject Filter: {msg.Subject}");
                        if (_settings.DeleteOnFiltered)
                        {
                            _logger?.LogInformation("Deleting");
                            await client.DeleteMessageAsync(i);
                        }
                        continue; // Skip to next message if subject doesn't match
                    }

                    // Attachment Check
                    if (msg.Attachments.ToList().Where(x => x is MimePart part).Count() == 0)
                    {
                        _logger?.LogInformation($"Skipping as no attachments: {msg.Subject}");
                        if (_settings.DeleteOnFiltered)
                        {
                            _logger?.LogInformation("Deleting");
                            await client.DeleteMessageAsync(i);
                        }
                        continue; //skip to next message if no attachments
                    }

                    var outputQueue = Regex.Match(msg.Subject, PRINTER_REGEX_MATCHING_GROUP).Groups[1].Value;
                    if (string.IsNullOrEmpty(outputQueue)) 
                    { 
                        outputQueue = _settings.DefaultOutputQueue;
                        _logger?.LogInformation($"Using Default Output Queue: {outputQueue}");
                    } else 
                    {
                        _logger?.LogInformation($"Output Queue from Subject: {outputQueue}");
                    }

                    foreach (var attachment in msg.Attachments)
                    {
                        if (attachment is MimePart part)
                        {
                            // Attachment type filter check
                            if (!string.IsNullOrEmpty(_settings.AttachmentTypeFilter) && Path.GetExtension(part.FileName).ToUpper() != _settings.AttachmentTypeFilter.ToUpper())
                            {
                                _logger?.LogInformation($"Skipping by Attachment Type Filter: {part.FileName}");
                                if (_settings.DeleteOnFiltered) { markForDelete = true; }
                                continue; // Skip to next attachment if type doesn't match
                            }

                            var attachmentFilename = Utils.GetUniqueFileName(Path.Combine(_settings.DestinationDirectory, part.FileName));
                            using (var stream = File.Create(attachmentFilename))
                            {
                                part.Content.DecodeTo(stream);

                            }
                            PrintJob job = new PrintJob() { DocumentName = part.FileName, FilePath = attachmentFilename, PrintQueue = outputQueue, JobStatus = PrintJobStatus.queued };
                            OnPrintJobAvailable(new PrintJobEventArgs(job));
                            markForDelete = true;
                        }
                    }
                    if (markForDelete)
                    {
                        _logger?.LogInformation("Deleting");
                        await client.DeleteMessageAsync(i);
                    }

                }
                await client.DisconnectAsync(true);
            }
        }

        protected void OnPrintJobAvailable(PrintJobEventArgs e)
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