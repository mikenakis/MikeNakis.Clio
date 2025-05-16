namespace MikeNakis.Clio;

static partial class Helpers
{
	static readonly RegEx.Regex namedArgumentNameValidationRegex = new( "^[a-zA-Z][a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex singleLetterNameValidationRegex = new( "^[a-zA-Z0-9\\?]$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex optionParameterNameValidationRegex = new( "^[a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex parameterNameValidationRegex = new( "^[a-zA-Z][a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );
	static readonly RegEx.Regex verbNameValidationRegex = new( "^[a-zA-Z0-9-]+$", RegEx.RegexOptions.CultureInvariant );

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
		Assert( nameIsValid( name, regex ), () => throw new InvalidArgumentNameException( name ) );
		return true;
	}

	static bool nameIsValid( string shortFormName, RegEx.Regex regex )
	{
		return regex.IsMatch( shortFormName );
	}

	internal static bool IsTerminator( char c ) => !singleLetterNameValidationRegex.IsMatch( new string( c, 1 ) );

	internal static bool ArgumentMustPrecedeVerbAssertion( BaseArgumentParser argumentParser, string name )
	{
		Assert( argumentParser.Arguments.OfType<VerbArgument>().FirstOrDefault(), //
			verb => verb == null, //
			verb => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.ArgumentMustPrecedeVerb, name, verb!.Name ) );
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
		if( token.Length > 2 + name.Length && !IsTerminator( token[2 + name.Length] ) )
			return 0;
		return 2 + name.Length;
	}

	internal static void SplitCombinedSingleLetterArguments( List<string> tokens )
	{
		for( int i = 0; i < tokens.Count; i++ )
		{
			string token = tokens[i];
			if( token == "--" )
				break;
			if( token[0] == '-' && token.Length > 2 && token[1] != '-' )
			{
				tokens.RemoveAt( i );
				foreach( char c in token.Skip( 1 ) )
					tokens.Insert( i++, $"-{c}" );
			}
		}
	}

	internal static void AddArgumentsFromResponseFiles( List<string> tokens, Sys.Func<string, string> fileReader )
	{
		for( int i = 0; i < tokens.Count; i++ )
		{
			string token = tokens[i];
			if( token == "--" )
				break;
			if( token[0] == '@' )
				i = HandleResponseFileToken( tokens, fileReader, i );
		}
	}

	internal static int HandleResponseFileToken( List<string> tokens, Sys.Func<string, string> fileReader, int tokenIndex )
	{
		string token = tokens[tokenIndex];
		Assert( token[0] == '@' );
		tokens.RemoveAt( tokenIndex );
		IEnumerable<string> lines = ReadResponseFile( SysIo.Path.GetFullPath( token[1..] ), fileReader );
		foreach( string line in lines )
			tokens.Insert( tokenIndex++, "--" + line );
		return tokenIndex;
	}

	internal static IEnumerable<string> ReadResponseFile( string filename, Sys.Func<string, string> fileReader )
	{
		string responseFileName = SysIo.Path.GetFullPath( filename );
		return fileReader.Invoke( responseFileName ) //
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
