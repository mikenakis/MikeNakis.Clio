namespace MikeNakis.Clio;

using System.Collections.Generic;
using System.Linq;
using LegacyCollections = System.Collections;
using Sys = System;
using SysCompiler = System.Runtime.CompilerServices;
using SysText = System.Text;
using SysThreading = System.Threading;

static class KitHelpers
{
	public static readonly SysThreading.ThreadLocal<bool> FailureTesting = new( false );

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
			338 => true, // Œ
			339 => true, // œ
			352 => true, // Š
			353 => true, // š
			376 => true, // Ÿ
			381 => true, // Ž
			382 => true, // ž
			402 => true, // ƒ
			710 => true, // ˆ
			732 => true, // ˜
			8211 => true, // –
			8212 => true, // —
			8216 => true, // ‘
			8217 => true, // ’
			8218 => true, // ‚
			8220 => true, // “
			8221 => true, // ”
			8222 => true, // „
			8224 => true, // †
			8225 => true, // ‡
			8226 => true, // •
			8230 => true, // …
			8240 => true, // ‰
			8249 => true, // ‹
			8250 => true, // ›
			8364 => true, // €
			8482 => true, // ™
			_ => false
		};
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// IReadOnlyList

	internal class ComparisonComparer<T> : Comparer<T>
	{
		readonly Sys.Comparison<T> comparison;

		public ComparisonComparer( Sys.Comparison<T> comparison )
		{
			this.comparison = comparison;
		}

		public override int Compare( T? x, T? y )
		{
			return comparison( x!, y! );
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// System.Type

	// Obtains the full name of a type using C# notation.
	// PEARL: DotNet represents the full names of types in a cryptic way which does not correspond to any language in particular:
	//        - Generic types are suffixed with a back-quote character, followed by the number of generic parameters.
	//        - Constructed generic types are further suffixed with a list of assembly-qualified type names, one for each generic parameter.
	//        Plus, a nested class is denoted with the '+' sign. (Handling of which is TODO.)
	//        This method returns the full name of a type using C#-specific notation instead of DotNet's cryptic notation.
	public static string GetCSharpTypeName( Sys.Type type )
	{
		if( type.IsArray )
		{
			SysText.StringBuilder stringBuilder = new();
			stringBuilder.Append( GetCSharpTypeName( NotNull( type.GetElementType() ) ) );
			stringBuilder.Append( '[' );
			int rank = type.GetArrayRank();
			Assert( rank >= 1 );
			for( int i = 0; i < rank - 1; i++ )
				stringBuilder.Append( ',' );
			stringBuilder.Append( ']' );
			return stringBuilder.ToString();
		}
		if( type.IsGenericType )
		{
			SysText.StringBuilder stringBuilder = new();
			stringBuilder.Append( getBaseTypeName( type ) );
			stringBuilder.Append( '<' );
			stringBuilder.Append( string.Join( ",", type.GenericTypeArguments.Select( GetCSharpTypeName ).ToArray() ) );
			stringBuilder.Append( '>' );
			return stringBuilder.ToString();
		}
		return type.Namespace + '.' + type.Name.Replace( '+', '.' );

		static string getBaseTypeName( Sys.Type type )
		{
			string typeName = NotNull( type.GetGenericTypeDefinition().FullName );
			int indexOfTick = typeName.LastIndexOf( '`' );
			Assert( indexOfTick == typeName.IndexOf( '`' ) );
			return typeName[..indexOfTick];
		}
	}
}
