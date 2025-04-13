namespace MikeNakis.Clio;

using Sys = System;

/// <summary>Represents a mistake made by the programmer.</summary>
public abstract class ProgrammerException : Sys.Exception;

/// <summary>Thrown when an attempt is made to add an argument after the command-line has been parsed.</summary>
public sealed class CommandLineHasAlreadyBeenParsedException() : ProgrammerException;

/// <summary>Thrown when an attempt is made to read the value of an argument without first having parsed the command-line.</summary>
public sealed class CommandLineHasNotBeenParsedException() : ProgrammerException;

/// <summary>Thrown when an attempt is made to add an argument with the same name as an already-added argument.</summary>
public sealed class DuplicateArgumentNameException( string argumentName ) : ProgrammerException
{
	public string ArgumentName => argumentName;
}

/// <summary>Thrown when an attempt is made to add an argument with the same single-letter name as an already-added argument.</summary>
public sealed class DuplicateArgumentSingleLetterNameException( char argumentShortFormName ) : ProgrammerException
{
	public char ArgumentShortFormName => argumentShortFormName;
}

/// <summary>Thrown when an attempt is made to add an argument with an invalid name.</summary>
public sealed class InvalidArgumentNameException( string argumentName ) : ProgrammerException
{
	public string ArgumentName => argumentName;
}

public enum ArgumentOrderingRule
{
	NamedArgumentMustPrecedePositional,
	RequiredPositionalMustPrecedeOptionalPositional,
	ArgumentMustPrecedeVerb,
	VerbMayNotBePrecededByPositionalArgument,
	VerbMayNotBePrecededByRequiredArgument
}

/// <summary>Thrown when an attempt is made to add arguments in the wrong order.</summary>
public sealed class InvalidArgumentOrderingException( ArgumentOrderingRule argumentOrderingRule, string violatingArgumentName, string precedingArgumentName ) : ProgrammerException
{
	public ArgumentOrderingRule ArgumentOrderingRule => argumentOrderingRule;
	public string ViolatingArgumentName => violatingArgumentName;
	public string PrecedingArgumentName => precedingArgumentName;
}

public sealed class TryParseWasNotInvokedException( string verbName ) : ProgrammerException
{
	public string VerbName => verbName;
}

public sealed class TryParseInvokedMoreThanOnceException( string verbName ) : ProgrammerException
{
	public string VerbName => verbName;
}
