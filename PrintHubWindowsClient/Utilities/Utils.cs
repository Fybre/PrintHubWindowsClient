using System.Diagnostics;
using System.Drawing.Printing;

public static class Utils
{

    public static string GetUniqueFileName(string destPath)
    {
        string newPath = destPath;
        if (!File.Exists(newPath)) { return newPath; }
        int counter = 1;
        do
        {
            newPath = Path.Combine(Path.GetDirectoryName(destPath) ?? "",
                $"{Path.GetFileNameWithoutExtension(destPath)}_{counter++}{Path.GetExtension(destPath)}");
        } while (File.Exists(newPath));
        return newPath;
    }
    public static async Task<bool> FileCanOpenAsync(string path, int timeout = 1000)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        bool res = false;
        do
        {
            try
            {
                using (var f = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    res = true;
                }
            }
            catch { }
            await Task.Delay(500);
        } while (!res || stopwatch.Elapsed.TotalMilliseconds > timeout);
        return res;
    }

    public static bool DoesPrinterExist(string printerName)
    {
        var res = false;
        var _printers = PrinterSettings.InstalledPrinters;
        for (int i = 0; i < _printers.Count; i++)
        {
            if (string.Compare(_printers[i], printerName, StringComparison.OrdinalIgnoreCase) == 0) { res = true; break; }
        }
        return res;
    }
}
