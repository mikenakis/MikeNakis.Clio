namespace MikeNakis.Clio;

using Sys = System;
using SysDiag = System.Diagnostics;

/// <summary>Represents a command-line argument.</summary>
public interface IArgument
{
	///<summary>The name of the argument.</summary>
	string Name { get; }

	///<summary>The description of the argument.</summary>
	string? Description { get; }

	///<summary><c>true</c> if the argument is required; <c>false</c> otherwise.</summary>
	bool IsRequired { get; }

	///<summary><c>true</c> if the argument was supplied in the command-line; <c>false</c> otherwise.</summary>
	bool IsSupplied { get; }

	///<summary>The value of the argument as a nullable object.</summary>
	object? RawValue { get; }
}

/// <summary>Represents a command-line argument with a value.</summary>
public interface IArgument<T> : IArgument
{
	///<summary>The value of the argument.</summary>
	T Value { get; }
}

///<summary>Represents a switch argument.</summary>
///<remarks>A switch is a named argument without a parameter, e.g. <c>Acme.Cli --verbose</c>. The value of a switch
///argument is of type <c>bool</c>, and it is <c>true</c> if the switch was supplied in the command-line, <c>false</c>
///otherwise.</remarks>
public interface ISwitchArgument : IArgument<bool>
{
	char? SingleLetterName { get; }
}

///<summary>Represents an option argument.</summary>
///<remarks>An option is a named argument with a parameter, e.g. <c>Acme.Cli --verbosity quiet</c>.</remarks>
public interface IOptionArgument<T> : IArgument<T>
{
}

///<summary>Represents a positional argument.</summary>
///<remarks>A positional argument is a free-standing value in the command-line, identified by its position relative
///to other positional arguments. For example: <c>Acme.Cli InputFile.txt</c>.</remarks>
public interface IPositionalArgument<T> : IArgument<T>
{
}

///<summary>Represents a verb argument.</summary>
///<remarks>A verb is a word followed by a new set of arguments. For example: <c>Acme.Cli create ...</c>
///or <c>Acme.Cli list ...</c>.</remarks>
public interface IVerbArgument : IArgument<BaseArgumentParser?>
{
}

[SysDiag.DebuggerDisplay( "{ToString(),nq}" )]
abstract class Argument : IArgument
{
	public string Name { get; }
	private protected BaseArgumentParser ArgumentParser { get; }
	public abstract object? RawValue { get; }
	public string? Description { get; }
	public bool IsRequired { get; }
	internal bool IsOptional => !IsRequired;
	public abstract bool IsSupplied { get; }
	internal abstract string ShortUsage { get; }
	public abstract int TryParse( int tokenIndex, IReadOnlyList<string> tokens );
	public sealed override string ToString() => $"'{Name}' = {ValueToString()}";

	private protected Argument( BaseArgumentParser argumentParser, string name, string? description, bool isRequired )
	{
		Assert( !argumentParser.Arguments.Where( argument => argument.Name == name ).Any(), () => throw new DuplicateArgumentNameException( name ) );
		ArgumentParser = argumentParser;
		Name = name;
		Description = description;
		IsRequired = isRequired;
		ArgumentParser.AddArgument( this );
	}

	internal string ValueToString()
	{
		if( !ArgumentParser.HasBeenParsed )
			return "not yet parsed";
		return KitHelpers.SafeToString( RawValue );
	}

	internal virtual void CollectLongUsageLines( Sys.Action<string> lineConsumer )
	{
		lineConsumer.Invoke( $"{Description ?? Helpers.DefaultDescription}." );
	}

	private protected bool HasBeenParsedAssertion()
	{
		Assert( ArgumentParser.HasBeenParsed, () => throw new CommandLineHasNotBeenParsedException() );
		return true;
	}
}
