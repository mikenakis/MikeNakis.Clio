namespace MikeNakis.Clio;

using System.Collections.Generic;
using Sys = System;

///<summary>Represents a verb handler.</summary>
public delegate void VerbHandler( ChildArgumentParser argumentParser );

///<summary>Parses command-line arguments for a program.</summary>
public sealed class ArgumentParser : BaseArgumentParser
{
	public string VerbTerm { get; }
	internal override BaseArgumentParser? Parent => null;
	internal override ArgumentParser GetRootArgumentParser() => this;
	internal int ScreenWidth { get; }
	readonly Sys.Func<string, string> fileReader;

	/// <summary>Constructor.</summary>
	/// <param name="verbTerm" >Specifies the term to use in place of 'verb' when displaying help.</param>
	/// <param name="screenWidth" >Specifies the screen width to target when word-wrapping help text.
	/// If omitted, a reasonable default is used.</param>
	/// <param name="testingOptions" >Supplies options used for testing.</param>
	public ArgumentParser( string? verbTerm = null, int? screenWidth = null, TestingOptions? testingOptions = null )
		: base( testingOptions?.ProgramName ?? Sys.AppDomain.CurrentDomain.FriendlyName )
	{
		VerbTerm = verbTerm ?? "verb";
		ScreenWidth = screenWidth ?? 120;
		fileReader = testingOptions?.FileReader ?? Sys.IO.File.ReadAllText;
	}

	///<summary>Adds a verb.</summary>
	///<param name="name">The name of the verb.</param>
	///<param name="description">The description of the verb, for use when displaying help.</param>
	///<param name="verbHandler">The handler of the verb.</param>
	public IVerbArgument AddVerb( string name, string description, VerbHandler verbHandler )
	{
		return new VerbArgument( this, name, description, verbHandler );
	}

	/// <summary>Parses an array of command-line tokens, stores values in arguments, invokes verb handlers, etc.</summary>
	/// <remarks>If something goes wrong, (or if the `--help` option is supplied,) it displays all necessary messages
	/// and returns <c>false</c>, meaning that the current process should terminate.</remarks>
	/// <param name="arrayOfToken">The command-line tokens to parse.</param>
	/// <param name="lineOutputConsumer">A consumer for text output.
	/// If omitted, it defaults to the <see cref="Sys.IO.TextWriter.WriteLine( string )"/> method of <see cref="Sys.Console.Error"/>.</param>
	/// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
	public bool TryParse( string[] arrayOfToken, Sys.Action<string>? lineOutputConsumer = null )
	{
		List<string> tokens = new( arrayOfToken );
		return tryParse( tokens, lineOutputConsumer ?? Sys.Console.Error.WriteLine );

		bool tryParse( List<string> tTokens, Sys.Action<string> lineOutputConsumer )
		{
			List<string> tokens = new( arrayOfToken );
			Helpers.SplitCombinedSingleLetterArguments( tokens );
			Helpers.AddArgumentsFromResponseFiles( tokens, fileReader );
			try
			{
				ParseRemainingTokens( 0, tokens );
			}
			catch( HelpException helpException )
			{
				helpException.ArgumentParser.OutputHelp( lineOutputConsumer );
				return false;
			}
			catch( UserException userException )
			{
				Helpers.OutputExceptionMessage( userException, lineOutputConsumer );
				string fullName = GetFullName( ' ' );
				lineOutputConsumer.Invoke( $"Try '{fullName} --help' for more information." );
				return false;
			}
			return true;
		}
	}
}
