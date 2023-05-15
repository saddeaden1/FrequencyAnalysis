using LanguageExt;

namespace FrequencyAnalysis;

public interface ITextFileReader
{
    Either<FileReaderError, string> ReadTextFile(string filename);
}