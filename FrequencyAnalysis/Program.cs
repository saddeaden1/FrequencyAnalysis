using Autofac;
using FluentValidation;

namespace FrequencyAnalysis;

public class Program
{

    private static IContainer _container;
    private static IEnvironmentExiter _exiter;
    private static ITextFileReader _textFileReader;
    private static IFrequencyAnalyser _frequencyAnalyser;
    private static IValidator<string[]> _validator;

    public Program(IContainer container)
    {
        _container = container;
    }

    private static void BuildContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<EnvironmentExiter>().As<IEnvironmentExiter>();
        builder.RegisterType<TextFileReader>().As<ITextFileReader>();
        builder.RegisterType<FrequencyAnalyser>().As<IFrequencyAnalyser>();
        builder.RegisterType<ArgsValidator>().As<IValidator<string[]>>();

        _container = builder.Build();
    }

    public static async Task Main(string[] args)
    {
        if (_container == null)
        {
            BuildContainer();
        }

        _exiter = _container!.Resolve<IEnvironmentExiter>();
        _textFileReader = _container!.Resolve<ITextFileReader>();
        _frequencyAnalyser = _container!.Resolve<IFrequencyAnalyser>();
        _validator = _container!.Resolve<IValidator<string[]>>();

        await ValidateArgs(args);

        try
        {
            var textEither = _textFileReader.ReadTextFile(args[0]);

            textEither.Match(text =>
            {

                if (string.IsNullOrWhiteSpace(text))
                {
                    HandleError("The file selected is empty");
                }

                var caseSensitive = args.Length == 2 && args[1].ToLower() == "true";

                if (!caseSensitive)
                { 
                    text = text!.ToLower();
                }

                var frequency = _frequencyAnalyser.GetCharacterFrequencies(text);

                Console.WriteLine($"Total characters: {_frequencyAnalyser.GetCharacterCount(text)}");

                foreach (var f in frequency)
                {
                    
                    Console.WriteLine($"{f.Key} ({f.Value})");
                }
                _exiter.Exit(0);

            }, error =>
            {
                HandleError(error.Reason);
            });
        }
        catch (Exception ex)
        {
            HandleError(ex.Message);
        }
    }

    private static void HandleError(string errorReason)
    {
        Console.WriteLine($"The program has failed for the following reason: {errorReason}");
        _exiter.Exit(1);
    }

    private static async Task ValidateArgs(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No file path entered please enter a file path");
            _exiter.Exit(1);
        }

        var validationResult = await _validator.ValidateAsync(args);

        if (!validationResult.IsValid)
        {
            Console.WriteLine(validationResult.ToString());
            _exiter.Exit(1);
        }
    }
}