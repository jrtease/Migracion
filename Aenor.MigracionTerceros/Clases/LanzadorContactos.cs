using Aenor.MigracionTerceros.Clases;
using Aenor.MigracionTerceros.Objetos;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Clases
{
    public class LanzadorContactos
    {
        #region Propiedades
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, Contactos> ContactosOracle, ContactosCRM;
        public Dictionary<string, Guid> MaestroCargos;
        public Dictionary<string, Guid> MaestroTerceros;

        List<string> ContactosADesactivar { get; set; }
        public Dictionary<string, Guid> MaestroContactosPostCarga;
        #endregion Propiedades


        public void Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                Stopwatch sW_TODO = new Stopwatch(), sW_SOLOCARGA = new Stopwatch();
                OracleGlobal = ora;
                ComunGlobal = com;
                ComunGlobal.LogText("----Iniciando sincronización contactos------");
                CrmGlobal = crm;
                
                ContactosADesactivar = new List<string>();

                sW_TODO.Start();

                //1. Carga Maestros
                CargarMaestroCargos();
                //2.A Carga de Terceros para campo Parentcustomerid/aen_claveintegracionparent
                //2.B Carga de Contactos existentes
                CargarMaestroTercerosFromCRM();
                CargarMaestroContactos();


                //3. Lectura de Contactos de la tabla de Oracle (Query: oracleGlobal.QueryContactos)
                LeerContactosOracle();
                if (!ContactosOracle.Any())
                {
                    ComunGlobal.LogText("No hay contactos en origen, terminamos");
                    return;
                }
                

                //4. Inicializa PFECore
                CrmGlobal.InicializarPFE(OracleGlobal);


                //5. Procesamos Contactos de Oracle:
                //  -> Si existen, comprobamos campo a campo si hay que actualizar
                //  -> Sino existen, creamos el contacto nuevo.
                sW_SOLOCARGA.Start();
                #region Procesar CONTACTOS
                bool ok;
                Contactos auxContCRM = new Contactos();
                Entity contUpdate;

                foreach (var cont in ContactosOracle)
                {
                    ok = ContactosCRM.TryGetValue(cont.Key, out auxContCRM);

                    if (ok)
                    { //Existe, comprobamos actualizacion
                        try
                        {
                            contUpdate = new Entity("contact");

                            bool ret = cont.Value.ContactosIguales(auxContCRM, ref contUpdate);

                            if (ret)
                            {
                                EmparentaTercero(cont, ref contUpdate);
                                CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = contUpdate });
                            }
                            else
                                CrmGlobal.Iguales++;
                        }
                        catch (Exception ex)
                        {
                            ComunGlobal.LogText("ERROR al ACTUALIZAR el Entity equivalente del Contacto " + cont.Value.Aen_ClaveIntegracion + " ::: " + ex.ToString());
                        }
                    }
                    else
                    { //No existe, creamos
                        try
                        {
                            //Guardamos los Terceros que a posteriori desactivaremos (en create no se puede mandar registro inactivo)
                            if (cont.Value.Statecode.Equals("Inactivo"))
                                ContactosADesactivar.Add(cont.Value.Aen_ClaveIntegracion);

                            Entity newContact = cont.Value.GetEntityFromContacto();

                            EmparentaTercero(cont, ref newContact);
                            CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = newContact});
                        }
                        catch (Exception ex)
                        {
                            ComunGlobal.LogText("ERROR al CREAR el Entity equivalente del Contacto " + cont.Value.Aen_ClaveIntegracion + " ::: " + ex.ToString());
                        }
                    }
                }
                CrmGlobal.ProcesarUltimosEmr();

                #endregion Procesar CONTACTOS
                sW_SOLOCARGA.Stop();
                ComunGlobal.LogText(" -----> END; TIEMPO CARGA: " + sW_SOLOCARGA.Elapsed.ToString() + " <-----\n\n");


                //6. Desactivar contactos
                CargaDiccionarioContactosPostCarga();
                DesactivarContactos();

                sW_TODO.Stop();
                CrmGlobal.MostrarEstadisticas("CONTACTOS");
                ComunGlobal.LogText(" -----> END; TIEMPO TOTAL: " + sW_TODO.Elapsed.ToString() + " <-----\n\n");



                #region Limpieza diccionarios para liberar memoria
                ContactosOracle = null; //.Clear();
                ContactosCRM = null; //.Clear();
                MaestroCargos = null; //.Clear();
                MaestroTerceros = null; //.Clear();
                MaestroContactosPostCarga = null; //.Clear();
                #endregion Limpieza diccionarios para liberar memoria
            }
            catch (Exception ex)
            {
                ComunGlobal.LogText("ERROR en Lanzador de CONTACTOS ::: " + ex.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();
            }
        }






        #region Funciones
        private void CargarMaestroCargos()
        {
            string[] campos = { "aen_tiposdecargoid", "aen_name", "aen_codigocargo" };
            string entityName = "aen_tiposdecargo";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionActives = new ConditionExpression();
            conditionActives.AttributeName = "statecode";
            conditionActives.Operator = ConditionOperator.Equal;
            conditionActives.Values.Add(0);
            filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_codigocargo";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection collections = CrmGlobal.GetIOS().RetrieveMultiple(query);
            MaestroCargos= new Dictionary<string, Guid>();

            foreach (Entity sub in collections.Entities)
            {
                string cod = sub.GetAttributeValue<string>("aen_codigocargo").Trim().ToUpper();
                MaestroCargos.Add(cod, sub.Id);
            }
        }


        private void CargarMaestroContactos()
        {
            ContactosCRM = new Dictionary<string, Contactos>();
            
            string[] campos = {
                NombreCamposContacto.Aen_ClaveIntegracionCRM, NombreCamposContacto.Aen_ClaveIntegracionParentCRM,
                NombreCamposContacto.Aen_CargoprincipalIdCRM, NombreCamposContacto.Aen_NumerodocumentoCRM,
                NombreCamposContacto.Aen_TipodocumentoCRM, NombreCamposContacto.Aen_TratamientoCRM,
                NombreCamposContacto.Aen_ObservacionesCRM, NombreCamposContacto.Aen_ObservacionesmigracionCRM,
                NombreCamposContacto.DonotsendmmCRM, NombreCamposContacto.Emailaddress1CRM,
                NombreCamposContacto.FirstnameCRM, NombreCamposContacto.LastnameCRM,
                NombreCamposContacto.GendercodeCRM, NombreCamposContacto.MobilephoneCRM,
                NombreCamposContacto.StatecodeCRM, NombreCamposContacto.Telephone1CRM,
                NombreCamposContacto.Aen_OrigenCRM, NombreCamposContacto.Aen_IdentificadorcontactoCRM
            };
            string entityName = "contact";

            #region Query
            FilterExpression filter = new FilterExpression();
            //ConditionExpression conditionActives = new ConditionExpression();
            //conditionActives.AttributeName = "statecode";
            //conditionActives.Operator = ConditionOperator.Equal;
            //conditionActives.Values.Add(0);
            //filter.Conditions.Add(conditionActives);

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_claveintegracion";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

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
                EntityCollection contColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (contColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity contCRM in contColeccion.Entities)
                    {
                        Contactos cont = new Contactos();
                        cont.ContactoFromCRM(contCRM);

                        ContactosCRM.Add(cont.Aen_ClaveIntegracion, cont);
                    }
                }

                if (contColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = contColeccion.PagingCookie;
                }
                else
                    break;
            }
        }


        private void CargarMaestroTercerosFromCRM()
        {
            MaestroTerceros = new Dictionary<string, Guid>();

            //1.Obtener lista de Terceros CRM (+5mil)
            #region Preparar Query
            string entityName = "account";

            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionPKey = new ConditionExpression();
            conditionPKey.AttributeName = "aen_claveintegracion";
            conditionPKey.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionPKey);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(new string[] {"aen_claveintegracion", "accountid" });
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
                EntityCollection terColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (terColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity terceroCRM in terColeccion.Entities)
                        MaestroTerceros.Add(terceroCRM.GetAttributeValue<string>("aen_claveintegracion"), terceroCRM.Id);
                }

                if (terColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = terColeccion.PagingCookie;
                }
                else
                    break;
            }
        }


        public void LeerContactosOracle()
        {
            OracleDataAdapter da = new OracleDataAdapter(Oracle.QueryContactos, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ContactosOracle = new Dictionary<string, Contactos>();
            Contactos cont;
            foreach (DataRow fila in dt.Rows)
            {
                cont = new Contactos();
                cont.ContactoFromOracle(fila, MaestroTerceros);
                if (cont == null)
                    continue;
                //Para el guid de Cargoprincipal
                if (!cont.Aen_CargoprincipalId_str.Equals(string.Empty))
                {
                    Guid aux = Guid.Empty;
                    MaestroCargos.TryGetValue(cont.Aen_CargoprincipalId_str, out aux);
                    if (aux != Guid.Empty)
                        cont.Aen_CargoprincipalId = aux;
                }
                ContactosOracle.Add(cont.Aen_ClaveIntegracion, cont);
            }
        }


        private void EmparentaTercero(KeyValuePair<string, Contactos> cont, ref Entity contUpdate)
        {
            //Enlazamos con el tercero
            if (!cont.Value.Aen_ClaveIntegracionParent.Equals(Guid.Empty))
            {
                contUpdate[NombreCamposContacto.Aen_ClaveIntegracionParentCRM] = new EntityReference("account", cont.Value.Aen_ClaveIntegracionParent);
            }
        }


        private void DesactivarContactos()
        {
            List<Guid> a_desactivar = new List<Guid>();
            Guid valor_guid = Guid.Empty;
            bool ok;

            //Buscar guids
            foreach (string str in ContactosADesactivar)
            {
                ok = MaestroContactosPostCarga.TryGetValue(str, out valor_guid);

                if (ok)
                    a_desactivar.Add(valor_guid);
            }

            //Realizar desactivaciones
            UpdateRequest desactivar;
            Entity auxUpdate;

            foreach (Guid g in a_desactivar)
            {
                desactivar = new UpdateRequest();
                auxUpdate = new Entity("contact");
                auxUpdate.Id = g;
                auxUpdate["statecode"] = new OptionSetValue(1);
                auxUpdate["statuscode"] = new OptionSetValue(2);
                desactivar.Target = auxUpdate;
                CrmGlobal.AnadirElementoEmr(desactivar);
            }
            CrmGlobal.ProcesarUltimosEmr();
        }


        private void CargaDiccionarioContactosPostCarga()
        {
            MaestroContactosPostCarga = new Dictionary<string, Guid>();

            string[] campos = { "aen_claveintegracion", "contactid"};
            string entityName = "contact";

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_claveintegracion";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

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
                EntityCollection contColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (contColeccion.Entities.Count > 0)
                {
                    foreach (Entity contCRM in contColeccion.Entities)
                        MaestroContactosPostCarga.Add(contCRM[NombreCamposContacto.Aen_ClaveIntegracionCRM].ToString(), contCRM.Id);
                }

                if (contColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = contColeccion.PagingCookie;
                }
                else
                    break;
            }
        }

        #endregion Funciones

    }
}
