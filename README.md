# PrintHubWindowsClient
Enables printing to a local printer from Therefore Online. Runs as a Windows Service. 
It will print via PrintHub (signalr from cloud server), Email (pop3 mailbox) and/or local folder.

Rename the AppConfig/Sample_appSettings.json to AppConfig/appSettings.json and edit accordingly.

Multiple print sources can be configured for each of the PrintJobRetrievers. Ensure the printer name is the same as the queue name in the config, and set the default print settings for the printer to how you want the document to print (paper size etc). For different print settings, make different print queues.
