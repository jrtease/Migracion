using Aenor.MigracionProductos.Objetos;
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

namespace Aenor.MigracionProductos.Clases
{
    class LanzadorComitesTecnicos
    {
        #region PROPIEDADES
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, ComiteTecnico> ComitesOracle, ComitesCRM;
        public Dictionary<string, Guid> MaestroComitesCRM;

        List<KeyValuePair<string, string>> EmparentarComites;   //Comite , Comite padre.
        #endregion PROPIEDADES


        #region METODOS
        public Dictionary<string,Guid> Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;


                //1. Leer Comités de Oracle y CRM
                    //Inicializa PFECore
                CrmGlobal.InicializarPFE(OracleGlobal);
                LeerComitesFromCRM();
                LeerComitesFromOracle();
                

                //2. Cargar Comités sin comité padre (son comités padres)   |
                //3. Cargar Resto de comités                                |  Cargar todos y emparentar despues
                if (ComitesOracle.Any())
                {
                    ComunGlobal.LogText("----   Iniciando sincronización COMITES   ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + ComitesOracle.Count);
                    #region PROCESA COMITES
                    bool ok;
                    ComiteTecnico auxContCRM = new ComiteTecnico();
                    Entity comiteUpdate;

                    foreach (var cmt in ComitesOracle)
                    {
                        ok = ComitesCRM.TryGetValue(cmt.Value.Aen_Codigo_Comite, out auxContCRM);

                        try
                        {
                            if (ok) // Existe, actualizamos
                            {
                                comiteUpdate = new Entity(NombresCamposComiteTecnico.EntityName);

                                bool res = cmt.Value.ComitesIguales(auxContCRM , ref comiteUpdate);

                                if (res)
                                {
                                    //Emparentar en update (por si el padre del comité se crea en este bloque también)
                                    //if (!cmt.Value.Aen_Codigo_PadreSTR.Equals(string.Empty) && cmt.Value.Aen_Codigo_Padre.Equals(Guid.Empty))
                                    //    EmparentarComites.Add(new KeyValuePair<string, string>(cmt.Value.Aen_Codigo_Comite, cmt.Value.Aen_Codigo_PadreSTR));

                                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = comiteUpdate });
                                }
                                else
                                    CrmGlobal.Iguales++;
                            }
                            else //No existe, creamos
                            {
                                Entity newCom = cmt.Value.GetEntity();

                                //Emparentar post-creación
                                if (!cmt.Value.Aen_Codigo_PadreSTR.Equals(string.Empty) && cmt.Value.Aen_Codigo_Padre.Equals(Guid.Empty))
                                    EmparentarComites.Add(new KeyValuePair<string, string>(cmt.Value.Aen_Codigo_Comite, cmt.Value.Aen_Codigo_PadreSTR));

                                CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = newCom});
                            }
                        }
                        catch (Exception e)
                        {
                            ComunGlobal.LogText("ERROR con el comité " + cmt.Value.Aen_Codigo_Comite + " ::: " + e.ToString());
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();
                    #endregion PROCESA COMITES
                    CrmGlobal.MostrarEstadisticas("COMITES");
                    ComunGlobal.LogText("----   FIN sincronización COMITES   ------");
                }
                else
                    ComunGlobal.LogText("No hay Comités Técnicos en origen, terminamos");


                //4. Indexar Comités <códigoComite, Guid>
                CargaDiccionarioGuidsComites();

                //5. Emparentar Comités
                EmparentacionDeComites();

                LimpiezaDiccionarios();

                return MaestroComitesCRM;
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de COMITES ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();

                return null;
            }
        }




        public void LeerComitesFromOracle()
        {
            #region COMITES TECNICOS
            ComitesOracle = new Dictionary<string, ComiteTecnico>();

            #region Query a ejecutar (carga inicial o incremental)
            string queryTot = Oracle.QueryComitesTecnicos;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombresCamposComiteTecnico.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                ComiteTecnico comTAux = new ComiteTecnico();
                comTAux.ComiteFromOracle(fila, MaestroComitesCRM);
                if (comTAux != null)
                    ComitesOracle.Add(comTAux.Aen_Codigo_Comite, comTAux);
            }
            #endregion COMITES TECNICOS
        }

        public void LeerComitesFromCRM()
        {
            ComitesCRM = new Dictionary<string, ComiteTecnico>();
            MaestroComitesCRM = new Dictionary<string, Guid>();
            EmparentarComites = new List<KeyValuePair<string, string>>();

            string[] campos = {
                NombresCamposComiteTecnico.EntityId,
                NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM, NombresCamposComiteTecnico.Aen_Codigo_PadreCRM ,
                NombresCamposComiteTecnico.Aen_Nombre_ComiteCRM, NombresCamposComiteTecnico.Aen_Nombre_Comite_ENCRM,
                NombresCamposComiteTecnico.Aen_OrganismoCRM
            };
            string entityName = NombresCamposComiteTecnico.EntityName;

            #region Query
            FilterExpression filter = new FilterExpression();

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM;
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
                        ComiteTecnico com = new ComiteTecnico();
                        com.ComiteFromCRM(contCRM);
                        ComitesCRM.Add(com.Aen_Codigo_Comite, com);
                        MaestroComitesCRM.Add(com.Aen_Codigo_Comite, com.Aen_ComiteGUID);
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

        public void CargaDiccionarioGuidsComites()
        {
            MaestroComitesCRM.Clear();

            //1.Obtener lista de Terceros CRM (+5mil)
            #region Preparar Query
            string entityName = NombresCamposComiteTecnico.EntityName;

            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionPKey = new ConditionExpression();
            conditionPKey.AttributeName = NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM;
            conditionPKey.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionPKey);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM);
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
                EntityCollection cmColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (cmColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity comCRM in cmColeccion.Entities)
                    {
                        MaestroComitesCRM.Add(comCRM.GetAttributeValue<string>(NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM), comCRM.Id);
                    }
                }

                if (cmColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = cmColeccion.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        private void EmparentacionDeComites()
        {
            Guid padre, hijo;
            Entity auxComite;

            foreach (var l in EmparentarComites)
            {
                padre = hijo = Guid.Empty;
                bool ok_hijo = MaestroComitesCRM.TryGetValue(l.Key, out hijo);
                bool ok_padre = MaestroComitesCRM.TryGetValue(l.Value, out padre);

                if (ok_padre && ok_hijo
                    && padre != Guid.Empty && hijo != Guid.Empty && padre != hijo)
                {
                    auxComite = new Entity(NombresCamposComiteTecnico.EntityName);
                    auxComite.Id = hijo;
                    auxComite[NombresCamposComiteTecnico.EntityId] = hijo;
                    auxComite[NombresCamposComiteTecnico.Aen_Codigo_PadreCRM] = new EntityReference(NombresCamposComiteTecnico.EntityName, padre);
                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = auxComite });
                }   
            }

            CrmGlobal.ProcesarUltimosEmr();
        }

        private void LimpiezaDiccionarios()
        {
            ComitesOracle.Clear();
            ComitesCRM.Clear();
            EmparentarComites.Clear();
        }
        #endregion METODOS
    }
}
