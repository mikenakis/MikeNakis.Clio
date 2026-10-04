namespace MikeNakis.Clio_Test;

using System.Collections.Generic;
using System.Linq;
using MikeNakis.Clio;
using MikeNakis.Clio.Extensions;
using MikeNakis.Kit.Extensions;
using static Statics;
using Sys = System;
using SysIo = System.IO;
using VSTesting = Microsoft.VisualStudio.TestTools.UnitTesting;

[VSTesting.TestClass]
public sealed class T102_ClioRainyDay
{
	static ArgumentParser newArgumentParser( Sys.Func<string, string>? fileReader = null )
	{
		TestingOptions testingOptions = new( fileReader );
		return new ArgumentParser( "TestApp", null, null, testingOptions );
	}

	static string[] split( string commandLine ) => commandLine.Split( ' ', Sys.StringSplitOptions.RemoveEmptyEntries | Sys.StringSplitOptions.TrimEntries );

	static bool tryParse( ArgumentParser argumentParser, string commandLine )
	{
		string[] tokens = split( commandLine );
		return argumentParser.TryParse( tokens, lineOutputConsumer );
	}

	static void lineOutputConsumer( string text )
	{
		Assert( false ); //we do not expect the line-output-consumer to ever be invoked.
	}

	enum Enum1
	{
		Value1,
		Value2,
		Value3
	}

	static readonly VerbHandler emptyVerbHandler = argumentParser => //
			{
				argumentParser.TryParse(); //must be invoked because by design, failure to invoke causes exception.
			};

	[VSTesting.TestMethod]
	public void T201_Switch_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "alpha" ) );
		var exception = (DuplicateArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T202_Option_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringOption( "alpha" ) );
		var exception = (DuplicateArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T203_Parameter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringPositional( "alpha" ) );
		var exception = (DuplicateArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T204_Switch_Single_Letter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "bravo", 'a' ) );
		var exception = (DuplicateArgumentSingleLetterNameException)caughtException.OrThrow();
		Assert( exception.ArgumentShortFormName == 'a' );
	}

	[VSTesting.TestMethod]
	public void T205_Option_Single_Letter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringOption( "bravo", 'a' ) );
		var exception = (DuplicateArgumentSingleLetterNameException)caughtException.OrThrow();
		Assert( exception.ArgumentShortFormName == 'a' );
	}

	[VSTesting.TestMethod]
	public void T206_Positional_Value_Cannot_Be_Accessed_Before_Parsing()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddRequiredStringPositional( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => _ = alpha.Value );
		var exception = (CommandLineHasNotBeenParsedException)caughtException.OrThrow();
	}

	[VSTesting.TestMethod]
	public void T207_Argument_Cannot_Be_Added_After_Parsing()
	{
		ArgumentParser argumentParser = newArgumentParser();
		tryParse( argumentParser, "" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddRequiredStringPositional( "alpha" ) );
		var exception = (CommandLineHasAlreadyBeenParsedException)caughtException.OrThrow();
	}

	[VSTesting.TestMethod]
	public void T208_Named_Argument_Name_Must_Be_Valid()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "-" ) );
		var exception = (InvalidArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "-" );
	}

	[VSTesting.TestMethod]
	public void T209_Positional_Argument_Name_Must_Be_Valid()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringPositional( "-invalid" ) );
		var exception = (InvalidArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "-invalid" );
	}

	[VSTesting.TestMethod]
	public void T210_Required_Positional_Must_Precede_Optional_Positional()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddRequiredStringPositional( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.RequiredPositionalMustPrecedeOptionalPositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T211_Required_Positional_Must_Precede_Positional_With_Default()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositionalWithDefault( "alpha", "alpha-default" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddRequiredStringPositional( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.RequiredPositionalMustPrecedeOptionalPositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T212_Named_Argument_Must_Precede_Positional()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.NamedArgumentMustPrecedePositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T213_Switch_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T214_Option_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringOption( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T215_Positional_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringPositional( "bravo" ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T216_Verb_May_Not_Be_Preceded_By_Positional_Argument()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddVerb( "bravo", "bravo-description", emptyVerbHandler ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.VerbMayNotBePrecededByPositionalArgument );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T217_Verb_May_Not_Be_Preceded_By_Required_Argument()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddRequiredStringOption( "alpha" );
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddVerb( "bravo", "bravo-description", emptyVerbHandler ) );
		var exception = (InvalidArgumentOrderingException)caughtException.OrThrow();
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.VerbMayNotBePrecededByRequiredArgument );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T218_Switch_Name_Must_Be_Longer_Than_One_Character()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddSwitch( "a" ) );
		var exception = (InvalidArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "a" );
	}

	[VSTesting.TestMethod]
	public void T219_Option_Name_Must_Be_Longer_Than_One_Character()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddStringOption( "a" ) );
		var exception = (InvalidArgumentNameException)caughtException.OrThrow();
		Assert( exception.ArgumentName == "a" );
	}

	[VSTesting.TestMethod]
	public void T220_Verb_Handler_Must_Invoke_TryParse()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddVerb( "juliett", "", argumentParser => { } ) );
		var exception = (VerbHandlerDidNotInvokeTryParseMethodException)caughtException.OrThrow();
		Assert( exception.VerbName == "juliett" );
	}

	[VSTesting.TestMethod]
	public void T221_Verb_Handler_Must_Not_Invoke_TryParse_More_Than_Once()
	{
		ArgumentParser argumentParser = newArgumentParser();
		Sys.Exception? caughtException = TryCatch( () => //
				argumentParser.AddVerb( "juliett", "", argumentParser => //
					{
						argumentParser.TryParse();
						argumentParser.TryParse();
					} )
				);
		var exception = (TryParseInvokedMoreThanOnceException)caughtException.OrThrow();
		Assert( exception.VerbName == "juliett" );
	}

	[VSTesting.TestMethod]
	public void T222_Unexpected_Empty_Token_Is_Reported_As_User_Error()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "alpha" );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( [""], outputLines.Add );
		Assert( !result );
		Assert( outputLines.Count > 0 );
	}

	[VSTesting.TestMethod]
	public void T223_Missing_Response_File_Is_Reported_As_User_Error()
	{
		const string responseFilename = "missing.txt";
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( [$"@{responseFilename}"], outputLines.Add );
		Assert( !result );
		Assert( outputLines.Any( line => line.Contains( responseFilename, Sys.StringComparison.Ordinal ) ) );
		return;

		static string fileReader( string filename ) => throw new SysIo.FileNotFoundException( $"Could not find file '{filename}'.", filename );
	}

	[VSTesting.TestMethod]
	public void T224_Self_Referencing_Response_File_Is_Reported_As_User_Error()
	{
		const string responseFilename = "self.txt";
		const int maxInvocationCount = 100; //guards against an infinite loop, so that the test fails instead of hanging.
		int invocationCount = 0;
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( [$"@{responseFilename}"], outputLines.Add );
		Assert( invocationCount < maxInvocationCount );
		Assert( !result );
		Assert( outputLines.Count > 0 );
		return;

		string fileReader( string filename )
		{
			if( ++invocationCount >= maxInvocationCount )
				throw new Sys.InvalidOperationException( "Response file recursion was not detected." );
			return $"@{responseFilename}";
		}
	}

	[VSTesting.TestMethod]
	public void T225_Bare_At_Sign_Is_Reported_As_User_Error()
	{
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( ["@"], outputLines.Add );
		Assert( !result );
		Assert( outputLines.Count > 0 );
		Assert( outputLines[0] == "Expected a file name after '@'." );
		return;

		static string fileReader( string filename ) => throw new Sys.InvalidOperationException( "The file reader should not have been invoked." );
	}

	//TODO: this test documents a limitation: the same response file may not be used both before and after a verb, even
	//      though this does not cause an endless loop. This is because the set of response files already read is kept
	//      in the root argument parser and shared by all verb parsers, so any second use of a response file is rejected.
	//      A better approach would be to track real nesting: record which response file each expanded token came from,
	//      and reject a response file only if it is currently being expanded, i.e. if it appears in its own chain of
	//      inclusions. This would allow any number of repeated uses, (even at the same level, as in `@a.rsp @a.rsp`,)
	//      while still catching every loop. It would require the list of tokens to carry the origin of each token.
	//      When this is done, this test should be changed to expect success.
	[VSTesting.TestMethod]
	public void T226_Same_Response_File_Before_And_After_Verb_Is_Reported_As_User_Error()
	{
		const string responseFilename = "common.txt";
		int invocationCount = 0;
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		argumentParser.AddVerb( "bravo", "bravo-description", argumentParser => //
			{
				argumentParser.AddSwitch( "alpha" );
				argumentParser.TryParse();
			} );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( [$"@{responseFilename}", "bravo", $"@{responseFilename}"], outputLines.Add );
		Assert( !result );
		Assert( invocationCount == 1 );
		Assert( outputLines.Count > 0 );
		Assert( outputLines[0] == $"Response file '{responseFilename}' is included more than once." );
		return;

		string fileReader( string filename )
		{
			invocationCount++;
			return "--alpha";
		}
	}
}

