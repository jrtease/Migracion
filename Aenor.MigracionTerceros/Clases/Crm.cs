using Aenor.MigracionTerceros.Objetos;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Pfe.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Clases
{
    public class Crm
    {
        #region Declarar Globales
        private Dictionary<string, Guid> MaestroDepartamentos;
        private Dictionary<string, Guid> MaestroSubtipoTerceros;
        private Dictionary<string, Guid> MaestroSectorAENOR;
        private Dictionary<string, Guid> MaestroIndustriaAENOR;
        private Dictionary<string, Guid> MaestroDelegacion;
        private Dictionary<string, Guid> MaestroCNAEs;
        private Dictionary<string, Guid> MaestroPais;
        public static Dictionary<string, Guid> MaestroTransactionCurrency;
        private Dictionary<string, Guid> MaestroCondicionesdepago;
        private Dictionary<string, Guid> MaestroFormasdepago;

        public static IOrganizationService IOS;

        public List<string> TercerosADesactivar { get; set; }  //String = aen_claveintegracion
        private Dictionary<string, Guid> ClaveGuid_Tercero { get; set; } //Diccionario auxiliar de pares claveintegracion / Accountid
        public static List<KeyValuePair<string, string>> TercerosAEmparentar { get; set; }  //aen_claveintegracion (hijo), aen_claveintegracion (padre)

        public Dictionary<string, Tercero> TercerosCRM { get; set; }
        private static string[] CamposTercerosCRM = {NombreCamposTercero.Aen_claveintegracionCRM, NombreCamposTercero.Aen_AcronimoCRM,
            NombreCamposTercero.Aen_An8CRM,
            NombreCamposTercero.Aen_AlumnoCRM, NombreCamposTercero.Aen_ProfesorCRM, 
            NombreCamposTercero.Aen_ResponsableCRM, NombreCamposTercero.Aen_ApellidosCRM,
            NombreCamposTercero.Emailaddress1CRM, NombreCamposTercero.ParentaccountidCRM,
            NombreCamposTercero.Aen_DepartamentoidCRM, NombreCamposTercero.Aen_claveintegracionCRM,
            NombreCamposTercero.Aen_EsclienteCRM, NombreCamposTercero.Aen_EsclientecertoolCRM,
            NombreCamposTercero.Aen_EsclientelaboratorioCRM, 
            NombreCamposTercero.Aen_EscompradordenormasCRM, NombreCamposTercero.Aen_EsempleadoCRM,
            NombreCamposTercero.Aen_EslibreriaCRM, NombreCamposTercero.Aen_EsmiembroctcCRM,
            NombreCamposTercero.Aen_EsmiembrouneCRM, NombreCamposTercero.Aen_EsorganismoCRM,
            NombreCamposTercero.Aen_EspotencialclienteCRM, NombreCamposTercero.Aen_EsclienteweberratumCRM,
            NombreCamposTercero.Aen_EsproveedorCRM, NombreCamposTercero.Aen_EssuscriptorCRM,
            NombreCamposTercero.Aen_RevistaaenorCRM,
            NombreCamposTercero.Aen_BloqueadoclienteCRM, NombreCamposTercero.Aen_BloqueadoproveedorCRM,
            NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM, NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM,
            NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM,NombreCamposTercero.Aen_FechadealtaCRM,
            NombreCamposTercero.Aen_FechadebajaCRM, NombreCamposTercero.Aen_IdentificadorterceroCRM,
            NombreCamposTercero.Aen_ClienteerpidCRM, NombreCamposTercero.Aen_EmpleadoerpidCRM,
            NombreCamposTercero.Aen_ProveedorerpidCRM, NombreCamposTercero.Aen_IndustriaaenorCRM,
            NombreCamposTercero.RevenueCRM, NombreCamposTercero.FaxCRM,
            NombreCamposTercero.Aen_loginempleadoCRM, NombreCamposTercero.Aen_NombredelclienteCRM,
            NombreCamposTercero.NameCRM, NombreCamposTercero.Aen_NumerodocumentoCRM,
            NombreCamposTercero.Aen_ObservacionesCRM, NombreCamposTercero.Aen_OrigenCRM,
            NombreCamposTercero.Aen_RiesgopagoaxesorCRM,
            NombreCamposTercero.Aen_SectoraenorCRM, NombreCamposTercero.Aen_GeneroCRM,
            NombreCamposTercero.Aen_SiglasCRM, NombreCamposTercero.WebsiteurlCRM,
            NombreCamposTercero.Aen_SubtipodeterceroCRM, NombreCamposTercero.Telephone1CRM,
            NombreCamposTercero.Aen_TipodocumentoCRM, NombreCamposTercero.Aen_TipopersonaCRM,
            //NombreCamposTercero.Aen_TipoproveedorCRM, 
            NombreCamposTercero.Aen_ObservacionesmigracionCRM,
            NombreCamposTercero.Aen_PaisdocumentoidCRM, NombreCamposTercero.Aen_DelegacionidCRM,
            NombreCamposTercero.NumberofemployeesCRM, NombreCamposTercero.StatecodeCRM, NombreCamposTercero.StatuscodeCRM,
            NombreCamposTercero.TransactioncurrencyidCRM, NombreCamposTercero.Aen_WebcorporativaCRM,
            NombreCamposTercero.Aen_TelefonocorporativoCRM, NombreCamposTercero.Aen_CorreoelectronicocorporativoCRM,
            NombreCamposTercero.Aen_CondicionesdepagoidCRM, NombreCamposTercero.Aen_FormasdepagoidCRM,
            NombreCamposTercero.Aen_EntradadelclienteCRM, NombreCamposTercero.Aen_EvaluaciondelaconformidadCRM,
            NombreCamposTercero.Aen_EscompradordelibrosCRM, NombreCamposTercero.Aen_TipodocumentoempleadoCRM,
            NombreCamposTercero.Aen_NumerodocumentoempleadoCRM};

        //Diccionario de CNAES existentes en CRM al inicio <claveintegracion_tercero,
        //  lista de <CNAEs que tiene asignado, true-false si se marca o no al recorrerlo. 
        //  A posteriori los 'false' se eliminarán, ya que significa que no vienen de oracle (no existen en el tercero)>>
        public Dictionary<Guid, List<KeyValuePair<Guid, bool>>> CnaesTerceroCRM { get; set; }

        private  ExecuteMultipleRequest Emr;

        //26 nov, props para el uso de PFE
        public OrganizationServiceManager OSManager;
        public OrganizationServiceProxyOptions OSOptions;
        public IDictionary<string, ExecuteMultipleRequest> PFERequests;
        public int RequestKey;
        //Contadores Globales para errores
        public int Iguales = 0;
        private int Eliminados = 0, Actualizados = 0, Creados = 0, Otros = 0, Errores = 0;

        private Comun Comun;
        private Oracle Oracle { get; set; }

        #endregion Declarar Globales




        public Crm(Comun comun)
        {
            Comun = comun;

            //Conexion a CRM
            CrmServiceClient conn = new CrmServiceClient(Comun.ConnStringCrm);
            IOS = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
        }

        public void CachearInfoCRM()
        {
            ClaveGuid_Tercero = new Dictionary<string, Guid>();
            TercerosADesactivar = new List<string>();
            TercerosAEmparentar = new List<KeyValuePair<string, string>>();

            #region Carga Diccionarios maestros
                CargarMaestroSubtipoTercero();
                CargarMaestroDepartamentos();
                CargarMaestroSectorAENOR();
                CargarMaestroIndustriaAENOR();
                CargarMaestroDelegacion();
                CargarMaestroCNAE();
                CargarMaestroPais();
                CargarMaestroCondicionesdepago();
                CargarMaestroFormasdepago();
                CargarMaestroTransactioncurrency();
            #endregion Carga Diccionarios maestros

            CargarTercerosFromCRM();
            CargarCNAESFromCRM();
        }


        private void CargarTercerosFromCRM()
        {
            TercerosCRM = new Dictionary<string, Tercero>();

            //1.Obtener lista de Terceros CRM (+5mil)
            #region Preparar Query
            string entityName = "account";

            FilterExpression filter = new FilterExpression();
            //ConditionExpression conditionActives = new ConditionExpression();
            //conditionActives.AttributeName = "statecode";
            //conditionActives.Operator = ConditionOperator.Equal;
            //conditionActives.Values.Add(0);
            ConditionExpression conditionPKey = new ConditionExpression();
            conditionPKey.AttributeName = "aen_claveintegracion";
            conditionPKey.Operator = ConditionOperator.NotNull;
            //filter.Conditions.Add(conditionActives);
            filter.Conditions.Add(conditionPKey);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(CamposTercerosCRM);
            query.Criteria.AddFilter(filter);
            #endregion Preparar Query

            #region PagingCookie
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            #endregion PagingCookie

            while (true)
            {
                EntityCollection terColeccion = IOS.RetrieveMultiple(query);

                if (terColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity terceroCRM in terColeccion.Entities)
                    {
                        Tercero ter = new Tercero();
                        ter.TerceroFromCRM(terceroCRM);

                        TercerosCRM.Add(ter.Aen_claveintegracion, ter);
                    }
                }

                if (terColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = terColeccion.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        private void CargarCNAESFromCRM()
        {
            CnaesTerceroCRM = new Dictionary<Guid, List<KeyValuePair<Guid, bool>>>();

            //1.Obtener lista de Cnaes CRM (+5mil)
            #region Preparar Query
            string entityName = "aen_cnaetercero";

            FilterExpression filter = new FilterExpression();
            //ConditionExpression conditionActives = new ConditionExpression();
            //conditionActives.AttributeName = "statecode";
            //conditionActives.Operator = ConditionOperator.Equal;
            //conditionActives.Values.Add(0);

            ConditionExpression conditionPKey1 = new ConditionExpression();
            conditionPKey1.AttributeName = "aen_terceroid";
            conditionPKey1.Operator = ConditionOperator.NotNull;

            ConditionExpression conditionPKey2 = new ConditionExpression();
            conditionPKey2.AttributeName = "aen_cnaeid";
            conditionPKey2.Operator = ConditionOperator.NotNull;

            //filter.Conditions.Add(conditionActives);
            filter.Conditions.Add(conditionPKey1);
            filter.Conditions.Add(conditionPKey2);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(new string[]{"aen_terceroid", "aen_cnaeid", "aen_esprincipal"});
            query.Criteria.AddFilter(filter);
            query.AddOrder("aen_terceroid",OrderType.Ascending);
            #endregion Preparar Query

            #region PagingCookie
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            #endregion PagingCookie

            bool cambio_tercero = false;
            List<KeyValuePair<Guid, bool>> aux_cnaes = new List<KeyValuePair<Guid, bool>>();

            while (true)
            {
                EntityCollection cnaeColeccion = IOS.RetrieveMultiple(query);

                if (cnaeColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista de CNAES (del tercero) CRM, el diccionario
                    for (int i=0; i< cnaeColeccion.Entities.Count; i++)
                    {
                        Entity cne = cnaeColeccion.Entities[i];
                        Guid guidTercero = cne.GetAttributeValue<EntityReference>("aen_terceroid").Id;
                        aux_cnaes.Add(new KeyValuePair<Guid, bool>(cne.GetAttributeValue<EntityReference>("aen_cnaeid").Id, false));

                        //Si es el último elemento o el siguiente no es un CNAE del mismo tercero
                        if ((i + 1) == cnaeColeccion.Entities.Count
                            || (!guidTercero.Equals(cnaeColeccion.Entities[i + 1].GetAttributeValue<EntityReference>("aen_terceroid").Id)))
                            cambio_tercero = true;

                        //Añadimos la lista de sus terceros al diccionario
                        if (cambio_tercero)
                        {
                            List<KeyValuePair<Guid, bool>> kvp = new List<KeyValuePair<Guid, bool>>(aux_cnaes);
                            CnaesTerceroCRM.Add(guidTercero, kvp);
                            cambio_tercero = false;
                            aux_cnaes.Clear();
                        }
                    }
                }

                if (cnaeColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = cnaeColeccion.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        //public void EliminaOldCNAEs()
        //{
        //    var objetivosAEliminar = CnaesTerceroCRM.Where(elem => elem.Value.Exists( elemList => (elemList.Value == false) ))
        //        .ToDictionary(ky => ky.Key, ky => ky.Value);

        //    Guid terceGuid = Guid.Empty, elimCnaeGuid = Guid.Empty;


        //    foreach (var elim in objetivosAEliminar)
        //    {
        //        terceGuid = elim.Key;
        //        foreach (var elimCnae in elim.Value)
        //        {
        //            if(elimCnae.Value == false)
        //            {
        //                elimCnaeGuid = elimCnae.Key;
        //                //Eliminar el Cnae de tercero con tercero = terceGuid y cnae = elimCnaeGuid
        //                #region Obtener el ID CnaeTercero para eliminarlo
        //                QueryExpression queryDel = new QueryExpression("aen_cnaetercero");

        //                FilterExpression filter = new FilterExpression();
        //                ConditionExpression condTer = new ConditionExpression();
        //                condTer.AttributeName = "aen_terceroid";
        //                condTer.Operator = ConditionOperator.Equal;
        //                condTer.Values.Add(terceGuid);
        //                ConditionExpression condCn = new ConditionExpression();
        //                condCn.AttributeName = "aen_cnaeid";
        //                condCn.Operator = ConditionOperator.Equal;
        //                condCn.Values.Add(elimCnaeGuid);
        //                filter.Conditions.Add(condTer);
        //                filter.Conditions.Add(condCn);

        //                queryDel.ColumnSet.AddColumns(new string[] { "aen_cnaeterceroid" });
        //                queryDel.Criteria.AddFilter(filter);
        //                EntityCollection cnaeAEliminar= IOS.RetrieveMultiple(queryDel);
        //                #endregion 

        //                if(cnaeAEliminar!=null && cnaeAEliminar.Entities.Count>0)
        //                    AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_cnaetercero",cnaeAEliminar.Entities[0].Id) });
        //            }
        //        }
        //    }
        //    ProcesarUltimosEmr();
            
        //}



        public void CargaDiccionarioGuidsTercero()
        {
            ClaveGuid_Tercero.Clear();

            //1.Obtener lista de Terceros CRM (+5mil)
            #region Preparar Query
            string entityName = "account";

            FilterExpression filter = new FilterExpression();
            //ConditionExpression conditionActives = new ConditionExpression();
            //conditionActives.AttributeName = "statecode";
            //conditionActives.Operator = ConditionOperator.Equal;
            //conditionActives.Values.Add(0);
            ConditionExpression conditionPKey = new ConditionExpression();
            conditionPKey.AttributeName = "aen_claveintegracion";
            conditionPKey.Operator = ConditionOperator.NotNull;
            //filter.Conditions.Add(conditionActives);
            filter.Conditions.Add(conditionPKey);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns("aen_claveintegracion");
            query.Criteria.AddFilter(filter);
            #endregion Preparar Query

            #region PagingCookie
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            #endregion PagingCookie

            while (true)
            {
                EntityCollection terColeccion = IOS.RetrieveMultiple(query);

                if (terColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity terceroCRM in terColeccion.Entities)
                    {
                        ClaveGuid_Tercero.Add(terceroCRM.GetAttributeValue<string>("aen_claveintegracion"), terceroCRM.Id);
                    }
                }

                if (terColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = terColeccion.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public IOrganizationService GetIOS()
        {
            return IOS;
        }




        #region Funciones Carga Maestros
        private void CargarMaestroDepartamentos()
        {
            List<Entity> list_aux = new List<Entity>();
            string[] campos = { "aen_departamentoid", "aen_name", "aen_codigodeldepartamento" };
            string entityName = "aen_departamento";

            #region Prepara Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionCodNotNull = new ConditionExpression();
            conditionCodNotNull.AttributeName = "aen_codigodeldepartamento";
            conditionCodNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionCodNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);

            #region PagingCookie
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            #endregion PagingCookie

            while (true)
            {
                EntityCollection collections =IOS.RetrieveMultiple(query);

                if (collections.Entities.Count > 0)
                {
                    list_aux.AddRange(collections.Entities);
                }

                if (collections.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = collections.PagingCookie;
                }
                else
                {
                    break;
                }
            }
            #endregion Prepara Query

            MaestroDepartamentos = new Dictionary<string, Guid>();
            string cod;

            foreach (Entity dep in list_aux)
            {
                cod = dep.GetAttributeValue<string>("aen_codigodeldepartamento").Trim().ToUpper();
                MaestroDepartamentos.Add(cod, dep.Id);
            }
        }
        
        private void CargarMaestroSubtipoTercero()
        {
            string[] campos = { "aen_subtipodeterceroid", "aen_name" };
            string entityName = "aen_subtipodetercero";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_name";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroSubtipoTerceros = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_name").Trim().ToUpper();
                MaestroSubtipoTerceros.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroSectorAENOR()
        {
            string[] campos = { "aen_name", "aen_codigo" };
            string entityName = "aen_sectoraenor";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigo";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroSectorAENOR = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigo").Trim().ToUpper();
                MaestroSectorAENOR.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroIndustriaAENOR()
        {
            string[] campos = { "aen_name", "aen_codigo" };
            string entityName = "aen_industriaaenor";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigo";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroIndustriaAENOR = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigo").Trim().ToUpper();
                MaestroIndustriaAENOR.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroDelegacion()
        {
            string[] campos = { "aen_codigodelegacion" };
            string entityName = "aen_delegacion";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigodelegacion";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroDelegacion = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigodelegacion").Trim().ToUpper();
                MaestroDelegacion.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroCNAE()
        {
            string[] campos = { "aen_cnaeid", "aen_codigocnae", "aen_codigonexo" };
            string entityName = "aen_cnae";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigonexo";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroCNAEs = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigonexo").Trim().ToUpper();
                MaestroCNAEs.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroPais()
        {
            string[] campos = { "aen_name", "aen_codigopais" };
            string entityName = "aen_pais";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigopais";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroPais = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigopais").Trim().ToUpper();
                MaestroPais.Add(cod, sub.Id);
            }
        }
        
        private void CargarMaestroFormasdepago()
        {
            string[] campos = { "aen_codigo" };
            string entityName = "aen_formadepago";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigo";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroFormasdepago = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigo").Trim().ToUpper();
                MaestroFormasdepago.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroCondicionesdepago()
        {
            string[] campos = { "aen_codigo" };
            string entityName = "aen_condicionesdepago";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "aen_codigo";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroCondicionesdepago = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigo").Trim().ToUpper();
                MaestroCondicionesdepago.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroTransactioncurrency()
        {
            string[] campos = { "transactioncurrencyid", "isocurrencycode" };
            string entityName = "transactioncurrency";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = "isocurrencycode";
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            MaestroTransactionCurrency = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("isocurrencycode").Trim().ToUpper();
                MaestroTransactionCurrency.Add(cod, sub.Id);
            }
        }

        private void CargarMaestroParametrizado(string entidad, string[] campos, string campoEquivalencia)
        {
            string entityName = entidad;

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNameNotNull = new ConditionExpression();
            conditionNameNotNull.AttributeName = campoEquivalencia;
            conditionNameNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNameNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = IOS.RetrieveMultiple(query);
            //MaestroProvincia = new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>(campoEquivalencia).Trim().ToUpper();
                //MaestroProvincia.Add(cod, sub.Id);
            }
        }

        #endregion Funciones Carga Maestros



        #region Operaciones Búsqueda en CRM para completar campos Guid/Revenue
        public Guid FindDelegacion(string v)
        {
            Guid res = Guid.Empty;

            MaestroDelegacion.TryGetValue(v.ToUpper(), out res);

            return res;
        }


        public Guid FindTransactioncurrencyCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroTransactionCurrency.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindSubtipoTerceroCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroSubtipoTerceros.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindSectorAenorCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroSectorAENOR.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindPaisCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroPais.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindIndustriaAenorCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroIndustriaAENOR.TryGetValue(v.ToUpper(), out res);

            return res;
        }


        public Guid FindDepartamentoCrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroDepartamentos.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindTerceroDiccByClave(string claveintegracion)
        {
            Guid g = Guid.Empty;

            ClaveGuid_Tercero.TryGetValue(claveintegracion, out g);

            return g;
        }


        public Guid FindTerceroCrmByClave(string claveintegracion)
        {
            Guid g = Guid.Empty;

            #region Preparar Query
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AddColumn("accountid");
            FilterExpression filter = new FilterExpression();
            ConditionExpression cond = new ConditionExpression("aen_claveintegracion", ConditionOperator.Equal, claveintegracion);
            filter.AddCondition(cond);
            query.Criteria.AddFilter(filter);

            EntityCollection col = IOS.RetrieveMultiple(query);
            #endregion Preparar Query

            if (col.Entities != null & col.Entities.Count > 0)
                g = col.Entities[0].Id;

            return g;
        }

        public Guid FindCNAECrm(string v)
        {
            Guid res = Guid.Empty;

            MaestroCNAEs.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindCondiciondepago(string v)
        {
            Guid res = Guid.Empty;

            MaestroCondicionesdepago.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        public Guid FindFormadepago(string v)
        {
            Guid res = Guid.Empty;

            MaestroFormasdepago.TryGetValue(v.ToUpper(), out res);

            return res;
        }

        #endregion Operaciones Búsqueda en CRM para completar campos Guid




        #region ACTUALIZACIONES POST-CARGA TERCEROS
        //Realizar asignaciones de jerarquia de terceros (parentaccountid) a posteriori de ser creados/actualizados.
        public void EmparentaTerceros()
        {
            List<KeyValuePair<Guid, Guid>> a_emparentar = new List<KeyValuePair<Guid, Guid>>();
            Guid valor_hijo = Guid.Empty;
            Guid valor_padre = Guid.Empty;
            bool ok1, ok2;
            
            //Obtener guids
            foreach (KeyValuePair<string, string> par in TercerosAEmparentar)
            {
                ok1 = ClaveGuid_Tercero.TryGetValue(par.Key, out valor_hijo);
                ok2 = ClaveGuid_Tercero.TryGetValue(par.Value, out valor_padre);

                if (ok1 && ok2)
                    a_emparentar.Add(new KeyValuePair<Guid, Guid>(valor_hijo, valor_padre));
            }

            //Hacer Updates en PFCore
            foreach (KeyValuePair<Guid,Guid> terg in a_emparentar)
            {
                if(terg.Key != Guid.Empty && terg.Value != Guid.Empty && terg.Key != terg.Value)
                { 
                    Entity terUpd = new Entity("account");
                    terUpd.Id = terg.Key;
                    terUpd[NombreCamposTercero.ParentaccountidCRM] = new EntityReference("account", terg.Value);

                    AnadirElementoEmr(new UpdateRequest { Target = terUpd});
                }
            }
            ProcesarUltimosEmr();
        }


        //Cambia de estado(desactiva) los terceros que vienen inactivos, a posteriori de ser creados/actualizados.
        public void DesactivarTerceros()
        {
            List<Guid> a_desactivar = new List<Guid>();
            Guid valor_guid = Guid.Empty;
            bool ok;

            //Buscar guids
            foreach (string str in TercerosADesactivar)
            {
                ok = ClaveGuid_Tercero.TryGetValue(str, out valor_guid);

                if (ok)
                    a_desactivar.Add(valor_guid);
            }

            //Realizar desactivaciones
            UpdateRequest desactivar;
            Entity auxUpdate;

            foreach (Guid g in a_desactivar)
            {
                desactivar = new UpdateRequest();
                auxUpdate = new Entity("account");
                auxUpdate.Id = g;
                auxUpdate["statecode"] = new OptionSetValue(1);
                auxUpdate["statuscode"] = new OptionSetValue(2);
                desactivar.Target = auxUpdate;
                AnadirElementoEmr(desactivar);
            }
            ProcesarUltimosEmr();
        }


        //Carga de CNAEs en la tabla de 'aen_cnaetercero'
        public void CargaCNAEs(List<CNAE> listaCNAEs)
        {
            // Tercero (aen_claveintegracion) >----------< aen_cnaetercero >----------< CNAE (aen_cnaeid -> aen_codigocnae) 
            bool ok1, ok2, okcnae;
            List<KeyValuePair<Guid, bool>> cenaeAux = new List<KeyValuePair<Guid, bool>>();

            foreach (CNAE cn in listaCNAEs)
            {
                Entity auxCnaeTercero = new Entity("aen_cnaetercero");
                Guid terID = Guid.Empty;
                Guid cenae = Guid.Empty;

                ok1 = ClaveGuid_Tercero.TryGetValue(cn.aen_claveintegracion, out terID);
                ok2 = MaestroCNAEs.TryGetValue(cn.aen_cnaeid, out cenae);

                if (ok1 && ok2)
                {
                    //Comprobamos que este cnae no exista para ese tercero ya, para no crearlo
                    okcnae = CnaesTerceroCRM.TryGetValue(terID, out cenaeAux);
                    if(okcnae)
                    { 
                        int indx = cenaeAux.FindIndex(e => e.Key.Equals(cenae));
                        if (indx != -1)
                        {
                            //CnaesTerceroCRM[terID].ElementAt(indx).Value=true;
                            CnaesTerceroCRM[terID].RemoveAt(indx);
                            CnaesTerceroCRM[terID].Add(new KeyValuePair<Guid, bool>(cenae, true));
                            Iguales++;
                            continue;
                        }
                    }
                    auxCnaeTercero["aen_terceroid"] = new EntityReference("account", terID);
                    auxCnaeTercero["aen_cnaeid"] = new EntityReference("aen_cnae", cenae);
                    auxCnaeTercero["aen_esprincipal"] = (cn.aen_esprincipal.Equals(string.Empty)) ? false : (cn.aen_esprincipal.Equals("1"));
                    //Para saltar plugins de envío de datos a NEXO
                    auxCnaeTercero["aen_vienedeintegracion"] = true;

                    AnadirElementoEmr(new CreateRequest { Target = auxCnaeTercero });
                }
            }

            ProcesarUltimosEmr();

        }
        #endregion ACTUALIZACIONES POST-CARGA TERCEROS


        public void LiberaDiccionarios()
        {
            MaestroDepartamentos = null; //.Clear();
            MaestroSubtipoTerceros = null; //.Clear();
            MaestroSectorAENOR = null; //.Clear();
            MaestroIndustriaAENOR = null; //.Clear();
            MaestroDelegacion = null; //.Clear();
            MaestroCNAEs = null; //.Clear();
            MaestroPais = null; //.Clear();
            MaestroCondicionesdepago = null; //.Clear();
            MaestroFormasdepago = null; //.Clear();
            MaestroTransactionCurrency = null; //.Clear();

            TercerosADesactivar = null; //.Clear();
            TercerosCRM = null; //.Clear();
            ClaveGuid_Tercero = null; //.Clear();
            TercerosAEmparentar = null; //.Clear();

            CnaesTerceroCRM = null; //.Clear();
        }

        public void MostrarEstadisticas(string msj)
        {
            Comun.LogText("ESTADISTICAS "+msj+": \n #CREADAS: " + Creados + "\n\t #ACTUALIZADAS: " + Actualizados + "\n\t #ELIMINADAS: " + Eliminados + "\n\t #IGUALES: " + Iguales + "\n\t #OTROS: " + Otros);
            Creados = Actualizados = Eliminados = Iguales = Otros = 0;
        }


        #region PFE Core
        public void InicializarPFE(Oracle ora)
        {
            Oracle = ora;
            Emr = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            var url = XrmServiceUriFactory.CreateOrganizationServiceUri(Comun.ConnStringCrm.Split(';')[0].Split('=')[1]);
            var usr = Comun.ConnStringCrm.Split(';')[1].Split('=')[1];
            var pwd = Comun.ConnStringCrm.Split(';')[2].Split('=')[1];
            OSManager = new OrganizationServiceManager(url, usr, pwd);
            OSOptions = new OrganizationServiceProxyOptions
            {
                Timeout = new TimeSpan(0, Comun.TimeOutEnMinutos, 0)
            };
            PFERequests = new Dictionary<string, ExecuteMultipleRequest>();
            RequestKey = 0;
        }

        public void AnadirElementoEmr(OrganizationRequest elemento)
        {
            Emr.Requests.Add(elemento);
            if (Emr.Requests.Count > 0 && Emr.Requests.Count % Comun.Buffer == 0)
            {
                RequestKey++; //Es solo para tener un key o índice de cada "bloque" de ejecución, ya que se devuelve en el response 
                var emr = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings() { ContinueOnError = true, ReturnResponses = true },
                    Requests = new OrganizationRequestCollection()
                };
                foreach (var r in Emr.Requests)
                    emr.Requests.Add(r);

                PFERequests.Add(new KeyValuePair<string, ExecuteMultipleRequest>(RequestKey.ToString(), emr));
                Emr.Requests.Clear();

                if (PFERequests.Count == Comun.GradoDeParalelismo)
                {
                    Comun.LogText("Mandamos ejecutar " + PFERequests.Count);
                    EjecutarPFE();
                    Comun.LogText("Regresa");
                }
            }
        }

        public void ProcesarUltimosEmr()
        {
            if (PFERequests.Count > 0 || Emr.Requests.Count > 0)
            {
                RequestKey++; //Es solo para tener un key o índice de cada "bloque" de ejecución, ya que se devuelve en el response 
                var emr = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings() { ContinueOnError = true, ReturnResponses = true },
                    Requests = new OrganizationRequestCollection()
                };
                foreach (var r in Emr.Requests)
                    emr.Requests.Add(r);
                PFERequests.Add(new KeyValuePair<string, ExecuteMultipleRequest>(RequestKey.ToString(), emr));
                Comun.LogText("Mandamos ejecutar últimas " + PFERequests.Count);
                EjecutarPFE();
                Comun.LogText("Regresa últimas");
            }
        }


        public bool EjecutarPFE()
        {
            IDictionary<string, ExecuteMultipleResponse> responses = null;
            try
            {
                responses =
                    OSManager.ParallelProxy.Execute<ExecuteMultipleRequest, ExecuteMultipleResponse>(PFERequests, OSOptions);
            }
            catch (TimeoutException extime)
            {
                Comun.LogText("TIMEOUT EXCEPTION (extime): " + extime.Message);
                Comun.LogText("----> INNER EXCEPTION: " + extime.InnerException.Message);
                //Limpia Buffers
                Emr.Requests.Clear();
                PFERequests.Clear();
            }
            catch (SqlException exsql)
            {
                switch (exsql.Number)
                {
                    case 11:
                        Comun.LogText("SQLTIMEOUT EXCEPTION (exsql): ((11))" + exsql.Message);
                        Comun.LogText("----> INNER EXCEPTION: " + exsql.InnerException.Message);
                        break;

                    case -2:
                        Comun.LogText("SQLTIMEOUT EXCEPTION (exsql): ((-2))" + exsql.Message);
                        Comun.LogText("----> INNER EXCEPTION: " + exsql.InnerException.Message);
                        break;

                    default:
                        Comun.LogText("SQLTIMEOUT EXCEPTION (exsql): ((default))" + exsql.Message);
                        Comun.LogText("----> INNER EXCEPTION: " + exsql.InnerException.Message);
                        break;
                }
                //Limpia Buffers
                Emr.Requests.Clear();
                PFERequests.Clear();
            }
            catch (Exception ex)
            {
                Comun.LogText("EXCEPTION (ex): " + ex.Message);
                Comun.LogText("----> INNER EXCEPTION: " + ex.InnerException.Message);
                //Limpia Buffers
                Emr.Requests.Clear();
                PFERequests.Clear();
            }


            #region ANALISIS RESPUESTAS
            int j = 0, ko = 0;
            if (responses == null)
                Comun.LogText("Respuestas NULO");
            else
            { 
                Comun.LogText(string.Format("Respuestas obtenidas: " + responses.Count));
                for (var k = 0; k < responses.Count; k++)
                {
                    Comun.LogText(string.Format(
                        "{0} respuestas and {1} errores para ExecuteMultipleRequest con key={2}",
                        responses.ElementAt(k).Value.Responses.Count(r => r.Response != null),
                        responses.ElementAt(k).Value.Responses.Count(r => r.Fault != null),
                        responses.ElementAt(k).Key));
                    int numErrores = responses.ElementAt(k).Value.Responses.Count(r => r.Fault != null);
                    for (int i = 0; i < responses.ElementAt(k).Value.Responses.Count(); i++)
                    {
                        subeContadores(responses, k, i);

                        if (responses.ElementAt(k).Value.Responses[i].Fault != null)
                        {
                            if (!string.IsNullOrEmpty(responses.ElementAt(k).Value.Responses[i].Fault.Message))
                                Comun.LogText("Error en emr, fault message: " + responses.ElementAt(k).Value.Responses[i].Fault.Message); // + ", codigo = " + ids[i]);

                            Errores++;
                            ko++;

                            if (i < PFERequests.Values.ElementAt(k).Requests.Count)
                            { 
                            var orgReq = PFERequests.Values.ElementAt(k).Requests[i];
                            MandarALogOracle(orgReq,
                                responses.ElementAt(k).Value.Responses[i].Fault.Message,
                                responses.ElementAt(k).Value.Responses[i].Fault.ErrorCode);
                
                            Guid faultID = PFERequests[responses.ElementAt(k).Key].Requests[i].RequestId.GetValueOrDefault(Guid.Empty);
                            if (faultID != Guid.Empty) //si es empty es insert
                                Comun.LogText("Id de entidad que provocó error: " + faultID);

                            var textoError = "Se ha producido error de ejecución, consultar logs texto y CRM";
                            if (!string.IsNullOrEmpty(responses.ElementAt(k).Value.Responses[i].Fault.Message))
                                textoError = "Error ejecución emr:" + responses.ElementAt(k).Value.Responses[i].Fault.Message;
                            }
                            //throw new Exception(textoError)
                        }
                    }
                    j++;

                }
            }
            #endregion ANALISIS RESPUESTAS

            //Vacía Buffers
            Emr.Requests.Clear();
            PFERequests.Clear();
            return ko == 0;
        }

        //Contadores de datos
        private void subeContadores(IDictionary<string, ExecuteMultipleResponse> responses, int k, int i)
        {
            if(responses.Values.ElementAt(k).Responses[i].Response != null)
            { 
                switch (responses.Values.ElementAt(k).Responses[i].Response.ResponseName)
                {
                    case "Update":
                        Actualizados++;
                        break;
                    case "Create":
                        Creados++;
                        break;
                    case "Delete":
                        Eliminados++;
                        break;
                    default:
                        Otros++;
                        break;
                }
            }
        }


        private void MandarALogOracle(OrganizationRequest orgReq, string textoError, int numError)
        {
            var claveIntegracion = "";
            var tipoEntidad = -1;
            char tipoAccion = ' ';
            if (orgReq.RequestName == "Create")
            {
                claveIntegracion = (string)((CreateRequest)orgReq).Target.Attributes["aen_claveintegracion"];
                tipoAccion = Oracle.TipoAccionInsert;
                tipoEntidad = MapearNombreEntidad(((CreateRequest)orgReq).Target.LogicalName);
            }
            else if (orgReq.RequestName == "Update")
            {
                claveIntegracion = (string)((UpdateRequest)orgReq).Target.Attributes["aen_claveintegracion"];
                tipoAccion = Oracle.TipoAccionUpdate;
                tipoEntidad = MapearNombreEntidad(((UpdateRequest)orgReq).Target.LogicalName);
            }
            else if (orgReq.RequestName == "Delete")
            {
                claveIntegracion = ((DeleteRequest)orgReq).Target.Id.ToString(); //en delete el target es entiyreference, no tenemos la clave int
                tipoAccion = Oracle.TipoAccionDelete;
                tipoEntidad = MapearNombreEntidad(((DeleteRequest)orgReq).Target.LogicalName);
            }
            else
            { 
                claveIntegracion = (string)((SetStateRequest)orgReq).EntityMoniker.Id.ToString(); //en setstate entitymoniker
                tipoAccion = Oracle.TipoAccionUpdate;
                tipoEntidad = MapearNombreEntidad(((SetStateRequest)orgReq).EntityMoniker.LogicalName);
            }
            //Si hubiera más tipos de request en nuestro proceso, meterlas aquí


            try
            {
                Oracle.MandarErrorIntegracion(claveIntegracion,
                    Comun.Left(textoError, 1000), tipoEntidad, tipoAccion, numError);
            }
            catch (Exception ex)
            {
                Comun.LogText("Error al INSERTAR en TABLA LOG ERRORES: " + ex.ToString());
            }

        }

        private int MapearNombreEntidad(string nombreEntidad)
        {
            switch (nombreEntidad)
            {
                case "contact":
                    return Oracle.TipoEntidadContacto;
                case "aen_direccion":
                    return Oracle.TipoEntidadDireccion;
                case "aen_certificacion":
                    return Oracle.TipoEntidadCertificacion;
                case "aen_normacomprada":
                    return Oracle.TipoEntidadNormaComprada;
                case "aen_publicacionesadquiridas":
                    return Oracle.TipoEntidadPublicacionAdquirida;
                case "aen_potencialcliente":
                    return Oracle.TipoEntidadPotencialCliente;
                case "aen_suscripcionadquirida":
                    return Oracle.TipoEntidadSuscripcionAdquirida;
                default:
                    return Oracle.TipoEntidadTercero;
            }
        }


        public bool EjecutarPFEOld()
        {
            IDictionary<string, ExecuteMultipleResponse> responses =
                OSManager.ParallelProxy.Execute<ExecuteMultipleRequest, ExecuteMultipleResponse>(PFERequests, OSOptions);
            int ok = 0, ko = 0, j = 0;
            if (responses == null)
                Comun.LogText("Respuestas NULO");
            else
                Comun.LogText(string.Format("Respuestas obtenidas: " + responses.Count));
            foreach (var response in responses)
            {
                Comun.LogText(string.Format(
                    "{0} respuestas and {1} errores para ExecuteMultipleRequest con key={2}",
                    response.Value.Responses.Count(r => r.Response != null),
                    response.Value.Responses.Count(r => r.Fault != null),
                    response.Key));
                int numErrores = response.Value.Responses.Count(r => r.Fault != null);
                for (int i = 0; i < response.Value.Responses.Count(); i++)
                {
                    if (response.Value.Responses[i].Fault != null)
                    {
                        if (!string.IsNullOrEmpty(response.Value.Responses[i].Fault.Message))
                            Comun.LogText("Error en emr, fault message: " + response.Value.Responses[i].Fault.Message); // + ", codigo = " + ids[i])

                        //if (response.Value.Responses[i].Fault.ErrorDetails != null && response.Value.Responses[i].Fault.ErrorDetails.Any())
                        //    foreach (var errorDetail in response.Value.Responses[i].Fault.ErrorDetails)
                        //        Comun.LogText("Emr, error detail key: " + errorDetail.Key + ", value: " + errorDetail.Value);
                        //if (response.Value.Responses[i].Fault.InnerFault != null &&
                        //    !string.IsNullOrEmpty(response.Value.Responses[i].Fault.InnerFault.Message))
                        //{
                        //    Comun.LogText("Error en emr: " + response.Value.Responses[i].Fault.InnerFault.Message); // + ", codigo = " + ids[i]);
                        //    if (response.Value.Responses[i].Fault.InnerFault.InnerFault != null &&
                        //        !string.IsNullOrEmpty(response.Value.Responses[i].Fault.InnerFault.InnerFault.Message))
                        //        Comun.LogText("Error en emr inner: " + response.Value.Responses[i].Fault.InnerFault.InnerFault.Message);
                        //}

                        Guid faultID = PFERequests[response.Key].Requests[i].RequestId.GetValueOrDefault(Guid.Empty);
                        if (faultID != Guid.Empty) //si es empty es insert
                            Comun.LogText("Id de entidad que provocó error: " + faultID);

                        ko++;

                        var textoError = "Se ha producido error de ejecución, consultar logs texto y CRM";
                        if (!string.IsNullOrEmpty(response.Value.Responses[i].Fault.Message))
                            textoError = "Error ejecución emr:" + response.Value.Responses[i].Fault.Message;
                        //throw new Exception(textoError)
                    }
                    else
                    {
                        ok++;
                    }
                }
                j++;

            }

            Emr.Requests.Clear();
            PFERequests.Clear();
            return ko == 0;
        }
        #endregion PFE Core

    }
}
