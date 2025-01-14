namespace MikeNakis.Clio;

using LegacyCollections = System.Collections;
using System.Collections.Generic;
using SysCompiler = System.Runtime.CompilerServices;

sealed class ReferenceEqualityComparer : IEqualityComparer<object?>, LegacyCollections.IEqualityComparer
{
	ReferenceEqualityComparer() { }

	public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

	public new bool Equals( object? x, object? y ) => ReferenceEquals( x, y );

	public int GetHashCode( object? obj )
	{
		// Depending on target framework, RuntimeHelpers.GetHashCode might not be annotated
		// with the proper nullability attribute. We'll suppress any warning that might
		// result.
		return SysCompiler.RuntimeHelpers.GetHashCode( obj! );
	}
}
