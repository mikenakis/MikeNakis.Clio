namespace MikeNakis.Clio;

public sealed class TestingOptions
{
	public string? ProgramName { get; }
	public Sys.Func<string, string>? FileReader { get; }

	public TestingOptions( string? programName, Sys.Func<string, string>? fileReader )
	{
		ProgramName = programName;
		FileReader = fileReader;
	}
}
