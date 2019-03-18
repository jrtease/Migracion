using Aenor.MigracionPublicaciones.Clases;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionPublicaciones
{
    public class Oracle
    {
        #region Propiedades
        public Crm Crm;
        public Comun Comun;

        public const char TipoAccionInsert = 'I', TipoAccionUpdate = 'U',
            TipoAccionDelete = 'D', TipoAccionValidacion = 'V', TipoAccionAssociate = 'A';

        //public const int TipoEntidadComiteTecnico = 1, TipoEntidadICS = 2, TipoEntidadGruposPrecio = 3,
        //TipoEntidadNormasRaiz = 4, TipoEntidadNormasProductos = 5, TipoEntidadNormasVersin = 6;

        public OracleConnection OraConnParaLog;
        #endregion Propiedades





        public Oracle(Crm crm, Comun comun)
        {
            Crm = crm;
            Comun = comun;

            OraConnParaLog = new OracleConnection(Comun.ConnStringOracle);
            OraConnParaLog.Open();
        }

        public void CierraConexionOracle()
        {
            if (this != null && this.OraConnParaLog.State == ConnectionState.Open)
                this.OraConnParaLog.Dispose();
        }

        public void MandarErrorIntegracion(string guidRegistroCRM, string textoError,
            long tipoEntidad, char tipoAccion, long? numError)
        {
                //Cuando se migre esquema Oracle, hay que ejecutar: create sequence sq_integracion start with 1 increment by 1 nocycle cache 2;  
                var insertQuery = @"insert into TCRM_INTEGRACION_ERROR_LOG
                (CLAVEINTEGRACION, DS_ERROR, ID_INTEGRACION, ID_TIPOACCION, ID_TIPOENTIDAD, NU_ERROR) VALUES
                (:CLAVEINTEGRACION, :DS_ERROR, :ID_INTEGRACION, :ID_TIPOACCION, :ID_TIPOENTIDAD, :NU_ERROR)";

                OracleCommand cmd = OraConnParaLog.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select sq_integracion.nextval from dual";
                long idIntegracion = Convert.ToInt64(cmd.ExecuteScalar());

                //cmd = OraConnParaLog.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add("CLAVEINTEGRACION", OracleDbType.Varchar2).Value = guidRegistroCRM;
                cmd.Parameters.Add("DS_ERROR", OracleDbType.Varchar2).Value = textoError;
                //cmd.Parameters.Add("FE_INTEGRACION", OracleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("ID_INTEGRACION", OracleDbType.Long).Value = idIntegracion;
                cmd.Parameters.Add("ID_TIPOACCION", OracleDbType.Char).Value = tipoAccion;
                cmd.Parameters.Add("ID_TIPOENTIDAD", OracleDbType.Long).Value = tipoEntidad;
                cmd.Parameters.Add("NU_ERROR", OracleDbType.Long).Value =
                    numError.HasValue ? numError.Value : (long?)null;
                cmd.ExecuteNonQuery();
        }




        #region Query 
        public const string Query = @"";
        //where rownum<101

        

        #endregion Query
    }
}