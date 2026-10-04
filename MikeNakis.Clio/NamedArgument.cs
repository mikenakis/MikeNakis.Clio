namespace MikeNakis.Clio;

using System.Collections.Generic;
using System.Linq;
using static MikeNakis.Clio.Internal.Statics;

abstract class NamedArgument : Argument
{
	public IReadOnlyList<char> SingleLetterNames { get; }

	private protected NamedArgument( BaseArgumentParser argumentParser, string name, IReadOnlyList<char> singleLetterNames, string? description, bool isRequired )
			: base( argumentParser, name, description, isRequired )
	{
		Assert( singleLetterNamesAreValidAssertion( singleLetterNames ) );
		Assert( Helpers.ArgumentMustPrecedeVerbAssertion( argumentParser, name ) );
		Assert( argumentParser.Arguments.OfType<PositionalArgument>().FirstOrDefault(), //
			positionalArgument => positionalArgument == null, //
			positionalArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.NamedArgumentMustPrecedePositional, name, positionalArgument!.Name ) );
		Assert( singleLetterNamesAreUniqueAssertion( argumentParser, this, singleLetterNames ) );
		SingleLetterNames = singleLetterNames.ToArray();
		return;

		static bool singleLetterNamesAreValidAssertion( IReadOnlyList<char> singleLetterNames )
		{
			foreach( char singleLetterName in singleLetterNames )
				Assert( Helpers.SingleLetterNameIsValidAssertion( singleLetterName ) );
			return true;
		}

		static bool singleLetterNamesAreUniqueAssertion( BaseArgumentParser argumentParser, NamedArgument self, IReadOnlyList<char> singleLetterNames )
		{
			for( int i = 0; i < singleLetterNames.Count; i++ )
			{
				char singleLetterName = singleLetterNames[i];
				Assert( !singleLetterNames.Take( i ).Contains( singleLetterName ), () => throw new DuplicateArgumentSingleLetterNameException( singleLetterName ) );
				Assert( argumentParser.Arguments.OfType<NamedArgument>().FirstOrDefault( existingArgument => existingArgument != self && existingArgument.SingleLetterNames.Contains( singleLetterName ) ), //
					existingArgument => existingArgument == null, //
					existingArgument => throw new DuplicateArgumentSingleLetterNameException( singleLetterName ) );
			}
			return true;
		}
	}

	private protected int SingleLetterNameMatch( string token )
	{
		foreach( char singleLetterName in SingleLetterNames )
		{
			int skip = Helpers.ShortFormNameMatch( token, singleLetterName );
			if( skip != 0 )
			{
				Assert( skip == 2 );
				return skip;
			}
		}
		return 0;
	}

	private protected string SingleLetterNamesShortUsage => string.Concat( SingleLetterNames.Select( singleLetterName => $"-{singleLetterName}, " ) );
}
