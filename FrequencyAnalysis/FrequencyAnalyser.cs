namespace FrequencyAnalysis;

public class FrequencyAnalyser : IFrequencyAnalyser
{
    public Dictionary<char, int> GetCharacterFrequencies(string text)
    {
        return text
            .Where(c => !char.IsWhiteSpace(c))
            .GroupBy(c => c)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public int GetCharacterCount(string text)
    {
        return text!.Count(c => !char.IsWhiteSpace(c));
    }
}