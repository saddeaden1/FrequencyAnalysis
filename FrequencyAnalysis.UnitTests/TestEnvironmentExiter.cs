namespace FrequencyAnalysis.UnitTests;

public class TestEnvironmentExiter : IEnvironmentExiter
{
    public int ExitCode { get; set; }

    public void Exit(int exitCode)
    {
        ExitCode = exitCode;
        throw new UnitTestException();
    }
}