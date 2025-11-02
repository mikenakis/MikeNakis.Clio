namespace MikeNakis.Clio;

using System.Collections.Generic;
using LegacyCollections = System.Collections;
using SysCompiler = System.Runtime.CompilerServices;

// As of net5.0 there is System.Collections.Generic.ReferenceEqualityComparer, but it is not available in netstandard.
sealed class ReferenceEqualityComparer : IEqualityComparer<object?>, LegacyCollections.IEqualityComparer
{
	ReferenceEqualityComparer() { }

	public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

	public new bool Equals( object? x, object? y ) => ReferenceEquals( x, y );

	public int GetHashCode( object? obj ) => SysCompiler.RuntimeHelpers.GetHashCode( obj! );
}
