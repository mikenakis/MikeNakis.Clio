namespace MikeNakis.Clio.Extensions;

public static class ArgumentParserExtensions
{
	///<summary>Adds an option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<string?> AddStringOption( this BaseArgumentParser self, string name, //
		char? singleLetterName = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddOption( name, StringCodec.Instance, singleLetterName, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>string</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddStringOptionWithDefault( this BaseArgumentParser self, string name, //
		string defaultValue, char? singleLetterName = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddOptionWithDefault( name, StringCodec.Instance, defaultValue, singleLetterName, description, parameterName, presetValue );
	}

	///<summary>Adds a required option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddRequiredStringOption( this BaseArgumentParser self, string name, //
		char? singleLetterName = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddRequiredOption( name, StringCodec.Instance, singleLetterName, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>int</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<int?> AddIntOption( this BaseArgumentParser self, string name, char? singleLetterName = null, //
		string? description = null, string? parameterName = null, int? presetValue = null )
	{
		return self.AddOption( name, IntCodec.Instance, singleLetterName, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>int</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The (optional) single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="defaultValue">The default value, which will be the value of the option if the option is omitted.</param>
	public static IOptionArgument<int> AddIntOptionWithDefault( this BaseArgumentParser self, string name, int defaultValue, char? singleLetterName = null, //
		string? description = null, string? parameterName = null )
	{
		return self.AddOptionWithDefault( name, IntCodec.Instance, defaultValue, singleLetterName, description, parameterName );
	}

	///<summary>Adds a positional argument of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	public static IPositionalArgument<string?> AddStringPositional( this BaseArgumentParser self, string name, string? description = null )
	{
		return self.AddPositional( name, StringCodec.Instance, description );
	}

	///<summary>Adds a positional argument of type <c>string</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="defaultValue">The default value for the positional argument, which will be the value of the argument
	///if the argument is not supplied.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	public static IPositionalArgument<string> AddStringPositionalWithDefault( this BaseArgumentParser self, string name, string defaultValue, string? description = null )
	{
		return self.AddPositionalWithDefault( name, StringCodec.Instance, defaultValue, description );
	}

	///<summary>Adds a required positional argument of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the positional argument, for use in response files, and when displaying help.</param>
	///<param name="description">The description of the positional argument, for use when displaying help.</param>
	public static IPositionalArgument<string> AddRequiredStringPositional( this BaseArgumentParser self, string name, string? description = null )
	{
		return self.AddRequiredPositional( name, StringCodec.Instance, description );
	}
}
