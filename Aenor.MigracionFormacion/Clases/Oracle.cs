using Aenor.MigracionFormacion.Clases;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionFormacion
{
    public class Oracle
    {
        #region Propiedades
        public Crm Crm;
        public Comun Comun;

        public const char TipoAccionInsert = 'I', TipoAccionUpdate = 'U',
            TipoAccionDelete = 'D', TipoAccionValidacion = 'V', TipoAccionAssociate = 'A';

        //public const int TipoEntidadComiteTecnico = 1;

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
        }




        #region Query
        public const string QueryModalidades = "select " +
        "cd_modalidad as " + AenModalidad.AenCodigomodalidad +
        ", ds_modalidad as " + AenModalidad.PrimaryName +
        " from formacion.tfo_modalidad where cduecono = 'AEN'";

        public const string QueryAreasDeConocimiento = @"select
        cd_areacon as " + AenAreadeconocimiento.PrimaryName +
        ", in_activo as " + AenAreadeconocimiento.StateCode + 
        " from formacion.tfo_areaconoc where cduecono = 'AEN'";

        public const string QuerySubareasDeConocimiento = @"select
        cd_subareacon as " + AenSubareadeconocimiento.PrimaryName + 
        ", in_activo as " + AenSubareadeconocimiento.StateCode +
        " from formacion.tfo_subareaconoc where cduecono = 'AEN'";

        public const string QueryCursos = @"select cd_curso as aen_codigo, nu_anyo as aen_anio, ds_corto_curso as aen_name, ds_curso as aen_descripcion,
        case cd_modalidad when 'PR' then 1 when 'SM' then 2 when 'INT' then 3 end as aen_modalidad, --en Oracle es solo una, en CRM es multivalor
        f.cd_areacon as aen_codigoareadeconocimiento, --search contra aen_areadeconocimiento por la clave doble
        cd_subareacon as aen_subareadeconocimiento, --search contra aen_subareadeconocimiento por la clave doble
        case cd_tipo when 'CU' then 5 end as aen_tipologia, fe_creacion as overriddencreatedon,
        nu_precio_gral as aen_precio,nu_precio_miembro as aen_precio_miembro,
        nu_min_alumnos as aen_minimo, nu_max_alumnos as aen_maximo, nu_alumesperados as aen_esperado,
        nu_convalpermit as aen_convalidaciones, nu_asistenciamin as aen_asistenciaminima, nu_notamin as aen_notaminima,
        case in_web when 'S' then 'true' else 'false' end as aen_mostrarenweb,
        case in_gratis when 'S' then 'true' else 'false' end as aen_enabierto,
        case in_catalogo when 'S' then 'true' else 'false' end as aen_decatalogo,
        case in_controlasistencia when 'S' then 'true' else 'false' end as aen_controlasistencia,
        ds_objetivo as aen_objetivos, ds_dirigidoa as aen_dirigidoa, ds_contenido as aen_contenido,
        ds_certificado as aen_certificado, ds_colaboraciones as aen_colaboraciones, ds_requisitos as aen_requisitos,
        ds_observaciones as aen_observaciones, ds_duracionestim as aen_duracionyhorariodelcurso,
        nu_dias as aen_dias, nu_horas as aen_numerodehoras,
        case in_obligatorio when 'S' then 'true' else 'false' end as aen_obligatorio,
        ds_profesorado as aen_equipopedagogico,
        case in_examen when 'S' then 'true' else 'false' end as aen_examen,
        ds_docadicional as aen_documentacinaentregaralalu, --campo crm: aen_documentacinaentregaralalumno, oracle solo deja 30 caracteres para alias
        ds_promo as aen_promocionespecial,
        case in_modincompany when 'S' then 'true' else 'false' end as aen_incompany,
        case in_estadoact when '0' then 277220000 when '1' then 277220001 when '2' then 277220002 else null end as aen_estado, --revisar mapeo
        ds_titulacion_propia as aen_titulacionpropiadeaenor,
        cddivisa as transactioncurrencyid, --siempre vale 'EUR', search contra transactioncurrency 
        in_ordenweb as aen_ordendelcurso,
        replace(id_areas_web,'|',',') as aen_areasclasificacionweb,
        replace(id_sectores_web,'|',',') as aen_sectoresclasificacionweb,
        ds_brevecurso as aen_presentacioncursointroducc, --aen_presentacioncursointroduccion
        case cd_modalidad when 'PR' then 'true' else 'false' end as aen_presencial,
        case cd_modalidad when 'SM' then 'true' else 'false' end as aen_semipresencial,
        case cd_modalidad when 'IT' then 'true' else 'false' end as aen_online

        from formacion.tfo_cursos f where cduecono='AEN'
        ";

        #endregion Query
    }
}