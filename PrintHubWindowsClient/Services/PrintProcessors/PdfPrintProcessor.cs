using PdfiumViewer;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;

namespace PrintHubWindowsClient.Services.PrintProcessors
{
    internal class PdfPrintProcessor : IPrintProcessor
    {
        private ILogger<PdfPrintProcessor> _logger;
        public PdfPrintProcessor(ILogger<PdfPrintProcessor> logger)
        {
            _logger = logger;
        }
        public bool Init()
        {
            return true;
        }

        public async Task<bool> PrintJobAsync(PrintJob job)
        {
            _logger.LogDebug($"Print Job to Print: {job.PrintQueue}");
            bool res = false;
            if (!Utils.DoesPrinterExist(job.PrintQueue)) { _logger.LogInformation($"Printer {job.PrintQueue} not found"); return res; }
            await Task.Run(() =>
            {
                using (var document = PdfDocument.Load(job.FilePath))
                {
                    using (var printDoc = document.CreatePrintDocument())
                    { 
                        printDoc.PrinterSettings.PrinterName = job.PrintQueue;
                        _logger.LogInformation($"Printing {job.DocumentName} on {printDoc.PrinterSettings.PrinterName}");
                        printDoc.Print();
                        res = true;
                    }
                }
            });
            return res;
        }
    }

}