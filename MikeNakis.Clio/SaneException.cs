namespace MikeNakis.Clio;

using Sys = System;
using SysDiag = System.Diagnostics;
using SysReflect = System.Reflection;
using SysText = System.Text;

[SysDiag.DebuggerDisplay( "{GetType().Name,nq}: {Message,nq}" )]
public abstract class SaneException : Sys.Exception
{
	public override string Message => buildMessage();

	string buildMessage()
	{
		SysText.StringBuilder stringBuilder = new();
		foreach( SysReflect.MemberInfo? memberInfo in GetType().GetMembers( SysReflect.BindingFlags.Public | SysReflect.BindingFlags.Instance ) )
		{
			if( memberInfo.Name == nameof( Message ) )
				continue;
			if( !GetType().IsAssignableFrom( memberInfo.DeclaringType ) )
				continue;
			object? value;
			switch( memberInfo )
			{
				case SysReflect.FieldInfo fieldInfo:
					value = fieldInfo.GetValue( this );
					break;
				case SysReflect.PropertyInfo propertyInfo:
					value = propertyInfo.GetValue( this );
					break;
				default:
					continue;
			}
			if( stringBuilder.Length > 0 )
				stringBuilder.Append( "; " );
			stringBuilder.Append( memberInfo.Name );
			stringBuilder.Append( " = " );
			stringBuilder.Append( KitHelpers.SafeToString( value ) );
		}
		return stringBuilder.ToString();
	}

	protected SaneException( Sys.Exception? cause )
			: base( null, cause )
	{ }

	protected SaneException()
			: base( null, null )
	{ }
}
