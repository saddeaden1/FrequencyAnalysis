namespace FrequencyAnalysis;

public class FileReaderError
{
    public FileReaderError(string reason)
    {
        Reason = reason;
    }

    public string Reason { get; set; }
}