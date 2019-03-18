using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Aenor.MigracionTerceros;
using Microsoft.Xrm.Sdk.Query;

namespace PruebaCargaSDK
{
    class Program
    {
        public static IOrganizationService IOS;
        const string UserNamePRE = "indra1@aenor.com";
        const string UserPasswordPRE = "DynamicsAdmin18";
        const string RequestUrlPRE = "https://aenorpreproduccion.crm4.dynamics.com/";

        const string UserNameDES = "indra1@aenor.com";
        const string UserPasswordDES = "DynamicsAdmin18";
        const string RequestUrlDES = "https://aenorcrmdesarrollo.crm4.dynamics.com/";

        private static List<Entity> cuentas_a_cargar;

        #region ExecuteMultipleRequest
        private static ExecuteMultipleRequest Emr;
        private const int BufferEmr = 50; //TAMAÑO BUFFER
        private static int Errores = 0;
        #endregion ExecuteMultipleRequest


        static Crm Crm;

        static void Main(string[] args)
        {
            var Comun = new Comun();
            Comun.LogText("Inicio prueba");
            Comun.InicializarVariables();
            Crm = new Crm();
            Crm.Comun = Comun;
            Crm.InicializarPFE();

            //GetD365_9x_Connection(ref IOS, RequestUrlDES, UserNameDES, UserPasswordDES);
            //IOS = InstanciaCRM(RequestUrlPRE, UserNamePRE, UserPasswordPRE);
            //InicializaEMR();
            //Connect655_SDK(ref IOS, RequestUrlPRE, UserNamePRE, UserPasswordPRE);

            //Uso de multihilo
            //ServicePointManager.UseNagleAlgorithm = true;
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.CheckCertificateRevocationList = true;
            //ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = Comun.DefaultConnectionLimit;
            //BorrarPrueba();

            //1. Carga desde fichero 1000 registros en memoria
            cuentas_a_cargar = new List<Entity>();
            CargaCuentasFromArchivo(ref cuentas_a_cargar);

            //2. Esos registros, con MultipleRequest, incluirlos en 'Terceros' (Accounts)
            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            foreach (Entity ter in cuentas_a_cargar)
            {
                //AnadirElementoEmr(new CreateRequest { Target = ter });
                Crm.AnadirElementoEmr(new CreateRequest { Target = ter });
            }
            //ProcesarUltimosEmr();
            Crm.ProcesarUltimosEmr();
            stopWatch2.Stop();
            Comun.LogText("Tamaño de buffer y tiempo " + BufferEmr + "\t" + stopWatch2.Elapsed.ToString());
        }

        private static void BorrarPrueba()
        {
            var q = new QueryExpression("account");
            q.Criteria.Conditions.AddRange(
                new ConditionExpression("aen_nombrejuridico", ConditionOperator.BeginsWith, "ejemplo_cuenta_")
                );
            var terceros = new EntityCollection();
            while (true)
            {
                var res = IOS.RetrieveMultiple(q);
                terceros.Entities.AddRange(res.Entities);
                if (res.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = res.PagingCookie;
                }
                else
                    break;
            }
            foreach (var t in terceros.Entities)
            {
                Crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("account", t.Id)
                });
            }
            Crm.ProcesarUltimosEmr();
        }


        private static void CargaCuentasFromArchivo(ref List<Entity> cuentas)
        {
            //string[] lines = System.IO.File.ReadAllLines(@"C:\NEORIS\terceros.txt");
            string[] lines = System.IO.File.ReadAllLines(@"d:\proyectos\aenor\terceros2.txt");

            Entity auxiliar;

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                //comun.LogText("\t" + line);
                string[] linea = line.Split(',');
                auxiliar = new Entity();
                auxiliar.LogicalName = "account";
                auxiliar["aen_nombrejuridico"] = linea[0];
                auxiliar["aen_identificadortercero"] = linea[2];
                auxiliar["aen_descripcionlargatercero"] = linea[1];

                cuentas.Add(auxiliar);
            }
        }



        ///<summary>Crea la conexión con Dynamics 365 mediante el SDK.</summary>  
        public static void GetD365_9x_Connection(ref IOrganizationService IOS, string server, string userName, string userPassword)
        {
            try
            {
                IOS = null;
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = userName;
                clientCredentials.UserName.Password = userPassword;

                // Para Dynamics 365 Customer Engagement V9.X // Net Framework < 4.6 
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls12;


                IOS = (IOrganizationService)new OrganizationServiceProxy(new Uri(server), null, clientCredentials, null);


                if (IOS != null)
                {
                    Guid userid = ((WhoAmIResponse)IOS.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {
                        comun.LogText("Conectamos correctamente...");
                    }
                }
                else
                {
                    comun.LogText("No hemos podido conectar...");
                }
            }
            catch (Exception e)
            {
                comun.LogText("ERROR al obtener servicio por SDK!" + e.ToString());
            }
        }
        public static IOrganizationService InstanciaCRM(string url, string user, string pass)
        {
            IOrganizationService service = null;
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = user;
            credentials.UserName.Password = pass;
            Uri serviceUri = new Uri(url);
            credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            Uri HomeRealm = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, HomeRealm, credentials, null);
            {
                //proxy.AuthenticateDevice();
                //proxy.Authenticate();
                //proxy.EnableProxyTypes();
                return service = (IOrganizationService)proxy;
            }
        }
        public static void Connect655_SDK(ref IOrganizationService IOS, string url, string user, string pass)
        {
            CrmServiceClient conn = new CrmServiceClient( "Url="+url+"; Username="+user+"; Password="+pass+"; authtype=Office365");
            IOS = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

        }



        #region ExecuteMultipleRequest  (REVISAR PARA SACAR LOG EN TXT)
        public static void InicializaEMR()
        {
            Emr = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
        }



        //USO  AnadirElementoEmr(new UpdateRequest { Target = cuenta }); + ProcesarUltimosEmr
        //public static static void AnadirElementoEmr(OrganizationRequest elemento)
        //{
        //    Emr.Requests.Add(elemento);
        //    if (Emr.Requests.Count % BufferEmr == 0)
        //        EjecutarEMR();
        //}

        //public static static void ProcesarUltimosEmr()
        //{
        //    if (Emr.Requests.Count % BufferEmr != 0)
        //        EjecutarEMR();
        //}

        //public static static int EjecutarEMR()
        //{
        //    ExecuteMultipleResponse responseWithResults = null;

        //    try
        //    {
        //        responseWithResults = (ExecuteMultipleResponse)IOS.Execute(Emr);
        //    }
        //    catch (Exception ex)
        //    {
        //        comun.LogText("Error en EjecutarEMR: " + ex.Message);
        //        if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
        //            comun.LogText("Error en EjecutarEMR inner: " + ex.InnerException.Message);
        //        Emr.Requests.Clear();
        //    }

        //    int ok = 0;
        //    int ko = 0;

        //    for (int i = 0; i < responseWithResults.Responses.Count; i++)
        //    {
        //        if (responseWithResults.Responses[i].Fault != null)
        //        {
        //            if (!string.IsNullOrEmpty(responseWithResults.Responses[i].Fault.Message))
        //                comun.LogText("Error en emr, fault message: " + responseWithResults.Responses[i].Fault.Message); // + ", codigo = " + ids[i]);
        //            if (responseWithResults.Responses[i].Fault.ErrorDetails != null && responseWithResults.Responses[i].Fault.ErrorDetails.Any())
        //                foreach (var errorDetail in responseWithResults.Responses[i].Fault.ErrorDetails)
        //                    comun.LogText("Emr, error detail key: " + errorDetail.Key + ", value: " + errorDetail.Value);
        //            if (responseWithResults.Responses[i].Fault.InnerFault != null &&
        //                !string.IsNullOrEmpty(responseWithResults.Responses[i].Fault.InnerFault.Message))
        //            {
        //                comun.LogText("Error en emr: " + responseWithResults.Responses[i].Fault.InnerFault.Message); // + ", codigo = " + ids[i]);
        //                if (responseWithResults.Responses[i].Fault.InnerFault.InnerFault != null &&
        //                    !string.IsNullOrEmpty(responseWithResults.Responses[i].Fault.InnerFault.InnerFault.Message))
        //                    comun.LogText("Error en emr inner: " + responseWithResults.Responses[i].Fault.InnerFault.InnerFault.Message);
        //            }

        //            Guid faultID = Emr.Requests[i].RequestId.GetValueOrDefault(Guid.Empty);
        //            if (faultID != Guid.Empty) //si es empty es insert
        //                comun.LogText("Id de entidad que provocó error: " + faultID);

        //            ko++;
        //            Errores++;
        //        }
        //        else
        //        {
        //            ok++;
        //            //bien++;
        //        }

        //    }
        //    Emr.Requests.Clear();
        //    return ok;
        //}
        #endregion ExecuteMultipleRequest
    }
}
