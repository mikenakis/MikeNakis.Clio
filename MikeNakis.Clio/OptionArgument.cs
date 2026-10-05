namespace MikeNakis.Clio;

using System.Collections.Generic;
using MikeNakis.Clio.Internal;
using static MikeNakis.Clio.Internal.Statics;
using Sys = System;
using SysText = System.Text;

abstract class OptionArgument : NamedArgument
{
	readonly string? parameterName;
	string effectiveParameterName => $"<{parameterName ?? TypeName}>";
	private protected abstract string TypeName { get; }
	internal sealed override string ShortUsage => buildShortUsage();
	private protected abstract object? RawDefaultValue { get; }
	internal bool HasDefault => RawDefaultValue != null;
	private protected abstract object? RawPresetValue { get; }
	bool hasPreset => RawPresetValue != null;

	internal OptionArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, string? description, bool isRequired )
			: base( argumentParser, name, singleLetterNames, description, isRequired )
	{
		Assert( Helpers.OptionNameIsValidAssertion( name ) );
		Assert( parameterName == null || Helpers.OptionParameterNameIsValidAssertion( parameterName ) );
		this.parameterName = parameterName;
	}

	string buildShortUsage()
	{
		SysText.StringBuilder stringBuilder = new();
		stringBuilder.Append( SingleLetterNamesShortUsage );
		stringBuilder.Append( "--" ).Append( Name );
		if( hasPreset )
			stringBuilder.Append( '[' );
		stringBuilder.Append( '=' ).Append( effectiveParameterName );
		if( hasPreset )
			stringBuilder.Append( ']' );
		return stringBuilder.ToString();
	}

	internal override void CollectLongUsageLines( Sys.Action<string> lineConsumer )
	{
		base.CollectLongUsageLines( lineConsumer );
		if( HasDefault )
			lineConsumer.Invoke( $"If omitted, the default is {KitHelpers.SafeToString( RawDefaultValue )}." );
		if( hasPreset )
			lineConsumer.Invoke( $"If supplied without a value, the preset is {KitHelpers.SafeToString( RawPresetValue )}." );
	}

	public sealed override int OnTryParse( int tokenIndex, List<string> tokens )
	{
		string token = tokens[tokenIndex];
		int skip = SingleLetterNameMatch( token );
		if( skip == 0 )
			skip = Helpers.LongFormNameMatch( token, Name );
		if( skip == 0 )
			return tokenIndex;
		string remainder = token[skip..];
		if( remainder.Length == 0 )
		{
			if( !hasPreset )
				throw new EqualsSignExpectedUserException( Name );
			RealizePreset(); //value = presetValue;
		}
		else
		{
			if( remainder[0] != '=' )
				throw new UnexpectedCharactersAfterNamedArgumentUserException( Name, remainder );
			try
			{
				RealizeStringValue( remainder[1..] );
			}
			catch( Sys.Exception exception )
			{
				throw new UnparsableValueUserException( Name, remainder[1..], exception );
			}
		}
		return tokenIndex + 1;
	}

	private protected abstract void RealizePreset();
	private protected abstract void RealizeStringValue( string stringValue );
}

sealed class NullableStructOption<T> : OptionArgument, IOptionArgument<T?> where T : struct
{
	readonly StructCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => null;
	private protected override object? RawPresetValue => presetValue;
	public T? Value => getValue();
	public override bool IsSupplied => value != null;

	readonly T? presetValue;
	T? value;

	private protected override void RealizePreset() => realizeValue( presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string stringValue ) => realizeValue( codec.ValueFromText( stringValue ) );

	void realizeValue( T value )
	{
		if( this.value != null )
			throw new ArgumentSuppliedMoreThanOnceUserException( Name );
		this.value = value;
	}

	T? getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	internal NullableStructOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, //
		string? description, StructCodec<T> codec, T? presetValue )
			: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: false )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}

sealed class NullableClassOption<T> : OptionArgument, IOptionArgument<T?> where T : class
{
	readonly ClassCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => null;
	private protected override object? RawPresetValue => presetValue;
	public T? Value => getValue();

	public override bool IsSupplied => value != null;

	readonly T? presetValue;
	T? value;
	private protected override void RealizePreset() => realizeValue( presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string stringValue ) => realizeValue( codec.ValueFromText( stringValue ) );

	void realizeValue( T value )
	{
		if( this.value != null )
			throw new ArgumentSuppliedMoreThanOnceUserException( Name );
		this.value = value;
	}

	T? getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	internal NullableClassOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, //
		string? description, ClassCodec<T> codec, T? presetValue )
			: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: false )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}

sealed class NonNullableStructOption<T> : OptionArgument, IOptionArgument<T> where T : struct
{
	readonly StructCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => defaultValue;
	private protected override object? RawPresetValue => presetValue;
	public T Value => getValue();
	public override bool IsSupplied => value != null;

	readonly T? presetValue;
	readonly T? defaultValue;
	T? value;
	private protected override void RealizePreset() => realizeValue( presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string stringValue ) => realizeValue( codec.ValueFromText( stringValue ) );

	void realizeValue( T value )
	{
		if( this.value != null )
			throw new ArgumentSuppliedMoreThanOnceUserException( Name );
		this.value = value;
	}

	T getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value ?? defaultValue ?? throw Failure();
	}

	public NonNullableStructOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, StructCodec<T> codec, string? description, T? presetValue, T? defaultValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: defaultValue is null )
	{
		this.codec = codec;
		this.presetValue = presetValue;
		this.defaultValue = defaultValue;
	}
}

sealed class NonNullableClassOption<T> : OptionArgument, IOptionArgument<T> where T : class
{
	readonly ClassCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => defaultValue;
	private protected override object? RawPresetValue => presetValue;
	public T Value => getValue();
	public override bool IsSupplied => value != null;

	readonly T? presetValue;
	readonly T? defaultValue;
	T? value;
	private protected override void RealizePreset() => realizeValue( presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string stringValue ) => realizeValue( codec.ValueFromText( stringValue ) );

	void realizeValue( T value )
	{
		if( this.value != null )
			throw new ArgumentSuppliedMoreThanOnceUserException( Name );
		this.value = value;
	}

	T getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value ?? defaultValue ?? throw Failure();
	}

	public NonNullableClassOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, ClassCodec<T> codec, string? description, T? presetValue, T? defaultValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: defaultValue is null )
	{
		this.codec = codec;
		this.presetValue = presetValue;
		this.defaultValue = defaultValue;
	}
}

sealed class RepeatedStructOption<T> : OptionArgument, IRepeatedOptionArgument<T> where T : struct
{
	readonly StructCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => null;
	private protected override object? RawPresetValue => presetValue;
	public IEnumerable<T> Value => getValue();
	public override bool IsSupplied => value.Count > 0;

	readonly T? presetValue;
	readonly List<T> value = new();

	private protected override void RealizePreset() => realizeValue( presetValue ?? throw new Sys.InvalidOperationException() );

	private protected override void RealizeStringValue( string stringValue ) => realizeValue( codec.ValueFromText( stringValue ) );

	void realizeValue( T value )
	{
		this.value.Add( value );
	}

	IEnumerable<T> getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	public RepeatedStructOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, //
		string? parameterName, StructCodec<T> codec, string? description, T? presetValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: false )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}

sealed class RepeatedClassOption<T> : OptionArgument, IRepeatedOptionArgument<T> where T : class
{
	readonly ClassCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawDefaultValue => null;
	private protected override object? RawPresetValue => presetValue;
	public IEnumerable<T> Value => getValue();

	public override bool IsSupplied => value.Count > 0;

	readonly T? presetValue;
	readonly List<T> value = new();

	private protected override void RealizePreset()
	{
		value.Add( presetValue ?? throw new Sys.InvalidOperationException() );
	}

	private protected override void RealizeStringValue( string stringValue )
	{
		value.Add( codec.ValueFromText( stringValue ) );
	}

	IEnumerable<T> getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	public RepeatedClassOption( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, //
		string? parameterName, ClassCodec<T> codec, string? description, T? presetValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description, isRequired: false )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}
