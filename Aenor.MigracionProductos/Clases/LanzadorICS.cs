using Aenor.MigracionProductos.Objetos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductos.Clases
{
    class LanzadorICS
    {
        #region PROPIEDADES
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, ICS> ICSOracle, ICSCRM;
        public Dictionary<string, Guid> MaestroICSCRM;

        List<KeyValuePair<string, string>> EmparentarICS;   //Comite , Comite padre.
        #endregion PROPIEDADES


        #region METODOS
        public Dictionary<string, Guid> Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;


                //1. Leer ICS de Oracle y CRM
                //Inicializa PFECore
                CrmGlobal.InicializarPFE(OracleGlobal);
                LeerICSFromCRM();
                LeerICSFromOracle();
                

                //2. Cargar iCS sin ics padre (son ICS padres)   |
                //3. Cargar Resto de ICSs                        |  Cargar todos y emparentar despues
                if (ICSOracle.Any())
                {
                    ComunGlobal.LogText("----   Iniciando sincronización ICS   ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + ICSOracle.Count);
                    #region PROCESA ICS
                    bool ok;
                    ICS auxICSCRM = new ICS();
                    Entity icsUpdate;

                    foreach (var ics in ICSOracle)
                    {
                        ok = ICSCRM.TryGetValue(ics.Value.Aen_Codigo_Ics, out auxICSCRM);

                        try
                        {
                            if (ok) // Existe, actualizamos
                            {
                                icsUpdate = new Entity(NombresCamposICS.EntityName);

                                bool res = ics.Value.ICSIguales(auxICSCRM, ref icsUpdate);

                                if (res)
                                {
                                    //Emparentar en update (por si el padre del ICS se crea en este bloque también)
                                    //if (!ics.Value.Aen_Codigo_Ics_PadreSTR.Equals(string.Empty) && ics.Value.Aen_Codigo_Ics_Padre.Equals(Guid.Empty))
                                    //    EmparentarICS.Add(new KeyValuePair<string, string>(ics.Value.Aen_Codigo_Ics, ics.Value.Aen_Codigo_Ics_PadreSTR));

                                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = icsUpdate });
                                }
                                else
                                    CrmGlobal.Iguales++;
                            }
                            else //No existe, creamos
                            {
                                Entity newI= ics.Value.GetEntity();

                                //Emparentar post-creación
                                if (!ics.Value.Aen_Codigo_Ics_PadreSTR.Equals(string.Empty) && ics.Value.Aen_Codigo_Ics_Padre.Equals(Guid.Empty))
                                    EmparentarICS.Add(new KeyValuePair<string, string>(ics.Value.Aen_Codigo_Ics, ics.Value.Aen_Codigo_Ics_PadreSTR));

                                CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = newI });
                            }
                        }
                        catch (Exception e)
                        {
                            ComunGlobal.LogText("ERROR con el ICS " + ics.Value.Aen_Codigo_Ics + " ::: " + e.ToString());
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();
                    #endregion PROCESA ICS
                    CrmGlobal.MostrarEstadisticas("ICS");
                    ComunGlobal.LogText("----   FIN sincronización ICS   ------");
                }
                else
                    ComunGlobal.LogText("No hay ICS en origen, terminamos");




                //4. Indexar Comités <códigoComite, Guid>
                CargaDiccionarioGuidsICS();

                //5. Emparentar Comités
                EmparentacionDeICS();

                LimpiezaDiccionarios();

                return MaestroICSCRM;
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de ICS ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();

                return null;
            }
        }

       



        public void LeerICSFromOracle()
        {
            #region ICS
            ICSOracle = new Dictionary<string, ICS>();

            #region Query a ejecutar (carga inicial o incremental)
            string queryTot = Oracle.QueryICS;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombresCamposICS.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                ICS icsAux = new ICS();
                icsAux.ICSFromOracle(fila, MaestroICSCRM);
                if (icsAux != null)
                    ICSOracle.Add(icsAux.Aen_Codigo_Ics, icsAux);
            }
            #endregion ICS
        }

        public void LeerICSFromCRM()
        {
            ICSCRM = new Dictionary<string, ICS>();
            MaestroICSCRM = new Dictionary<string, Guid>();
            EmparentarICS = new List<KeyValuePair<string, string>>();

            string[] campos = {
                NombresCamposICS.EntityId,
                NombresCamposICS.Aen_Codigo_IcsCRM, NombresCamposICS.Aen_Codigo_Ics_PadreCRM,
                NombresCamposICS.Aen_Descripcion_IcsCRM, NombresCamposICS.Aen_Descripcion_IcsENCRM,
                NombresCamposICS.Aen_Ics_ActivoCRM
            };
            string entityName = NombresCamposICS.EntityName;

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombresCamposICS.Aen_Codigo_IcsCRM;
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
                EntityCollection icsColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (icsColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista ICS CRM, el diccionario
                    foreach (Entity icCRM in icsColeccion.Entities)
                    {
                        ICS i = new ICS();
                        i.ICSFromCRM(icCRM);
                        ICSCRM.Add(i.Aen_Codigo_Ics, i);
                        MaestroICSCRM.Add(i.Aen_Codigo_Ics, i.Aen_ICSGUID);
                    }
                }

                if (icsColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = icsColeccion.PagingCookie;
                }
                else
                    break;
            }
        }

        private void CargaDiccionarioGuidsICS()
        {
            MaestroICSCRM.Clear();

            //1.Obtener lista de Terceros CRM (+5mil)
            #region Preparar Query
            string entityName = NombresCamposICS.EntityName;

            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionPKey = new ConditionExpression();
            conditionPKey.AttributeName = NombresCamposICS.Aen_Codigo_IcsCRM;
            conditionPKey.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionPKey);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(NombresCamposICS.Aen_Codigo_IcsCRM);
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
                EntityCollection icsColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (icsColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista terceros CRM, el diccionario
                    foreach (Entity iCRM in icsColeccion.Entities)
                    {
                        MaestroICSCRM.Add(iCRM.GetAttributeValue<string>(NombresCamposICS.Aen_Codigo_IcsCRM), iCRM.Id);
                    }
                }

                if (icsColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = icsColeccion.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        private void EmparentacionDeICS()
        {
            Guid padre, hijo;
            Entity auxICS;

            foreach (var l in EmparentarICS)
            {
                padre = hijo = Guid.Empty;
                bool ok_hijo = MaestroICSCRM.TryGetValue(l.Key, out hijo);
                bool ok_padre = MaestroICSCRM.TryGetValue(l.Value, out padre);

                if(ok_hijo && ok_padre
                        && hijo != Guid.Empty && padre != Guid.Empty && hijo != padre)
                {
                    auxICS = new Entity(NombresCamposICS.EntityName);
                    auxICS.Id = hijo;
                    auxICS[NombresCamposICS.EntityId] = hijo;
                    auxICS[NombresCamposICS.Aen_Codigo_Ics_PadreCRM] = new EntityReference(NombresCamposICS.EntityName, padre);
                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = auxICS });
                }
            }
            CrmGlobal.ProcesarUltimosEmr();
        }

        public void LimpiezaDiccionarios()
        {
            ICSOracle.Clear();
            ICSCRM.Clear();
            EmparentarICS.Clear();
        }
        #endregion METODOS
    }
}
