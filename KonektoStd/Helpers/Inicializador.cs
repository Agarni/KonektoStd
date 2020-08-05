using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KonektoStd.Helpers
{
    public class Inicializador
    {
        static bool _DLLsInicializadas;

        public Inicializador()
        {
            if (_DLLsInicializadas)
                return;

            //EmbeddedAssembly.Load("KonektoStd.Referencias.BouncyCastle.Crypto.dll", "BouncyCastle.Crypto.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.FirebirdSql.Data.FirebirdClient.dll", "FirebirdSql.Data.FirebirdClient.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Google.Protobuf.dll", "Google.Protobuf.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.K4os.Compression.LZ4.dll", "K4os.Compression.LZ4.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.K4os.Compression.LZ4.Streams.dll", "K4os.Compression.LZ4.Streams.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.K4os.Hash.xxHash.dll", "K4os.Hash.xxHash.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Microsoft.Bcl.AsyncInterfaces.dll", "Microsoft.Bcl.AsyncInterfaces.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Microsoft.CSharp.dll", "Microsoft.CSharp.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Microsoft.Data.Sqlite.dll", "Microsoft.Data.Sqlite.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Microsoft.Win32.Registry.dll", "Microsoft.Win32.Registry.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.MySql.Data.dll", "MySql.Data.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Npgsql.dll", "Npgsql.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Renci.SshNet.dll", "Renci.SshNet.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.SQLitePCLRaw.core.dll", "SQLitePCLRaw.core.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.SshNet.Security.Cryptography.dll", "SshNet.Security.Cryptography.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Buffers.dll", "System.Buffers.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Collections.Concurrent.dll", "System.Collections.Concurrent.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Configuration.ConfigurationManager.dll", "System.Configuration.ConfigurationManager.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Data.Odbc.dll", "System.Data.Odbc.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Data.SqlClient.dll", "System.Data.SqlClient.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Data.SQLite.dll", "System.Data.SQLite.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Diagnostics.DiagnosticSource.dll", "System.Diagnostics.DiagnosticSource.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Dynamic.Runtime.dll", "System.Dynamic.Runtime.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.IO.FileSystem.Primitives.dll", "System.IO.FileSystem.Primitives.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Linq.dll", "System.Linq.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Linq.Expressions.dll", "System.Linq.Expressions.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Memory.dll", "System.Memory.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Numerics.Vectors.dll", "System.Numerics.Vectors.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.ObjectModel.dll", "System.ObjectModel.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Reflection.Emit.dll", "System.Reflection.Emit.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Reflection.Emit.ILGeneration.dll", "System.Reflection.Emit.ILGeneration.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Reflection.Emit.Lightweight.dll", "System.Reflection.Emit.Lightweight.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Reflection.TypeExtensions.dll", "System.Reflection.TypeExtensions.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Runtime.CompilerServices.Unsafe.dll", "System.Runtime.CompilerServices.Unsafe.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Runtime.Numerics.dll", "System.Runtime.Numerics.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Security.AccessControl.dll", "System.Security.AccessControl.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Security.Cryptography.Primitives.dll", "System.Security.Cryptography.Primitives.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Security.Cryptography.ProtectedData.dll", "System.Security.Cryptography.ProtectedData.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Security.Permissions.dll", "System.Security.Permissions.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Security.Principal.Windows.dll", "System.Security.Principal.Windows.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Text.Encoding.CodePages.dll", "System.Text.Encoding.CodePages.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Text.Encodings.Web.dll", "System.Text.Encodings.Web.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Text.Json.dll", "System.Text.Json.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Text.RegularExpressions.dll", "System.Text.RegularExpressions.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Threading.dll", "System.Threading.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Threading.Tasks.Extensions.dll", "System.Threading.Tasks.Extensions.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Threading.Thread.dll", "System.Threading.Thread.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Threading.ThreadPool.dll", "System.Threading.ThreadPool.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Xml.ReaderWriter.dll", "System.Xml.ReaderWriter.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Xml.XmlDocument.dll", "System.Xml.XmlDocument.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Xml.XPath.dll", "System.Xml.XPath.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.System.Xml.XPath.XmlDocument.dll", "System.Xml.XPath.XmlDocument.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Ubiety.Dns.Core.dll", "Ubiety.Dns.Core.dll");
            //EmbeddedAssembly.Load("KonektoStd.Referencias.Zstandard.Net.dll", "Zstandard.Net.dll");

            //// Verifica se a plataforma é x86 ou x64
            //bool x86 = System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr))
            //           != sizeof(long);
            //var caminhoSQLite = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x86 ? "x86" :
            //    "x64");
            //caminhoSQLite = Path.Combine(caminhoSQLite, "SQLite.Interop.dll");

            //if (!File.Exists(caminhoSQLite))
            //{
            //    // Verifica se existe o caminho
            //    if (!Directory.Exists(Path.GetDirectoryName(caminhoSQLite)))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(caminhoSQLite));
            //    }

            //    // Extraindo DLL do SQLite
            //    if (x86)
            //    {
            //        EmbeddedAssembly.WriteResourceToFile("Konekto.Referencias.x86.SQLite.Interop.dll", caminhoSQLite);
            //    }
            //    else
            //    {
            //        EmbeddedAssembly.WriteResourceToFile("Konekto.Referencias.x64.SQLite.Interop.dll", caminhoSQLite);
            //    }
            //}

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            _DLLsInicializadas = true;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
