using Aenor.MigracionProductos.Clases;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductos
{
    public class Oracle
    {
        #region Propiedades
        public Crm Crm;
        public Comun Comun;

        public const char TipoAccionInsert = 'I', TipoAccionUpdate = 'U',
            TipoAccionDelete = 'D', TipoAccionValidacion = 'V', TipoAccionAssociate = 'A';

        public const int TipoEntidadComiteTecnico = 4, TipoEntidadICS = 5, TipoEntidadGruposPrecio = 6,
        TipoEntidadNormasRaiz = 7, TipoEntidadNormasProductos =8, TipoEntidadNormasVersin = 9;

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
                if (OraConnParaLog.State != ConnectionState.Open)
                    OraConnParaLog.Open();

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
        public const string QueryComitesTecnicos = @"SELECT 
        AEN_COMITE_NUEVO, AEN_FECHA_ACTUALIZACION, AEN_ORGANISMO, 
        AEN_CODIGO_COMITE, AEN_COMITE_PADRE, AEN_NOMBRE_COMITE, 
        AEN_NOMBRE_COMITE_EN
        FROM YINYANG.TCRM_COMITES_TECNICOS";
        //where rownum<101

        public const string QueryICS = @"SELECT 
        AEN_ICS_ACTIVO, AEN_ICS_NUEVO, AEN_FECHA_ACTUALIZACION, 
        AEN_CODIGO_ICS, AEN_CODIGO_ICS_PADRE, AEN_DESCRIPCION_ICS, 
        AEN_DESCRIPCION_ICS_EN
        FROM YINYANG.TCRM_ICS";

        //private const string QueryGruposPrecio = @"SELECT 
        //AEN_GRUPO_NUEVO, AEN_FECHA_ACTUALIZACION, AEN_NU_PAGINAS, 
        //AEN_NU_PRECIO_EUROS, AEN_NU_PRECIO_DIVISA, AEN_GRUPO_PRECIO, 
        //AEN_ORGANISMO_GRUPO
        //FROM YINYANG.TCRM_GRUPOS_PRECIO";

        public const string QueryNormasRaiz = @"select distinct (aen_raiz_norma) 
        from YINYANG.TCRM_NORMAS ";

        public const string QueryNormasAll = @"SELECT 
        AEN_ES_RATIFICADA, AEN_ROYALTY_UNE, AEN_ROYALTY_ORGANISMO, 
           AEN_NORMA_NUEVA, AEN_IDENTIFICADOR_NEXO, AEN_FECHA_EDICION, 
           AEN_FECHA_ANULACION, AEN_FECHA_ACTUALIZACION, AEN_NU_PAGINAS, 
           AEN_GRUPO_PRECIO, AEN_ORGANISMO, AEN_ARTICULO, 
           AEN_ORGANISMO_NORMA, AEN_FORMATO_ESPECIAL, AEN_ORGANISMO_INTERNACIONAL, 
           AEN_ORGANISMO_GRUPO, AEN_ESTADO, AEN_CODIGO_NORMA, 
           AEN_RAIZ_NORMA, AEN_AMBITO_NORMA, AEN_CODIGO_COMITE, 
           AEN_TITULO_NORMA_ES, AEN_TITULO_NORMA_EN, AEN_CLAVEINTEGRACION_ORG,
           AEN_CLAVEINTEGRACION_ORG_N, AEN_CLAVEINTEGRACION_ORG_I, 
           AEN_TIPO
        FROM YINYANG.TCRM_NORMAS";

        public const string QueryNormasProductos = @"SELECT 
        AEN_VENDIBLE_WEB, AEN_PRODUCTO_NUEVO, AEN_DOCUMENTO_MOD, 
           AEN_IDIOMA, AEN_IDENTIFICADOR_NEXO, AEN_FECHA_DOCUMENTO, 
           AEN_FECHA_ACTUALIZACION, AEN_PRECIO, AEN_SOPORTE, 
           AEN_ORGANISMO, AEN_ARTICULO, AEN_CODIGO_NORMA, 
           AEN_DOCUMENTO, AEN_PATH, AEN_URL_ORGANISMO, 
           AEN_CODIGO_PRODUCTO, AEN_NOMBRE_PRODUCTO
        FROM YINYANG.TCRM_NORMAS_PRODUCTOS";

        public const string QueryNormasICS = @"SELECT 
        AEN_ICS_NUEVO, AEN_IDENTIFICADOR_NEXO, AEN_FECHA_ACTUALIZACION, 
           AEN_CODIGO_ICS, AEN_ORGANISMO, AEN_ARTICULO, 
           AEN_CODIGO_NORMA
        FROM YINYANG.TCRM_NORMAS_ICS";

        #endregion Query
    }
}