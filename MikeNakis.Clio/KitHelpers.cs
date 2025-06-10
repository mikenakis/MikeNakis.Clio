namespace MikeNakis.Clio;

static class KitHelpers
{
	public static readonly SysThread.ThreadLocal<bool> FailureTesting = new( false );

	public static string SafeToString( object? value )
	{
		SysText.StringBuilder stringBuilder = new();
		Sys.Action<string> textConsumer = s => stringBuilder.Append( s );
		SafeToString( value, textConsumer );
		return stringBuilder.ToString();
	}

	public static void SafeToString( object? value, Sys.Action<string> textConsumer )
	{
		ISet<object> visitedObjects = new HashSet<object>( ReferenceEqualityComparer.Instance );
		recurse( value );
		return;

		void recurse( object? value )
		{
			if( value == null )
			{
				//PEARL: In dotnet, all default conversions of `object` to `string` will convert `null` to the empty
				//       string instead of the string "null". Thus, when we print a string, we can never tell whether it
				//       was `null` or empty, because it always looks empty. We fix this here.
				//       I guess this is happening because dotnet is also used by Visual Basic, and Visual Basic
				//       programmers might have epileptic seizures if they see the word `null`.
				Assert( $"{value}" == "" ); //Ensure that if the behavior of the runtime ever gets fixed, we will notice.
				textConsumer.Invoke( "null" );
			}
			else if( value is char c )
				EscapeForCSharp( c, textConsumer );
			else if( value is string s )
				EscapeForCSharp( s, textConsumer );
			//else if( value is Sys.Type t )
			//	textConsumer.Invoke( t.GetCSharpName( CSharpTypeNames.Options.EmitTypeDefinitionKeyword ) );
			else if( value is LegacyCollections.IEnumerable enumerable )
			{
				textConsumer.Invoke( "[" );
				textConsumer.Invoke( string.Join( ", ", enumerable.Cast<object>().ToArray() ) );
				textConsumer.Invoke( "]" );
			}
			else if( value.GetType().IsValueType || visitedObjects.Add( value ) )
				textConsumer.Invoke( value.ToString() ?? "\u2620" ); //U+2620 Skull and Crossbones Unicode Character to indicate that `ToString()` returned `null`.
			else
				textConsumer.Invoke( $"{value.GetType()}@{SysCompiler.RuntimeHelpers.GetHashCode( value )}" );
		}
	}

	public static void EscapeForCSharp( string content, Sys.Action<string> textConsumer ) => EscapeForCSharp( content, '"', textConsumer );

	public static void EscapeForCSharp( char content, Sys.Action<string> textConsumer ) => EscapeForCSharp( content.ToString(), '\'', textConsumer );

	public static void EscapeForCSharp( string content, char quote, Sys.Action<string> textConsumer )
	{
		ScribeStringLiteral( quote, content, textConsumer );
	}

	public static void ScribeStringLiteral( char quoteCharacter, string instance, Sys.Action<string> textConsumer )
	{
		textConsumer.Invoke( new string( quoteCharacter, 1 ) );
		foreach( char c in instance )
			if( c == quoteCharacter )
				emitEscapedCharacter( textConsumer, c );
			else
				switch( c )
				{
					case '\t':
						emitEscapedCharacter( textConsumer, 't' );
						break;
					case '\r':
						emitEscapedCharacter( textConsumer, 'r' );
						break;
					case '\n':
						emitEscapedCharacter( textConsumer, 'n' );
						break;
					case '\\':
						emitEscapedCharacter( textConsumer, '\\' );
						break;
					default:
						emitOtherCharacter( textConsumer, c );
						break;
				}
		textConsumer.Invoke( new string( quoteCharacter, 1 ) );
		return;

		static void emitEscapedCharacter( Sys.Action<string> textConsumer, char c )
		{
			textConsumer.Invoke( $"\\{c}" );
		}

		static void emitOtherCharacter( Sys.Action<string> textConsumer, char c )
		{
			if( IsPrintable( c ) )
				textConsumer.Invoke( new string( c, 1 ) );
			else if( c < 256 ) // no need to check for >= 0 because char is unsigned.
			{
				char c1 = digitFromNibble( c >> 4 );
				char c2 = digitFromNibble( c & 0x0f );
				textConsumer.Invoke( $"\\x{c1}{c2}" );
			}
			else
			{
				char c1 = digitFromNibble( c >> 12 & 0x0f );
				char c2 = digitFromNibble( c >> 8 & 0x0f );
				char c3 = digitFromNibble( c >> 4 & 0x0f );
				char c4 = digitFromNibble( c & 0x0f );
				textConsumer.Invoke( $"\\x{c1}{c2}{c3}{c4}" );
			}
			return;

			static char digitFromNibble( int nibble )
			{
				Assert( nibble is >= 0 and < 16 );
				return (char)((nibble >= 10 ? 'a' - 10 : '0') + nibble);
			}
		}
	}

	public static bool IsPrintable( char c )
	{
		// see https://www.johndcook.com/blog/2013/04/11/which-unicode-characters-can-you-depend-on/
		// see https://en.wikipedia.org/wiki/Windows-1252
		return (int)c switch
		{
			< 32 => false,
			< 127 => true,
			< 160 => false,
			173 => false,
			< 256 => true,
			0x0152 => true, // Œ
			0x0153 => true, // œ
			0x0160 => true, // Š
			0x0161 => true, // š
			0x0178 => true, // Ÿ
			0x017D => true, // Ž
			0x017E => true, // ž
			0x0192 => true, // ƒ
			0x02C6 => true, // ˆ
			0x02DC => true, // ˜
			0x2013 => true, // –
			0x2014 => true, // —
			0x2018 => true, // ‘
			0x2019 => true, // ’
			0x201A => true, // ‚
			0x201C => true, // “
			0x201D => true, // ”
			0x201E => true, // „
			0x2020 => true, // †
			0x2021 => true, // ‡
			0x2022 => true, // •
			0x2026 => true, // …
			0x2030 => true, // ‰
			0x2039 => true, // ‹
			0x203A => true, // ›
			0x20AC => true, // €
			0x2122 => true, // ™
			_ => false
		};
	}
}
