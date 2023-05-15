using FluentAssertions;

namespace FrequencyAnalysis.UnitTests;

[TestFixture]
public class ArgsValidatorTests
{
    private ArgsValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ArgsValidator();
    }

    [Test]
    public void Validate_NoArgs_ValidationError()
    {
        var result = _validator.Validate(new string[] { });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "No arguments supplied, please provide a file path and an optional case sensitivity option");
    }

    [Test]
    public void Validate_MoreThanTwoArgs_ValidationError()
    {
        var result = _validator.Validate(new[] { "arg1", "arg2", "arg3" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Too many arguments provided please provide only a file path and an optional case sensitivity option");
    }

    [Test]
    public void Validate_FirstArgIsEmpty_ValidationError()
    {
        var result = _validator.Validate(new[] { "", "true" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "No file path entered please enter a file path");
    }

    [Test]
    public void Validate_SecondArgIsNotBoolean_ValidationError()
    {
        var result = _validator.Validate(new[] { "filePath", "not_boolean" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "The second argument should be a Boolean value (case sensitivity option)");
    }

    [TestCase("true")]
    [TestCase("false")]
    public void Validate_ValidArgs_ValidationSuccess(string caseSensitiveOption)
    {
        var result = _validator.Validate(new[] { "filePath", caseSensitiveOption });

        result.IsValid.Should().BeTrue();
    }
}