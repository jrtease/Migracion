using Aenor.MigracionPublicaciones.Objetos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionPublicaciones.Clases
{
    class LanzadorPublicaciones
    {
        #region Propiedades
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        #endregion Propiedades

        public void Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;




            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de PUBLICACIONES ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();
            }
        }
    }
}
