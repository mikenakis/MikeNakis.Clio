namespace MikeNakis.Clio;

/// <summary>Represents a mistake made by the programmer.</summary>
public abstract class ProgrammerException : SaneException;

/// <summary>Thrown when an attempt is made to add an argument after the command-line has been parsed.</summary>
public sealed class CommandLineHasAlreadyBeenParsedProgrammerException() : ProgrammerException;

/// <summary>Thrown when an attempt is made to read the value of an argument without first having parsed the command-line.</summary>
public sealed class CommandLineHasNotBeenParsedProgrammerException() : ProgrammerException;

/// <summary>Thrown when an attempt is made to add an argument with the same name as an already-added argument.</summary>
public sealed class DuplicateArgumentNameProgrammerException( string argumentName ) : ProgrammerException
{
	public string ArgumentName => argumentName;
}

/// <summary>Thrown when an attempt is made to add an argument with the same single-letter name as an already-added argument.</summary>
public sealed class DuplicateArgumentSingleLetterNameProgrammerException( char argumentShortFormName ) : ProgrammerException
{
	public char ArgumentShortFormName => argumentShortFormName;
}

/// <summary>Thrown when an attempt is made to add an argument with an invalid name.</summary>
public sealed class InvalidArgumentNameProgrammerException( string argumentName ) : ProgrammerException
{
	public string ArgumentName => argumentName;
}

/// <summary>Thrown when an attempt is made to add an argument with a default value of <c>null</c>.</summary>
public sealed class NullDefaultValueProgrammerException( string argumentName ) : ProgrammerException
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
public sealed class InvalidArgumentOrderingProgrammerException( ArgumentOrderingRule argumentOrderingRule, string violatingArgumentName, string precedingArgumentName ) : ProgrammerException
{
	public ArgumentOrderingRule ArgumentOrderingRule => argumentOrderingRule;
	public string ViolatingArgumentName => violatingArgumentName;
	public string PrecedingArgumentName => precedingArgumentName;
}

public sealed class VerbHandlerDidNotInvokeTryParseMethodProgrammerException( string verbName ) : ProgrammerException
{
	public string VerbName => verbName;
}

public sealed class TryParseInvokedMoreThanOnceProgrammerException( string verbName ) : ProgrammerException
{
	public string VerbName => verbName;
}
