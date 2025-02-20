
using Newtonsoft.Json;

public class ThereforeServer
{
    private readonly string _Tenant;
    private readonly string _Auth;
    private readonly string _BaseUrl;
    private string _LastError = string.Empty;

    private const string ExecuteSimpleQuery_Endpoint = "/theservice/v0001/restun/ExecuteSimpleQuery";
    private const string GetDocument_Endpoint = "/theservice/v0001/restun/GetDocument";
    private const string GetConvertedDocStreamsRaw_Endpoint = "/theservice/v0001/restun/GetConvertedDocStreamsRaw";

    public ThereforeServer(string Tenant, string BaseUrl, string Auth)
    {
        _Tenant = Tenant;
        _Auth = Auth;
        _BaseUrl = BaseUrl;
    }

    public string GetLastError()
    {
        return _LastError;
    }
    private async Task<string?> SendRESTRequest(HttpMethod method, string requestEndpoint, string jsonContent)
    {
        string? res = null;
        var client = new HttpClient();
        var request = new HttpRequestMessage(method, $"{_BaseUrl}{requestEndpoint}");
        request.Headers.Add("TenantName", _Tenant);
        request.Headers.Add("Authorization", _Auth);
        var content = new StringContent(jsonContent, null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            res = await response.Content.ReadAsStringAsync();
        }
        else
        {
            _LastError = $"{response.StatusCode}: {response.ReasonPhrase}";
        }
        return res;
    }

    /// <summary>
    /// Returns byte array
    /// </summary>
    /// <param name="method"></param>
    /// <param name="requestEndpoint"></param>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    private async Task<byte[]> SendRESTRequestRAW(HttpMethod method, string requestEndpoint, string jsonContent)
    {
        List<byte> res = new List<byte>();
        var client = new HttpClient();
        var request = new HttpRequestMessage(method, $"{_BaseUrl}{requestEndpoint}");
        request.Headers.Add("TenantName", _Tenant);
        request.Headers.Add("Authorization", _Auth);
        var content = new StringContent(jsonContent, null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            res = response.Content.ReadAsByteArrayAsync().Result.ToList();
        }
        else
        {
            _LastError = $"{response.StatusCode}: {response.ReasonPhrase}";
        }
        return res.ToArray();
    }

    public async Task<string> GetConvertedDocStreamsRawAsync(GetConvertedDocStreamsRawRequest request, string sourceFilename = "", string destinationDirectory = "")
    {
        string savedFile = "";
        destinationDirectory = (!string.IsNullOrEmpty(destinationDirectory) ? destinationDirectory : Path.GetTempPath());
        sourceFilename = !string.IsNullOrEmpty(sourceFilename) ? sourceFilename : Guid.NewGuid().ToString();
        string tempOutputFilename = Utils.GetUniqueFileName(Path.Combine(destinationDirectory, Path.ChangeExtension(sourceFilename, "tmp")));
        var rawBytes = await SendRESTRequestRAW(HttpMethod.Post, GetConvertedDocStreamsRaw_Endpoint, JsonConvert.SerializeObject(request));
        if (rawBytes.Length > 0)
        {
            using (FileStream fs = new FileStream(tempOutputFilename, FileMode.Create))
            {
                await fs.WriteAsync(rawBytes);
            }
            savedFile = Utils.GetUniqueFileName(Path.ChangeExtension(tempOutputFilename, ThereforeUtils.GetExtensionForConversionType(request.ConversionOptions.ConvertTo)));
            File.Move(tempOutputFilename, savedFile);
        }
        return savedFile;
    }

    public static string Stringify(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }


}


