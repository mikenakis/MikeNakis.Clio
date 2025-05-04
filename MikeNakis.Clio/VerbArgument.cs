namespace MikeNakis.Clio;

[SysDiag.DebuggerDisplay( "{ToString(),nq}" )]
sealed class VerbArgument : Argument, IVerbArgument
{
	readonly VerbHandler verbHandler;
	VerbExecutionArgumentParser? verbExecutionArgumentParser;
	public BaseArgumentParser? Value => verbExecutionArgumentParser;
	public override object? RawValue => Value;
	public override bool IsSupplied => verbExecutionArgumentParser != null;
	internal sealed override string ShortUsage => Name; //TODO: add ellipsis if it accepts arguments.

	internal VerbArgument( BaseArgumentParser argumentParser, string name, string? description, VerbHandler verbHandler )
			: base( argumentParser, name, description, isRequired: false ) //each individual verb is not required; the parser sees to it that if any verbs have been added, then one must be supplied.
	{
		Assert( Helpers.VerbNameIsValidAssertion( name ) );
		Assert( argumentParser.Arguments.OfType<PositionalArgument>().FirstOrDefault(), //
			positionalArgument => positionalArgument == null, //
			positionalArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.VerbMayNotBePrecededByPositionalArgument, name, positionalArgument!.Name ) );
		Assert( argumentParser.Arguments.Where( argument => argument.IsRequired ).FirstOrDefault(), //
			requiredArgument => requiredArgument == null, //
			requiredArgument => throw new InvalidArgumentOrderingException( ArgumentOrderingRule.VerbMayNotBePrecededByRequiredArgument, name, requiredArgument!.Name ) );
		this.verbHandler = verbHandler;
		if( DebugMode )
		{
			VerbInitializationArgumentParser verbInitializationArgumentParser = new( argumentParser, name );
			verbHandler.Invoke( verbInitializationArgumentParser );
			Assert( verbInitializationArgumentParser.TryParseWasInvoked, () => throw new TryParseWasNotInvokedException( Name ) );
		}
	}

	public sealed override int OnTryParse( int tokenIndex, List<string> tokens )
	{
		Assert( verbExecutionArgumentParser == null ); //cannot happen
		if( tokens[tokenIndex] != Name )
			return tokenIndex;
		//ArgumentParser.VerbFound();
		verbExecutionArgumentParser = new( ArgumentParser, Name, tokenIndex + 1, tokens );
		verbHandler.Invoke( verbExecutionArgumentParser );
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
		readonly List<string> tokens;

		internal VerbExecutionArgumentParser( BaseArgumentParser parent, string name, int tokenIndex, List<string> tokens )
			: base( parent, name )
		{
			this.tokenIndex = tokenIndex;
			this.tokens = tokens;
		}

		public override bool TryParse()
		{
			Parse( tokens, tokenIndex );
			return true;
		}
	}
}
