namespace MikeNakis.Clio_Test;

using MikeNakis.Clio;
using MikeNakis.Clio.Extensions;
using MikeNakis.Kit;
using static Statics;
using Sys = System;
using VSTesting = Microsoft.VisualStudio.TestTools.UnitTesting;

[VSTesting.TestClass]
public sealed class T103_ClioAudits
{
	const string verbTerm = "subcommand";

	static ArgumentParser newArgumentParser( int? screenWidth = null, Sys.Func<string, string>? fileReader = null )
	{
		TestingOptions testingOptions = new( fileReader );
		return new ArgumentParser( "TestApp", verbTerm, screenWidth, testingOptions );
	}

	static bool tryParse( ArgumentParser argumentParser, string commandLine, Sys.Action<string>? lineOutputConsumer = null )
	{
		string[] tokens = commandLine.Split( ' ', Sys.StringSplitOptions.RemoveEmptyEntries | Sys.StringSplitOptions.TrimEntries );
		return argumentParser.TryParse( tokens, lineOutputConsumer );
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Audits

	[VSTesting.TestMethod]
	public void T101_No_Optional_Named_Arguments_Help_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddRequiredStringPositional( "india", "This is the description of india" );
			bool ok = tryParse( argumentParser, "--help", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok ); //because help was requested.
		} );
	}

	[VSTesting.TestMethod]
	public void T102_Simple_Help_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddSwitch( "alpha", 'a', "This is the description of alpha" );
			argumentParser.AddSwitch( "bravo", 'b' );
			argumentParser.AddStringOptionWithDefault( "charlie", "charlie-default", singleLetterName: 'c', description: "This is the description of charlie", parameterName: "charlie-parameter" );
			argumentParser.AddStringOption( "delta", 'd', "This is the description of delta", parameterName: "delta-parameter" );
			argumentParser.AddRequiredStringOption( "echo", description: "This is the description of echo", parameterName: "echo-parameter" );
			argumentParser.AddOptionWithDefault( "foxtrot", EnumCodec<Enum1>.Instance, Enum1.Value2, description: "This is the description of foxtrot", parameterName: "foxtrot-parameter" );
			argumentParser.AddOption( "golf", EnumCodec<Enum1>.Instance, description: "This is the description of golf", parameterName: "golf-parameter" );
			argumentParser.AddRequiredOption( "hotel", EnumCodec<Enum1>.Instance, description: "This is the description of hotel", parameterName: "hotel-parameter" );
			argumentParser.AddStringOptionWithDefault( "mike", "mike-default", description: "This is the description of mike", presetValue: "mike-preset" );
			argumentParser.AddStringOptionWithDefault( "november", "november-default", description: "This is the description of november", presetValue: "november-preset" );
			argumentParser.AddStringOptionWithDefault( "oscar", "oscar-default", description: "This is the description of oscar", presetValue: "oscar-preset" );
			argumentParser.AddRequiredStringPositional( "india", "This is the description of india" );
			argumentParser.AddStringPositional( "juliett", "This is the description of juliett" );
			argumentParser.AddStringPositionalWithDefault( "papa", "papa-default", description: "This is the description of papa" );
			bool ok = tryParse( argumentParser, "--help", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok ); //because help was requested.
		} );
	}

	[VSTesting.TestMethod]
	public void T103_Option_With_Long_Usage_Help_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser( screenWidth: 80 );
			argumentParser.AddSwitch( "alpha", 'a', "This is the description of alpha" );
			argumentParser.AddRequiredStringOption( "echo", description: "This is the description of echo", parameterName: "rather-lengthy-echo-parameter" );
			argumentParser.AddRequiredOption( "hotel", EnumCodec<Enum1>.Instance, description: "This is the description of hotel", parameterName: "rather-lengthy-hotel-parameter" );
			argumentParser.AddStringOptionWithDefault( "mike", "this is mike-default", description: "This is the description of mike, which is a very long description in order to test word-breaking", presetValue: "this is mike-preset" );
			bool ok = tryParse( argumentParser, "--help", lineOutputConsumer: lineOutputConsumer );
			Assert( ok == false ); //because help was requested.
		} );
	}

	[VSTesting.TestMethod]
	public void T104_Root_With_Verbs_Help_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = setup();
			bool ok = tryParse( argumentParser, "--help", lineOutputConsumer );
			Assert( !ok ); //because help was requested.
		} );
	}

	[VSTesting.TestMethod]
	public void T105_Verb_Help_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = setup();
			bool ok = tryParse( argumentParser, "juliett --help", lineOutputConsumer );
			Assert( !ok ); //because help was requested.
		} );
	}

	static ArgumentParser setup()
	{
		ArgumentParser argumentParser = newArgumentParser( screenWidth: 80 );
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a', "This is the description of alpha" );
		//IArgument<string> echo = argumentParser.AddRequiredStringOption( "echo", description: "This is the description of echo", parameterName: "echo-parameter" );
		IArgument juliett = argumentParser.AddVerb( "juliett", "This is the description of juliett", argumentParser => //
			{
				IArgument<bool> lima = argumentParser.AddSwitch( "lima", description: "This is the description of lima" );
				IArgument<string> papa = argumentParser.AddStringPositionalWithDefault( "papa", "papa-default", description: "This is the description of papa" );
				if( !argumentParser.TryParse() )
					return;
			} );
		IArgument kilo = argumentParser.AddVerb( "kilo", "This is the description of kilo", argumentParser =>
			{
				IArgument<bool> lima = argumentParser.AddSwitch( "lima", description: "This is the description of lima" );
				IArgument<string?> mike = argumentParser.AddStringPositional( "mike", "This is the description of mike" );
				if( !argumentParser.TryParse() )
					return;
				Assert( lima.Value == false );
				Assert( mike.Value == null );
			} );
		return argumentParser;
	}

	[VSTesting.TestMethod]
	public void T106_Argument_Dump_Audit()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			ISwitchArgument alpha = argumentParser.AddSwitch( "alpha", 'a' );
			ISwitchArgument bravo = argumentParser.AddSwitch( "bravo", 'b' );
			IOptionArgument<string> charlie = argumentParser.AddStringOptionWithDefault( "charlie", "charlie-default", singleLetterName: 'c' );
			IOptionArgument<string?> delta = argumentParser.AddStringOption( "delta", 'd' );
			IOptionArgument<Enum1> foxtrot = argumentParser.AddOptionWithDefault( "foxtrot", EnumCodec<Enum1>.Instance, Enum1.Value3 );
			IOptionArgument<Enum1?> golf = argumentParser.AddOption( "golf", EnumCodec<Enum1>.Instance );
			IOptionArgument<string> india = argumentParser.AddStringOptionWithDefault( "india", "india-default", presetValue: "india-preset" );
			IOptionArgument<string> juliett = argumentParser.AddStringOptionWithDefault( "juliett", "juliett-default", presetValue: "juliett-preset" );
			IOptionArgument<string> kilo = argumentParser.AddStringOptionWithDefault( "kilo", "kilo-default", presetValue: "kilo-preset" );
			bool mikeHandlerWasInvoked = false;
			IVerbArgument mike = argumentParser.AddVerb( "mike", "This is the description of mike", argumentParser => //
				{
					IOptionArgument<string> echo = argumentParser.AddRequiredStringOption( "echo" );
					IOptionArgument<Enum1> hotel = argumentParser.AddRequiredOption( "hotel", EnumCodec<Enum1>.Instance );
					IArgument<bool> november = argumentParser.AddSwitch( "november" );
					IArgument<bool> oscar = argumentParser.AddSwitch( "oscar" );
					IPositionalArgument<string> lima = argumentParser.AddRequiredStringPositional( "lima" );
					IPositionalArgument<string?> papa = argumentParser.AddStringPositional( "papa" );
					IPositionalArgument<string> quebec = argumentParser.AddStringPositionalWithDefault( "quebec", "quebec-default" );
					IPositionalArgument<string?> romeo = argumentParser.AddStringPositional( "romeo" );
					if( !argumentParser.TryParse() )
						return;
					Assert( alpha.Value == true );
					Assert( bravo.Value == false );
					Assert( charlie.Value == "charlie-default" );
					Assert( delta.Value == null );
					Assert( echo.Value == "echo-value" );
					Assert( foxtrot.Value == Enum1.Value3 );
					Assert( golf.Value == null );
					Assert( hotel.Value == Enum1.Value2 );
					Assert( india.Value == "india-preset" );
					Assert( juliett.Value == "juliett-default" );
					Assert( kilo.Value == "kilo-value" );
					Assert( lima.Value == "lima-value" );
					Assert( november.Value == true );
					Assert( oscar.Value == false );
					Assert( papa.Value == "papa-value" );
					Assert( quebec.Value == "quebec-default" );
					Assert( romeo.Value == null );
					mikeHandlerWasInvoked = true;
				} );
			bool ok = tryParse( argumentParser, "-a --india --kilo=kilo-value mike --echo=echo-value --hotel=Value2 --november lima-value papa-value", lineOutputConsumer: lineOutputConsumer );
			Assert( ok );
			Assert( alpha.Value == true );
			Assert( bravo.Value == false );
			Assert( charlie.Value == "charlie-default" );
			Assert( delta.Value == null );
			Assert( foxtrot.Value == Enum1.Value3 );
			Assert( golf.Value == null );
			Assert( india.Value == "india-preset" );
			Assert( juliett.Value == "juliett-default" );
			Assert( kilo.Value == "kilo-value" );
			Assert( mike.Value != null );
			Assert( mikeHandlerWasInvoked );

			dumpArguments( lineOutputConsumer, argumentParser );
		} );
	}

	static void dumpArguments( Sys.Action<string> lineOutputConsumer, BaseArgumentParser argumentParser )
	{
		recurse( lineOutputConsumer, "", argumentParser );
		return;

		static void recurse( Sys.Action<string> lineOutputConsumer, string prefix, BaseArgumentParser argumentParser )
		{
			foreach( IArgument argument in argumentParser.Arguments )
			{
				string fullArgumentName = dotSeparated( prefix, argument.Name );
				if( argument.RawValue is BaseArgumentParser childArgumentParser )
					recurse( lineOutputConsumer, fullArgumentName, childArgumentParser );
				else
					lineOutputConsumer.Invoke( $"{fullArgumentName} = {KitHelpers.SafeToString( argument.RawValue )}" );
			}

			static string dotSeparated( string a, string b ) => a == "" ? b : $"{a}.{b}";
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Tests for user mistakes

	[VSTesting.TestMethod]
	public void T107_Argument_Supplied_More_Than_Once_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddSwitch( "alpha" );
			bool ok = tryParse( argumentParser, "--alpha --alpha", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T108_Option_Value_Not_Supplied_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddStringOption( "alpha" );
			bool ok = tryParse( argumentParser, "--alpha", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T109_Required_Option_Not_Supplied_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddRequiredStringOption( "alpha" );
			bool ok = tryParse( argumentParser, "", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T110_Required_Positional_Not_Supplied_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddRequiredStringPositional( "alpha" );
			bool ok = tryParse( argumentParser, "", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T111_Unparsable_Option_Value_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddOption( "alpha", EnumCodec<Enum1>.Instance );
			bool ok = tryParse( argumentParser, "--alpha=Unparsable", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T112_Unparsable_Positional_Value_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddPositional( "alpha", IntCodec.Instance );
			bool ok = tryParse( argumentParser, "X", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T113_Unexpected_Token_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			bool ok = tryParse( argumentParser, "unexpected", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T114_Unknown_Single_Letter_Name_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddSwitch( "alpha", 'a' );
			argumentParser.AddSwitch( "bravo", 'b' );
			bool ok = tryParse( argumentParser, "-abc", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T115_Non_String_Option_Value_Cannot_Be_Empty()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddOption( "alpha", IntCodec.Instance );
			bool ok = tryParse( argumentParser, "--alpha=", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}

	[VSTesting.TestMethod]
	public void T116_Missing_Verb_Is_Caught()
	{
		Audit.With( lineOutputConsumer =>
		{
			ArgumentParser argumentParser = newArgumentParser();
			argumentParser.AddVerb( "alpha", "This is the description of alpha", emptyVerbHandler );
			bool ok = tryParse( argumentParser, "", lineOutputConsumer: lineOutputConsumer );
			Assert( !ok );
		} );
	}
}
