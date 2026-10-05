namespace MikeNakis.Clio;

using System.Collections.Generic;
using Sys = System;

/// <summary>Represents a verb handler.</summary>
public delegate void VerbHandler( ChildArgumentParser argumentParser );

/// <summary>Parses command-line arguments for a program.</summary>
public sealed class ArgumentParser : BaseArgumentParser
{
	public string VerbTerm { get; }
	internal override BaseArgumentParser? Parent => null;
	internal override ArgumentParser GetRootArgumentParser() => this;
	internal int ScreenWidth { get; }
	internal readonly Sys.Func<string, string> FileReader;
	//Case-insensitive because Windows and macOS file systems usually are; otherwise a self-including response file
	//referred to with different casing would still cause an endless loop.
	internal readonly HashSet<string> ResponseFilesRead = new( Sys.StringComparer.OrdinalIgnoreCase );

	/// <summary>Constructor.</summary>
	/// <param name="programName" >Specifies the name of the program to use when displaying help.</param>
	/// <param name="verbTerm" >Specifies the term to use in place of 'verb' when displaying help.</param>
	/// <param name="screenWidth" >Specifies the screen width to target when word-wrapping help text.
	/// If omitted, a reasonable default is used.</param>
	/// <param name="testingOptions" >Supplies options used for testing.</param>
	public ArgumentParser( string? programName = null, string? verbTerm = null, int? screenWidth = null, TestingOptions? testingOptions = null )
		: base( programName ?? Sys.AppDomain.CurrentDomain.FriendlyName )
	{
		VerbTerm = verbTerm ?? "verb";
		ScreenWidth = screenWidth ?? 120;
		FileReader = testingOptions?.FileReader ?? Sys.IO.File.ReadAllText;
	}

	/// <summary>Adds a verb.</summary>
	/// <param name="name">The name of the verb.</param>
	/// <param name="description">The description of the verb, for use when displaying help.</param>
	/// <param name="verbHandler">The handler of the verb.</param>
	public IVerbArgument AddVerb( string name, string description, VerbHandler verbHandler )
	{
		return new VerbArgument( this, name, description, verbHandler );
	}

	/// <summary>Parses an array of command-line tokens, stores values in arguments, invokes verb handlers, etc.</summary>
	/// <remarks>If something goes wrong, an exception is thrown.</remarks>
	/// <param name="arrayOfToken">The command-line tokens to parse.</param>
	public void Parse( string[] arrayOfToken )
	{
		List<string> tokens = new( arrayOfToken );
		Parse( tokens, 0 );
	}
}
