public enum ConvertToType { Original = 0, SingleTIFF = 1, SinglePDF = 2, MultipageTIFF = 3, MultipagePDF = 4, SearchablePDF = 5, SearchablePDFA = 6, Jpeg = 50 }

public static partial class ThereforeUtils
{
    public static string GetExtensionForConversionType(ConvertToType t)
    {
        return t switch
        {
            ConvertToType.SinglePDF or ConvertToType.MultipagePDF or ConvertToType.SearchablePDF or ConvertToType.SearchablePDFA => "pdf",
            ConvertToType.SingleTIFF or ConvertToType.MultipageTIFF => "tif",
            ConvertToType.Jpeg => "jpg",
            _ => string.Empty,
        };
    }
}

public class GetConvertedDocStreamsRawRequest
{
    required public Conversionoptions ConversionOptions { get; set; }
    required public int DocNo { get; set; }
    public int[] StreamNos { get; set; } = [];
    public bool ArchiveConvertedFiles { get; set; }
    public string? CustomArchiveFileName { get; set; }
    public int VersionNo { get; set; }
}

public class Conversionoptions
{
    public int AnnotationMode { get; set; }
    public string? CertificateName { get; set; }
    public ConvertToType ConvertTo { get; set; }
    public int SignatureMode { get; set; }
    public string? TimeStampPwd { get; set; }
    public string? TimeStampServer { get; set; }
    public string? TimeStampUser { get; set; }
    public string? MultipageStreamName { get; set; }
}



