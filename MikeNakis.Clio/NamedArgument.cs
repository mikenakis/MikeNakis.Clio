namespace MikeNakis.Clio;

using System.Linq;
using static MikeNakis.Clio.Statics;

abstract class NamedArgument : Argument
{
	public char? SingleLetterName { get; }

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
}
