namespace MikeNakis.Clio_Test;

using System.Collections.Generic;
using MikeNakis.Clio;
using Sys = System;
using SysDiag = System.Diagnostics;

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

	///<summary>Identity function.</summary>
	///<remarks>useful as a no-op lambda and sometimes as a debugging aid.</remarks>
	public static T Identity<T>( T value ) => value;

	/// <summary>Performs an assertion.</summary>
	/// <remarks>If the given <paramref name="condition"/> is <c>false</c>, the supplied <paramref name="exceptionFactory"/> is invoked, and the returned <see cref="Sys.Exception"/> is thrown.
	/// (Though the factory may just as well throw the exception instead of returning it.)
	/// This function is only executed (and the supplied <paramref name="condition"/> is only evaluated) when running a debug build.</remarks>
	[SysDiag.DebuggerHidden]
	[SysDiag.Conditional( "DEBUG" )]
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
	[SysDiag.Conditional( "DEBUG" )]
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
			if( SysDiag.Debugger.IsAttached )
			{
				SysDiag.Debugger.Break(); //Note: this is problematic due to some Visual Studio bug: when it hits, you are prevented from setting the next statement either within the calling function or within this function.
				return;
			}
		}
		throw exception;
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

	public static IEnumerable<T> EnumerableOf<T>( params T[] arguments ) => arguments;
}
