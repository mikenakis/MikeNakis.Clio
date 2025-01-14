namespace MikeNakis.Clio;

using SysThreading = System.Threading;

public static class FailureTesting
{
	static readonly SysThreading.ThreadLocal<bool> value = new( false );

	public static bool Value { get => value.Value; set => FailureTesting.value.Value = value; }
}
