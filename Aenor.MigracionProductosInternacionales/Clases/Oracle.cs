using Aenor.MigracionProductosInternacionales.Clases;
using Aenor.MigracionProductosInternacionales.Objetos;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Aenor.MigracionProductosInternacionales
{
    public class Oracle
    {
        #region Propiedades

        public const char TipoAccionInsert = 'I', TipoAccionUpdate = 'U',
            TipoAccionDelete = 'D', TipoAccionValidacion = 'V', TipoAccionAssociate = 'A';
        public const int TipoNormaISO = 10, TipoProductoISO = 11, TipoRelacionISO = 12,
            TipoNormaIEC = 21, TipoProductoIEC = 22,
            TipoNormaASTM = 31, TipoProductoASTM = 32,
            TipoNormaIEEE = 41, TipoProductoIEEE = 42, TipoRelacionIEEE = 43;

        public OracleConnection OraConn;
        public string ConnStringOracle;
        public string SchemaStringOracle;
        public const string SchemaNameSelectOrigen = "NORMASINT";
        public const string SchemaNameSelectDestino = "YINYANG";
        public const string SchemaNameFunction = "YINYANG";

        #endregion Propiedades





        public Oracle(Comun com)
        {
            ConnStringOracle = com.OracleConnString;
            SchemaStringOracle = com.OracleEsquema;
            OraConn = new OracleConnection(ConnStringOracle);
            OraConn.Open();
        }

        public void CierraConexionOracle()
        {
            if (this != null && this.OraConn.State == ConnectionState.Open)
            {
                this.OraConn.Close();
                this.OraConn.Dispose();
            }
        }

        public void MandarErrorIntegracion(string guidClave, string textoError,
            int tipoEntidad, char tipoAccion, long? numError)
        {
                if (OraConn.State != ConnectionState.Open)
                OraConn.Open();

                //Cuando se migre esquema Oracle, hay que ejecutar: create sequence sq_integracion start with 1 increment by 1 nocycle cache 2;  
                var insertQuery = @"insert into TCRM_INTEGRACION_ERROR_LOG
                (CLAVEINTEGRACION, DS_ERROR, ID_INTEGRACION, ID_TIPOACCION, ID_TIPOENTIDAD, NU_ERROR) VALUES
                (:CLAVEINTEGRACION, :DS_ERROR, :ID_INTEGRACION, :ID_TIPOACCION, :ID_TIPOENTIDAD, :NU_ERROR)";

                OracleCommand cmd = OraConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select sq_integracion.nextval from dual";
                long idIntegracion;
                try
                {
                    idIntegracion = Convert.ToInt64(cmd.ExecuteScalar());
                }
                catch (Exception e)
                {   
                }
                finally
                {
                    var querySequence = @"create sequence sq_integracion start with 1 increment by 1 nocycle cache 2";
                    cmd.CommandText = "select sq_integracion.nextval from dual";
                    cmd.ExecuteNonQuery();
                    idIntegracion = Convert.ToInt64(cmd.ExecuteScalar());
                }

                //cmd = OraConnParaLog.CreateCommand();
                
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add("CLAVEINTEGRACION", OracleDbType.Varchar2).Value = guidClave;
                cmd.Parameters.Add("DS_ERROR", OracleDbType.Varchar2).Value = textoError;
                //cmd.Parameters.Add("FE_INTEGRACION", OracleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("ID_INTEGRACION", OracleDbType.Long).Value = idIntegracion;
                cmd.Parameters.Add("ID_TIPOACCION", OracleDbType.Char).Value = tipoAccion;
                cmd.Parameters.Add("ID_TIPOENTIDAD", OracleDbType.Long).Value = tipoEntidad;
                cmd.Parameters.Add("NU_ERROR", OracleDbType.Long).Value =
                    numError.HasValue ? numError.Value : (long?)null;
                cmd.ExecuteNonQuery();
        }

        public bool ExistElementByKey(string elque, string campo, string key)
        {
            bool seencuentra = false;

            var query = string.Format(@"select {0} from {2} where {0} = '{1}'", campo, key, elque);

            OracleDataAdapter oraadap = new OracleDataAdapter();
            OracleCommand orac = new OracleCommand();
            orac.Connection = OraConn;

            if (OraConn.State != ConnectionState.Open)
                OraConn.Open();

            orac.CommandText = query;
            orac.CommandType = CommandType.Text;
            oraadap.SelectCommand = orac;
            DataTable dt = new DataTable();
            oraadap.Fill(dt);

            if (dt.Rows.Count > 0)
                seencuentra = true;

            return seencuentra;
        }

        public DataTable GetElementsFromQuery(string query)
        {
            DataTable resultado = new DataTable();

            OracleDataAdapter da = new OracleDataAdapter();
            OracleCommand command = new OracleCommand();
            command.Connection = OraConn;

            if (OraConn.State != ConnectionState.Open)
                OraConn.Open();

            command.CommandText = query;
            command.CommandType = CommandType.Text;
            da.SelectCommand = command;
            da.Fill(resultado);

            return resultado;
        }


        #region Ejecuta Stored Procedures ORACLE
        public string EjecutarStoredProcedure(string storedProcedure, List<StoreProcedureParam> parametros)
        {
            string res;

            OracleCommand command = new OracleCommand();
            command.Connection = OraConn;
            command.BindByName = true;

            command.CommandText = @storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            #region PARAMETROS
            string outputParam = string.Empty;
            foreach (StoreProcedureParam st in parametros)
            {
                if(st.DirectionParam == ParameterDirection.Output)
                {
                    outputParam = st.NombreParam;
                    command.Parameters.Add(st.NombreParam, st.TipoParam, st.DirectionParam);
                }
                else
                {
                    if (st.TipoParam == OracleDbType.Date)
                    {
                        var okFecha = DateTime.TryParse(st.ValorParam, out DateTime f1);
                        if (okFecha)
                            command.Parameters.Add(st.NombreParam, st.TipoParam, f1, st.DirectionParam);
                        else
                            command.Parameters.Add(st.NombreParam, st.TipoParam, null, st.DirectionParam);
                    }
                    else if (st.TipoParam == OracleDbType.Decimal)
                    {
                        command.Parameters.Add(st.NombreParam, st.TipoParam, Convert.ToDecimal(st.ValorParam), st.DirectionParam);
                    }
                    else //Varchar2
                        command.Parameters.Add(st.NombreParam, st.TipoParam, st.ValorParam, st.DirectionParam);
                }
            }
            #endregion PARAMETROS

            var x = command.ExecuteNonQuery();
            res = outputParam.Equals(string.Empty) ? "-1" : command.Parameters[outputParam].Value.ToString();

            return res;
        }

        #endregion Ejecuta Stored Procedures ORACLE



        #region Query / Nombres Funciones / ... 
        //EJECUCION ORACLE: select <<funcion (parametros)>> from dual;
        public const string FC_INSERTA_NORMAS = SchemaNameFunction + @".PCRM_CARGA_DATOS_NORMASINT.fc_INSERTA_NORMAS";
        public const string FC_INSERTA_PRODUCTOS = SchemaNameFunction + @".PCRM_CARGA_DATOS_NORMASINT.FC_INSERTA_PRODUCTOS";
        public const string FC_ACTUALIZA_NORMAS = SchemaNameFunction + @".PCRM_CARGA_DATOS_NORMASINT.fc_ACTUALIZA_NORMAS";

        public const string TABLA_NORMAS_INTERMEDIA = SchemaNameSelectDestino + ".TCRM_NORMAS";
        public const string TABLA_PRODUCTOS_INTERMEDIA = SchemaNameSelectDestino + ".TCRM_NORMAS_PRODUCTOS";
        public const string TABLA_RELACIONESICS_INTERMEDIA = SchemaNameSelectDestino + ".TCRM_NORMAS_ICS";

        public static string QueryISONormas = string.Format(@"SELECT 
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_articulo,
        NRM.ISO_PROJECT_NUMBER as aen_identificador_nexo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_EDICION_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_fecha_edicion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_ANULACION_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_fecha_anulacion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ESTADO_NORMA('ISO',NRM.ESTADO) as aen_estado,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TITULO_NORMA_EN('ISO',NRM.ISO_PROJECT_NUMBER) AS aen_titulo_norma_en,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_OBTENER_RAIZ_NORMASINT(NRM.CODIGO) as aen_raiz_norma
        FROM {1}.TNI_NORMAS_ISO NRM
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = NRM.ISO_PROJECT_NUMBER
        where VEND.CD_ORGANISMO = 'ISO' AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ISO',NRM.ISO_PROJECT_NUMBER) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryISOProductos = string.Format(@"SELECT
        case DOC.IN_VENDIBLE when '1' then 'S' else 'N' end as aen_vendible_web,
        DOC.DS_IDIOMA as aen_idioma,
        DOC.ISO_PROJECT_NUMBER as aen_identificador_nexo,
        DOC.FS_CREACION as aen_fecha_documento,
        DOC.DS_DOCFORMAT as aen_soporte,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_DOCUMENTO_PRODUCTO('ISO',NRM.ISO_PROJECT_NUMBER,DOC.DS_IDIOMA,DOC.DS_DOCFORMAT) as aen_documento,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_PRODUCTO('ISO',NRM.ISO_PROJECT_NUMBER,DOC.DS_IDIOMA,DOC.DS_DOCFORMAT) as aen_codigo_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_NOMBRE_PRODUCTO('ISO',NRM.ISO_PROJECT_NUMBER,DOC.DS_IDIOMA,DOC.DS_DOCFORMAT) as aen_nombre_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PRECIO_NORMA('ISO',NRM.ISO_PROJECT_NUMBER) as aen_precio,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PATH_NORMA('ISO',DOC.DS_IDIOMA) as aen_path
        FROM {1}.TNI_NORMAS_ISO_DOC DOC 
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = DOC.ISO_PROJECT_NUMBER
        INNER JOIN {1}.TNI_NORMAS_ISO NRM on NRM.ISO_PROJECT_NUMBER = DOC.ISO_PROJECT_NUMBER
        where VEND.CD_ORGANISMO = 'ISO' AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ISO',NRM.ISO_PROJECT_NUMBER) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryISOIcsNormaVersion = string.Format(@"SELECT
        REL.CD_ICS as aen_codigo_ics,
        VEND.CD_ART_VENDIBLE as aen_articulo
        from {1}.TNI_ICS_NORMAS_ISO REL 
        inner join {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = REL.ISO_PROJECT_NUMBER
        where VEND.CD_ORGANISMO = 'ISO' AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ISO',REL.ISO_PROJECT_NUMBER) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryIECNormas = string.Format(@"SELECT 
        NRM.ID as aen_identificador_nexo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('IEC',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_EDICION_NORMA('IEC',NRM.ID) as aen_fecha_edicion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_ANULACION_NORMA('IEC',NRM.ID) as aen_fecha_anulacion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ESTADO_NORMA('IEC',NRM.IN_ESTADO) as aen_estado,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEC',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TITULO_NORMA_EN('IEC',NRM.ID) AS aen_titulo_norma_en,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_OBTENER_RAIZ_NORMASINT({0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEC',NRM.ID)) as aen_raiz_norma
        FROM {1}.TNI_NORMAS_IEC NRM
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = NRM.ID
        where VEND.CD_ORGANISMO = 'IEC' AND NRM.VISIBLE_WEB='S'  AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('IEC',NRM.ID) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryIECProductos = string.Format(@"SELECT
        DOC.DS_LANGUAGE as aen_idioma,
        NRM.ID as aen_identificador_nexo,
        DOC.FE_CIRCULATIONDATE as aen_fecha_documento,
        DOC.DS_COD_FORMATO as aen_soporte,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('IEC',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEC',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_DOCUMENTO_PRODUCTO('IEC',NRM.ID,DOC.DS_LANGUAGE,DOC.DS_COD_FORMATO) as aen_documento,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_PRODUCTO('IEC',NRM.ID,DOC.DS_LANGUAGE,DOC.DS_COD_FORMATO) as aen_codigo_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_NOMBRE_PRODUCTO('IEC',NRM.ID,DOC.DS_LANGUAGE,DOC.DS_COD_FORMATO) as aen_nombre_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PRECIO_NORMA('IEC',NRM.ID) as aen_precio,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PATH_NORMA('IEC',DOC.DS_LANGUAGE) as aen_path   
        FROM {1}.TNI_NORMAS_IEC_DOC DOC 
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = DOC.ID
        INNER JOIN {1}.TNI_NORMAS_IEC NRM on NRM.ID = DOC.ID
        where VEND.CD_ORGANISMO = 'IEC' and NRM.VISIBLE_WEB = 'S' 
        and {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('IEC',NRM.ID) >0", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryIEEENormas = string.Format(@"SELECT 
        NRM.ID as aen_identificador_nexo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('IEEE',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_EDICION_NORMA('IEEE',NRM.ID) as aen_fecha_edicion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_ANULACION_NORMA('IEEE',NRM.ID) as aen_fecha_anulacion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ESTADO_NORMA('IEEE',NRM.IN_ESTADO) as aen_estado,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEEE',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TITULO_NORMA_EN('IEEE',NRM.ID) AS aen_titulo_norma_en,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_OBTENER_RAIZ_NORMASINT({0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEEE',NRM.ID)) as aen_raiz_norma
        FROM {1}.TNI_NORMAS_IEQ NRM
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = NRM.ID
        where VEND.CD_ORGANISMO = 'IEQ'   AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('IEEE',NRM.ID) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryIEEEProductos = string.Format(@"SELECT
        case DOC.IN_VENDIBLE when '1' then 'S' else 'N' end as aen_vendible_web,
        'EN' as aen_idioma,
        NRM.ID as aen_identificador_nexo,
        NRM.FE_PUBLICACION as aen_fecha_documento,
        DOC.DS_TYPE as aen_soporte,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('IEEE',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('IEEE',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_DOCUMENTO_PRODUCTO('IEEE',NRM.ID,'EN',DOC.DS_TYPE) as aen_documento,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_PRODUCTO('IEEE',NRM.ID,'EN',DOC.DS_TYPE) as aen_codigo_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_NOMBRE_PRODUCTO('IEEE',NRM.ID,'EN',DOC.DS_TYPE) as aen_nombre_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PRECIO_NORMA('IEEE',NRM.ID) as aen_precio,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PATH_NORMA('IEEE','EN') as aen_path 
        FROM {1}.TNI_NORMAS_IEQ_DOC DOC 
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = DOC.ID
        INNER JOIN {1}.TNI_NORMAS_IEQ NRM on NRM.ID = DOC.ID
        where VEND.CD_ORGANISMO = 'IEQ' AND DOC.DS_TYPE='PDF' AND
        YINYANG.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('IEEE',NRM.ID) >0", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryIEEEIcsNormasVersion = string.Format(@"SELECT 
        VEND.CD_ART_VENDIBLE as aen_articulo, 
        REL.CD_ICS as aen_codigo_ics
        FROM {0}.TNI_ICS_NORMAS_IEQ REL
        inner join {0}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = REL.ID
        where VEND.CD_ORGANISMO = 'IEQ'
        ", SchemaNameSelectOrigen);

        public static string QueryASTMNormas = string.Format(@"SELECT 
        NRM.ID as aen_identificador_nexo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('ASTM',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_EDICION_NORMA('ASTM',NRM.ID) as aen_fecha_edicion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_FE_ANULACION_NORMA('ASTM',NRM.ID) as aen_fecha_anulacion,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ESTADO_NORMA('ASTM',NRM.DS_TYPE) as aen_estado,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ASTM',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TITULO_NORMA_EN('ASTM',NRM.ID) AS aen_titulo_norma_en,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_OBTENER_RAIZ_NORMASINT({0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ASTM',NRM.ID)) as aen_raiz_norma
        FROM {1}.TNI_NORMAS_ASTM NRM
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = NRM.ID
        where VEND.CD_ORGANISMO = 'ASTM'   AND  
              {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ASTM',NRM.ID) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);

        public static string QueryASTMProductos = string.Format(@"SELECT
        case DOC.IN_VENDIBLE when '1' then 'S' else 'N' end as aen_vendible_web,
        'EN' as aen_idioma,
        NRM.ID as aen_identificador_nexo,
        NRM.FE_APPROVAL as aen_fecha_documento,
        'PDF' as aen_soporte,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('ASTM',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ASTM',NRM.ID) as aen_codigo_norma,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_DOCUMENTO_PRODUCTO('ASTM',NRM.ID,'EN','PDF') as aen_documento,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_PRODUCTO('ASTM',NRM.ID,'EN','PDF') as aen_codigo_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_NOMBRE_PRODUCTO('ASTM',NRM.ID,'EN','PDF') as aen_nombre_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PRECIO_NORMA('ASTM',NRM.ID) as aen_precio,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PATH_NORMA('ASTM','EN') as aen_path
        FROM {1}.TNI_NORMAS_ASTM_DOC DOC
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = DOC.ID
        INNER JOIN {1}.TNI_NORMAS_ASTM NRM on NRM.ID = DOC.ID
        where VEND.CD_ORGANISMO = 'ASTM' AND
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ASTM',NRM.ID) >0
        UNION
        SELECT
        case DOC.IN_VENDIBLE when '1' then 'S' else 'N' end as aen_vendible_web,
        'EN' as aen_idioma,
        NRM.ID as aen_identificador_nexo,
        NRM.FE_APPROVAL as aen_fecha_documento,
        'FIS' as aen_soporte,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_ARTICULO_NORMA('ASTM',NRM.ID) as aen_articulo,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_NORMA('ASTM',NRM.ID) as aen_codigo_norma,
        '' as aen_documento,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_CODIGO_PRODUCTO('ASTM',NRM.ID,'EN','FIS') as aen_codigo_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_NOMBRE_PRODUCTO('ASTM',NRM.ID,'EN','FIS') as aen_nombre_producto,
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_PRECIO_NORMA('ASTM',NRM.ID) as aen_precio,
        '' as aen_path
        FROM {1}.TNI_NORMAS_ASTM_DOC DOC
        INNER JOIN {1}.TNI_ART_VEND VEND on VEND.CD_PROYECTO = DOC.ID
        INNER JOIN {1}.TNI_NORMAS_ASTM NRM on NRM.ID = DOC.ID
        where VEND.CD_ORGANISMO = 'ASTM' AND 
        {0}.PCRM_CARGA_DATOS_NORMASINT.FC_TIENE_DOCUMENTOS('ASTM',NRM.ID) >0
        ", SchemaNameFunction, SchemaNameSelectOrigen);
        #endregion Query / Nombres Funciones / ...
    }
}