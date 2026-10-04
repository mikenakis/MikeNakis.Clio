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

#pragma warning disable IDE0008 // Use explicit type

[VSTesting.TestClass]
public sealed class T102_ClioRainyDay
{
	static ArgumentParser newArgumentParser( Sys.Func<string, string>? fileReader = null )
	{
		TestingOptions testingOptions = new( fileReader );
		return new ArgumentParser( "TestApp", null, null, testingOptions );
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
		var exception = Catch<DuplicateArgumentNameException>( () => //
				argumentParser.AddSwitch( "alpha" ) );
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T202_Option_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		var exception = Catch<DuplicateArgumentNameException>( () => //
				argumentParser.AddStringOption( "alpha" ) );
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T203_Parameter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		var exception = Catch<DuplicateArgumentNameException>( () => //
				argumentParser.AddStringPositional( "alpha" ) );
		Assert( exception.ArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T204_Switch_Single_Letter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		var exception = Catch<DuplicateArgumentSingleLetterNameException>( () => //
				argumentParser.AddSwitch( "bravo", 'a' ) );
		Assert( exception.ArgumentShortFormName == 'a' );
	}

	[VSTesting.TestMethod]
	public void T205_Option_Single_Letter_Names_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		var exception = Catch<DuplicateArgumentSingleLetterNameException>( () => //
				argumentParser.AddStringOption( "bravo", 'a' ) );
		Assert( exception.ArgumentShortFormName == 'a' );
	}

	[VSTesting.TestMethod]
	public void T206_Positional_Value_Cannot_Be_Accessed_Before_Parsing()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddRequiredStringPositional( "alpha" );
		var exception = Catch<CommandLineHasNotBeenParsedException>( () => _ = alpha.Value );
	}

	[VSTesting.TestMethod]
	public void T207_Argument_Cannot_Be_Added_After_Parsing()
	{
		ArgumentParser argumentParser = newArgumentParser();
		tryParse( argumentParser, "" );
		var exception = Catch<CommandLineHasAlreadyBeenParsedException>( () => //
				argumentParser.AddRequiredStringPositional( "alpha" ) );
		return;

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
	}

	[VSTesting.TestMethod]
	public void T208_Named_Argument_Name_Must_Be_Valid()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<InvalidArgumentNameException>( () => //
				argumentParser.AddSwitch( "-" ) );
		Assert( exception.ArgumentName == "-" );
	}

	[VSTesting.TestMethod]
	public void T209_Positional_Argument_Name_Must_Be_Valid()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<InvalidArgumentNameException>( () => //
				argumentParser.AddStringPositional( "-invalid" ) );
		Assert( exception.ArgumentName == "-invalid" );
	}

	[VSTesting.TestMethod]
	public void T210_Required_Positional_Must_Precede_Optional_Positional()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddRequiredStringPositional( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.RequiredPositionalMustPrecedeOptionalPositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T211_Required_Positional_Must_Precede_Positional_With_Default()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositionalWithDefault( "alpha", "alpha-default" );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddRequiredStringPositional( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.RequiredPositionalMustPrecedeOptionalPositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T212_Named_Argument_Must_Precede_Positional()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddSwitch( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.NamedArgumentMustPrecedePositional );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T213_Switch_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddSwitch( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T214_Option_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddStringOption( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T215_Positional_May_Not_Be_Added_After_Verb()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddVerb( "alpha", "alpha-description", emptyVerbHandler );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddStringPositional( "bravo" ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.ArgumentMustPrecedeVerb );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T216_Verb_May_Not_Be_Preceded_By_Positional_Argument()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddStringPositional( "alpha" );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddVerb( "bravo", "bravo-description", emptyVerbHandler ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.VerbMayNotBePrecededByPositionalArgument );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T217_Verb_May_Not_Be_Preceded_By_Required_Argument()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddRequiredStringOption( "alpha" );
		var exception = Catch<InvalidArgumentOrderingException>( () => //
				argumentParser.AddVerb( "bravo", "bravo-description", emptyVerbHandler ) );
		Assert( exception.ArgumentOrderingRule == ArgumentOrderingRule.VerbMayNotBePrecededByRequiredArgument );
		Assert( exception.ViolatingArgumentName == "bravo" );
		Assert( exception.PrecedingArgumentName == "alpha" );
	}

	[VSTesting.TestMethod]
	public void T218_Switch_Name_Must_Be_Longer_Than_One_Character()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<InvalidArgumentNameException>( () => //
				argumentParser.AddSwitch( "a" ) );
		Assert( exception.ArgumentName == "a" );
	}

	[VSTesting.TestMethod]
	public void T219_Option_Name_Must_Be_Longer_Than_One_Character()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<InvalidArgumentNameException>( () => //
				argumentParser.AddStringOption( "a" ) );
		Assert( exception.ArgumentName == "a" );
	}

	[VSTesting.TestMethod]
	public void T220_Verb_Handler_Must_Invoke_TryParse()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<VerbHandlerDidNotInvokeTryParseMethodException>( () => //
				argumentParser.AddVerb( "juliett", "", argumentParser => { } ) );
		Assert( exception.VerbName == "juliett" );
	}

	[VSTesting.TestMethod]
	public void T221_Verb_Handler_Must_Not_Invoke_TryParse_More_Than_Once()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<TryParseInvokedMoreThanOnceException>( () => //
				argumentParser.AddVerb( "juliett", "", argumentParser => //
					{
						argumentParser.TryParse();
						argumentParser.TryParse();
					} )
				);
		Assert( exception.VerbName == "juliett" );
	}

	[VSTesting.TestMethod]
	public void T222_Unexpected_Empty_Token_Is_Reported_As_User_Error()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "alpha" );
		var exception = Catch<UnexpectedTokenException>( () => //
				argumentParser.Parse( [""] ) );
		Assert( exception.Token == "" );
	}

	[VSTesting.TestMethod]
	public void T223_Missing_Response_File_Is_Reported_As_User_Error()
	{
		const string responseFilename = "missing.txt";
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		var exception = Catch<ResponseFileUnreadableException>( () => //
				argumentParser.Parse( [$"@{responseFilename}"] ) );
		Assert( exception.FileName == responseFilename );
		Assert( exception.InnerException is SysIo.FileNotFoundException );
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
		var exception = Catch<ResponseFileIncludedMoreThanOnceException>( () => //
				argumentParser.Parse( [$"@{responseFilename}"] ) );
		Assert( exception.FileName == responseFilename );
		Assert( invocationCount == 1 );
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
		Catch<ResponseFileNameExpectedException>( () => //
				argumentParser.Parse( ["@"] ) );
		return;

		static string fileReader( string filename ) => throw new Sys.InvalidOperationException( "The file reader should not have been invoked." );
	}

	//This test documents a known limitation: the same response file may not be used both before and after a verb, even
	//though this does not cause an endless loop. This is because the set of response files already read is kept in the
	//root argument parser and shared by all verb parsers, so any second use of a response file is rejected, (even at the
	//same level, as in `@a.rsp @a.rsp`.) Lifting this limitation would require tracking real nesting, (each token would
	//have to carry the chain of response files it came from,) which was deemed not worth the complexity, since repeated
	//use of a response file is rare, and when it happens, it is reported as a clear user error.
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
		var exception = Catch<ResponseFileIncludedMoreThanOnceException>( () => //
				argumentParser.Parse( [$"@{responseFilename}", "bravo", $"@{responseFilename}"] ) );
		Assert( exception.FileName == responseFilename );
		Assert( invocationCount == 1 );
		return;

		string fileReader( string filename )
		{
			invocationCount++;
			return "--alpha";
		}
	}

	[VSTesting.TestMethod]
	public void T227_TryParse_Outputs_Message_Causes_And_Hint()
	{
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		argumentParser.AddSwitch( "alpha" );
		List<string> outputLines = new();
		bool result = argumentParser.TryParse( ["@missing.txt"], outputLines.Add );
		Assert( !result );
		Assert( outputLines.SequenceEqual( [ //
				"Could not read response file 'missing.txt'.",
				"Because: file-reader-message",
				"Try 'TestApp --help' for more information."] ) );
		return;

		static string fileReader( string filename ) => throw new SysIo.FileNotFoundException( "file-reader-message", filename );
	}

	[VSTesting.TestMethod]
	public void T228_Single_Letter_Name_H_Is_Reserved_For_Help()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "hotel", 'h' );
		var exception = Catch<DuplicateArgumentSingleLetterNameException>( () => //
				argumentParser.Parse( [] ) );
		Assert( exception.ArgumentShortFormName == 'h' );
	}

	[VSTesting.TestMethod]
	public void T229_Single_Letter_Names_Of_An_Argument_Must_Be_Unique()
	{
		ArgumentParser argumentParser = newArgumentParser();
		var exception = Catch<DuplicateArgumentSingleLetterNameException>( () => //
				argumentParser.AddSwitch( "alpha", ['a', 'a'] ) );
		Assert( exception.ArgumentShortFormName == 'a' );
	}

	[VSTesting.TestMethod]
	public void T230_Single_Letter_Names_Must_Be_Unique_Across_All_Arguments()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "alpha", ['a', 'x'] );
		var exception = Catch<DuplicateArgumentSingleLetterNameException>( () => //
				argumentParser.AddSwitch( "bravo", ['b', 'x'] ) );
		Assert( exception.ArgumentShortFormName == 'x' );
	}

	[VSTesting.TestMethod]
	public void T231_Dash_After_Single_Letter_Name_Is_Not_A_Single_Letter_Group()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "alpha", 'a' );
		argumentParser.AddStringPositional( "positional" );
		var exception = Catch<UnexpectedCharactersAfterNamedArgumentException>( () => //
				argumentParser.Parse( ["-a-", "value"] ) );
		Assert( exception.ArgumentName == "alpha" );
		Assert( exception.UnexpectedCharacters == "-" );
	}

	[VSTesting.TestMethod]
	public void T232_Equals_Sign_After_Single_Letter_Names_Is_Not_A_Single_Letter_Group()
	{
		ArgumentParser argumentParser = newArgumentParser();
		argumentParser.AddSwitch( "alpha", 'a' );
		argumentParser.AddStringOption( "bravo", 'b' );
		var exception = Catch<UnexpectedTokenException>( () => //
				argumentParser.Parse( ["-ab=5"] ) );
		Assert( exception.Token == "-ab=5" );
	}

	[VSTesting.TestMethod]
	public void T233_Enum_Option_Accepts_Only_Names_Of_Enum_Members()
	{
		test( "999" );
		test( "1" );
		test( "Value1,Value2" );
		test( " Value1" );
		return;

		static void test( string value )
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddOption( "alpha", EnumCodec<Enum1>.Instance );
			var exception = Catch<UnparsableValueException>( () => //
					argumentParser.Parse( [$"--alpha={value}"] ) );
			Assert( exception.ArgumentName == "alpha" );
			Assert( exception.Token == value );
		}
	}
}

