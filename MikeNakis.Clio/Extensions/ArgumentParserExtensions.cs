namespace MikeNakis.Clio.Extensions;

using System.Collections.Generic;
using Sys = System;

public static class ArgumentParserExtensions
{
	///<summary>Adds an option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterNames">The (optional) single-letter names for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<string?> AddStringOption( this BaseArgumentParser self, string name, //
		IReadOnlyList<char>? singleLetterNames = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddOption( name, StringCodec.Instance, singleLetterNames, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<string?> AddStringOption( this BaseArgumentParser self, string name, //
		char singleLetterName, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddStringOption( name, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>string</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterNames">The (optional) single-letter names for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddStringOptionWithDefault( this BaseArgumentParser self, string name, //
		string defaultValue, IReadOnlyList<char>? singleLetterNames = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddOptionWithDefault( name, StringCodec.Instance, defaultValue, singleLetterNames, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>string</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddStringOptionWithDefault( this BaseArgumentParser self, string name, //
		string defaultValue, char singleLetterName, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddStringOptionWithDefault( name, defaultValue, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a required option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterNames">The (optional) single-letter names for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddRequiredStringOption( this BaseArgumentParser self, string name, //
		IReadOnlyList<char>? singleLetterNames = null, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddRequiredOption( name, StringCodec.Instance, singleLetterNames, description, parameterName, presetValue );
	}

	///<summary>Adds a required option of type <c>string</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<string> AddRequiredStringOption( this BaseArgumentParser self, string name, //
		char singleLetterName, string? description = null, string? parameterName = null, //
		string? presetValue = null )
	{
		return self.AddRequiredStringOption( name, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>int</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterNames">The (optional) single-letter names for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<int?> AddIntOption( this BaseArgumentParser self, string name, IReadOnlyList<char>? singleLetterNames = null, //
		string? description = null, string? parameterName = null, int? presetValue = null )
	{
		return self.AddOption( name, IntCodec.Instance, singleLetterNames, description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>int</c>.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<int?> AddIntOption( this BaseArgumentParser self, string name, char singleLetterName, //
		string? description = null, string? parameterName = null, int? presetValue = null )
	{
		return self.AddIntOption( name, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option of type <c>int</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterNames">The (optional) single-letter names for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="defaultValue">The default value, which will be the value of the option if the option is omitted.</param>
	public static IOptionArgument<int> AddIntOptionWithDefault( this BaseArgumentParser self, string name, int defaultValue, IReadOnlyList<char>? singleLetterNames = null, //
		string? description = null, string? parameterName = null )
	{
		return self.AddOptionWithDefault( name, IntCodec.Instance, defaultValue, singleLetterNames, description, parameterName );
	}

	///<summary>Adds an option of type <c>int</c> with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="defaultValue">The default value, which will be the value of the option if the option is omitted.</param>
	public static IOptionArgument<int> AddIntOptionWithDefault( this BaseArgumentParser self, string name, int defaultValue, char singleLetterName, //
		string? description = null, string? parameterName = null )
	{
		return self.AddIntOptionWithDefault( name, defaultValue, [singleLetterName], description, parameterName );
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

	///<summary>Adds a switch.</summary>
	///<remarks>A switch is a named argument without a parameter, e.g. <c>AcmeCli --verbose</c>. The value of a
	/// switch is of type <c>bool</c>, indicating whether the switch was supplied or not.</remarks>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the switch.</param>
	///<param name="singleLetterName">The single-letter name for the switch.</param>
	///<param name="description">The description of the switch, for use when displaying help.</param>
	public static ISwitchArgument AddSwitch( this BaseArgumentParser self, string name, char singleLetterName, string? description = null )
	{
		return self.AddSwitch( name, [singleLetterName], description );
	}

	///<summary>Adds an option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<T?> AddOption<T>( this BaseArgumentParser self, string name, StructCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return self.AddOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IOptionArgument<T?> AddOption<T>( this BaseArgumentParser self, string name, ClassCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return self.AddOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<T> AddOptionWithDefault<T>( this BaseArgumentParser self, string name, StructCodec<T> codec, T defaultValue, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return self.AddOptionWithDefault( name, codec, defaultValue, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds an option with a default value.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="defaultValue">The default value for the option, which will be the value of the option if the option
	///is not supplied.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<T> AddOptionWithDefault<T>( this BaseArgumentParser self, string name, ClassCodec<T> codec, T defaultValue, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return self.AddOptionWithDefault( name, codec, defaultValue, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a required option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<T> AddRequiredOption<T>( this BaseArgumentParser self, string name, StructCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return self.AddRequiredOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a required option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between
	///<c>string</c> and the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is supplied without an equals-sign and a value.</param>
	public static IOptionArgument<T> AddRequiredOption<T>( this BaseArgumentParser self, string name, ClassCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return self.AddRequiredOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a repeated option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IRepeatedOptionArgument<T> AddRepeatedOption<T>( this BaseArgumentParser self, string name, StructCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return self.AddRepeatedOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a repeated option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the option; specifies how to convert between string and
	///the actual type of the option.</param>
	///<param name="singleLetterName">The single-letter name for the option.</param>
	///<param name="description">The description of the option, for use when displaying help.</param>
	///<param name="parameterName">The name of the parameter of the option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the option, which will be the value of the option if the
	///option is specified without an equals-sign and a value.</param>
	public static IRepeatedOptionArgument<T> AddRepeatedOption<T>( this BaseArgumentParser self, string name, ClassCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return self.AddRepeatedOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a meta-option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the meta-option.</param>
	///<param name="codec">The <see cref="StructCodec{T}"/> of the meta-option; specifies how to convert between string
	///and the actual type of the values of the meta-option.</param>
	///<param name="singleLetterName">The single-letter name for the meta-option.</param>
	///<param name="description">The description of the meta-option, for use when displaying help.</param>
	///<param name="parameterName">The name of the value parameter of the meta-option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the meta-option, which will be the value for a name if
	///the name is supplied without an equals-sign and a value.</param>
	public static IMetaOptionArgument<T> AddMetaOption<T>( this BaseArgumentParser self, string name, StructCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : struct
	{
		return self.AddMetaOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	///<summary>Adds a meta-option.</summary>
	///<param name="self">The <see cref="ArgumentParser" />.</param>
	///<param name="name">The name of the meta-option.</param>
	///<param name="codec">The <see cref="ClassCodec{T}"/> of the meta-option; specifies how to convert between string
	///and the actual type of the values of the meta-option.</param>
	///<param name="singleLetterName">The single-letter name for the meta-option.</param>
	///<param name="description">The description of the meta-option, for use when displaying help.</param>
	///<param name="parameterName">The name of the value parameter of the meta-option, for use when displaying help.</param>
	///<param name="presetValue">The (optional) preset value of the meta-option, which will be the value for a name if
	///the name is supplied without an equals-sign and a value.</param>
	public static IMetaOptionArgument<T> AddMetaOption<T>( this BaseArgumentParser self, string name, ClassCodec<T> codec, char singleLetterName, //
		string? description = null, string? parameterName = null, T? presetValue = default ) where T : class
	{
		return self.AddMetaOption( name, codec, [singleLetterName], description, parameterName, presetValue );
	}

	/// <summary>Parses an array of command-line tokens, stores values in arguments, invokes verb handlers, etc.</summary>
	/// <remarks>If something goes wrong, (or if the `--help` option is supplied,) it displays all necessary messages
	/// and returns <c>false</c>, meaning that the current process should terminate.</remarks>
	/// <param name="self">The <see cref="ArgumentParser" />.</param>
	/// <param name="arrayOfToken">The command-line tokens to parse.</param>
	/// <param name="lineOutputConsumer">A consumer for text output. Defaults to the <see cref="Sys.IO.TextWriter.WriteLine( string )"/> method of <see cref="Sys.Console.Error"/>.</param>
	/// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
	public static bool TryParse( this ArgumentParser self, string[] arrayOfToken, Sys.Action<string>? lineOutputConsumer = null )
	{
		lineOutputConsumer ??= Sys.Console.Error.WriteLine;
		try
		{
			self.Parse( arrayOfToken );
			return true;
		}
		catch( HelpUserException helpException )
		{
			helpException.OutputHelp( lineOutputConsumer );
			return false;
		}
		catch( UserException userException )
		{
			Helpers.OutputExceptionMessage( userException, lineOutputConsumer );
			string fullName = self.GetFullName();
			lineOutputConsumer.Invoke( $"Try '{fullName} --help' for more information." );
			return false;
		}
	}
}
