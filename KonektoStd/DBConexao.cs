using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;
using Npgsql;
using FirebirdSql.Data.Isql;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

namespace KonektoStd
{
    public enum TipoBD
    {
        Firebird,
        SqlServer,
        PostgreSQL,
        MySQL,
        ODBC,
        SQLite,
        Access,
        DBF,
        Indefinido
    }

    public class DBConexao : IDisposable
    {
        #region Variáveis globais
        private IDbConnection dbConn = null;
        private IDbTransaction dbTransaction = null;
        private bool _manterConectado = false;

        // Verifica chamadas redundantes
        private bool _disposed = false;
        #endregion Variáveis globais

        #region Propriedades
        public string UltimoErro { get; private set; }
        public TipoBD TipoBancoDados { get; private set; }
        public bool TransactionAtiva { get => (dbTransaction != null); }
        private bool _conexaoPersistente = true;
        public bool ConexaoPersistente
        {
            get => _conexaoPersistente;
            set
            {
                if (value == _conexaoPersistente)
                    return;

                if (!value && _conexaoPersistente && TransactionAtiva)
                    Commit();

                _conexaoPersistente = value;
            }
        }
        #endregion Propriedades

        #region Construtores
        public DBConexao()
        {
            InicializaVariaveis();
        }

        private void InicializaVariaveis()
        {
            TipoBancoDados = TipoBD.Indefinido;
            UltimoErro = string.Empty;
            dbTransaction = null;
        }

        public DBConexao(string dsnOdbc, string extraConnString = null, string usuario = null, string senha = null)
        {
            InicializaVariaveis();
            dbConn = ConexaoODBC(dsnOdbc, extraConnString, usuario, senha);
        }

        public DBConexao(TipoBD tipoBancoDados, string servidor, string database, string usuario,
            string senha, int porta = 0, string charSet = null, int timeOut = 30, string extraConnString = null)
        {
            InicializaVariaveis();
            TipoBancoDados = tipoBancoDados;
            extraConnString = (extraConnString != null ? ";" + extraConnString : "");

            // Gerando conexão
            switch (tipoBancoDados)
            {
                case TipoBD.Firebird:
                    FbConnectionStringBuilder cs = new FbConnectionStringBuilder()
                    {
                        UserID = usuario,
                        Password = senha,
                        DataSource = servidor.Contains("/") ? servidor.Substring(0, servidor.IndexOf("/")) : servidor,
                        Database = database,
                        Port = (porta > 0 ? porta : (servidor.Contains("/") ?
                            int.Parse(servidor.Substring(servidor.IndexOf("/") + 1)) :
                            3050)),
                        Charset = (charSet ?? "NONE"),
                        ConnectionLifeTime = 0,
                        ConnectionTimeout = timeOut,
                        PacketSize = 8192
                    };
                    dbConn = new FbConnection(cs.ConnectionString + extraConnString);
                    break;

                case TipoBD.SqlServer:
                    SqlConnectionStringBuilder csSql = new SqlConnectionStringBuilder()
                    {
                        UserID = usuario,
                        Password = senha,
                        DataSource = servidor + (porta > 0 ? $",{porta}" : ""),
                        InitialCatalog = database,
                        ConnectTimeout = timeOut
                    };
                    dbConn = new SqlConnection(csSql.ConnectionString + extraConnString);
                    break;

                case TipoBD.MySQL:
                    MySqlConnectionStringBuilder csMy = new MySqlConnectionStringBuilder()
                    {
                        UserID = usuario,
                        Password = senha,
                        Database = database,
                        Server = servidor,
                        ConnectionProtocol = MySqlConnectionProtocol.Tcp,
                        Port = Convert.ToUInt32((porta > 0 ? porta : 3306)),
                        ConnectionTimeout = Convert.ToUInt32(timeOut)
                    };
                    dbConn = new MySqlConnection(csMy.ConnectionString + extraConnString);
                    break;

                case TipoBD.PostgreSQL:
                    NpgsqlConnectionStringBuilder csPgsql = new NpgsqlConnectionStringBuilder()
                    {
                        Host = servidor,
                        Database = database,
                        Username = usuario,
                        Password = senha,
                        Port = (porta > 0 ? porta : 5432)
                    };
                    dbConn = new NpgsqlConnection(csPgsql.ConnectionString);
                    break;

                case TipoBD.ODBC:
                    dbConn = ConexaoODBC(database, extraConnString, usuario, senha);
                    break;

                case TipoBD.DBF:
                case TipoBD.Access:
                    dbConn = ConexaoDBF_Access(database, extraConnString, usuario, senha);
                    break;
            }
        }
        #endregion Construtores

        public bool ConexaoOk
        {
            get
            {
                if (dbConn == null || TipoBancoDados == TipoBD.Indefinido)
                {
                    UltimoErro = "Banco de dados não definido";
                    return false;
                }

                try
                {
                    if (dbConn.State != ConnectionState.Open)
                        dbConn.Open();

                    if (!_conexaoPersistente && !_manterConectado && !TransactionAtiva)
                        dbConn.Close();

                    _manterConectado = false;
                }
                catch (Exception ex)
                {
                    UltimoErro = ex.Message;
                    return false;
                }

                return true;
            }
        }

        private bool Conectado()
        {
            _manterConectado = true;
            return ConexaoOk;
        }

        private IDbConnection ConexaoODBC(string dsnOdbc, string extraConnString, string usuario, string senha)
        {
            TipoBancoDados = TipoBD.ODBC;
            return new OdbcConnection(($"Dsn={dsnOdbc}"
                                         + (!string.IsNullOrWhiteSpace(usuario) ? $";Uid={usuario}" : "")
                                         + (!string.IsNullOrWhiteSpace(senha) ? $";Pwd={senha}" : "")
                                         + (!string.IsNullOrWhiteSpace(extraConnString) ?
                                            $";{extraConnString}" : "")).Replace(";;", ""));
        }

        private IDbConnection ConexaoDBF_Access(string caminhoDBFs, string extraConnString = null,
            string usuario = null, string senha = null)
        {
            var conn = new OdbcConnection();

            try
            {
                var connString = string.Empty;

                if (TipoBancoDados == TipoBD.DBF)
                {
                    connString = "Driver={Microsoft dBASE Driver (*.dbf)};"
                                 + "Driverid=277;";
                }
                else
                {
                    connString = "Driver={Microsoft Access Driver (*.mdb)};"
                                 + (!string.IsNullOrEmpty(usuario) ? $";Uid={usuario}" : "")
                                 + (!string.IsNullOrEmpty(senha) ? $";Uid={senha}" : "");
                }

                conn.ConnectionString = connString.Replace(";;", "") +
                    $"Dbq={caminhoDBFs}" + (extraConnString ?? "");
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return null;
            }

            return (dbConn = conn);
        }

        public IDataReader ExecutarDataReader(string instrucaoSQL, Dictionary<string, object> parametros = null)
        {
            if (!Conectado())
                return null;

            try
            {
                using (IDbCommand command = GetCommand(instrucaoSQL, parametros))
                {
                    if (command == null)
                        return null;

                    return command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return null;
            }
        }

        public DataTable ExecutarConsulta(string instrucaoSQL, Dictionary<string, object> parametros = null)
        {
            if (!Conectado())
                return null;

            try
            {
                DataSet ds = new DataSet();

                using (IDbCommand command = GetCommand(instrucaoSQL, parametros))
                {
                    if (command == null)
                        return null;

                    IDbDataAdapter adapter = null;

                    switch (TipoBancoDados)
                    {
                        case TipoBD.Firebird:
                            adapter = new FbDataAdapter((FbCommand)command);
                            break;

                        case TipoBD.SqlServer:
                            adapter = new SqlDataAdapter((SqlCommand)command);
                            break;

                        case TipoBD.MySQL:
                            adapter = new MySqlDataAdapter((MySqlCommand)command);
                            break;

                        case TipoBD.PostgreSQL:
                            adapter = new NpgsqlDataAdapter((NpgsqlCommand)command);
                            break;

                        case TipoBD.SQLite:
                            adapter = new SQLiteDataAdapter((SQLiteCommand)command);
                            break;

                        case TipoBD.DBF:
                        case TipoBD.ODBC:
                        case TipoBD.Access:
                            adapter = new OdbcDataAdapter((OdbcCommand)command);
                            break;
                    }

                    adapter.Fill(ds);
                }

                return (ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable());
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return null;
            }
            finally
            {
                if (!_conexaoPersistente)
                    Desconectar();
            }
        }

        public List<T> ExecutarConsulta<T>(string instrucaoSQL, Dictionary<string, object> parametros = null)
        {
            var dt = ExecutarConsulta(instrucaoSQL, parametros);

            try
            {
                if (dt != null)
                {
                    return dt.ToListof<T>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return null;
            }
        }

        public int ExecutarInstrucao(string instrucaoSQL, Dictionary<string, object> parametros = null)
        {
            int result = 0;

            if (!Conectado())
                return -1;

            try
            {
                // Quando for Firebird e for batch de execução
                if (TipoBancoDados == TipoBD.Firebird && parametros == null)
                {
                    FbScript fbScript = new FbScript(instrucaoSQL);

                    if (fbScript.Parse() > 1)
                    {
                        var transacaoAtiva = TransactionAtiva;
                        var erros = false;

                        if (!transacaoAtiva)
                            IniciarTransaction();

                        foreach(var cmd in fbScript.Results)
                        {
                            int iFetch = ExecutarInstrucao(cmd.Text);
                            if (!erros)
                                erros = iFetch < 0;

                            result += iFetch;
                        }

                        if (erros)
                        {
                            result = -1;

                            Rollback();
                        }
                        else
                        {
                            if (!transacaoAtiva)
                                Commit();
                        }

                        return result;
                    }
                }

                using (IDbCommand command = GetCommand(instrucaoSQL, parametros))
                {
                    if (command == null)
                        return -1;

                    result = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                result = -1;
            }
            finally
            {
                if (!_conexaoPersistente)
                    Desconectar();
            }

            return result;
        }


        public bool IniciarTransaction()
        {
            try
            {
                dbTransaction = dbConn.BeginTransaction();
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return false;
            }

            return true;
        }

        public bool Commit()
        {
            try
            {
                if (ConexaoOk && TransactionAtiva)
                {
                    dbTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return false;
            }

            return true;
        }

        public bool Rollback()
        {
            try
            {
                if (ConexaoOk && TransactionAtiva)
                {
                    dbTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return false;
            }

            return true;
        }

        public IDbCommand GetCommand(string instrucaoSQL, Dictionary<string, object> parametros = null,
            CommandType commandType = CommandType.Text)
        {
            if (!Conectado())
                return null;

            IDbCommand command = dbConn.CreateCommand();
            command.CommandText = instrucaoSQL;
            command.CommandType = commandType;

            if (TransactionAtiva)
                command.Transaction = dbTransaction;

            if (parametros != null)
            {
                foreach (var parametro in parametros)
                {
                    switch (TipoBancoDados)
                    {
                        case TipoBD.Firebird:
                            // Verifica se o parâmetro é identificado como Blob de texto
                            if (parametro.Key.Length >= 2 && parametro.Key.Substring(1, 1).Equals("_"))
                            {
                                (command as FbCommand).Parameters.Add(parametro.Key, FbDbType.Text);
                                (command as FbCommand)
                                    .Parameters[(command as FbCommand).Parameters.Count - 1]
                                    .Value = Convert.ToString(parametro.Value);
                            }
                            else
                            {
                                (command as FbCommand).Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                            break;

                        case TipoBD.SqlServer:
                            (command as SqlCommand).Parameters.AddWithValue(parametro.Key, parametro.Value);
                            break;

                        case TipoBD.PostgreSQL:
                            (command as NpgsqlCommand).Parameters.AddWithValue(parametro.Key, parametro.Value);
                            break;

                        case TipoBD.MySQL:
                            (command as MySqlCommand).Parameters.AddWithValue(parametro.Key, parametro.Value);
                            break;

                        case TipoBD.SQLite:
                            (command as SqliteCommand).Parameters.AddWithValue(parametro.Key, parametro.Value);
                            break;

                        case TipoBD.ODBC:
                        case TipoBD.DBF:
                        case TipoBD.Access:
                            (command as OdbcCommand).Parameters
                                .AddWithValue(parametro.Key, parametro.Value);
                            break;
                    }
                }
            }

            return command;
        }

        public void Desconectar()
        {
            if (dbConn != null && !dbConn.State.Equals(ConnectionState.Closed))
            {
                if (TransactionAtiva)
                    Commit();

                dbConn.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Desconectar();
            }

            _disposed = true;
        }
    }
}
