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
    class LanzadorNormas
    {
        #region Propiedades
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, Guid> MaestroComitesCRM;  //Codigo Comite
        public Dictionary<string, Guid> MaestroICSCRM;   // Codigo ICS
        public Dictionary<string, Normas> NormasOracle, NormasCRM;  //Articulo
        public Dictionary<string, NormasProductos> NormasProductosOracle, NormasProductosCRM; //Codigo Producto
        public List<KeyValuePair<Guid, Guid>> NormasICSOracle; //Tabla intermedia ICS-Normas ORACLE
        public Dictionary<string, KeyValuePair<Guid, Guid>> NormasICSCRM;   //Diccionario KEY=GuidVersion&GuidICS, VALUE: <Key:Version, Value:ICS>

        List<string> RaicesNormas;
        public Dictionary<string, Guid> RaicesNormasCRM;
        public Dictionary<string, Guid> VersionesNormasCRM;
        public Dictionary<string, Guid> IdiomasCRM;
        public Dictionary<string, Guid> FormatoCRM;
        public Dictionary<string, Guid> TercerosOrganismosCRM;
        #endregion Propiedades

        public void Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;

                ComunGlobal.LogText("*************************************************************************");

                /*1. Carga Comités Padres (Tabla 'Comites Tecnicos')
                  2. Carga resto Comités (Tabla 'Comites Tecnicos')*/
                MaestroComitesCRM = new LanzadorComitesTecnicos().Iniciar(OracleGlobal, ComunGlobal, CrmGlobal);
                ComunGlobal.LogText("*************************************************************************");


                /*3. Carga Normas ICS sin padres (Tabla 'ICS')
                  4. Carga resto de Normas ICS (Tabla 'ICS') */
                MaestroICSCRM = new LanzadorICS().Iniciar(OracleGlobal, ComunGlobal, CrmGlobal);
                ComunGlobal.LogText("*************************************************************************");


                LeeFromCRMMaestros();
                LeeFromOracleRaices();

                //5. Cargar Normas (Raíz: De las tablas de 'Normas' obtener los distintos valores de Aen_Raiz_Norma y cargarlos en la entidad aen_norma)
                //  5.1 Diccionario <código, Guid> de Normas Raices
                #region PROCESAR NORMAS RAIZ
                bool ok;
                Guid normRGuid;
                Entity auxRaiz;
                if (RaicesNormas.Any())
                {
                    ComunGlobal.LogText("----   Iniciando CARGA NORMAS RAIZ  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + RaicesNormas.Count);
                    foreach (var rz in RaicesNormas)
                    {
                        ok = RaicesNormasCRM.TryGetValue(rz, out normRGuid);
                        //No existe, creamos
                        if (!ok)
                        {
                            auxRaiz = new Entity(NombreCamposNormas.EntityNameRaiz);
                            auxRaiz[NombreCamposNormas.Aen_CodigoRaizNorma] = rz;
                            CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = auxRaiz });
                        }
                        else
                            CrmGlobal.Iguales++;
                    }
                    CrmGlobal.ProcesarUltimosEmr();

                    if (RaicesNormas.Count > 0)
                        CargaMaestroRaicesCRM();

                    CrmGlobal.MostrarEstadisticas("NORMAS RAIZ");
                    ComunGlobal.LogText("----   FINALIZADA CARGA NORMAS RAIZ  ------");
                }
                else
                    ComunGlobal.LogText("No hay NORMAS RAIZ en origen, terminamos");
                #endregion PROCESAR NORMAS RAIZ
                ComunGlobal.LogText("*************************************************************************");


                //6. Cargar Normas-Version (Tabla 'Normas' enlazando ya el campo Aen_Raiz_Norma)
                LeeFromCRMVersiones();
                LeeFromOracleVersiones();
                if (NormasOracle.Any())
                {
                    #region PROCESA NORMAS VERSION
                    ComunGlobal.LogText("----   Iniciando CARGA NORMAS VERSION  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + NormasOracle.Count);
                    Normas auxNormCRM = new Normas();
                    Entity norUpdate;

                    foreach (var nor in NormasOracle)
                    {
                        ok = NormasCRM.TryGetValue(nor.Value.Aen_Articulo, out auxNormCRM);

                        try
                        {
                            if (ok) // Existe, actualizamos
                            {
                                norUpdate = new Entity(NombresCamposICS.EntityName);

                                bool res = nor.Value.VersionesIguales(auxNormCRM, ref norUpdate);

                                if (res)
                                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = norUpdate });
                                else
                                    CrmGlobal.Iguales++;
                            }
                            else //No existe, creamos
                            {
                                Entity newI = nor.Value.GetEntity();
                                CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = newI });
                            }
                        }
                        catch (Exception e)
                        {
                            ComunGlobal.LogText("ERROR con la Norma Versión " + nor.Value.Aen_Articulo + " ::: " + e.ToString());
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();
                    CrmGlobal.MostrarEstadisticas("NORMAS VERSION");
                    ComunGlobal.LogText("----   Finalizada CARGA NORMAS VERSION  ------");
                    #endregion PROCESA NORMAS VERSION
                }
                else
                    ComunGlobal.LogText("No hay NORMAS VERSION en origen, terminamos");
                ComunGlobal.LogText("*************************************************************************");


                //7. Cargar Productos (Tabla 'Normas-Producto')
                CargaMaestroVersionesCRM();
                LeeFromCRMProductoseICS();
                LeeFromOracleProductoseICS();
                if (NormasProductosOracle.Any())
                {
                    #region PROCESA PRODUCTOS
                    ComunGlobal.LogText("----   Iniciando CARGA PRODUCTOS  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + NormasProductosOracle.Count);
                    NormasProductos auxNpCRM = new NormasProductos();
                    Entity npUpdate;

                    foreach (var norP in NormasProductosOracle)
                    {
                        ok = NormasProductosCRM.TryGetValue(norP.Value.Aen_Codigo_Producto, out auxNpCRM);

                        try
                        {
                            if (ok) // Existe, actualizamos
                            {
                                npUpdate = new Entity(NombreCamposNormasProductos.EntityName);

                                bool res = norP.Value.NormasProductosIguales(auxNpCRM, ref npUpdate, CrmGlobal.TipoProductoNorma, CrmGlobal.UOMID, CrmGlobal.SheduleUOMID);

                                if (res)
                                    CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = npUpdate });
                                else
                                    CrmGlobal.Iguales++;
                            }
                            else //No existe, creamos
                            {
                                Entity newE = norP.Value.GetEntity(CrmGlobal.TipoProductoNorma, CrmGlobal.UOMID, CrmGlobal.SheduleUOMID);
                                CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = newE });
                            }
                        }
                        catch (Exception e)
                        {
                            ComunGlobal.LogText("ERROR con el producto " + norP.Value.Aen_Codigo_Producto + " ::: " + e.ToString());
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();
                    CrmGlobal.MostrarEstadisticas("PRODUCTOS Carga");
                    ComunGlobal.LogText("----   Finalizada CARGA PRODUCTOS  ------");
                    #endregion PROCESA PRODUCTOS
                }
                else
                    ComunGlobal.LogText("No hay PRODUCTOS en origen, terminamos");
                ComunGlobal.LogText("*************************************************************************");


                //8. Asociar Normas ICS (Tabla 'ICS' + intermedia 'Normas-ICS') a Normas Versión
                if (NormasICSOracle.Any())
                {
                    #region PROCESA RELACIONES NORMAS VERSION - ICS
                    ComunGlobal.LogText("----   Iniciando ENLACE ICS-VERSION  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + NormasICSOracle.Count);
                    KeyValuePair<Guid, Guid> aux;
                    foreach (var nicsOra in NormasICSOracle)
                    {
                        if (nicsOra.Key != Guid.Empty && nicsOra.Value != Guid.Empty)
                        { 
                            bool enlazado = NormasICSCRM.TryGetValue(nicsOra.Key.ToString() + "&" + nicsOra.Value.ToString(),
                                out aux);
                            if (!enlazado)
                                CrmGlobal.AnadirElementoEmr(CrmGlobal.getAssociateRequest(NombresCamposICS.EntityName, nicsOra.Value, NombresCamposNormasICS.RelationshipName, NombreCamposNormas.EntityNameVersion, nicsOra.Key ));
                            else
                                CrmGlobal.Iguales++;
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();
                    CrmGlobal.MostrarEstadisticas("ENLACES ICS-VERSION");
                    ComunGlobal.LogText("----   Finalizado ENLACE ICS-VERSION  ------");
                    #endregion PROCESA RELACIONES NORMAS VERSION - ICS
                }
                else
                    ComunGlobal.LogText("No hay ICS a enlazar en NORMAS VERSION, terminamos");
                ComunGlobal.LogText("*************************************************************************");

                LimpiezaDiccionarios();

                ComunGlobal.LogText("----   FIN CARGA   ------");
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de NORMAS ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();
            }
        }


        #region LECTURAS
        private void LeeFromOracleRaices()
        {
            #region NORMAS RAIZ
            RaicesNormas = new List<string>();

            #region Query a ejecutar (carga inicial o incremental)
            string queryTot = Oracle.QueryNormasRaiz;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombreCamposNormas.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            var dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                if(fila[NombreCamposNormas.Aen_Raiz_NormaORACLE] != DBNull.Value)
                    RaicesNormas.Add(fila[NombreCamposNormas.Aen_Raiz_NormaORACLE].ToString().Trim());
            }
            #endregion NORMAS RAIZ
        }

        private void LeeFromOracleVersiones()
        {
            #region NORMAS VERSION
            NormasOracle = new Dictionary<string, Normas>();

            #region Query a ejecutar (carga inicial o incremental)
            string queryTot = Oracle.QueryNormasAll;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombreCamposNormas.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            var dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                Normas norAux = new Normas(); 
                norAux.NormasFromOracle(fila, TercerosOrganismosCRM, MaestroComitesCRM, RaicesNormasCRM);

                if (norAux != null)
                    NormasOracle.Add(norAux.Aen_Articulo, norAux);
            }
            #endregion NORMAS VERSION
        }

        private void LeeFromOracleProductoseICS()
        {
            #region NORMAS-ICS (relaciones)
            NormasICSOracle = new List<KeyValuePair<Guid, Guid>>();

            #region Query a ejecutar (carga inicial o incremental)
            string queryTot = Oracle.QueryNormasICS;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombresCamposNormasICS.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            var dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                var norICSAux = new NormasICS();
                norICSAux.NormasICSFromOracle(fila, MaestroICSCRM, VersionesNormasCRM);
                if (norICSAux != null)
                    NormasICSOracle.Add(new KeyValuePair<Guid,Guid>(norICSAux.Aen_ArticuloGUID, norICSAux.Aen_Codigo_IcsGUID));
            }
            #endregion NORMAS-ICS (relaciones)

            #region NORMAS-PRODUCTOS
            NormasProductosOracle = new Dictionary<string, NormasProductos>();

            #region Query a ejecutar (carga inicial o incremental)
            queryTot = Oracle.QueryNormasProductos;

            if (!ComunGlobal.FechaFinIncremental.Equals("0") && !ComunGlobal.FechaIniIncremental.Equals("0"))
                queryTot = queryTot + " WHERE " + NombreCamposNormasProductos.Aen_Fecha_ActualizacionORACLE + " BETWEEN '" + ComunGlobal.FechaIniIncremental + "' AND '" + ComunGlobal.FechaFinIncremental + "'";

            #endregion Query a ejecutar  (carga inicial o incremental)

            oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            dt = new DataTable();
            oda.Fill(dt);

            foreach (DataRow fila in dt.Rows)
            {
                NormasProductos norPAux = new NormasProductos();
                norPAux.NormasProductosFromOracle(fila, IdiomasCRM, FormatoCRM, VersionesNormasCRM);
                if (norPAux != null)
                {
                    NormasProductosOracle.Add(norPAux.Aen_Codigo_Producto, norPAux);
                }
            }
            #endregion NORMAS-PRODUCTOS
        }

        private void LeeFromCRMMaestros()
        {
            CargaMaestroRaicesCRM();

            #region IDIOMAS
            IdiomasCRM = new Dictionary<string, Guid>();

            string[] campos = { "aen_idiomaid", "aen_codigoweb", "aen_codigodelidioma" };
            string entityName = "aen_idioma";

            #region Query
            FilterExpression filter = new FilterExpression();

            ConditionExpression conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_codigodelidioma";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(campos);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection idiColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

            if (idiColeccion.Entities.Count > 0)
            {
                foreach (Entity idioCRM in idiColeccion.Entities)
                {
                    IdiomasCRM.Add(idioCRM["aen_codigodelidioma"].ToString(), idioCRM.Id);
                }
            }
            #endregion IDIOMAS

            #region SOPORTE-FORMATO
            FormatoCRM = new Dictionary<string, Guid>();

            string [] camposSop = { "aen_formatoid", "aen_codigodelformato"};
            entityName = "aen_formato";

            #region Query
            filter = new FilterExpression();

            conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_codigodelformato";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposSop);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection forColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

            if (forColeccion.Entities.Count > 0)
            {
                foreach (Entity forCRM in forColeccion.Entities)
                {
                    FormatoCRM.Add(forCRM["aen_codigodelformato"].ToString(), forCRM.Id);
                }
            }
            #endregion SOPORTE-FORMATO

            #region TERCEROS-ORGANISMOS
            TercerosOrganismosCRM = new Dictionary<string, Guid>();

            string[] camposter = { "accountid", "aen_claveintegracion" };
            entityName = "account";

            #region Query
            filter = new FilterExpression();

            conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = "aen_claveintegracion";
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            var conditionEsOrganismo = new ConditionExpression();
            conditionEsOrganismo.AttributeName = "aen_esorganismo";
            conditionEsOrganismo.Operator = ConditionOperator.Equal;
            conditionEsOrganismo.Values.Add(true);
            filter.Conditions.Add(conditionEsOrganismo);

            query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposter);
            query.Criteria.AddFilter(filter);
            #endregion Query

            EntityCollection terCol = CrmGlobal.GetIOS().RetrieveMultiple(query);

            if (terCol.Entities.Count > 0)
            {
                foreach (Entity tCRM in terCol.Entities)
                {
                    TercerosOrganismosCRM.Add(tCRM["aen_claveintegracion"].ToString(), tCRM.Id);
                }
            }
            #endregion TERCEROS-ORGANISMOS
        }

        private void CargaMaestroRaicesCRM()
        {
            #region NORMAS RAIZ
            if(RaicesNormas != null)
                RaicesNormasCRM.Clear();
            RaicesNormasCRM = new Dictionary<string, Guid>();

            string[] campos = {
                NombreCamposNormas.Aen_CodigoRaizNorma
            };
            string entityName = NombreCamposNormas.EntityNameRaiz;

            #region Query
            FilterExpression filter = new FilterExpression();
            ConditionExpression conditionNull = new ConditionExpression();
            conditionNull.AttributeName = NombreCamposNormas.Aen_CodigoRaizNorma;
            conditionNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNull);

            //ConditionExpression conditionNotNull = new ConditionExpression();
            //conditionNotNull.AttributeName = NombreCamposNormas.Aen_ArticuloCRM;
            //conditionNotNull.Operator = ConditionOperator.NotNull;
            //filter.Conditions.Add(conditionNotNull);

            //filter.FilterOperator = LogicalOperator.And;

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
                EntityCollection raizColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (raizColeccion.Entities.Count > 0)
                {
                    //2.Construir a partir de la lista normas raices CRM, el diccionario codigo-guid
                    foreach (Entity raizCRM in raizColeccion.Entities)
                        RaicesNormasCRM.Add(raizCRM.GetAttributeValue<string>(NombreCamposNormas.Aen_CodigoRaizNorma), raizCRM.Id);
                }

                if (raizColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = raizColeccion.PagingCookie;
                }
                else
                    break;
            }
            #endregion NORMAS RAIZ
        }

        private void LeeFromCRMVersiones()
        {
            #region NORMAS VERSION
            NormasCRM = new Dictionary<string, Normas>();
            VersionesNormasCRM = new Dictionary<string, Guid>();

            string[] camposN = {
                NombreCamposNormas.EntityIDVersion,
                NombreCamposNormas.Aen_Es_RatificadaCRM, NombreCamposNormas.Aen_Royalty_UneCRM,
                NombreCamposNormas.Aen_Royalty_OrganismoCRM, NombreCamposNormas.Aen_Identificador_NexoCRM,
                NombreCamposNormas.Aen_Fecha_EdicionCRM, NombreCamposNormas.Aen_Fecha_AnulacionCRM,
                NombreCamposNormas.Aen_Nu_PaginasCRM, NombreCamposNormas.Aen_Grupo_PrecioCRM,
                NombreCamposNormas.Aen_OrganismoCRM, NombreCamposNormas.Aen_ArticuloCRM,
                NombreCamposNormas.Aen_Organismo_NormaCRM, NombreCamposNormas.Aen_Formato_EspecialCRM,
                NombreCamposNormas.Aen_Organismo_InternacionalCRM, NombreCamposNormas.Aen_Organismo_GrupoCRM,
                NombreCamposNormas.Aen_EstadoCRM, NombreCamposNormas.Aen_Codigo_NormaCRM,
                NombreCamposNormas.Aen_Raiz_NormaCRM, NombreCamposNormas.Aen_Ambito_NormaCRM,
                NombreCamposNormas.Aen_Codigo_ComiteCRM, NombreCamposNormas.Aen_Titulo_Norma_ENCRM,
                NombreCamposNormas.Aen_Titulo_Norma_ESCRM, NombreCamposNormas.Aen_EstatusCRM,
                NombreCamposNormas.Aen_TipoNormaCRM
            };
            var entityName = NombreCamposNormas.EntityNameVersion;

            #region Query
            var filter = new FilterExpression();
            var conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombreCamposNormas.Aen_ArticuloCRM;
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            var query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposN);
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
                EntityCollection versionColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (versionColeccion.Entities.Count > 0)
                {
                    foreach (Entity versCRM in versionColeccion.Entities)
                    {
                        Normas v = new Normas();
                        v.VersionFromCRM(versCRM);
                        NormasCRM.Add(v.Aen_Articulo, v);
                        VersionesNormasCRM.Add(v.Aen_Articulo, versCRM.Id);
                    }
                }

                if (versionColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = versionColeccion.PagingCookie;
                }
                else
                    break;
            }
            #endregion NORMAS VERSION
        }

        private void CargaMaestroVersionesCRM()
        {
            #region NORMAS VERSION
            if(VersionesNormasCRM != null)
                VersionesNormasCRM.Clear();
            VersionesNormasCRM = new Dictionary<string, Guid>();

            string[] camposN = {NombreCamposNormas.EntityIDVersion, NombreCamposNormas.Aen_ArticuloCRM};
            var entityName = NombreCamposNormas.EntityNameVersion;

            #region Query
            var filter = new FilterExpression();
            var conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombreCamposNormas.Aen_ArticuloCRM;
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            var query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposN);
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
                EntityCollection versionColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (versionColeccion.Entities.Count > 0)
                {
                    foreach (Entity versCRM in versionColeccion.Entities)
                    {
                        VersionesNormasCRM.Add(versCRM.GetAttributeValue<string>(NombreCamposNormas.Aen_ArticuloCRM).ToString(), versCRM.Id);
                    }
                }

                if (versionColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = versionColeccion.PagingCookie;
                }
                else
                    break;
            }
            #endregion NORMAS VERSION
        }

        private void LeeFromCRMProductoseICS()
        {
            #region NORMAS-PRODUCTOS
            NormasProductosCRM = new Dictionary<string, NormasProductos>();

            string[] camposNP = {
                NombreCamposNormasProductos.EntityId,
                NombreCamposNormasProductos.Aen_Vendible_WebCRM, NombreCamposNormasProductos.Aen_Documento_ModCRM,
                NombreCamposNormasProductos.Aen_IdiomaCRM, NombreCamposNormasProductos.Aen_Fecha_DocumentoCRM,
                NombreCamposNormasProductos.Aen_PrecioCRM, NombreCamposNormasProductos.Aen_SoporteCRM,
                NombreCamposNormasProductos.Aen_ArticuloCRM, NombreCamposNormasProductos.Aen_Nombre_ProductoCRM,
                NombreCamposNormasProductos.Aen_DocumentoCRM, NombreCamposNormasProductos.Aen_PathCRM,
                NombreCamposNormasProductos.Aen_Url_OrganismoCRM, NombreCamposNormasProductos.Aen_Codigo_ProductoCRM
            };
            var entityName = NombreCamposNormasProductos.EntityName;

            #region Query
            var filter = new FilterExpression();
            var conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombreCamposNormasProductos.Aen_Codigo_ProductoCRM;
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);

            var query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposNP);
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
                EntityCollection npColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (npColeccion.Entities.Count > 0)
                {
                    foreach (Entity npCRM in npColeccion.Entities)
                    {
                        NormasProductos prod = new NormasProductos();
                        prod.NormasProductosFromCRM(npCRM);
                        NormasProductosCRM.Add(prod.Aen_Codigo_Producto, prod);
                    }
                }

                if (npColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = npColeccion.PagingCookie;
                }
                else
                    break;
            }
            #endregion NORMAS-PRODUCTOS

            #region NORMAS-ICS (relaciones)
            NormasICSCRM = new Dictionary<string, KeyValuePair<Guid, Guid>>();

            string[] camposVI = { NombresCamposNormasICS.RelationshipName + "id", NombresCamposNormasICS.EntityIDICS, NombresCamposNormasICS.EntityIDVersin };
            entityName = NombresCamposNormasICS.RelationshipName;

            #region Query
            filter = new FilterExpression();
            conditionNotNull = new ConditionExpression();
            conditionNotNull.AttributeName = NombresCamposNormasICS.EntityIDICS;
            conditionNotNull.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull);
            var conditionNotNull2 = new ConditionExpression();
            conditionNotNull2.AttributeName = NombresCamposNormasICS.EntityIDVersin;
            conditionNotNull2.Operator = ConditionOperator.NotNull;
            filter.Conditions.Add(conditionNotNull2);

            filter.FilterOperator = LogicalOperator.And;

            query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(camposVI);
            query.Criteria.AddFilter(filter);
            #endregion Query
            #region PagingCookie
            fetchCount = 5000;
            pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            #endregion PagingCookie

            Guid Ics = Guid.Empty;
            Guid Versin = Guid.Empty;

            while (true)
            {
                EntityCollection viColeccion = CrmGlobal.GetIOS().RetrieveMultiple(query);

                if (viColeccion.Entities.Count > 0)
                {
                    foreach (Entity vCRM in viColeccion.Entities)
                    {
                        Ics = vCRM.GetAttributeValue<Guid>(NombresCamposNormasICS.EntityIDICS);
                        Versin = vCRM.GetAttributeValue<Guid>(NombresCamposNormasICS.EntityIDVersin);
                        KeyValuePair<Guid, Guid> prod = new KeyValuePair<Guid, Guid>(Versin, Ics);

                        NormasICSCRM.Add(Versin.ToString() + "&" + Ics.ToString(),
                            prod);
                    }
                }

                if (viColeccion.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = viColeccion.PagingCookie;
                }
                else
                    break;
            }
            #endregion NORMAS-ICS (relaciones)
        }
        #endregion LECTURAS


        public void LimpiezaDiccionarios()
        {
            MaestroComitesCRM.Clear();
            MaestroICSCRM.Clear();
            NormasOracle.Clear();
            NormasCRM.Clear();
            NormasProductosOracle.Clear();
            NormasProductosCRM.Clear();
            NormasICSOracle.Clear();
            NormasICSCRM.Clear();

            RaicesNormas.Clear();
            RaicesNormasCRM.Clear();
            VersionesNormasCRM.Clear();
            IdiomasCRM.Clear();
            FormatoCRM.Clear();
            TercerosOrganismosCRM.Clear();
        }
    }
}
