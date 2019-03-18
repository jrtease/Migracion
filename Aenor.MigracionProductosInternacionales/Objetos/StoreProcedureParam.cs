using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductosInternacionales.Objetos
{
    public class StoreProcedureParam
    {
        public string NombreParam { get; set; }
        public string ValorParam { get; set; }
        public ParameterDirection DirectionParam { get; set; }
        public OracleDbType TipoParam { get; set; }



        public StoreProcedureParam(string name, string valor, ParameterDirection direc, OracleDbType tipo)
        {
            this.NombreParam = name;
            this.ValorParam = valor;
            this.DirectionParam = direc;
            this.TipoParam = tipo;
        }
    }
}
