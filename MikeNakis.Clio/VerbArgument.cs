namespace MikeNakis.Clio;
using Sys = System;
using SysDiag = System.Diagnostics;

[SysDiag.DebuggerDisplay( "{ToString(),nq}" )]
sealed class VerbArgument : Argument, IVerbArgument
{
	readonly Sys.Action<ChildArgumentParser> handler;
	VerbExecutionArgumentParser? verbExecutionArgumentParser;
	public BaseArgumentParser? Value => verbExecutionArgumentParser;
	public override object? RawValue => Value;
	public override bool IsSupplied => verbExecutionArgumentParser != null;
	internal sealed override string ShortUsage => Name; //TODO: add ellipsis if it accepts arguments.

	internal VerbArgument( BaseArgumentParser argumentParser, string name, string? description, Sys.Action<ChildArgumentParser> handler )
			: base( argumentParser, name, description, isRequired: false ) //each individual verb is not required; the parser sees to it that if any verbs have been added, then one must be supplied.
	{
		Assert( Helpers.VerbNameIsValidAssertion( name ) );
		Assert( argumentParser.Arguments.OfType<PositionalArgument>().Where( positionalArgument => positionalArgument.IsOptional ).FirstOrDefault(), //
			optionalPositionalArgument => optionalPositionalArgument == null, //
			optionalPositionalArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.VerbMayNotBePrecededByOptionalPositional, name, optionalPositionalArgument!.Name ) );
		this.handler = handler;
		if( DebugMode )
		{
			VerbInitializationArgumentParser verbInitializationArgumentParser = new( argumentParser, name );
			handler.Invoke( verbInitializationArgumentParser );
			Assert( verbInitializationArgumentParser.TryParseWasInvoked, () => throw new TryParseWasNotInvokedException( Name ) );
		}
	}

	public sealed override int TryParse( int tokenIndex, IReadOnlyList<string> tokens )
	{
		Assert( verbExecutionArgumentParser == null ); //cannot happen
		if( tokens[tokenIndex] != Name )
			return tokenIndex;
		verbExecutionArgumentParser = new( ArgumentParser, Name, tokenIndex + 1, tokens );
		handler.Invoke( verbExecutionArgumentParser );
		return tokens.Count;
	}

	sealed class VerbInitializationArgumentParser : ChildArgumentParser
	{
		internal bool TryParseWasInvoked { get; private set; }

		internal VerbInitializationArgumentParser( BaseArgumentParser parent, string name )
			: base( parent, name )
		{ }

		public override bool TryParse()
		{
			Assert( !TryParseWasInvoked, () => throw new TryParseInvokedMoreThanOnceException( Name ) );
			TryParseWasInvoked = true;
			return false;
		}
	}

	sealed class VerbExecutionArgumentParser : ChildArgumentParser
	{
		readonly int tokenIndex;
		readonly IReadOnlyList<string> tokens;

		internal VerbExecutionArgumentParser( BaseArgumentParser parent, string name, int tokenIndex, IReadOnlyList<string> tokens )
			: base( parent, name )
		{
			this.tokenIndex = tokenIndex;
			this.tokens = tokens;
		}

		public override bool TryParse()
		{
			ParseRemainingTokens( tokenIndex, tokens );
			return true;
		}
	}
}
