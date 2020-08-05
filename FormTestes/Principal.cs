using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormTestes
{
    public partial class Principal : Form
    {
        KonektoStd.DBConexao conexao;

        public Principal()
        {
            InitializeComponent();
        }

        private void lnkTestarConexao_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            conexao = new KonektoStd.DBConexao(KonektoStd.TipoBD.SqlServer,
                txtServidor.Text, txtDatabase.Text, txtUsuario.Text, txtSenha.Text);

            if (conexao.ConexaoOk)
            {
                MessageBox.Show("Dados de conexão Ok", "Conexão", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erro ao conectar!" + Environment.NewLine + Environment.NewLine +
                    conexao.UltimoErro, "Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
