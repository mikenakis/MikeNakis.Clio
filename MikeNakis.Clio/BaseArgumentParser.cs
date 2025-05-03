namespace MikeNakis.Clio;

/// <summary>Common base class for command-line parsers.</summary>
public abstract class BaseArgumentParser
{
	internal abstract BaseArgumentParser? Parent { get; }
	internal abstract ArgumentParser GetRootArgumentParser();
	internal bool HasBeenParsed { get; private set; }
	internal string GetFullName( char delimiter ) => Parent == null ? Name : $"{Parent.GetFullName( delimiter )}{delimiter}{Name}";

	/// <summary>The name of the program or verb.</summary>
	public string Name { get; }

	readonly List<Argument> arguments = new();
	public IEnumerable<IArgument> Arguments => arguments;

	ISwitchArgument? helpSwitch;

	protected BaseArgumentParser( string name )
	{
		Name = name;
	}

	///<summary>Adds a switch.</summary>
	///<remarks>A switch is a named argument without a parameter, e.g. <c>AcmeCli --verbose</c>. The value of a
	/// switch is of type <c>bool</c>, indicating whether the switch was supplied or not.</remarks>
	///<param name="name">The name of the switch.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the switch.</param>
	///<param name="description">The description of the switch, for use when displaying help.</param>
	public ISwitchArgument AddSwitch( string name, char? singleLetterName = null, string? description = null )
	{
		return new SwitchArgument( this, name, singleLetterName, description );
	}

	///<summary>Adds an option.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public IOptionArgument<T?> AddOption<T>( string name, StructCodec<T> codec, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return new NullableStructOption<T>( this, name, singleLetterName, parameterName, description, codec, presetValue );
	}

	///<summary>Adds an option.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public IOptionArgument<T?> AddOption<T>( string name, ClassCodec<T> codec, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return new NullableClassOption<T>( this, name, singleLetterName, parameterName, description, codec, presetValue );
	}

	///<summary>Adds an option with a default value.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public IOptionArgument<T> AddOptionWithDefault<T>( string name, StructCodec<T> codec, T defaultValue, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return new NonNullableStructOption<T>( this, name, singleLetterName, parameterName, codec, description, presetValue, defaultValue );
	}

	///<summary>Adds an option with a default value.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public IOptionArgument<T> AddOptionWithDefault<T>( string name, ClassCodec<T> codec, T defaultValue, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return new NonNullableClassOption<T>( this, name, singleLetterName, parameterName, codec, description, presetValue, defaultValue );
	}

	///<summary>Adds a required option.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public IOptionArgument<T> AddRequiredOption<T>( string name, StructCodec<T> codec, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return new NonNullableStructOption<T>( this, name, singleLetterName, parameterName, codec, description, presetValue, default );
	}

	///<summary>Adds a required option.</summary>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public IOptionArgument<T> AddRequiredOption<T>( string name, ClassCodec<T> codec, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return new NonNullableClassOption<T>( this, name, singleLetterName, parameterName, codec, description, presetValue, default );
	}

	///<summary>Adds a positional argument.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	public IPositionalArgument<T?> AddPositional<T>( string name, StructCodec<T> codec, string? description = null ) where T : struct
	{
		return new NullableStructPositionalArgument<T>( this, name, codec, description );
	}

	///<summary>Adds a positional argument.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	public IPositionalArgument<T?> AddPositional<T>( string name, ClassCodec<T> codec, string? description = null ) where T : class
	{
		return new NullableClassPositionalArgument<T>( this, name, codec, description );
	}

	///<summary>Adds a positional argument with a default value.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	///<param name="defaultValue">The default value for the positional argument, which will be the value of the argument
	///if the argument is not supplied.</param>
	public IPositionalArgument<T> AddPositionalWithDefault<T>( string name, StructCodec<T> codec, T defaultValue, string? description = null ) where T : struct
	{
		return new NonNullableStructPositionalArgument<T>( this, name, codec, description, defaultValue );
	}

	///<summary>Adds a positional argument with a default value.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	///<param name="defaultValue">The default value for the positional argument, which will be the value of the argument
	///if the argument is not supplied.</param>
	public IPositionalArgument<T> AddPositionalWithDefault<T>( string name, ClassCodec<T> codec, T defaultValue, string? description = null ) where T : class
	{
		return new NonNullableClassPositionalArgument<T>( this, name, codec, description, defaultValue );
	}

	///<summary>Adds a required positional argument.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the parameter, for use when displaying help.</param>
	public IPositionalArgument<T> AddRequiredPositional<T>( string name, StructCodec<T> codec, string? description = null ) where T : struct
	{
		return new NonNullableStructPositionalArgument<T>( this, name, codec, description, default );
	}

	///<summary>Adds a required positional argument.</summary>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the positional argument; provides conversions between
	///<c>string</c> and the type of the argument.</param>
	///<param name="description">The description of the parameter, for use when displaying help.</param>
	public IPositionalArgument<T> AddRequiredPositional<T>( string name, ClassCodec<T> codec, string? description = null ) where T : class
	{
		return new NonNullableClassPositionalArgument<T>( this, name, codec, description, default );
	}

	internal void AddArgument( Argument argument )
	{
		Assert( !HasBeenParsed, () => throw new CommandLineHasAlreadyBeenParsedException() );
		if( helpSwitch == null && argument is not NamedArgument )
			addHelpSwitch();
		arguments.Add( argument );
	}

	internal void OutputHelp( Sys.Action<string> lineOutputConsumer )
	{
		int screenWidth = GetRootArgumentParser().ScreenWidth;
		string fullName = GetFullName( ' ' );
		HelpGenerator.OutputHelp( lineOutputConsumer, screenWidth, fullName, arguments, GetRootArgumentParser().VerbTerm );
		return;
	}

	ISwitchArgument addHelpSwitch()
	{
		Assert( helpSwitch == null );
		helpSwitch = AddSwitch( "help", '?', "Display this help" );
		return helpSwitch;
	}

	void reportAnyMissingRequiredArguments()
	{
		foreach( Argument argument in arguments.Where( argument => argument.IsRequired && !argument.IsSupplied ) )
			throw new RequiredArgumentNotSuppliedException( argument.Name );
	}

	internal void Parse( List<string> tokens, int tokenIndex )
	{
		Assert( !HasBeenParsed );
		ISwitchArgument helpSwitch = this.helpSwitch ?? addHelpSwitch();
		HasBeenParsed = true;
		IReadOnlyList<VerbArgument> verbArguments = Arguments.OfType<VerbArgument>().ToArray();
		VerbArgument? foundVerbArgument = null;
		bool endOfOptionsMarkerFound = false;
		for( ; tokenIndex < tokens.Count; tokenIndex++ )
		{
			string token = tokens[tokenIndex];

			if( token == "--" )
			{
				endOfOptionsMarkerFound = true;
				continue;
			}

			if( !endOfOptionsMarkerFound )
			{
				if( token[0] == '@' )
				{
					processResponseFile( tokens, tokenIndex, GetRootArgumentParser().FileReader );
					tokenIndex--;
					continue;
				}

				if( isSingleLetterArgumentGroup( token ) )
				{
					expandSingleLetterArgumentGroup( tokens, tokenIndex );
					tokenIndex--;
					continue;
				}

				if( token.StartsWith( "-", Sys.StringComparison.Ordinal ) )
					if( parseNamedArgument( tokenIndex, tokens ) )
						continue;
			}

			if( parsePositionalArgument( tokenIndex, tokens ) )
				continue;

			if( !endOfOptionsMarkerFound )
			{
				if( verbArguments.Count > 0 )
				{
					foreach( VerbArgument verbArgument in verbArguments )
					{
						if( verbArgument.Name == tokens[tokenIndex] )
						{
							foundVerbArgument = verbArgument;
							break;
						}
					}
					if( foundVerbArgument != null )
						break;
				}
			}

			throw new UnexpectedTokenException( tokens[tokenIndex] );
		}

		if( helpSwitch.Value )
			throw new HelpException( this );
		reportAnyMissingRequiredArguments();

		if( verbArguments.Count > 0 )
		{
			if( foundVerbArgument == null )
				throw new VerbExpectedException( GetRootArgumentParser().VerbTerm );
			else
			{
				int newTokenIndex = foundVerbArgument.TryParse( tokenIndex, tokens );
				Assert( newTokenIndex == tokens.Count );
			}
		}
		return;

		static void expandSingleLetterArgumentGroup( List<string> tokens, int tokenIndex )
		{
			string token = tokens[tokenIndex];
			tokens.RemoveAt( tokenIndex );
			tokens.InsertRange( tokenIndex, token.Skip( 1 ).Select( c => $"-{c}" ) );
		}

		static void processResponseFile( List<string> tokens, int tokenIndex, Sys.Func<string, string> fileReader )
		{
			IEnumerable<string> lines = Helpers.ReadResponseFile( Sys.IO.Path.GetFullPath( tokens[tokenIndex][1..] ), fileReader );
			tokens.RemoveAt( tokenIndex );
			tokens.InsertRange( tokenIndex, lines );
		}
	}

	bool parsePositionalArgument( int tokenIndex, List<string> tokens )
	{
		foreach( PositionalArgument positionalArgument in Arguments.OfType<PositionalArgument>() )
		{
			int newTokenIndex = positionalArgument.TryParse( tokenIndex, tokens );
			if( newTokenIndex != tokenIndex )
			{
				Assert( newTokenIndex == tokenIndex + 1 );
				return true;
			}
		}
		return false;
	}

	bool parseNamedArgument( int tokenIndex, List<string> tokens )
	{
		foreach( NamedArgument namedArgument in Arguments.OfType<NamedArgument>() )
		{
			int newTokenIndex = namedArgument.TryParse( tokenIndex, tokens );
			if( newTokenIndex != tokenIndex )
			{
				Assert( newTokenIndex == tokenIndex + 1 );
				return true;
			}
		}
		return false;
	}

	static bool isSingleLetterArgumentGroup( string token )
	{
		return token[0] == '-' && token.Length > 2 && token[1] != '-' && token[2] != '=';
	}
}
