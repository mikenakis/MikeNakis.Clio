namespace MikeNakis.Clio;

using System.Collections.Generic;
using MikeNakis.Clio.Internal;
using static MikeNakis.Clio.Internal.Statics;
using Sys = System;
using SysText = System.Text;

abstract class MetaOptionArgument : NamedArgument
{
	readonly string? parameterName;
	string effectiveParameterName => $"<{parameterName ?? TypeName}>";
	private protected abstract string TypeName { get; }
	internal sealed override string ShortUsage => buildShortUsage();
	private protected abstract object? RawPresetValue { get; }
	bool hasPreset => RawPresetValue != null;

	internal MetaOptionArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? parameterName, string? description )
			: base( argumentParser, name, singleLetterNames, description, isRequired: false )
	{
		Assert( Helpers.OptionNameIsValidAssertion( name ) );
		Assert( parameterName == null || Helpers.OptionParameterNameIsValidAssertion( parameterName ) );
		this.parameterName = parameterName;
	}

	string buildShortUsage()
	{
		SysText.StringBuilder stringBuilder = new();
		stringBuilder.Append( SingleLetterNamesShortUsage );
		stringBuilder.Append( "--" ).Append( Name ).Append( ":<name>" );
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
		if( hasPreset )
			lineConsumer.Invoke( $"If a name is supplied without a value, the preset is {KitHelpers.SafeToString( RawPresetValue )}." );
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
		if( remainder.Length == 0 || remainder[0] != ':' )
			throw new MetaOptionNameExpectedException( Name );
		int equalsSignIndex = remainder.IndexOf( '=' );
		string metaOptionName = equalsSignIndex == -1 ? remainder[1..] : remainder[1..equalsSignIndex];
		if( metaOptionName.Length == 0 )
			throw new MetaOptionNameExpectedException( Name );
		if( ContainsMetaOptionName( metaOptionName ) )
			throw new MetaOptionNameSuppliedMoreThanOnceException( Name, metaOptionName );
		if( equalsSignIndex == -1 )
		{
			if( !hasPreset )
				throw new MetaOptionValueExpectedException( Name, metaOptionName );
			RealizePreset( metaOptionName );
		}
		else
		{
			string stringValue = remainder[(equalsSignIndex + 1)..];
			try
			{
				RealizeStringValue( metaOptionName, stringValue );
			}
			catch( Sys.Exception exception )
			{
				throw new UnparsableValueException( Name, stringValue, exception );
			}
		}
		return tokenIndex + 1;
	}

	private protected abstract bool ContainsMetaOptionName( string metaOptionName );
	private protected abstract void RealizePreset( string metaOptionName );
	private protected abstract void RealizeStringValue( string metaOptionName, string stringValue );
}

sealed class StructMetaOptionArgument<T> : MetaOptionArgument, IMetaOptionArgument<T> where T : struct
{
	readonly StructCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawPresetValue => presetValue;
	public IReadOnlyDictionary<string, T> Value => getValue();
	public override bool IsSupplied => value.Count > 0;

	readonly T? presetValue;
	readonly Dictionary<string, T> value = new( Sys.StringComparer.Ordinal );

	private protected override bool ContainsMetaOptionName( string metaOptionName ) => value.ContainsKey( metaOptionName );
	private protected override void RealizePreset( string metaOptionName ) => value.Add( metaOptionName, presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string metaOptionName, string stringValue ) => value.Add( metaOptionName, codec.ValueFromText( stringValue ) );

	IReadOnlyDictionary<string, T> getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	public StructMetaOptionArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, //
		string? parameterName, StructCodec<T> codec, string? description, T? presetValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}

sealed class ClassMetaOptionArgument<T> : MetaOptionArgument, IMetaOptionArgument<T> where T : class
{
	readonly ClassCodec<T> codec;
	private protected override string TypeName => codec.Name;
	public override object? RawValue => Value;
	private protected override object? RawPresetValue => presetValue;
	public IReadOnlyDictionary<string, T> Value => getValue();
	public override bool IsSupplied => value.Count > 0;

	readonly T? presetValue;
	readonly Dictionary<string, T> value = new( Sys.StringComparer.Ordinal );

	private protected override bool ContainsMetaOptionName( string metaOptionName ) => value.ContainsKey( metaOptionName );
	private protected override void RealizePreset( string metaOptionName ) => value.Add( metaOptionName, presetValue ?? throw new Sys.InvalidOperationException() );
	private protected override void RealizeStringValue( string metaOptionName, string stringValue ) => value.Add( metaOptionName, codec.ValueFromText( stringValue ) );

	IReadOnlyDictionary<string, T> getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return value;
	}

	public ClassMetaOptionArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, //
		string? parameterName, ClassCodec<T> codec, string? description, T? presetValue )
		: base( argumentParser, name, singleLetterNames, parameterName, description )
	{
		this.codec = codec;
		this.presetValue = presetValue;
	}
}
