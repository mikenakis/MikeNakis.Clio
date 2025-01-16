namespace MikeNakis.Clio;

public sealed class TestingOptions
{
	public string? ProgramName { get; }

	public TestingOptions( string? programName )
	{
		ProgramName = programName;
	}
}
