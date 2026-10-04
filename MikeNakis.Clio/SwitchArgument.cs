namespace MikeNakis.Clio;

using System.Collections.Generic;
using static MikeNakis.Clio.Internal.Statics;

sealed class SwitchArgument : NamedArgument, ISwitchArgument
{
	internal override string ShortUsage => $"{SingleLetterNamesShortUsage}--{Name}";
	public override object? RawValue => Value;
	public bool Value => getValue();
	bool supplied;
	public override bool IsSupplied => supplied;

	bool getValue()
	{
		Assert( HasBeenParsedAssertion() );
		return supplied;
	}

	internal SwitchArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? description )
			: base( argumentParser, name, singleLetterNames, description, isRequired: false )
	{
		Assert( Helpers.SwitchNameIsValidAssertion( name ) );
	}

	public sealed override int OnTryParse( int tokenIndex, List<string> tokens )
	{
		string token = tokens[tokenIndex];
		int skip = SingleLetterNameMatch( token );
		if( skip == 0 )
			skip = Helpers.LongFormNameMatch( token, Name );
		if( skip == 0 )
			return tokenIndex;
		if( supplied )
			throw new ArgumentSuppliedMoreThanOnceException( Name );
		supplied = true;
		string remainder = token[skip..];
		if( remainder != "" )
			throw new UnexpectedCharactersAfterNamedArgumentException( Name, remainder );
		return tokenIndex + 1;
	}
}
