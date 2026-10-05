namespace MikeNakis.Clio;

using Sys = System;

public abstract class UserException( Sys.Exception? cause = null ) : Sys.Exception( "", cause );

public sealed class HelpUserException : UserException
{
	internal BaseArgumentParser ArgumentParser { get; }

	internal HelpUserException( BaseArgumentParser argumentParser )
	{
		ArgumentParser = argumentParser;
	}

	public void OutputHelp( Sys.Action<string> lineOutputConsumer ) => ArgumentParser.OutputHelp( lineOutputConsumer );
	public override string Message => "Help requested.";
}

sealed class ArgumentSuppliedMoreThanOnceUserException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Argument '{argumentName}' supplied more than once.";
}

sealed class UnexpectedCharactersAfterNamedArgumentUserException( string argumentName, string unexpectedCharacters ) : UserException
{
	public string ArgumentName => argumentName;
	public string UnexpectedCharacters => unexpectedCharacters;
	public override string Message => $"Unexpected characters found after '{argumentName}' : '{unexpectedCharacters}'.";
}

sealed class EqualsSignExpectedUserException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Argument '{argumentName}' must be followed by an equals sign ('=').";
}

sealed class MetaOptionNameExpectedUserException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Argument '{argumentName}' must be followed by a colon (':') and a name.";
}

sealed class MetaOptionValueExpectedUserException( string argumentName, string metaOptionName ) : UserException
{
	public string ArgumentName => argumentName;
	public string MetaOptionName => metaOptionName;
	public override string Message => $"Name '{metaOptionName}' of argument '{argumentName}' must be followed by an equals sign ('=') and a value.";
}

sealed class MetaOptionNameSuppliedMoreThanOnceUserException( string argumentName, string metaOptionName ) : UserException
{
	public string ArgumentName => argumentName;
	public string MetaOptionName => metaOptionName;
	public override string Message => $"Name '{metaOptionName}' of argument '{argumentName}' supplied more than once.";
}

sealed class RequiredArgumentNotSuppliedUserException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Required argument '{argumentName}' was not supplied.";
}

sealed class UnexpectedTokenUserException( string token ) : UserException
{
	public string Token => token;
	public override string Message => $"Unexpected token: '{token}'.";
}

sealed class UnparsableValueUserException( string argumentName, string token, Sys.Exception? cause = null ) : UserException( cause )
{
	public string ArgumentName => argumentName;
	public string Token => token;
	public override string Message => $"'{token}' is not a valid value for argument '{argumentName}'.";
}

sealed class VerbExpectedUserException( string verbTerm ) : UserException
{
	public string VerbTerm => verbTerm;
	public override string Message => $"Expected a {verbTerm}.";
}

sealed class ResponseFileNameExpectedUserException() : UserException
{
	public override string Message => "Expected a file name after '@'.";
}

sealed class ResponseFileUnreadableUserException( string fileName, Sys.Exception cause ) : UserException( cause )
{
	public string FileName => fileName;
	public override string Message => $"Could not read response file '{fileName}'.";
}

sealed class ResponseFileIncludedMoreThanOnceUserException( string fileName ) : UserException
{
	public string FileName => fileName;
	public override string Message => $"Response file '{fileName}' is included more than once.";
}
