namespace FrequencyAnalysis;

public interface IFrequencyAnalyser
{
    Dictionary<char, int> GetCharacterFrequencies(string text);

    int GetCharacterCount(string text);
}