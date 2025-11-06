namespace MikeNakis.Clio_Test;

using System.Linq;
using MikeNakis.Clio;
using MikeNakis.Clio.Extensions;
using static System.MemoryExtensions;
using static Statics;
using Sys = System;
using SysIo = System.IO;
using VSTesting = Microsoft.VisualStudio.TestTools.UnitTesting;

[VSTesting.TestClass]
public sealed class T102_ClioSunnyDay
{
	static ArgumentParser newArgumentParser( Sys.Func<string, string>? fileReader = null )
	{
		TestingOptions testingOptions = new( "TestApp", fileReader );
		return new ArgumentParser( null, null, testingOptions );
	}

	static void parse( ArgumentParser argumentParser, params string[] tokens )
	{
		argumentParser.Parse( tokens );
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
	public void T101_Empty_Clio_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		parse( argumentParser );
	}

	[VSTesting.TestMethod]
	public void T102_Switch_Receives_False_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		parse( argumentParser );
		Assert( !alpha.Value );
	}

	[VSTesting.TestMethod]
	public void T103_Switch_Receives_True_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		parse( argumentParser, "--alpha" );
		Assert( alpha.Value );
	}

	[VSTesting.TestMethod]
	public void T104_Optional_String_Option_Receives_Null_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha" );
		parse( argumentParser );
		Assert( alpha.Value == null );
	}

	[VSTesting.TestMethod]
	public void T105_Optional_String_Option_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha" );
		parse( argumentParser, "--alpha=alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T106_Defaulted_String_Option_Receives_Default_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddStringOptionWithDefault( "alpha", "alpha-default" );
		parse( argumentParser );
		Assert( alpha.Value == "alpha-default" );
	}

	[VSTesting.TestMethod]
	public void T107_Defaulted_String_Option_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddStringOptionWithDefault( "alpha", "alpha-default" );
		parse( argumentParser, "--alpha=alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T108_Optional_String_Option_With_Preset_Receives_Null_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha", presetValue: "alpha-preset" );
		parse( argumentParser );
		Assert( alpha.Value == null );
	}

	[VSTesting.TestMethod]
	public void T109_Optional_String_Option_With_Preset_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha", presetValue: "alpha-preset" );
		parse( argumentParser, "--alpha=alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T110_Optional_String_Option_With_Preset_Receives_Preset_When_Supplied_Without_Value()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha", presetValue: "alpha-preset" );
		parse( argumentParser, "--alpha" );
		Assert( alpha.Value == "alpha-preset" );
	}

	[VSTesting.TestMethod]
	public void T111_Required_String_Option_With_Preset_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddRequiredStringOption( "alpha", presetValue: "alpha-preset" );
		parse( argumentParser, "--alpha=alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T112_Required_String_Option_With_Preset_Receives_Preset_When_Supplied_Without_Value()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddRequiredStringOption( "alpha", presetValue: "alpha-preset" );
		parse( argumentParser, "--alpha" );
		Assert( alpha.Value == "alpha-preset" );
	}

	[VSTesting.TestMethod]
	public void T113_Enum_Option_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1?> alpha = argumentParser.AddOption( "alpha", EnumCodec<Enum1>.Instance );
		parse( argumentParser, $"--alpha={nameof( Enum1.Value2 )}" );
		Assert( alpha.Value == Enum1.Value2 );
	}

	[VSTesting.TestMethod]
	public void T114_Int_Option_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<int> alpha = argumentParser.AddOptionWithDefault( "alpha", IntCodec.Instance, 42 );
		parse( argumentParser, $"--alpha=5" );
		Assert( alpha.Value == 5 );
	}

	[VSTesting.TestMethod]
	public void T115_Optional_String_Positional_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringPositional( "alpha" );
		parse( argumentParser, "alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T116_Optional_String_Positional_Receives_Null_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringPositional( "alpha" );
		parse( argumentParser );
		Assert( alpha.Value == null );
	}

	[VSTesting.TestMethod]
	public void T117_Defaulted_String_Positional_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddStringPositionalWithDefault( "alpha", "alpha-default" );
		parse( argumentParser, "alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T118_Defaulted_String_Positional_Receives_Default_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddStringPositionalWithDefault( "alpha", "alpha-default" );
		parse( argumentParser );
		Assert( alpha.Value == "alpha-default" );
	}

	[VSTesting.TestMethod]
	public void T119_Required_String_Positional_Receives_Value()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string> alpha = argumentParser.AddRequiredStringPositional( "alpha" );
		parse( argumentParser, "alpha-value" );
		Assert( alpha.Value == "alpha-value" );
	}

	[VSTesting.TestMethod]
	public void T120_Optional_Struct_Positional_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1?> alpha = argumentParser.AddPositional( "alpha", EnumCodec<Enum1>.Instance );
		parse( argumentParser, nameof( Enum1.Value3 ) );
		Assert( alpha.Value == Enum1.Value3 );
	}

	[VSTesting.TestMethod]
	public void T121_Optional_Struct_Positional_Receives_Null_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1?> alpha = argumentParser.AddPositional( "alpha", EnumCodec<Enum1>.Instance );
		parse( argumentParser );
		Assert( alpha.Value == null );
	}

	[VSTesting.TestMethod]
	public void T122_Defaulted_Struct_Positional_Receives_Value_When_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1> alpha = argumentParser.AddPositionalWithDefault( "alpha", EnumCodec<Enum1>.Instance, Enum1.Value2 );
		parse( argumentParser, nameof( Enum1.Value3 ) );
		Assert( alpha.Value == Enum1.Value3 );
	}

	[VSTesting.TestMethod]
	public void T123_Defaulted_Struct_Positional_Receives_Default_When_Not_Supplied()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1> alpha = argumentParser.AddPositionalWithDefault( "alpha", EnumCodec<Enum1>.Instance, Enum1.Value2 );
		parse( argumentParser );
		Assert( alpha.Value == Enum1.Value2 );
	}

	[VSTesting.TestMethod]
	public void T124_Required_Struct_Positional_Receives_Value()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<Enum1> alpha = argumentParser.AddRequiredPositional( "alpha", EnumCodec<Enum1>.Instance );
		parse( argumentParser, nameof( Enum1.Value3 ) );
		Assert( alpha.Value == Enum1.Value3 );
	}

	[VSTesting.TestMethod]
	public void T125_Short_Form_Name_Gets_Parsed()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		parse( argumentParser, "-a" );
		Assert( alpha.Value );
	}

	[VSTesting.TestMethod]
	public void T126_Combined_Short_Form_Names_Get_Parsed()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a' );
		IArgument<bool> bravo = argumentParser.AddSwitch( "bravo", 'b' );
		parse( argumentParser, "-ab" );
		Assert( alpha.Value );
		Assert( bravo.Value );
	}

	[VSTesting.TestMethod]
	public void T127_Parameter_Supplied_Before_Named_Argument_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		IArgument<string?> bravo = argumentParser.AddStringPositional( "bravo" );
		parse( argumentParser, "bravo-value", "--alpha" );
		Assert( alpha.Value );
		Assert( bravo.Value == "bravo-value" );
	}

	[VSTesting.TestMethod]
	public void T128_Optional_Positional_Before_Defaulted_Positional_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> mike = argumentParser.AddStringPositional( "mike", "mike-description" );
		IArgument<string> papa = argumentParser.AddStringPositionalWithDefault( "papa", "papa-default", description: "papa-description" );
		parse( argumentParser, "mike-value" );
		Assert( mike.Value == "mike-value" );
		Assert( papa.Value == "papa-default" );
	}

	[VSTesting.TestMethod]
	public void T129_Argument_Names_May_Overlap()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> alphaBravo = argumentParser.AddSwitch( "alphaBravo" );
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha" );
		IArgument<bool> alphaBravoCharlie = argumentParser.AddSwitch( "alphaBravoCharlie" );
		parse( argumentParser, "--alpha", "--alphaBravo", "--alphaBravoCharlie" );
		Assert( alpha.Value == true );
		Assert( alphaBravo.Value == true );
		Assert( alphaBravoCharlie.Value == true );
	}

	[VSTesting.TestMethod]
	public void T130_String_Option_Value_Can_Be_Empty()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha" );
		parse( argumentParser, "--alpha=" );
		Assert( alpha.Value == "" );
	}

	[VSTesting.TestMethod]
	public void T131_Verbs_Work()
	{
		ArgumentParser argumentParser = newArgumentParser();
		bool verbHandlerInvoked = false;
		IArgument<bool> alpha = argumentParser.AddSwitch( "alpha", 'a', "alpha-description" );
		argumentParser.AddVerb( "juliett", "juliett-description", argumentParser => //
			{
				IArgument<bool> lima = argumentParser.AddSwitch( "lima", description: "lima-description" );
				IArgument<string> india = argumentParser.AddRequiredStringPositional( "india", "india-description" );
				if( !argumentParser.TryParse() )
					return;
				Assert( alpha.Value == true );
				Assert( lima.Value == true );
				Assert( india.Value == "india-value" );
				verbHandlerInvoked = true;
			} );
		argumentParser.AddVerb( "kilo", "kilo-description", emptyVerbHandler );
		parse( argumentParser, "-a", "juliett", "--lima", "india-value" );
		Assert( verbHandlerInvoked );
	}

	[VSTesting.TestMethod]
	public void T133_Response_Files_Work()
	{
		const string responseFilename = "responseFilename.txt";
		ArgumentParser argumentParser = newArgumentParser( fileReader );
		IArgument<string?> alpha = argumentParser.AddStringOption( "alpha" );
		IArgument<string?> mike = argumentParser.AddStringOption( "mike" );
		IArgument<string?> papa = argumentParser.AddStringPositional( "papa" );
		IArgument<string?> zulu = argumentParser.AddStringPositional( "zulu" );
		parse( argumentParser, "--alpha=alpha-value", $"@{responseFilename}", "zulu-value" );
		Assert( alpha.Value == "alpha-value" );
		Assert( mike.Value == "mike-value" );
		Assert( papa.Value == "papa-value" );
		Assert( zulu.Value == "zulu-value" );
		return;

		static string fileReader( string filename )
		{
			Assert( filename == SysIo.Path.GetFullPath( responseFilename ) );
			return @"--mike=mike-value
				#this is a comment
				papa-value";
		}
	}

	[VSTesting.TestMethod]
	public void T134_End_Of_Options_Marker_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IArgument<bool> fSwitch = argumentParser.AddSwitch( "f-switch", 'f' );
		IArgument<bool> xSwitch = argumentParser.AddSwitch( "x-switch", 'x' );
		IArgument<string?> positional = argumentParser.AddStringPositional( "positional" );
		parse( argumentParser, $"-f", "--", "-x" );
		Assert( fSwitch.Value );
		Assert( !xSwitch.Value );
		Assert( positional.Value == "-x" );
	}

	[VSTesting.TestMethod]
	public void T135_Single_Letter_Option_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IOptionArgument<string?> fOption = argumentParser.AddStringOption( "f-option", 'f' );
		parse( argumentParser, $"-f=covfefe" );
		Assert( fOption.Value == "covfefe" );
	}

	[VSTesting.TestMethod]
	public void T165_Repeated_Struct_Option_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IRepeatedOptionArgument<int> option = argumentParser.AddRepeatedOption( "f-option", IntCodec.Instance, 'f' );
		parse( argumentParser, $"-f=42", "-f=43" );
		Assert( option.Value.SequenceEqual( EnumerableOf( 42, 43 ) ) );
	}

	[VSTesting.TestMethod]
	public void T166_Repeated_Class_Option_Works()
	{
		ArgumentParser argumentParser = newArgumentParser();
		IRepeatedOptionArgument<string> option = argumentParser.AddRepeatedOption( "f-option", StringCodec.Instance, 'f' );
		parse( argumentParser, $"-f=covfefe", "-f=I have the best words" );
		Assert( option.Value.SequenceEqual( EnumerableOf( "covfefe", "I have the best words" ) ) );
	}
}
