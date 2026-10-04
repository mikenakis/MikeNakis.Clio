namespace MikeNakis.Clio;

using MikeNakis.Clio.Internal;
using Sys = System;

public sealed class EnumCodec<T> : StructCodec<T> where T : struct, Sys.Enum
{
	public static readonly StructCodec<T> Instance = new EnumCodec<T>();

	readonly Sys.Type enumType;

	EnumCodec()
			: this( typeof( T ) )
	{ }

	EnumCodec( Sys.Type enumType )
	{
		this.enumType = enumType;
	}

	public override string Name => enumType.Name;

	public override T ValueFromText( string text )
	{
		//PEARL: Enum.TryParse() also accepts numbers, comma-separated names, and surrounding whitespace, so we require an exact name.
		if( !Sys.Enum.IsDefined( enumType, text ) )
			throw new Sys.FormatException( $"Expected one of ({string.Join( ", ", enumType.GetEnumNames() )}), found '{text}'" );
		return (T)Sys.Enum.Parse( enumType, text );
	}

	public override string TextFromValue( T value ) => enumType.GetEnumName( value ).OrThrow();
}
