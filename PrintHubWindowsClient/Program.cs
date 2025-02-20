using Microsoft.Extensions.Logging;
using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Models;
using PrintHubWindowsClient.Services;
using PrintHubWindowsClient.Services.PrintProcessors;

namespace PrintHubWindowsClient
{
    public class Program
    {
        private const string APP_SETTINGS_FILE = "AppConfig/appSettings.json";
        public static async Task Main(string[] args)
        {
            string appDirectory = AppContext.BaseDirectory;
            string configFilePath = Path.Combine(appDirectory, "AppConfig/appSettings.json");
            AppSettings appSettings = AppSettings.FromJson(File.ReadAllText(configFilePath))!;
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddSingleton(appSettings);
            builder.Services.AddSingleton<IPrintProcessor, PdfPrintProcessor>();
            builder.Services.AddSingleton<PrintQueue>();

            builder.Services.AddWindowsService();

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Logging.ClearProviders().AddConsole();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddEventLog(settings: new Microsoft.Extensions.Logging.EventLog.EventLogSettings() { SourceName = "PrintHubWindowsClient" });


            var host = builder.Build();
            
            await host.RunAsync();
        }
    }
}