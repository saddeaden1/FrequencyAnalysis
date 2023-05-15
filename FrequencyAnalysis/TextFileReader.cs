using LanguageExt;

namespace FrequencyAnalysis;

public class TextFileReader : ITextFileReader
{
    public Either<FileReaderError, string> ReadTextFile(string filename)
    {
        try
        {
            if (!File.Exists(filename))
            {
                return new FileReaderError("The file does not exist.");
            }

            var text = File.ReadAllText(filename);

            return text;
        }
        catch (PathTooLongException)
        {
            return new FileReaderError("The specified path, file name, or both exceed the system-defined maximum length.");
        }
        catch (DirectoryNotFoundException)
        {
            return new FileReaderError("The specified path is invalid.");
        }
        catch (IOException)
        {
            return new FileReaderError("An I/O error occurred while opening the file.");
        }
        catch (ArgumentException)
        {
            return new FileReaderError("The specified path is empty, contains only white spaces, or contains invalid characters.");
        }
        catch (Exception ex)
        {
            return new FileReaderError($"An unexpected error occurred: {ex.Message}");
        }
    }
}