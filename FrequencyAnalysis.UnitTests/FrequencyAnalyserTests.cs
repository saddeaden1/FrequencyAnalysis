using FluentAssertions;

namespace FrequencyAnalysis.UnitTests;

public class FrequencyAnalyserTests
{
    private FrequencyAnalyser _analyser;

    [SetUp]
    public void Setup()
    {
        _analyser = new FrequencyAnalyser();
    }

    [Test]
    public void GetCharacterFrequencies_GivenEmptyText_ReturnsEmptyDictionary()
    {
        var result = _analyser.GetCharacterFrequencies("");
        result.Should().BeEmpty();
    }

    [Test]
    public void GetCharacterFrequencies_GivenWhiteSpaceOnly_ReturnsEmptyDictionary()
    {
        var result = _analyser.GetCharacterFrequencies("   ");
        result.Should().BeEmpty();
    }

    [Test]
    public void GetCharacterFrequencies_GivenTextWithNoRepeatingCharacters_ReturnsCorrectFrequencies()
    {
        var result = _analyser.GetCharacterFrequencies("abc");
        result.Count.Should().Be(3);
        result['a'].Should().Be(1);
        result['b'].Should().Be(1);
        result['c'].Should().Be(1);
    }

    [Test]
    public void GetCharacterFrequencies_GivenTextWithRepeatingCharacters_ReturnsCorrectFrequencies()
    {
        var result = _analyser.GetCharacterFrequencies("aabbcc");
        result.Count.Should().Be(3);
        result['a'].Should().Be(2);
        result['b'].Should().Be(2);
        result['c'].Should().Be(2);
    }

    [Test]
    public void GetCharacterFrequencies_GivenTextWithMixedCase_ReturnsCaseSensitiveFrequencies()
    {
        var result = _analyser.GetCharacterFrequencies("aAbB");
        result.Count.Should().Be(4);
        result['a'].Should().Be(1);
        result['A'].Should().Be(1);
        result['b'].Should().Be(1);
        result['B'].Should().Be(1);
    }

    [Test]
    public void GetCharacterFrequencies_GivenTextWithWhiteSpaces_IgnoresWhiteSpaces()
    {
        var result = _analyser.GetCharacterFrequencies(" a a ");
        result.Count.Should().Be(1);
        result['a'].Should().Be(2);
    }

    [Test]
    public void GetCharacterCount_GivenEmptyString_ReturnsZero()
    {
        var result = _analyser.GetCharacterCount("");
        result.Should().Be(0);
    }

    [Test]
    public void GetCharacterCount_GivenStringWithOnlySpaces_ReturnsZero()
    {
        var result = _analyser.GetCharacterCount("   ");
        result.Should().Be(0);
    }

    [Test]
    public void GetCharacterCount_GivenStringWithCharactersAndSpaces_ReturnsCharacterCountExcludingSpaces()
    {
        var result = _analyser.GetCharacterCount("a b c");
        result.Should().Be(3);
    }
}