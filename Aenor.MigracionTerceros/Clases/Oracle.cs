using Aenor.MigracionTerceros.Clases;
using Aenor.MigracionTerceros.Objetos;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros
{
    public class Oracle
    {
        #region Propiedades
        public Dictionary<string, Tercero> Terceros { get; set; }
        public List<CNAE> ListaCNAEs { get; set; }
        //public string CadenaConOracle { get; set; }

        public Crm Crm;
        public Comun Comun;

        public const char TipoAccionInsert = 'I', TipoAccionUpdate = 'U',
            TipoAccionDelete = 'D', TipoAccionValidacion = 'V';
        
        public const int TipoEntidadTercero = 1, TipoEntidadDireccion = 2, TipoEntidadContacto = 3,
        TipoEntidadCertificacion = 4, TipoEntidadNormaComprada = 5, TipoEntidadPublicacionAdquirida = 6,
        TipoEntidadPotencialCliente = 7, TipoEntidadSuscripcionAdquirida = 8;

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



        #region TERCEROS
        public void LeerTerceros()
        {
            //Dic_Oracle = new Dictionary<string, Tercero>();
            try
            {
                OracleDataAdapter da = new OracleDataAdapter(QueryTerceros, Comun.ConnStringOracle);
                DataTable dt = new DataTable();

                da.Fill(dt);
                //var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                //long totalBytesOfMemoryUsed = currentProcess.WorkingSet64 / 1048576;
                //Comun.LogText("\n\n############################# TOP MEMORIA POST FILL TABLE: " + totalBytesOfMemoryUsed.ToString() + " #############################\n\n");

                Terceros = new Dictionary<string, Tercero>();

                foreach (DataRow fila in dt.Rows)
                {
                    var tercero = TerceroFromOracle(fila);
                    Terceros.Add(tercero.Aen_claveintegracion, tercero);
                }
            }
            catch (Exception e)
            {
                Comun.LogText("Error al leer terceros (Oracle): " + e.ToString());
            }
        }

        private Tercero TerceroFromOracle(DataRow fila)
        {
            var tercero = new Tercero();
            try
            {
                tercero.Accountid = Guid.Empty;
                tercero.Aen_claveintegracion = ((string)fila[NombreCamposTercero.Aen_claveintegracionORACLE]).Trim();
                tercero.Aen_Acronimo = fila[NombreCamposTercero.Aen_AcronimoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_AcronimoORACLE]).Trim();
                tercero.Aen_An8 = fila[NombreCamposTercero.Aen_An8ORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposTercero.Aen_An8ORACLE].ToString().Trim();
                tercero.Aen_Alumno = fila[NombreCamposTercero.Aen_AlumnoORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_AlumnoORACLE] == 1;
                tercero.Aen_Profesor = fila[NombreCamposTercero.Aen_ProfesorORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_ProfesorORACLE] == 1;
                tercero.Aen_Responsable = fila[NombreCamposTercero.Aen_ResponsableORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_ResponsableORACLE] == 1;
                tercero.Aen_Apellidos = fila[NombreCamposTercero.Aen_ApellidosORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_ApellidosORACLE]).Trim();
                tercero.Emailaddress1 = fila[NombreCamposTercero.Emailaddress1ORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Emailaddress1ORACLE]).Trim();
                tercero.ParentaccountidSTR = fila[NombreCamposTercero.ParentaccountidORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposTercero.ParentaccountidORACLE].ToString().Trim();
                tercero.Parentaccountid = fila[NombreCamposTercero.ParentaccountidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindTerceroDiccByClave(tercero.ParentaccountidSTR);
                tercero.Aen_Delegacionid = fila[NombreCamposTercero.Aen_DelegacionidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindDelegacion(((string)fila[NombreCamposTercero.Aen_DelegacionidORACLE]).Trim());
                tercero.Aen_Departamentoid = fila[NombreCamposTercero.Aen_DepartamentoidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindDepartamentoCrm(((string)fila[NombreCamposTercero.Aen_DepartamentoidORACLE]).Trim());
                tercero.Aen_Paisdocumentoid = fila[NombreCamposTercero.Aen_PaisdocumentoidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindPaisCrm(((string)fila[NombreCamposTercero.Aen_PaisdocumentoidORACLE]).Trim());
                tercero.Aen_Escliente = fila[NombreCamposTercero.Aen_EsclienteORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsclienteORACLE] == 1;
                tercero.Aen_Esclientecertool = fila[NombreCamposTercero.Aen_EsclientecertoolORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsclientecertoolORACLE] == 1;
                tercero.Aen_Esclientelaboratorio = fila[NombreCamposTercero.Aen_EsclientelaboratorioORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsclientelaboratorioORACLE] == 1;
                tercero.Aen_Esclienteweberratum = fila[NombreCamposTercero.Aen_EsclienteweberratumORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsclienteweberratumORACLE] == 1;
                tercero.Aen_Escompradordenormas = fila[NombreCamposTercero.Aen_EscompradordenormasORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EscompradordenormasORACLE] == 1;
                tercero.Aen_Esempleado = fila[NombreCamposTercero.Aen_EsempleadoORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsempleadoORACLE] == 1;
                tercero.Aen_Eslibreria = fila[NombreCamposTercero.Aen_EslibreriaORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EslibreriaORACLE] == 1;
                tercero.Aen_Esmiembroctc = fila[NombreCamposTercero.Aen_EsmiembroctcORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsmiembroctcORACLE] == 1;
                tercero.Aen_Esmiembroune = fila[NombreCamposTercero.Aen_EsmiembrouneORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsmiembrouneORACLE] == 1;
                tercero.Aen_Esorganismo = fila[NombreCamposTercero.Aen_EsorganismoORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsorganismoORACLE] == 1;
                tercero.Aen_Esproveedor = fila[NombreCamposTercero.Aen_EsproveedorORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EsproveedorORACLE] == 1;
                tercero.Aen_Essuscriptor = fila[NombreCamposTercero.Aen_EssuscriptorORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_EssuscriptorORACLE] == 1;
                tercero.Aen_Revistaaenor = fila[NombreCamposTercero.Aen_RevistaaenorORACLE] == DBNull.Value ? false : (decimal)fila[NombreCamposTercero.Aen_RevistaaenorORACLE] == 1;
                tercero.Statecode = fila[NombreCamposTercero.StatecodeORACLE] == DBNull.Value ? string.Empty : (string)fila[NombreCamposTercero.StatecodeORACLE];
                tercero.Aen_Bloqueadocliente = fila[NombreCamposTercero.Aen_BloqueadoclienteORACLE] == DBNull.Value ? string.Empty : (fila[NombreCamposTercero.Aen_BloqueadoclienteORACLE]).ToString().Replace(".", "").Trim();
                tercero.Aen_Bloqueadoproveedor = fila[NombreCamposTercero.Aen_BloqueadoproveedorORACLE] == DBNull.Value ? string.Empty : (fila[NombreCamposTercero.Aen_BloqueadoproveedorORACLE]).ToString().Replace(".", "").Trim();
                tercero.Aen_Estadosolicitudclienteerp = fila[NombreCamposTercero.Aen_EstadosolicitudclienteerpORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_EstadosolicitudclienteerpORACLE]).Replace(".", "").Trim();
                tercero.Aen_Estadosolicitudempleadoerp = fila[NombreCamposTercero.Aen_EstadosolicitudempleadoerpORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_EstadosolicitudempleadoerpORACLE]).Replace(".", "").Trim();
                tercero.Aen_Estadosolicitudproveedorerp = fila[NombreCamposTercero.Aen_EstadosolicitudproveedorerpORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_EstadosolicitudproveedorerpORACLE]).Replace(".", "").Trim();
                tercero.Fax = fila[NombreCamposTercero.FaxORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposTercero.FaxORACLE].ToString().Trim();
                tercero.Aen_Fechadealta = fila[NombreCamposTercero.Aen_FechadealtaORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_FechadealtaORACLE]).Trim();
                tercero.Aen_Fechadebaja = fila[NombreCamposTercero.Aen_FechadebajaORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_FechadebajaORACLE]).Trim();
                tercero.Aen_Identificadortercero = fila[NombreCamposTercero.Aen_IdentificadorterceroORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_IdentificadorterceroORACLE]).Trim();
                tercero.Aen_Clienteerpid = fila[NombreCamposTercero.Aen_ClienteerpidORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_ClienteerpidORACLE]).Trim();
                tercero.Aen_Empleadoerpid = fila[NombreCamposTercero.Aen_EmpleadoerpidORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_EmpleadoerpidORACLE]).Trim();
                tercero.Aen_Proveedorerpid = fila[NombreCamposTercero.Aen_ProveedorerpidORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_ProveedorerpidORACLE]).Trim();
                tercero.Aen_Industriaaenor = fila[NombreCamposTercero.Aen_IndustriaaenorORACLE] == DBNull.Value ? Guid.Empty : Crm.FindIndustriaAenorCrm((fila[NombreCamposTercero.Aen_IndustriaaenorORACLE]).ToString().Trim());
                tercero.Revenue = fila[NombreCamposTercero.RevenueORACLE] == DBNull.Value ? decimal.MinValue : Convert.ToDecimal(fila[NombreCamposTercero.RevenueORACLE]);
                tercero.Aen_loginempleado = fila[NombreCamposTercero.Aen_loginempleadoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_loginempleadoORACLE]).Trim();
                tercero.Aen_Nombredelcliente = fila[NombreCamposTercero.Aen_NombredelclienteORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_NombredelclienteORACLE]).Trim();
                tercero.Name = fila[NombreCamposTercero.NameORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.NameORACLE]).Trim();
                tercero.Aen_Numerodocumento = fila[NombreCamposTercero.Aen_NumerodocumentoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_NumerodocumentoORACLE]).Trim();
                tercero.Aen_Observaciones = fila[NombreCamposTercero.Aen_ObservacionesORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_ObservacionesORACLE]).Trim();
                tercero.Aen_Origen = fila[NombreCamposTercero.Aen_OrigenORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_OrigenORACLE]).Replace(".", "").Trim();
                tercero.Aen_Riesgopagoaxesor = fila[NombreCamposTercero.Aen_RiesgopagoaxesorORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_RiesgopagoaxesorORACLE]).Replace(".", "").Trim();
                tercero.Aen_Sectoraenor = fila[NombreCamposTercero.Aen_SectoraenorORACLE] == DBNull.Value ? Guid.Empty : Crm.FindSectorAenorCrm((fila[NombreCamposTercero.Aen_SectoraenorORACLE]).ToString().Trim());
                tercero.Aen_Genero = fila[NombreCamposTercero.Aen_GeneroORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_GeneroORACLE]).Trim();
                tercero.Aen_Siglas = fila[NombreCamposTercero.Aen_SiglasORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_SiglasORACLE]).Trim();
                tercero.Websiteurl = fila[NombreCamposTercero.WebsiteurlORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.WebsiteurlORACLE]).Trim();
                tercero.Aen_Subtipodetercero = fila[NombreCamposTercero.Aen_SubtipodeterceroORACLE] == DBNull.Value ? Guid.Empty : Crm.FindSubtipoTerceroCrm(((string)fila[NombreCamposTercero.Aen_SubtipodeterceroORACLE]).Trim());
                tercero.Telephone1 = fila[NombreCamposTercero.Telephone1ORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Telephone1ORACLE]).Trim();
                tercero.Aen_Tipodocumento = fila[NombreCamposTercero.Aen_TipodocumentoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_TipodocumentoORACLE]).Replace(".", "").Trim();
                tercero.Aen_Tipopersona = fila[NombreCamposTercero.Aen_TipopersonaORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_TipopersonaORACLE]).Replace(".", "").Trim();
                //tercero.Aen_Tipoproveedor = fila[NombreCamposTercero.Aen_TipoproveedorORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_TipoproveedorORACLE]).Replace(".", "").Trim();
                tercero.Aen_Observacionesmigracion = fila[NombreCamposTercero.Aen_ObservacionesmigracionORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_ObservacionesmigracionORACLE]).Trim();
                tercero.Numberofemployees = fila[NombreCamposTercero.NumberofemployeesORACLE] == DBNull.Value ? string.Empty : (fila[NombreCamposTercero.NumberofemployeesORACLE]).ToString().Trim();
                tercero.Transactioncurrencyid = fila[NombreCamposTercero.TransactioncurrencyidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindTransactioncurrencyCrm(((string)fila[NombreCamposTercero.TransactioncurrencyidORACLE]).Trim());
                tercero.TransactioncurrencyidString = fila[NombreCamposTercero.TransactioncurrencyidORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.TransactioncurrencyidORACLE]).Trim();
                tercero.Aen_Webcorporativa = fila[NombreCamposTercero.Aen_WebcorporativaORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_WebcorporativaORACLE]).Trim();
                tercero.Aen_Telefonocorporativo = fila[NombreCamposTercero.Aen_TelefonocorporativoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_TelefonocorporativoORACLE]).Trim();
                tercero.Aen_Correoelectronicocorporativo = fila[NombreCamposTercero.Aen_CorreoelectronicocorporativoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_CorreoelectronicocorporativoORACLE]).Trim();
                tercero.Aen_Condicionesdepagoid = fila[NombreCamposTercero.Aen_CondicionesdepagoidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindCondiciondepago(((string)fila[NombreCamposTercero.Aen_CondicionesdepagoidORACLE]).Trim());
                tercero.Aen_Formasdepagoid = fila[NombreCamposTercero.Aen_FormasdepagoidORACLE] == DBNull.Value ? Guid.Empty : Crm.FindFormadepago(((string)fila[NombreCamposTercero.Aen_FormasdepagoidORACLE]).Trim());
                tercero.Aen_Entradadelcliente = fila[NombreCamposTercero.Aen_EntradadelclienteORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposTercero.Aen_EntradadelclienteORACLE]).Replace(".", "").Trim();
                tercero.Aen_Evaluaciondelaconformidad = fila[NombreCamposTercero.Aen_EvaluaciondelaconformidadORACLE] == DBNull.Value ? false : (short)fila[NombreCamposTercero.Aen_EvaluaciondelaconformidadORACLE] == 1;
                tercero.Aen_Escompradordelibros = fila[NombreCamposTercero.Aen_EscompradordelibrosORACLE] == DBNull.Value ? false : (short)fila[NombreCamposTercero.Aen_EscompradordelibrosORACLE] == 1;
                tercero.Aen_Tipodocumentoempleado = fila[NombreCamposTercero.Aen_TipodocumentoempleadoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_TipodocumentoempleadoORACLE]).Replace(".", "").Trim();
                tercero.Aen_Numerodocumentoempleado = fila[NombreCamposTercero.Aen_NumerodocumentoempleadoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombreCamposTercero.Aen_NumerodocumentoempleadoORACLE]).Trim();
            }
            catch (Exception e)
            {
                Comun.LogText("Error al crear el tercero: " + tercero.Aen_claveintegracion + " ::: " + e.ToString());
            }
            return tercero;
        }
        #endregion TERCEROS


        #region CNAEs
        public void LeerCNAES()
        {
            try
            {
                OracleDataAdapter db = new OracleDataAdapter(QueryCNAES, Comun.ConnStringOracle); // CadenaConOracle);
                DataTable dt2 = new DataTable();

                db.Fill(dt2);
                ListaCNAEs = new List<CNAE>();

                foreach (DataRow fila in dt2.Rows)
                {
                    CNAE cn = new CNAE();
                    cn.aen_claveintegracion = (fila["aen_claveintegracion"] == DBNull.Value) ? string.Empty:fila["aen_claveintegracion"].ToString().Trim();
                    cn.aen_cnaeid = (fila["aen_cnaeid"] == DBNull.Value) ? string.Empty : fila["aen_cnaeid"].ToString().Trim();
                    cn.aen_esprincipal = (fila["aen_cnaeprincipal"] == DBNull.Value) ? string.Empty : fila["aen_cnaeprincipal"].ToString().Trim();

                    ListaCNAEs.Add(cn);
                }
            }
            catch (Exception e)
            {
                Comun.LogText("Error al leer CNAES (Oracle): "+ e.ToString());
            }
        }
        #endregion CNAEs
 

        public void MandarErrorIntegracion(string claveIntegracion, string textoError,
            long tipoEntidad, char tipoAccion, long? numError)
        {
            if (OraConnParaLog.State != ConnectionState.Open)
                OraConnParaLog.Open();

            //Cuando se migre esquema Oracle, hay que ejecutar: create sequence sq_integracion start with 1 increment by 1 nocycle cache 2;  
            var insertQuery = @"insert into YINYANG.TCRM_INTEGRACION_ERROR_LOG
            (CLAVEINTEGRACION, DS_ERROR, ID_INTEGRACION, ID_TIPOACCION, ID_TIPOENTIDAD, NU_ERROR) VALUES
            (:CLAVEINTEGRACION, :DS_ERROR, :ID_INTEGRACION, :ID_TIPOACCION, :ID_TIPOENTIDAD, :NU_ERROR)";
            OracleCommand cmd = OraConnParaLog.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select sq_integracion.nextval from dual";
            long idIntegracion = Convert.ToInt64(cmd.ExecuteScalar());

            //cmd = OraConnParaLog.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("CLAVEINTEGRACION", OracleDbType.Varchar2).Value = claveIntegracion;
            cmd.Parameters.Add("DS_ERROR", OracleDbType.Varchar2).Value = textoError;
            //cmd.Parameters.Add("FE_INTEGRACION", OracleDbType.Date).Value = DateTime.Now;
            cmd.Parameters.Add("ID_INTEGRACION", OracleDbType.Long).Value = idIntegracion;
            cmd.Parameters.Add("ID_TIPOACCION", OracleDbType.Char).Value = tipoAccion;
            cmd.Parameters.Add("ID_TIPOENTIDAD", OracleDbType.Long).Value = tipoEntidad;
            cmd.Parameters.Add("NU_ERROR", OracleDbType.Long).Value =
            numError.HasValue ? numError.Value : (long?)null;
            cmd.ExecuteNonQuery();

            //            INSERT INTO NEORIS.TCRM_INTEGRACION_ERROR_LOG(
            //               CLAVEINTEGRACION, DS_ERROR, FE_INTEGRACION, ID_INTEGRACION,
            //                ID_TIPOACCION, ID_TIPOENTIDAD,
            //               NU_ERROR)
            //VALUES('antoñito',
            //'error haciendo algo',
            //to_date('10/12/2018'),
            //sq_integracion.nextval,
            //'I',
            //1,
            //null)
        }


        #region Query 
        private const string QueryTerceros = @"select
        Aen_Acronimo, Aen_Alumno, Aen_Profesor, 
        Aen_Responsable, Aen_Apellidos,
        Emailaddress1, Parentaccountid, aen_Codigodelegacion, 
        Aen_Departamentoid, Transactioncurrencyid, Aen_Escliente, 
        Aen_Esclientecertool, Aen_Esclientelaboratorio, 
        Aen_Escompradordenormas, Aen_Esempleado, Aen_Eslibreria, 
        Aen_Esmiembroctc, Aen_Esmiembroune, Aen_Esorganismo, 
        Aen_Espotencialcliente, Aen_Esproveedor, 
        Aen_Essuscriptor, Aen_revistaaenor, Statecode, 
        Aen_Estadosolclierp, Aen_Estadosolempleadoerp, Aen_Estadosolproveeerp,
        Aen_Fechadealta, Aen_Fechadebaja, 
        Aen_Identificadortercero, Aen_Claveintegracion, Aen_Clienteerpid, Aen_Empleadoerpid, 
        Aen_Proveedorerpid, Aen_Industriaaenor, Revenue, 
        Aen_loginempleado, Aen_Nombredelcliente,
        Name, Aen_Numerodocumento, Aen_Observaciones, 
        Aen_Origen, Aen_Paisdocumentoid,
        Aen_Riesgopagoaxesor, Aen_Sectoraenor, Aen_Genero, 
        Aen_Siglas, Websiteurl, Aen_Subtipodetercero, 
        Telephone1, Aen_Tipodocumento, Aen_Tipopersona, 
        --Aen_Tipoproveedor,
        Aen_Observacionesmigracion, Numberofemployees,
        Aen_Webcorporativa, Aen_Telefonocorporativo, Aen_emailcorporativo,
        Aen_formadepagoid, Aen_condicionesdepagoid,Aen_Entradadelcliente,
        Aen_Evalconformidad, Aen_Escompradordelibros, 
        Aen_Esclienteweberratum, fax, aen_an8,
        Aen_Bloqueadocliente, Aen_Bloqueadoproveedor,
        Aen_Tipodocumentoempleado, Aen_Numerodocumentoempleado "+
        @"from YINYANG.TCRM_TERCEROS " +
        @"where Statecode in ('Activo','Inactivo') and Aen_Claveintegracion is not null"; 
        //where rownum<101



        private const string QueryCNAES = @"select
        Aen_claveintegracion, Aen_cnaeid,
        Aen_cnaeprincipal " +
        @"from YINYANG.TCRM_TERCEROS_CNAE"; 
        //where rownum<101


        public const string QueryDirecciones = @"SELECT AEN_CLAVEINTEGRACION,
        AEN_COMUNIDADAUTONOMA, AEN_LOCALIDAD, AEN_OBSERVACIONESMIGRACION,
        AEN_DESCRIPCION,
        AEN_CLAVEINTEGRACIONPARENT, AEN_CODIGOPOSTAL, AEN_CLAVEINTEGRACIONCONTACTO,
        AEN_DESCRIPCION, AEN_EMAIL, AEN_FAX,
        AEN_NAME, AEN_NUMERODEVIA, AEN_OBSERVACIONES,
        AEN_ORIGEN, AEN_PAISID,
        AEN_PROVINCIAID, AEN_RAZONSOCIAL, AEN_RESTODIRECCION,
        AEN_TELEFONO1, AEN_TELEFONO2,
        AEN_TIPODEDIRECCION, AEN_TIPODEVIAID, STATECODE,
        AEN_NOMBREVIA , AEN_IDENTIFICADORDIREC "+
        @"FROM YINYANG.TCRM_DIRECCIONES where "+
        @"AEN_CLAVEINTEGRACIONPARENT is not null AND STATECODE in ('Activo','Inactivo')";


        public const string QueryContactos = @"SELECT AEN_CLAVEINTEGRACION, AEN_CLAVEINTEGRACIONPARENT,
        AEN_CARGOPRINCIPALID, AEN_NUMERODOCUMENTO, AEN_TIPODOCUMENTO,
        AEN_TRATAMIENTO, AEN_OBSERVACIONES, AEN_OBSERVACIONESMIGRACION,
        DONOTSENDMM, EMAILADDRESS1, FIRSTNAME, GENDERCODE, LASTNAME,
        MOBILEPHONE, STATECODE, TELEPHONE1, AEN_ORIGEN , AEN_IDENTIFICADORCONTACTO " +
        @"FROM YINYANG.TCRM_CONTACTOS " +
        @"WHERE AEN_CLAVEINTEGRACION IS NOT NULL AND STATECODE IN ('Activo','Inactivo')";

        public const string QueryTercerosES = @"select ft.AEN_CLAVEINTEGRACION, AEN_TERCEROID,
        ft.AEN_EVALCONFORMIDAD, ft.AEN_ESCOMPRADORDENORMAS, ft.AEN_ESSUSCRIPTOR, ft.AEN_ESCOMPRADORDELIBROS,
        ft.AEN_ESPOTENCIALCLIENTE, ft.AEN_ESMIEMBROCTC, ft.AEN_REVISTAAENOR "+
        @"from YINYANG.TCRM_FICHA_TERCEROS ft inner join YINYANG.TCRM_TERCEROS t on ft.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";


        public const string QueryEvaluacionConformidad = @"select e.AEN_CLAVEINTEGRACION,
        AEN_TERCEROID, AEN_CLAVECERTIFICADO, AEN_IDSUBEXPEDIENTE, AEN_CODIGODECERTIFICADO,
        AEN_ESTADO, AEN_FECHADEESTADO, AEN_NORMACERTIFICADACTC, AEN_SUBEXPEDIENTE,
        AEN_SUBNORMASPC "+
        @"from YINYANG.TCRM_FICHA_EVALCONFORMIDAD e inner join YINYANG.TCRM_TERCEROS t on e.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";


        public const string QueryCompradorNormas = @"select e.AEN_CLAVEINTEGRACION, AEN_CLAVENORMA,
        AEN_TERCEROID, AEN_IMPORTE, AEN_CODIGOCTN, AEN_CODIGOARTICULO, AEN_DESCRIPCIONCTN,
        AEN_DESCRIPCIONARTICULO, AEN_TITULO, AEN_CANTIDAD, AEN_IMPORTE "+
        @"from YINYANG.TCRM_FICHA_COMPRADORNORMAS e inner join YINYANG.TCRM_TERCEROS t on e.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";

        public const string QueryCompradorLibros = @"select e.AEN_CLAVEINTEGRACION, AEN_CLAVELIBROS,
        AEN_TERCEROID, AEN_IMPORTE, AEN_CODIGOARTICULO,
        AEN_DESCRIPCIONARTICULO, AEN_TITULO, AEN_CANTIDAD, AEN_IMPORTE "+
        @"from YINYANG.TCRM_FICHA_COMPRADORLIBROS e inner join YINYANG.TCRM_TERCEROS t on e.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";

        public const string QueryClientePotencial = @"select e.AEN_CLAVEINTEGRACION, AEN_CLAVEPOTENCIAL,
        AEN_TERCEROID, e.AEN_FECHAEMISION, e.AEN_ENTRADADELCLIENTE, AEN_CODIGOPEDIDO,
        AEN_EMAIL, AEN_TITULO, e.AEN_OBSERVACIONES " +
        @"from YINYANG.TCRM_FICHA_CLIENTEPOTENCIAL e inner join YINYANG.TCRM_TERCEROS t on e.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";

        public const string QuerySuscriptores = @"select e.AEN_CLAVEINTEGRACION, AEN_CLAVESUSCRIPTOR,
        AEN_TERCEROID, AEN_SITUACION, AEN_PRODUCTO,
        AEN_FECHAFINSUSCRIPCION, AEN_FECHABAJASUSCRIPCION "+
        @"from YINYANG.TCRM_FICHA_SUSCRIPTOR e inner join YINYANG.TCRM_TERCEROS t on e.AEN_CLAVEINTEGRACION = t.AEN_CLAVEINTEGRACION";


        #endregion Query
    }
}