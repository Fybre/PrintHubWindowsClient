using PrintHubWindowsClient.Interfaces;
using PrintHubWindowsClient.Services.JobRetrievers;

namespace PrintHubWindowsClient.Models
{
    public class AppSettings
    {
        public JobRetrieverSettings JobRetrieverSettings { get; set; } = new JobRetrieverSettings();

        public List<IPrintJobRetriever> CreateNewJobRetrieverList(ILogger? logger = null)
        {
            List<IPrintJobRetriever> res = new List<IPrintJobRetriever>();
            foreach (var s in JobRetrieverSettings.SignalrJobRetrieverSettings)
            {
                res.Add(new SignalrJobRetriever(s));
            }
            foreach (var f in JobRetrieverSettings.FolderJobRetrieverSettings)
            {
                res.Add(new FolderJobRetriever(f));
            }
            foreach (var p in JobRetrieverSettings.Pop3JobRetrieverSettings)
            {
                res.Add(new Pop3JobRetriever(p));
            }
            return res;
        }

        public static AppSettings? FromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json);
        }
    }

    public class JobRetrieverSettings
    {
        public List<FolderJobRetrieverSettings> FolderJobRetrieverSettings { get; set; } = new List<FolderJobRetrieverSettings>();
        public List<Pop3JobRetrieverSettings> Pop3JobRetrieverSettings { get; set; } = new List<Pop3JobRetrieverSettings>();
        public List<SignalrJobRetrieverSettings> SignalrJobRetrieverSettings { get; set; } = new List<SignalrJobRetrieverSettings>();
    }
}