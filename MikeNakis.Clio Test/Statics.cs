namespace MikeNakis.Clio_Test;

using MikeNakis.Clio;

///<summary>Frequently used stuff that needs to be conveniently accessible without a type name qualification.</summary>
///<remarks>NOTE: This class must be kept AS SMALL AS POSSIBLE.</remarks>
static class Statics
{
	///<summary>Always returns <c>true</c>.</summary>
	///<remarks>Same as <c>if( true )</c>, but without a "condition is always true" warning.
	///Allows code to be enabled/disabled while still having to pass compilation, thus preventing code rot.</remarks>
	public static bool True => true;

	///<summary>Always returns <c>false</c>.</summary>
	///<remarks>Same as <c>if( false )</c>, but without a "condition is always false" warning.
	///Allows code to be enabled/disabled while still having to pass compilation, thus preventing code rot.</remarks>
	public static bool False => false;

	///<summary>Returns <c>true</c> if <c>DEBUG</c> has been defined.</summary>
	///<remarks>Allows code to be enabled/disabled while still having to pass compilation, thus preventing code rot.</remarks>
#if DEBUG
	public static bool DebugMode => true;
#else
	public static bool DebugMode => false;
#endif

	///<summary>Identity function.</summary>
	///<remarks>useful as a no-op lambda and sometimes as a debugging aid.</remarks>
	public static T Identity<T>( T value ) => value;

	/// <summary>Performs an assertion.</summary>
	/// <remarks>If the given <paramref name="condition"/> is <c>false</c>, the supplied <paramref name="exceptionFactory"/> is invoked, and the returned <see cref="Sys.Exception"/> is thrown.
	/// (Though the factory may just as well throw the exception instead of returning it.)
	/// This function is only executed (and the supplied <paramref name="condition"/> is only evaluated) when running a debug build.</remarks>
	[SysDiag.DebuggerHidden]
	public static void Assert( bool condition, Sys.Func<Sys.Exception> exceptionFactory ) //
	{
		if( condition )
			return;
		fail( exceptionFactory );
	}

	/// <summary>Performs an assertion.</summary>
	/// <remarks>If the given <paramref name="condition"/> is <c>false</c>, an <see cref="AssertionFailureException"/> is thrown.
	/// This function is only executed (and the supplied <paramref name="condition"/> is only evaluated) when running a debug build.</remarks>
	[SysDiag.DebuggerHidden]
	public static void Assert( bool condition ) //
	{
		if( condition )
			return;
		fail( () => new AssertionFailureException() );
	}

	[SysDiag.DebuggerHidden]
	static void fail( Sys.Func<Sys.Exception> exceptionFactory )
	{
		Sys.Exception exception = exceptionFactory.Invoke();
		if( !FailureTesting.Value )
		{
			SysDiag.Debug.WriteLine( $"Assertion failed: {exception.GetType().FullName}: {exception.Message}" );
			if( Breakpoint() )
				return;
		}
		throw exception;
	}

	/// <summary>Returns the supplied pointer unchanged, while asserting that it is non-<c>null</c>.</summary>
	[SysDiag.DebuggerHidden]
	public static T NotNull<T>( T? nullableReference ) where T : class //
	{
		Assert( nullableReference != null );
		return nullableReference!;
	}

	/// <summary>If a debugger is attached, hits a breakpoint and returns <c>true</c>; otherwise, returns <c>false</c></summary>
	[SysDiag.DebuggerHidden]
	public static bool Breakpoint()
	{
		if( SysDiag.Debugger.IsAttached )
		{
			SysDiag.Debugger.Break(); //Note: this is problematic due to some Visual Studio bug: when it hits, you are prevented from setting the next statement either within the calling function or within this function.
			return true;
		}
		return false;
	}

	public static Sys.Exception? TryCatch( Sys.Action procedure )
	{
		Assert( !FailureTesting.Value );
		FailureTesting.Value = true;
		try
		{
			procedure.Invoke();
			return null;
		}
		catch( Sys.Exception exception )
		{
			return exception;
		}
		finally
		{
			FailureTesting.Value = false;
		}
	}

	[SysDiag.DebuggerHidden]
#pragma warning disable CA1021 //CA1021: "Avoid `out` parameters"
	public static void Cast<T, U>( this T input, out U output ) where U : T where T : class => output = (U)input;
#pragma warning restore CA1021 //CA1021: "Avoid `out` parameters"
}
