namespace MikeNakis.Clio;

using Sys = System;

public sealed class TestingOptions
{
	public Sys.Func<string, string>? FileReader { get; }

	public TestingOptions( Sys.Func<string, string>? fileReader )
	{
		FileReader = fileReader;
	}
}
