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
		string? remainder = TryParseNameAndGetRemainder( tokens[tokenIndex] );
		if( remainder == null )
			return tokenIndex;
		if( remainder != "" )
			throw new UnexpectedCharactersAfterNamedArgumentException( Name, remainder );
		return tokenIndex + 1;
	}
}
