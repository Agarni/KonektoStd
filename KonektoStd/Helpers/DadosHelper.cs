using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KonektoStd
{
    public static class DadosHelper
    {
        #region Variáveis globais
        public static string UltimoErro { get; private set; }
        #endregion Variáveis globais

        #region Conversões de tipos
        public static int? ToIntNull(this object valor)
        {
            try
            {
                if (valor == null || valor == DBNull.Value)
                    return null;

                if (int.TryParse(Convert.ToString(valor), out int result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string ToStringNull(this object valor)
        {
            try
            {
                if (valor == null || valor == DBNull.Value)
                    return null;

                return Convert.ToString(valor);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ToDateTimeNull(this object valor)
        {
            try
            {
                if (valor == null || valor == DBNull.Value)
                    return null;

                if (DateTime.TryParse(Convert.ToString(valor), out DateTime result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion Conversões de tipos

        #region Criação de instruções SQL
        public static string CriarSQL<T>(T tabela, string nomeTabela = null, string condicao = "",
            string[] ignorarPropriedades = null, TipoInstrucaoSQL tipoInstrucaoSQL = TipoInstrucaoSQL.Insert,
            TipoBD tipoBancoDados = TipoBD.SqlServer)
        {
            string retorno = "";
            StringBuilder sbCampos = new StringBuilder();
            StringBuilder sbValores = new StringBuilder();

            PropertyInfo[] propriedades = tabela.GetType().GetProperties();
            var propIgnoradasPadrao = new List<string>() { "ReadOnly", "NomeTabelaBD", "IncluirConversaoSPCM" };
            if (ignorarPropriedades == null)
            {
                ignorarPropriedades = propIgnoradasPadrao.ToArray();
            }
            else
            {
                List<string> lstIgnoraProps = ignorarPropriedades.ToList();
                lstIgnoraProps.AddRange(propIgnoradasPadrao);
                ignorarPropriedades = lstIgnoraProps.ToArray();
            }

            nomeTabela = nomeTabela ?? tabela.GetType().Name;

            foreach (var item in propriedades)
            {
                // Verificando propriedades ignorados pelo Data Annotations
                var prop = propriedades.ToList().Find(x => x.Name.ToUpper() == item.Name.ToUpper());
                var atributos = prop.GetCustomAttributes(true);
                var pertenceBD = true;
                var tamanhoCampo = 0;
                string atributoCampoBD = null;

                if (atributos != null && atributos.Count() > 0)
                {
                    foreach (var itemAtributo in atributos)
                    {
                        if (itemAtributo is AtributosBDAttribute)
                        {
                            var atributo = (itemAtributo as AtributosBDAttribute);
                            tamanhoCampo = atributo.Tamanho;
                            atributoCampoBD = atributo.NomeCampoBD;
                            pertenceBD = atributo.PertenceBD;
                        }
                    }
                }

                if (pertenceBD && item.CanWrite)
                {
                    if (!ignorarPropriedades.Contains(item.Name))
                    {
                        var nomeCampo = item.Name.ToString().Trim();
                        var atributoNome = (atributoCampoBD ?? nomeCampo);
                        string valorCampo = "null";

                        // Cria SQL dos valores
                        if (tipoInstrucaoSQL != TipoInstrucaoSQL.Select)
                        {
                            var valor = tabela.GetType().GetProperty(item.Name).GetValue(tabela, null);
                            valorCampo = "NULL";

                            if (valor != null)
                            {
                                valorCampo = tabela.ValorCampoSQL(nomeCampo);

                                if (!valorCampo.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (valor is bool)
                                    {
                                        valorCampo = (valorCampo.ToUpper() == "TRUE" ? "1" : "0");
                                    }
                                    else if (valor is decimal || valor is double)
                                    {
                                        valorCampo = Convert.ToString(valor,
                                            new System.Globalization.CultureInfo("en-US")).Replace(",", "");
                                    }
                                }
                            }

                            if (tipoInstrucaoSQL != TipoInstrucaoSQL.Update
                                && (!valorCampo.Equals("null", StringComparison.OrdinalIgnoreCase)
                                && valorCampo.Trim() != "" && valorCampo != "''"))
                            {
                                if (sbValores.Length > 0)
                                    sbValores.AppendLine(",");

                                sbValores.Append(valorCampo);
                            }
                        }

                        // Quando insere registro não há necessidade de informar os campos vazios
                        if (tipoInstrucaoSQL == TipoInstrucaoSQL.Select ||
                            tipoInstrucaoSQL == TipoInstrucaoSQL.Update ||
                            (!valorCampo.Equals("null", StringComparison.OrdinalIgnoreCase)
                            && valorCampo.Trim() != "" && valorCampo != "''"))
                        {
                            if (tipoInstrucaoSQL != TipoInstrucaoSQL.Update)
                            {
                                if (sbCampos.Length > 0)
                                    sbCampos.AppendLine(",");

                                sbCampos.Append(((tipoInstrucaoSQL == TipoInstrucaoSQL.Select
                                    && atributoNome != nomeCampo) ? $"{atributoNome} as {nomeCampo}" : atributoNome));
                            }
                            else
                            {
                                if (sbValores.Length > 0)
                                    sbValores.AppendLine(",");

                                sbValores.Append($"{atributoNome ?? nomeCampo} = {valorCampo}");
                            }
                        }
                    }
                }
            }

            switch (tipoInstrucaoSQL)
            {
                case TipoInstrucaoSQL.Select:
                    retorno = $"select {sbCampos} from {nomeTabela}";
                    break;

                case TipoInstrucaoSQL.Insert:
                    retorno = (tipoBancoDados == TipoBD.SqlServer ? "set dateformat ymd\r\n " : string.Empty) +
                        $"insert into {nomeTabela} ({sbCampos}) " +
                        $"values ({sbValores}) ";
                    break;

                case TipoInstrucaoSQL.Update:
                    retorno = (tipoBancoDados == TipoBD.SqlServer ? "set dateformat ymd\r\n " : string.Empty) +
                        $"update {nomeTabela} set {sbValores}";
                    break;
            }

            return retorno + (!string.IsNullOrWhiteSpace(condicao) && 
                tipoInstrucaoSQL != TipoInstrucaoSQL.Insert ? " where " + condicao : "") + ";";
        }

        public static string ValorCampoSQL<T>(this T tabela, string Campo)
        {
            PropertyInfo[] propriedades = tabela.GetType().GetProperties();
            var prop = propriedades.ToList().Find(x => x.Name.ToUpper() == Campo.ToUpper());
            var retorno = "";
            var tipoCampoOriginal = prop.PropertyType.ToString().ToUpper();
            var tipoCampo = tipoCampoOriginal;
            var tipoBase = prop.PropertyType.BaseType.Name.ToUpper();
            var valor = tabela.GetType().GetProperty(Campo).GetValue(tabela, null);
            var atributos = prop.GetCustomAttributes(true);
            var tamanhoCampo = 0;
            // Atualizando último SQL utilizado
            UltimoErro = "";

            if ((valor == null) ||
                (valor.ToString() == "" && !tipoCampo.ToUpper().Contains("STRING")))
            {
                retorno = "NULL";
            }
            else
            {
                var converteTipo = false;

                if (atributos != null && atributos.Count() > 0)
                {
                    foreach (var item in atributos)
                    {
                        if (item is AtributosBDAttribute)
                        {
                            var atributo = (item as AtributosBDAttribute);
                            tipoCampo = atributo.Tipo ?? tipoCampo;
                            converteTipo = (atributo.Tipo != null);
                            tamanhoCampo = atributo.Tamanho;
                        }
                    }
                }

                var verificarTamanhoCampo = false;
                var ehString = true;

                // Verifica se o tipo de campo é o nativo do .Net e adapta para BD
                if (tipoCampo.ToUpper().Contains("INT") || tipoBase.ToUpper().Contains("ENUM"))
                    tipoCampo = "INT";

                if (tipoCampo.ToUpper().Contains("STRING"))
                {
                    retorno = string.Format("'{0}'", valor.ToString().Replace("'", "''"));

                    verificarTamanhoCampo = true;
                }
                else if (tipoCampo.ToUpper().Contains("DATE"))
                {
                    DateTime dataCampo = (DateTime)valor;
                    retorno = string.Format("'{0}'", dataCampo.ToString(tipoCampo.ToUpper()
                        .Contains("DATETIME") ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd"));

                    if (retorno.Contains("000"))
                        retorno = "NULL";
                }
                else if (tipoCampo == "INT")
                {
                    retorno = Convert.ToString(tipoBase.ToUpper().Contains("ENUM") ? (int)valor : valor);
                    ehString = false;
                }
                else if (tipoCampo.ToUpper().Contains("BYTE"))
                {
                    retorno = $"'{Encoding.Default.GetString((byte[])valor)}'";
                    verificarTamanhoCampo = true;
                }
                else
                {
                    retorno = $"'{Convert.ToString(valor)}'";
                    verificarTamanhoCampo = true;

                    if (converteTipo)
                    {
                        if (tipoCampoOriginal.ToUpper().Contains("BYTE"))
                        {
                            retorno = $"Convert({tipoCampo}, " +
                                $"'{Encoding.Default.GetString((byte[])valor)}')";
                        }
                        else
                        {
                            retorno = $"Convert({tipoCampo}, {retorno})";
                        }
                        verificarTamanhoCampo = false;
                    }
                }

                // Verifica se possui limitação de tamanho de campo
                if (verificarTamanhoCampo && tamanhoCampo > 0)
                {
                    retorno = retorno.Substring((ehString ? 1 : 0),
                        Math.Min(tamanhoCampo, retorno.Length - (ehString ? 2 : 0)));
                }
            }

            return retorno;
        }
        #endregion Criação de instruções SQL

        public static T ToObject<T>(this DataRow dataRow) where T : new()
        {
            T item = new T();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

                if (property != null && dataRow[column] != DBNull.Value 
                    && dataRow[column].ToString() != "NULL")
                {
                    property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                }
            }

            return item;
        }

        private static PropertyInfo GetProperty(Type type, string attributeName)
        {
            PropertyInfo property = type.GetProperty(attributeName);

            if (property != null)
            {
                return property;
            }

            return type.GetProperties()
                 .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }
            else
            {
                if (type.IsEnum)
                    return Enum.Parse(type, value.ToString());
            }

            return Convert.ChangeType(value, type);
        }

        public enum TipoInstrucaoSQL
        {
            Insert,
            Update,
            Select
        }

        public static List<T> ToListof<T>(this DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var objectProperties = typeof(T).GetProperties(flags);

            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();
                var lstAtributos = objectProperties
                        .Where(p => Attribute.IsDefined(p, typeof(AtributosBDAttribute)) &&
                        !string.IsNullOrWhiteSpace(p.GetCustomAttribute<AtributosBDAttribute>().NomeCampoBD));

                foreach (var properties in objectProperties.Where(properties =>
                    (columnNames.Any(s => s.Equals(properties.Name, StringComparison.OrdinalIgnoreCase)) ||
                    (lstAtributos != null &&
                    lstAtributos.Any(p => p.Name.Equals(properties.Name, StringComparison.OrdinalIgnoreCase)))
                    )))
                {
                    try
                    {
                        // Verifica se o nome foi encontrado através do atributo de campo
                        var nomePorAtributo = lstAtributos
                            .FirstOrDefault(p => p.Name
                            .Equals(properties.Name, StringComparison.OrdinalIgnoreCase))?
                            .GetCustomAttribute<AtributosBDAttribute>()?.NomeCampoBD;
                        var nomeCampo = nomePorAtributo ?? properties.Name;

                        if (dataRow[nomeCampo] != DBNull.Value)
                            instanceOfT.AtribuiValor(dataRow[nomeCampo], properties);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{properties.Name} => {ex.Message}", ex.InnerException);
                    }
                }

                return instanceOfT;
            });

            return targetList.ToList();
        }

        public static void AtribuiValor<T>(this T objeto, object valor, PropertyInfo propriedade)
        {
            var tipoCampo = propriedade.PropertyType.ToString().ToUpper();

            if (valor == null && tipoCampo.Contains("NULL"))
            {
                propriedade.SetValue(objeto, null, null);
            }
            else if (tipoCampo.Contains("BOOL"))
            {
                propriedade.SetValue(objeto, Convert.ToBoolean(valor), null);
            }
            else if (tipoCampo.Contains("INT") || tipoCampo.Contains("LONG"))
            {
                if (tipoCampo.Contains("16"))
                {
                    propriedade.SetValue(objeto, Convert.ToInt16(valor), null);
                }
                else if (tipoCampo.Contains("32"))
                {
                    propriedade.SetValue(objeto, Convert.ToInt32(valor), null);
                }
                else
                {
                    propriedade.SetValue(objeto, Convert.ToInt64(valor), null);
                }
            }
            else if (tipoCampo.Contains("DECIMAL"))
            {
                propriedade.SetValue(objeto, Convert.ToDecimal(valor), null);
            }
            else if (tipoCampo.Contains("FLOAT"))
            {
                propriedade.SetValue(objeto, Convert.ToDouble(valor), null);
            }
            else if (tipoCampo.Contains("STRING"))
            {
                propriedade.SetValue(objeto, Convert.ToString(valor), null);
            }
            else
            {
                propriedade.SetValue(objeto, valor, null);
            }
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static IList<T> EnumToList<T>()
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T não é enumerador");

            IList<T> list = new List<T>();
            Type type = typeof(T);
            if (type != null)
            {
                Array enumValues = Enum.GetValues(type);
                foreach (T value in enumValues)
                {
                    list.Add(value);
                }
            }

            return list;
        }
    }
}
