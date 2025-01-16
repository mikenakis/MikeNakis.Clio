namespace MikeNakis.Clio;

using System.Collections.Generic;
using Sys = System;

///<summary>Parses command-line arguments for a program.</summary>
public sealed class ArgumentParser : BaseArgumentParser
{
	public string VerbTerm { get; }
	internal override BaseArgumentParser? Parent => null;
	internal override ArgumentParser GetRootArgumentParser() => this;
	internal int ScreenWidth { get; }

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
	}

	///<summary>Adds a verb.</summary>
	///<param name="name">The name of the verb.</param>
	///<param name="description">The description of the verb, for use when displaying help.</param>
	///<param name="handler">The handler of the verb.</param>
	public IVerbArgument AddVerb( string name, string description, Sys.Action<ChildArgumentParser> handler )
	{
		return new VerbArgument( this, name, description, handler );
	}

	/// <summary>Parses an array of command-line tokens, stores values in arguments, invokes verb handlers, etc.</summary>
	/// <remarks>If something goes wrong, (or if the `--help` option is supplied,) it displays all necessary messages
	/// and returns <c>false</c>, meaning that the current process should terminate.</remarks>
	/// <param name="arrayOfToken">The command-line tokens to parse.</param>
	/// <param name="lineOutputConsumer">A consumer for text output.
	/// If omitted, it defaults to the <see cref="Sys.IO.TextWriter.WriteLine( string )"/> methof of <see cref="Sys.Console.Error"/>.</param>
	/// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
	public bool TryParse( string[] arrayOfToken, Sys.Action<string>? lineOutputConsumer = null )
	{
		IReadOnlyList<string> tokens = splitCombinedSingleLetterArguments( arrayOfToken );
		try
		{
			ParseRemainingTokens( 0, tokens );
		}
		catch( HelpException helpException )
		{
			helpException.ArgumentParser.OutputHelp( lineOutputConsumer ?? Sys.Console.Error.WriteLine );
			return false;
		}
		catch( UserException userException )
		{
			string fullName = GetFullName( ' ' );
			outputExceptionMessage( fullName, userException, lineOutputConsumer ?? Sys.Console.Error.WriteLine );
			return false;
		}
		return true;

		static void outputExceptionMessage( string fullName, UserException userException, Sys.Action<string> lineOutputConsumer )
		{
			lineOutputConsumer.Invoke( userException.Message );
			for( Sys.Exception? innerException = userException.InnerException; innerException != null; innerException = innerException.InnerException )
				lineOutputConsumer.Invoke( "Because: " + innerException.Message );
			lineOutputConsumer.Invoke( $"Try '{fullName} --help' for more information." );
		}

		static IReadOnlyList<string> splitCombinedSingleLetterArguments( IReadOnlyList<string> tokens )
		{
			List<string> mutableTokens = new();
			foreach( string token in tokens )
				if( isCombined( token ) )
					foreach( char c in token.Skip( 1 ) )
						mutableTokens.Add( $"-{c}" );
				else
					mutableTokens.Add( token );
			return mutableTokens;

			static bool isCombined( string token ) => token[0] == '-' && token.Length > 2 && token[1] != '-' /* && token[1..].All( Helpers.ShortFormNameIsValid ) */ ;
		}
	}
}
