using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KonektoStd
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AtributosBDAttribute : Attribute
    {
        protected string descricao;
        public string Descricao
        {
            get
            {
                return descricao;
            }
            set
            {
                descricao = value;
            }
        }

        protected string nomeCampoBD;
        public string NomeCampoBD
        {
            get
            {
                return nomeCampoBD;
            }
            set
            {
                nomeCampoBD = value;
            }
        }

        protected bool chavePrimaria;
        public bool ChavePrimaria
        {
            get
            {
                return chavePrimaria;
            }
            set
            {
                chavePrimaria = value;
            }
        }
        protected int tamanho;
        public int Tamanho
        {
            get
            {
                return tamanho;
            }
            set
            {
                tamanho = value;
            }
        }
        protected string tipo;
        public string Tipo
        {
            get
            {
                return tipo;
            }
            set
            {
                tipo = value;
            }
        }

        protected string obrigatorio;
        public string Obrigatorio
        {
            get
            {
                return obrigatorio;
            }
            set
            {
                obrigatorio = value;
            }
        }

        protected object valorPadrao;
        public object ValorPadrao
        {
            get
            {
                return valorPadrao;
            }
            set
            {
                valorPadrao = value;
            }
        }

        protected bool pertenceBD;
        public bool PertenceBD
        {
            get { return pertenceBD; }
            set { pertenceBD = value; }
        }

        public AtributosBDAttribute()
        {
            descricao = null;
            obrigatorio = null;
            chavePrimaria = false;
            tamanho = 0;
            valorPadrao = null;
            pertenceBD = true;
        }
    }

}
