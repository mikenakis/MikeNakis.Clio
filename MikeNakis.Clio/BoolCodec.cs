namespace MikeNakis.Clio;

public sealed class BoolCodec : StructCodec<bool>
{
	public static readonly StructCodec<bool> Instance = new BoolCodec();

	BoolCodec()
	{ }

	public override string Name => "boolean";

	public override bool ValueFromText( string text )
	{
		if( !bool.TryParse( text, out bool value ) )
			throw new Sys.FormatException( $"Expected 'true' or 'false', found '{text}'" );
		return value;
	}

	public override string TextFromValue( bool value )
	{
		return value.ToString( SysGlob.CultureInfo.InvariantCulture );
	}
}
