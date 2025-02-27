# PrintHubWindowsClient
## Unless there is a pressing need to use the Printhub Signalr connection type, the better version to use is https://github.com/Fybre/PrintService
Enables printing to a local printer from Therefore Online. Runs as a Windows Service. 
It will print via PrintHub (signalr from cloud server), Email (pop3 mailbox) and/or local folder.

Rename the AppConfig/Sample_appSettings.json to AppConfig/appSettings.json and edit accordingly.

Multiple print sources can be configured for each of the PrintJobRetrievers. Ensure the printer name is the same as the queue name in the config, and set the default print settings for the printer to how you want the document to print (paper size etc). For different print settings, make different print queues.

A sample appSettings.json file to process a directory C:\temp\import to a printer 'Xerox Printer' could like like the following:

```
{
  "JobRetrieverSettings": {
    "FolderJobRetrieverSettings": [
      {
        "Name": "Input",
        "SourceDirectory": "c:/temp/import",
        "DestinationDirectory": "c:/temp/export",
        "PrintQueue": "Xerox Printer",
        "TimerInterval": 10000,
        "FileFilter": "*.pdf"
      }
    ],
    "Pop3JobRetrieverSettings": [
    ],
    "SignalrJobRetrieverSettings": [
    
    ]
  }
}
```
