namespace MikeNakis.Clio_Test;

using MikeNakis.Kit;
using MikeNakis.Kit.Extensions;
using MikeNakis.Kit.FileSystem;

sealed class Audit
{
	public const string FileExtension = ".audit";
	static readonly Dictionary<FilePath, Audit> auditsByPathName = new();

	public static void With( Sys.Action<Sys.Action<string>> procedure, //
		[SysCompiler.CallerFilePath] string? callerFileName = null, //
		[SysCompiler.CallerMemberName] string? callerMemberName = null )
	{
		Assert( callerFileName != null );
		Assert( callerMemberName != null );
		FilePath callerFilePath = FilePath.FromRelativeOrAbsolutePath( callerFileName.OrThrow(), DotNetHelpers.GetWorkingDirectoryPath() );
		FilePath auditFilePath = callerFilePath.WithReplacedExtension( FileExtension );
		Audit audit = getOrCreateAuditFile( auditFilePath );
		using( AuditFile auditFile = audit.newFile() )
		{
			auditFile.WriteLine( new string( '-', 80 ) );
			auditFile.WriteLine( callerMemberName.OrThrow() );
			auditFile.WriteLine( "" );
			procedure.Invoke( auditFile.WriteLine );
		}
	}

	readonly FilePath auditFilePath;
	bool isNew = true;

	Audit( FilePath auditFilePath )
	{
		this.auditFilePath = auditFilePath;
		auditFilePath.Truncate();
	}

	AuditFile newFile()
	{
		AuditFile auditFile = new( auditFilePath );
		if( isNew )
			isNew = false;
		else
			auditFile.WriteLine( "" );
		return auditFile;
	}

	static Audit getOrCreateAuditFile( FilePath auditFilePath )
	{
		return getOrCreate( auditsByPathName, auditFilePath, () => new Audit( auditFilePath ) );
	}

	static V getOrCreate<K, V>( IDictionary<K, V> dictionary, K key, Sys.Func<V> valueFactory )
	{
		lock( dictionary )
		{
			if( dictionary.TryGetValue( key, out V? existingValue ) )
				return existingValue;
			V newValue = valueFactory.Invoke();
			dictionary.Add( key, newValue );
			return newValue;
		}
	}

	[SysDiag.DebuggerDisplay( "{ToString(),nq}" )]
	sealed class AuditFile : Sys.IDisposable
	{
		readonly FilePath outputFilePath;
		readonly string endOfLine;
		readonly SysIo.StreamWriter streamWriter;

		public AuditFile( FilePath outputFilePath, string endOfLine = "\n" )
		{
			this.outputFilePath = outputFilePath;
			this.endOfLine = endOfLine;
			SysIo.Stream stream = outputFilePath.OpenBinaryForAppending(); // fileOptions: SysIo.FileOptions.WriteThrough | SysIo.FileOptions.SequentialScan
			streamWriter = new SysIo.StreamWriter( stream, DotNetHelpers.BomlessUtf8 );
		}

		public void WriteLine( string text )
		{
			streamWriter.Write( text );
			streamWriter.Write( endOfLine );
			streamWriter.Flush();
		}

		public void Dispose()
		{
			streamWriter.Dispose();
		}

		public override string ToString() => outputFilePath.Path;
	}
}
