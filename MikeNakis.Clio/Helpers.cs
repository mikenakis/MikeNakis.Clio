namespace MikeNakis.Clio;

using System.Collections.Generic;
using System.Linq;
using static MikeNakis.Clio.Internal.Statics;
using RegEx = System.Text.RegularExpressions;
using Sys = System;

static partial class Helpers
{
	static readonly RegEx.Regex namedArgumentNameValidationRegex = new( "^[a-zA-Z][a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex singleLetterNameValidationRegex = new( "^[a-zA-Z0-9\\?]$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex optionParameterNameValidationRegex = new( "^[a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex parameterNameValidationRegex = new( "^[a-zA-Z][a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex verbNameValidationRegex = new( "^[a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex namedArgumentNameCharacterRegex = new( "^[a-zA-Z0-9-]$", RegEx.RegexOptions.CultureInvariant );

	internal const string DefaultDescription = "See user's manual";

	internal static bool SwitchNameIsValidAssertion( string name )
	{
		Assert( nameIsValidAssertion( name, namedArgumentNameValidationRegex ) );
		return true;
	}

	internal static bool OptionNameIsValidAssertion( string name )
	{
		Assert( nameIsValidAssertion( name, namedArgumentNameValidationRegex ) );
		return true;
	}

	internal static bool ParameterNameIsValidAssertion( string name )
	{
		Assert( nameIsValidAssertion( name, parameterNameValidationRegex ) );
		return true;
	}

	internal static bool VerbNameIsValidAssertion( string name )
	{
		Assert( nameIsValidAssertion( name, verbNameValidationRegex ) );
		return true;
	}

	internal static bool SingleLetterNameIsValidAssertion( char singleLetterName )
	{
		Assert( nameIsValidAssertion( new string( singleLetterName, 1 ), singleLetterNameValidationRegex ) );
		return true;
	}

	internal static bool OptionParameterNameIsValidAssertion( string name )
	{
		Assert( nameIsValidAssertion( name, optionParameterNameValidationRegex ) );
		return true;
	}

	static bool nameIsValidAssertion( string name, RegEx.Regex regex )
	{
		Assert( nameIsValid( name, regex ), () => throw new InvalidArgumentNameProgrammerException( name ) );
		return true;
	}

	static bool nameIsValid( string shortFormName, RegEx.Regex regex )
	{
		return regex.IsMatch( shortFormName );
	}

	internal static bool IsTerminator( char c ) => !singleLetterNameValidationRegex.IsMatch( new string( c, 1 ) );

	static bool isNamedArgumentNameCharacter( char c ) => namedArgumentNameCharacterRegex.IsMatch( new string( c, 1 ) );

	internal static bool ArgumentMustPrecedeVerbAssertion( BaseArgumentParser argumentParser, string name )
	{
		Assert( argumentParser.Arguments.OfType<VerbArgument>().FirstOrDefault(), //
			verb => verb == null, //
			verb => throw new InvalidArgumentOrderingProgrammerException( ArgumentOrderingRule.ArgumentMustPrecedeVerb, name, verb!.Name ) );
		return true;
	}

	internal static int ShortFormNameMatch( string token, char? shortFormName )
	{
		if( !shortFormName.HasValue )
			return 0;
		if( token[0] != '-' )
			return 0;
		if( token.Length < 2 )
			return 0;
		if( token[1] != shortFormName.Value )
			return 0;
		if( token.Length > 2 && !IsTerminator( token[2] ) )
			return 0;
		return 2;
	}

	internal static int LongFormNameMatch( string token, string name )
	{
		if( token.Length < 2 + name.Length )
			return 0;
		if( !(token[0] == '-' && token[1] == '-') )
			return 0;
		if( !token[2..].StartsWith( name, Sys.StringComparison.Ordinal ) )
			return 0;
		if( token.Length > 2 + name.Length && isNamedArgumentNameCharacter( token[2 + name.Length] ) )
			return 0;
		return 2 + name.Length;
	}

	internal static IEnumerable<string> ReadResponseFile( string fullPath, Sys.Func<string, string> fileReader )
	{
		return fileReader.Invoke( fullPath ) //
			.Split( '\n' )
			.Select( s => s.Trim() )
			.Where( s => s.Length > 0 )
			.Where( s => s[0] != '#' );
	}

	internal static void OutputExceptionMessage( Sys.Exception userException, Sys.Action<string> lineOutputConsumer )
	{
		lineOutputConsumer.Invoke( userException.Message );
		for( Sys.Exception? innerException = userException.InnerException; innerException != null; innerException = innerException.InnerException )
			lineOutputConsumer.Invoke( "Because: " + innerException.Message );
	}
}
