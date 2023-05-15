using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Autofac;
using FluentAssertions;

namespace FrequencyAnalysis.UnitTests;

public class ProgramTests
{
    private Mock<ITextFileReader> _textFileReaderMock;
    private Mock<IFrequencyAnalyser> _frequencyAnalyserMock;
    private Mock<IValidator<string[]>> _validatorMock;
    private TestEnvironmentExiter _environmentExiter;
    private IFixture _fixture;
    private StringWriter _consoleOutput;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        _fixture = new Fixture();

        _environmentExiter = new TestEnvironmentExiter();
        builder.RegisterInstance(_environmentExiter).As<IEnvironmentExiter>();

        _textFileReaderMock = new Mock<ITextFileReader>();
        builder.RegisterInstance(_textFileReaderMock.Object).As<ITextFileReader>();

        _frequencyAnalyserMock = new Mock<IFrequencyAnalyser>();
        builder.RegisterInstance(_frequencyAnalyserMock.Object).As<IFrequencyAnalyser>();

        _validatorMock = new Mock<IValidator<string[]>>();
        builder.RegisterInstance(_validatorMock.Object).As<IValidator<string[]>>();
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _ = new Program(builder.Build());

        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [Test]
    public async Task Main_ValidInput_ReturnsCharacterFrequencies()
    {
        //arrange
        var filePath = _fixture.Create<string>();
        var text = _fixture.Create<string>();

        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath))
            .Returns(text);
        _frequencyAnalyserMock.Setup(x => x.GetCharacterFrequencies(text))
            .Returns(_fixture.Create<Dictionary<char, int>>());

        var characterCount = _fixture.Create<int>();
        _frequencyAnalyserMock.Setup(x => x.GetCharacterCount(It.IsAny<string>())).Returns(characterCount);

        //act
        try
        {
            await Program.Main(new[] { filePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _consoleOutput.ToString().Should().Contain($"Total characters: {characterCount}");
    }

    [Test]
    public async Task Main_ValidInput_CharactersArePrinted()
    {
        //arrange
        var filePath = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var frequencies = _fixture.Create<Dictionary<char, int>>();

        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath)).Returns(text);
        _frequencyAnalyserMock.Setup(x => x.GetCharacterFrequencies(text)).Returns(frequencies);

        //act
        try
        {
            await Program.Main(new[] { filePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        frequencies.ToList().ForEach(kvp =>
        {
            _consoleOutput.ToString().Should().Contain($"{kvp.Key} ({kvp.Value})");
        });
    }

    [Test]
    public async Task Main_InvalidFilePath_FailsWithErrorMessage()
    {
        //arrange
        var invalidFilePath = _fixture.Create<string>();
        _textFileReaderMock.Setup(x => x.ReadTextFile(invalidFilePath))
            .Returns(new FileReaderError("The file does not exist."));

        //act
        try
        {
            await Program.Main(new[] { invalidFilePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("The file does not exist.");
    }

    [Test]
    public async Task Main_InvalidArguments_ValidationErrorDisplayed()
    {
        //arrange
        var errorMessage = _fixture.Create<string>();
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = new List<ValidationFailure>
                {
                        new ()
                        {
                            ErrorMessage = errorMessage
                        }
                }
            });

        //act
        try
        {
            await Program.Main(new[] { "filePath", "invalid_option" });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain(errorMessage);
    }

    [Test]
    public async Task Main_NoArguments_ValidationErrorDisplayed()
    {
        //arrange
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = new List<ValidationFailure>
                {
                        new ()
                        {
                            ErrorMessage = "No file path entered please enter a file path"
                        }
                }
            });

        //act
        try
        {
            await Program.Main(Array.Empty<string>());
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("No file path entered please enter a file path");
    }

    [Test]
    public async Task Main_GetCharacterFrequenciesThrowsException_ErrorMessageDisplayed()
    {
        //arrange
        var filePath = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var exceptionMessage = _fixture.Create<string>();
        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath)).Returns(text);
        _frequencyAnalyserMock.Setup(x => x.GetCharacterFrequencies(text)).Throws(new Exception(exceptionMessage));

        //act
        try
        {
            await Program.Main(new[] { filePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain(exceptionMessage);
    }

    [Test]
    public async Task Main_GetCharacterCountThrowsException_ErrorMessageDisplayed()
    {
        //arrange
        var filePath = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var exceptionMessage = _fixture.Create<string>();

        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath)).Returns(text);
        _frequencyAnalyserMock.Setup(x => x.GetCharacterCount(text)).Throws(new Exception(exceptionMessage));
        _frequencyAnalyserMock.Setup(x => x.GetCharacterFrequencies(text))
            .Returns(_fixture.Create<Dictionary<char, int>>());

        //act
        try
        {
            await Program.Main(new[] { filePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain(exceptionMessage);
    }

    [TestCase("")]
    [TestCase("         ")]
    public async Task Main_TextFileNoCharacter_ErrorMessageDisplayed(string fileText)
    {
        //arrange
        var filePath = _fixture.Create<string>();
        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath)).Returns(fileText);

        //act
        try
        {
            await Program.Main(new[] { filePath });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("The file selected is empty");
    }


    [Test]
    public async Task Main_LowerCaseOptionSelected_TextFedIntoMockIsLowerCase()
    {
        //arrange
        var filePath = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var lowerCaseText = text.ToLower();
        var frequencies = _fixture.Create<Dictionary<char, int>>();

        _textFileReaderMock.Setup(x => x.ReadTextFile(filePath)).Returns(text);
        _frequencyAnalyserMock.Setup(x => x.GetCharacterFrequencies(lowerCaseText)).Returns(frequencies);

        //act
        try
        {
            await Program.Main(new[] { filePath, "true" });
        }
        catch (UnitTestException)
        {
        }

        //assert
        _frequencyAnalyserMock.Verify(x => x.GetCharacterFrequencies(lowerCaseText), Times.Once);
    }
}