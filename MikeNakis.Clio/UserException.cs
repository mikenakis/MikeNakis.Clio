namespace MikeNakis.Clio;

using Sys = System;

public abstract class UserException( Sys.Exception? cause = null ) : Sys.Exception( "", cause );

public sealed class HelpException : UserException
{
	internal BaseArgumentParser ArgumentParser { get; }

	internal HelpException( BaseArgumentParser argumentParser )
	{
		ArgumentParser = argumentParser;
	}

	public void OutputHelp( Sys.Action<string> lineOutputConsumer ) => ArgumentParser.OutputHelp( lineOutputConsumer );
	public override string Message => "Help requested.";
}

sealed class ArgumentSuppliedMoreThanOnceException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Argument '{argumentName}' supplied more than once.";
}

sealed class UnexpectedCharactersAfterNamedArgumentException( string argumentName, string unexpectedCharacters ) : UserException
{
	public string ArgumentName => argumentName;
	public string UnexpectedCharacters => unexpectedCharacters;
	public override string Message => $"Unexpected characters found after '{argumentName}' : '{unexpectedCharacters}'.";
}

sealed class EqualsSignExpectedException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Argument '{argumentName}' must be followed by an equals sign ('=').";
}

sealed class RequiredArgumentNotSuppliedException( string argumentName ) : UserException
{
	public string ArgumentName => argumentName;
	public override string Message => $"Required argument '{argumentName}' was not supplied.";
}

sealed class UnexpectedTokenException( string token ) : UserException
{
	public string Token => token;
	public override string Message => $"Unexpected token: '{token}'.";
}

sealed class UnparsableValueException( string argumentName, string token, Sys.Exception? cause = null ) : UserException( cause )
{
	public string ArgumentName => argumentName;
	public string Token => token;
	public override string Message => $"'{token}' is not a valid value for argument '{argumentName}'.";
}

sealed class VerbExpectedException( string verbTerm ) : UserException
{
	public string VerbTerm => verbTerm;
	public override string Message => $"Expected a {verbTerm}.";
}

sealed class ResponseFileNameExpectedException() : UserException
{
	public override string Message => "Expected a file name after '@'.";
}

sealed class ResponseFileUnreadableException( string fileName, Sys.Exception cause ) : UserException( cause )
{
	public string FileName => fileName;
	public override string Message => $"Could not read response file '{fileName}'.";
}

sealed class ResponseFileIncludedMoreThanOnceException( string fileName ) : UserException
{
	public string FileName => fileName;
	public override string Message => $"Response file '{fileName}' is included more than once.";
}
