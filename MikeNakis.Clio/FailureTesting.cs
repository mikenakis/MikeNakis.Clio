namespace MikeNakis.Clio;

public static class FailureTesting
{
	static readonly SysThread.ThreadLocal<bool> value = new( false );

	public static bool Value { get => value.Value; set => FailureTesting.value.Value = value; }
}
