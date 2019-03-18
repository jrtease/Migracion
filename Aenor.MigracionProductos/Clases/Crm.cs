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

namespace Aenor.MigracionProductos.Clases
{
    public class Crm
    {
        #region Declarar Globales
        public static IOrganizationService IOS;


        private ExecuteMultipleRequest Emr;

        //26 nov, props para el uso de PFE
        public OrganizationServiceManager OSManager;
        public OrganizationServiceProxyOptions OSOptions;
        public IDictionary<string, ExecuteMultipleRequest> PFERequests;
        public int RequestKey;
        //Contadores Globales para errores
        public int Iguales = 0;
        private int Eliminados = 0, Actualizados = 0, Creados = 0, Otros = 0, Errores = 0, Associate = 0;

        private Comun Comun;
        private Oracle Oracle { get; set; }
        public Guid UOMID { get; set; }
        public Guid SheduleUOMID {get;set;}
        public Guid TipoProductoNorma { get; set; }

        #endregion Declarar Globales




        public Crm(Comun comun)
        {
            Comun = comun;

            //Conexion a CRM
            CrmServiceClient conn = new CrmServiceClient(Comun.ConnStringCrm);
            IOS = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

            UOMID = GetDefaultUOMID();
            SheduleUOMID = GetSheduleDefaultUOMID();
            TipoProductoNorma = GetTipoProductoDefaultNorma();
        }
  

        #region Get Default Values
        private Guid GetTipoProductoDefaultNorma()
        {
            //"name" = Unidad principal, Entidad UOM
            Guid aux = Guid.Empty;

            string[] campos = { "aen_tipodeproductoid", "aen_name", "aen_codigotipodeproducto" };
            string entityName = "aen_tipodeproducto";

            #region Query
            FilterExpression filter = new FilterExpression();

            ConditionExpression conditionNorma = new ConditionExpression();
            conditionNorma.AttributeName = "aen_name";
            conditionNorma.Operator = ConditionOperator.Equal;
            conditionNorma.Values.Add("Norma");
            filter.Conditions.Add(conditionNorma);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection idiColeccion = GetIOS().RetrieveMultiple(query);

            if (idiColeccion.Entities.Count > 0)
            {
                aux = idiColeccion[0].Id;
            }

            return aux;
        }

        private Guid GetSheduleDefaultUOMID()
        {
            //"name" = Unidad principal, Entidad UOM
            Guid aux = Guid.Empty;

            string[] campos = { "uomscheduleid", "name" };
            string entityName = "uomschedule";

            #region Query
            FilterExpression filter = new FilterExpression();

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "name";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection idiColeccion = GetIOS().RetrieveMultiple(query);

            if (idiColeccion.Entities.Count > 0)
            {
                aux = idiColeccion[0].Id;
            }
            
            return aux;
        }

        private Guid GetDefaultUOMID()
        {
            //"name" = Unidad principal, Entidad UOM
            Guid aux = Guid.Empty;

            string[] campos = { "uomid","name" };
            string entityName = "uom";

            #region Query
            FilterExpression filter = new FilterExpression();

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "name";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection idiColeccion = GetIOS().RetrieveMultiple(query);

            if (idiColeccion.Entities.Count > 0)
            {
                aux =  idiColeccion[0].Id;
            }

            return aux;
        }

        #endregion Get Default Values


        public IOrganizationService GetIOS()
        {
            return IOS;
        }

        public void MostrarEstadisticas(string msj)
        {
            Comun.LogText("ESTADISTICAS " + msj + ": \n #CREADAS: " + Creados + "\n\t #ACTUALIZADAS: " + Actualizados + "\n\t #ELIMINADAS: " + Eliminados + "\n\t #IGUALES: " + Iguales + "\n\t #ASOCIADOS: " + Associate + "\n\t #OTROS: " + Otros);
            Creados = Actualizados = Eliminados = Iguales = Associate = Otros = 0;
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

        public AssociateRequest getAssociateRequest(string EntityName1, Guid g1, string relationName, string EntityName2, Guid g2)
        {
            return new AssociateRequest
            {
                RelatedEntities = new EntityReferenceCollection
                {
                    new EntityReference(EntityName1, g1)
                },
                Relationship = new Relationship(relationName),
                Target = new EntityReference(EntityName2, g2)
            };
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
                    case "Associate":
                        Associate++;
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
                //claveIntegracion = (string)((CreateRequest)orgReq).Target.Attributes["aen_claveintegracion"];
                claveIntegracion = string.Empty;
                tipoAccion = Oracle.TipoAccionInsert;
                tipoEntidad = MapearNombreEntidad(((CreateRequest)orgReq).Target.LogicalName);
            }
            else if (orgReq.RequestName == "Update")
            {
                //claveIntegracion = (string)((UpdateRequest)orgReq).Target.Attributes["aen_claveintegracion"];
                claveIntegracion = (string)((UpdateRequest)orgReq).Target.Id.ToString();
                tipoAccion = Oracle.TipoAccionUpdate;
                tipoEntidad = MapearNombreEntidad(((UpdateRequest)orgReq).Target.LogicalName);
            }
            else if (orgReq.RequestName == "Delete")
            {
                claveIntegracion = ((DeleteRequest)orgReq).Target.Id.ToString(); //en delete el target es entiyreference, no tenemos la clave int
                tipoAccion = Oracle.TipoAccionDelete;
                tipoEntidad = MapearNombreEntidad(((DeleteRequest)orgReq).Target.LogicalName);
            }
            else if (orgReq.RequestName == "Associate")
            {
                claveIntegracion = ((AssociateRequest)orgReq).Target.Id.ToString(); //en associate el target es entiyreference, no tenemos la clave int
                tipoAccion = Oracle.TipoAccionAssociate;
                tipoEntidad = MapearNombreEntidad(((AssociateRequest)orgReq).Target.LogicalName);
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
                case "aen_comitetecnicodenormalizacion":
                    return Oracle.TipoEntidadComiteTecnico;
                case "aen_ics":
                    return Oracle.TipoEntidadICS;
                case "aen_norma":
                    return Oracle.TipoEntidadNormasRaiz;
                case "aen_versin":
                    return Oracle.TipoEntidadNormasVersin;
                case "product":
                    return Oracle.TipoEntidadNormasProductos;
                //case "aen_normacomprada":
                //    return Oracle.TipoEntidadGruposPrecio;
                default:
                    return 1;
            }
        }

        #endregion PFE Core

    }
}
