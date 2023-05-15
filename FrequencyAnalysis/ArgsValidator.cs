using FluentValidation;

namespace FrequencyAnalysis;

public class ArgsValidator : AbstractValidator<string[]>
{
    public ArgsValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("No arguments supplied, please provide a file path and an optional case sensitivity option");
        RuleFor(x => x).Must(x => x.Length is 1 or 2).WithMessage("Too many arguments provided please provide only a file path and an optional case sensitivity option");
        When(x => x.Length > 0, () =>
        {
            RuleFor(x => x[0]).NotEmpty().WithMessage("No file path entered please enter a file path");
        });
        When(x => x.Length > 1, () =>
        {
            RuleFor(x => x[1]).Must(BeBoolean).WithMessage("The second argument should be a Boolean value (case sensitivity option)");
        });
    }

    private bool BeBoolean(string arg)
    {
        return bool.TryParse(arg, out _);
    }
}