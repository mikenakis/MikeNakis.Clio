namespace MikeNakis.Clio;

[SysDiag.DebuggerDisplay( "{ToString(),nq}" )]
abstract class PositionalArgument : Argument
{
	bool supplied;
	public override bool IsSupplied => supplied;
	internal sealed override string ShortUsage => "<" + Name + ">";
	private protected abstract string TypeName { get; }

	internal PositionalArgument( BaseArgumentParser argumentParser, string name, string? description, bool isRequired )
		: base( argumentParser, name, description, isRequired )
	{
		Assert( Helpers.ParameterNameIsValidAssertion( name ) );
		Assert( Helpers.ArgumentMustPrecedeVerbAssertion( argumentParser, name ) );
		if( !IsOptional )
			Assert( argumentParser.Arguments.OfType<PositionalArgument>().Where( positionalArgument => positionalArgument.IsOptional ).FirstOrDefault(), //
				optionalPositionalArgument => optionalPositionalArgument == null, //
				optionalPositionalArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.RequiredPositionalMustPrecedeOptionalPositional, Name, optionalPositionalArgument!.Name ) );
	}

	public sealed override int OnTryParse( int tokenIndex, List<string> tokens )
	{
		if( supplied )
			return tokenIndex;
		string valueToken = tokens[tokenIndex];
		supplied = true;
		try
		{
			RealizeValue( valueToken );
		}
		catch( Sys.Exception exception )
		{
			throw new UnparsableValueException( Name, valueToken, exception );
		}
		return tokenIndex + 1;
	}

	private protected abstract void RealizeValue( string valueToken );
}

sealed class NullableClassPositionalArgument<T> : PositionalArgument, IPositionalArgument<T?> where T : class
{
	readonly ClassCodec<T> codec;
	public override object? RawValue => Value;
	private protected override string TypeName => codec.Name;
	public T? Value
	{
		get
		{
			Assert( HasBeenParsedAssertion() );
			return value;
		}
	}

	T? value;
	private protected override void RealizeValue( string valueToken ) => value = codec.ValueFromText( valueToken );

	internal NullableClassPositionalArgument( BaseArgumentParser argumentParser, string name, ClassCodec<T> codec, string? description )
		: base( argumentParser, name, description, isRequired: false )
	{
		this.codec = codec;
	}
}

sealed class NullableStructPositionalArgument<T> : PositionalArgument, IPositionalArgument<T?> where T : struct
{
	readonly StructCodec<T> codec;
	public override object? RawValue => Value;
	private protected override string TypeName => codec.Name;
	public T? Value
	{
		get
		{
			Assert( HasBeenParsedAssertion() );
			return value;
		}
	}

	T? value;
	private protected override void RealizeValue( string valueToken ) => value = codec.ValueFromText( valueToken );

	internal NullableStructPositionalArgument( BaseArgumentParser argumentParser, string name, StructCodec<T> codec, string? description )
		: base( argumentParser, name, description, isRequired: false )
	{
		this.codec = codec;
	}
}

sealed class NonNullableClassPositionalArgument<T> : PositionalArgument, IPositionalArgument<T> where T : class
{
	readonly ClassCodec<T> codec;
	public override object? RawValue => Value;
	private protected override string TypeName => codec.Name;
	public T Value
	{
		get
		{
			Assert( HasBeenParsedAssertion() );
			return (IsSupplied ? value : defaultValue) ?? throw Failure();
		}
	}

	readonly T? defaultValue;
	T? value;
	private protected override void RealizeValue( string valueToken ) => value = codec.ValueFromText( valueToken );

	public NonNullableClassPositionalArgument( BaseArgumentParser argumentParser, string name, ClassCodec<T> codec, string? description, T? defaultValue )
		: base( argumentParser, name, description, isRequired: defaultValue is null )
	{
		this.codec = codec;
		this.defaultValue = defaultValue;
	}
}

sealed class NonNullableStructPositionalArgument<T> : PositionalArgument, IPositionalArgument<T> where T : struct
{
	readonly StructCodec<T> codec;
	public override object? RawValue => Value;
	private protected override string TypeName => codec.Name;
	public T Value
	{
		get
		{
			Assert( HasBeenParsedAssertion() );
			return (IsSupplied ? value : defaultValue) ?? throw new Sys.InvalidOperationException();
		}
	}

	readonly T? defaultValue;
	T? value;
	private protected override void RealizeValue( string valueToken ) => value = codec.ValueFromText( valueToken );

	public NonNullableStructPositionalArgument( BaseArgumentParser argumentParser, string name, StructCodec<T> codec, string? description, T? defaultValue )
		: base( argumentParser, name, description, isRequired: defaultValue is null )
	{
		this.codec = codec;
		this.defaultValue = defaultValue;
	}
}
