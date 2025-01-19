namespace MikeNakis.Clio;

abstract class NamedArgument : Argument
{
	public char? SingleLetterName { get; }
	protected bool Supplied { get; set; }
	public override bool IsSupplied => Supplied;

	private protected NamedArgument( BaseArgumentParser argumentParser, string name, char? singleLetterName, string? description, bool isRequired )
			: base( argumentParser, name, description, isRequired )
	{
		Assert( singleLetterName == null || Helpers.SingleLetterNameIsValidAssertion( singleLetterName.Value ) );
		Assert( Helpers.ArgumentMustPrecedeVerbAssertion( argumentParser, name ) );
		Assert( argumentParser.Arguments.OfType<PositionalArgument>().FirstOrDefault(), //
			positionalArgument => positionalArgument == null, //
			positionalArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.NamedArgumentMustPrecedePositional, name, positionalArgument!.Name ) );
		if( singleLetterName != null )
			Assert( argumentParser.Arguments.OfType<NamedArgument>().FirstOrDefault( existingArgument => existingArgument.SingleLetterName == singleLetterName ), //
				existingArgument => existingArgument == null, //
				existingArgument => throw new DuplicateArgumentSingleLetterNameException( singleLetterName.Value ) );
		SingleLetterName = singleLetterName;
	}

	protected string? TryParseNameAndGetRemainder( string token )
	{
		int skip = Helpers.ShortFormNameMatch( token, SingleLetterName );
		if( skip == 0 )
			skip = Helpers.LongFormNameMatch( token, Name );
		if( skip == 0 )
			return null;
		if( Supplied )
			throw new ArgumentSuppliedMoreThanOnceException( Name );
		Supplied = true;
		return token[skip..];
	}
}
