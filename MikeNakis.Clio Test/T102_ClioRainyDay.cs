namespace MikeNakis.Clio_Test;

using MikeNakis.Clio;
using MikeNakis.Clio.Extensions;
using MikeNakis.Kit.Extensions;
using static Statics;
using Sys = System;
using VSTesting = Microsoft.VisualStudio.TestTools.UnitTesting;

[VSTesting.TestClass]
public sealed class T102_ClioRainyDay
{
	static ArgumentParser newArgumentParser( Sys.Func<string, string>? fileReader = null )
	{
		TestingOptions testingOptions = new( "TestApp", fileReader );
		return new ArgumentParser( null, null, testingOptions );
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
}

