namespace MikeNakis.Clio;

sealed class SwitchArgument : NamedArgument, ISwitchArgument
{
	internal override string ShortUsage => $"{(SingleLetterName == null ? "" : $"-{SingleLetterName}, ")}--{Name}";
	public override object? RawValue => Value;
	public bool Value
	{
		get
		{
			Assert( HasBeenParsedAssertion() );
			return IsSupplied;
		}
	}

	internal SwitchArgument( BaseArgumentParser argumentParser, string name, char? singleLetterName, string? description )
			: base( argumentParser, name, singleLetterName, description, isRequired: false )
	{ }

	public sealed override int TryParse( int tokenIndex, IReadOnlyList<string> tokens )
	{
		string token = tokens[tokenIndex];
		int skip = shortFormNameMatch( token, SingleLetterName );
		if( skip == 0 )
			skip = longFormNameMatch( token, Name );
		if( skip == 0 )
			return tokenIndex;
		if( IsSupplied )
			throw new ArgumentSuppliedMoreThanOnceException( Name );
		Supplied = true;
		string remainder = token[skip..];
		if( remainder != "" )
			throw new UnexpectedCharactersAfterNamedArgumentException( Name, remainder );
		return tokenIndex + 1;
	}
}
