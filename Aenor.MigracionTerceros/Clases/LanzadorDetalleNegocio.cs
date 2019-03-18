using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using Aenor.MigracionTerceros.Objetos;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.Xrm.Sdk.Messages;
using System.Diagnostics;

namespace Aenor.MigracionTerceros.Clases
{

    public class LanzadorDetalleNegocio
    {
        public Crm Crm { get; set; }
        public Comun Comun { get; set; }
        public Oracle Oracle { get; set; }

        public Dictionary<string, TerceroEs> TercerosEsOracle, TercerosEsCRM;
        public EvaluacionConformidad EvaluacionConformidad;
        public Dictionary<string, EvaluacionConformidad> EvaluacionesConformidadOracle, EvaluacionesConformidadCRM;
        public Dictionary<string, CompradorNormas> CompradorNormasOracle, CompradorNormasCRM;
        public Dictionary<string, CompradorLibros> CompradorLibrosOracle, CompradorLibrosCRM;
        public Dictionary<string, CliPotencialWeb> CliPotencialWebOracle, CliPotencialWebCRM;
        public Dictionary<string, Suscriptor> SuscriptorOracle, SuscriptorCRM;

        public void Iniciar(Oracle oracleGlobal, Comun comunGlobal, Crm crmGlobal)
        {
            try
            { 
                Comun = comunGlobal;
                Comun.LogText("----Iniciando sincronización Detalles Negocio~Terceros ------");
                Crm = crmGlobal;
                Oracle = oracleGlobal;
                Crm.InicializarPFE(oracleGlobal);

                Stopwatch sW_Detalles = new Stopwatch(), sW_SoloCarga = new Stopwatch();
                sW_Detalles.Start();
                //BorrarDetallesNegocio();

                //Diccionarios y maestros
                CachearInfoDetalleNegocio();
                LeerTerceroEsOracle();
                LeerDetalleNegocioOracle();


                #region Tratamiento ES Terceros
                if (!TercerosEsOracle.Any())
                {
                    Comun.LogText("No hay filas en Terceros ES en origen");
                }

                TerceroEs taux;
                sW_SoloCarga.Start();
                foreach (var terOra in TercerosEsOracle)
                {
                    try
                    {
                        taux = new TerceroEs();
                        bool ok = TercerosEsCRM.TryGetValue(terOra.Key, out taux);

                        if (ok) //Si existe el tercero. Tiene que existir :)! Sino, algo ha fallado en la carga anterior de Terceros
                        {
                            Entity upTes;
                            upTes = new Entity("account");
                            bool compruebaUpdate = CompruebaUpdateTerceroES(terOra.Value, taux, ref upTes);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = upTes });
                            else
                                Crm.Iguales++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion de comprobación de checks ES de Terceros " + terOra.Value.Aen_claveintegracion + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA TERCEROS ES: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("TERCEROS ES");
                //TODO Probar Tratar a posteriori de la carga para eliminar de CRM Aquellos que ya no estan de origen en oracle
                LimpiarDetallesNegocioCRMAntiguos();
                #endregion Tratamiento ES Terceros


                #region Tratamiento Evaluaciones Conformidad
                if (!EvaluacionesConformidadOracle.Any())
                {
                    Comun.LogText("No hay evaluaciones de la conformidad (certificaciones) en origen");
                }
            
                EvaluacionConformidad evAux;
                sW_SoloCarga.Start();
                //Crear/Actualizar en CRM
                foreach (var evConfORA in EvaluacionesConformidadOracle)
                {
                    try
                    {
                        evAux = new EvaluacionConformidad();
                        bool ok = EvaluacionesConformidadCRM.TryGetValue(evConfORA.Key, out evAux);

                        if (ok) //Existe en CRM. Comprobar para actualizar
                        {
                            Entity upEvConf;
                            upEvConf = new Entity(EvaluacionConformidad.EntityName);
                            bool compruebaUpdate = CompruebaUpdateEvaluacionConformidad(evConfORA.Value, evAux, ref upEvConf);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = upEvConf });
                            else
                                Crm.Iguales++;

                        }
                        else //Antonio 22 ene, se te había pasado poner el create
                        {
                            var creEvConf = GetEntityFromEvaluacionConformidad(evConfORA.Value);
                            Crm.AnadirElementoEmr(new CreateRequest { Target = creEvConf });
                        }

                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion el Entity equivalente de Evaluaciones de conformidad (Certificaciones). Clave Tercero relacionado: " + evConfORA.Value.ClaveTercero + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA EVALUACION CONFORMIDAD: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("EVALUACIONES CONFORMIDAD");
                #endregion Tratamiento Evaluaciones Conformidad


                #region Tratamiento Comprador de Normas
                if (!CompradorNormasOracle.Any())
                {
                    Comun.LogText("No hay registros de comprador de normas (normas adquiridas) en origen");
                }

                CompradorNormas comNAux;
                //Crear/Actualizar en CR
                sW_SoloCarga.Start();
                foreach (var comNormORA in CompradorNormasOracle)
                {
                    try
                    {
                        comNAux = new CompradorNormas();
                        bool ok = CompradorNormasCRM.TryGetValue(comNormORA.Key, out comNAux);

                        if (ok) //Existe en CRM. Comprobar para actualizar
                        {
                            Entity cnConf;
                            cnConf = new Entity(CompradorNormas.EntityName);
                            bool compruebaUpdate = CompruebaUpdateCompradorNormas(comNormORA.Value, comNAux, ref cnConf);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = cnConf });
                            else
                                Crm.Iguales++;

                        }
                        else //Antonio 22 ene, se te había pasado poner el create
                        {
                            var creComprNormas = GetEntityFromCompradorNormas(comNormORA.Value);
                            Crm.AnadirElementoEmr(new CreateRequest { Target = creComprNormas });
                        }
                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion el Entity equivalente de Comprador normas (Normas adquiridas): Clave tercero relacionado: " + comNormORA.Value.ClaveTercero + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA COMPRADOR NORMAS: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("COMPRADOR NORMAS");
                #endregion Tratamiento Comprador de Normas


                #region Tratamiento Comprador de Libros
                if (!CompradorLibrosOracle.Any())
                {
                    Comun.LogText("No hay registros de comprador de Libros (libros adquiridos) en origen");
                }

                CompradorLibros comLAux;
                //Crear/Actualizar en CRM
                sW_SoloCarga.Start();
                foreach (var comLibORA in CompradorLibrosOracle)
                {
                    try
                    {
                        comLAux = new CompradorLibros();
                        bool ok = CompradorLibrosCRM.TryGetValue(comLibORA.Key, out comLAux);

                        if (ok) //Existe en CRM. Comprobar para actualizar
                        {
                            Entity clConf;
                            clConf = new Entity(CompradorLibros.EntityName);
                            bool compruebaUpdate = CompruebaUpdateCompradorLibros(comLibORA.Value, comLAux, ref clConf);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = clConf });
                            else
                                Crm.Iguales++;

                        }
                        else //Antonio 22 ene, se te había pasado poner el create
                        {
                            var creComprLibros = GetEntityFromCompradorLibros(comLibORA.Value);
                            Crm.AnadirElementoEmr(new CreateRequest { Target = creComprLibros });
                        }
                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion el Entity equivalente de Comprador Libros (Libros adquiridos): Clave tercero relacionado: " + comLibORA.Value.ClaveTercero + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA COMPRADOR LIBROS: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("COMPRADOR LIBROS");
                #endregion Tratamiento Comprador de Libros


                #region Tratamiento Cliente Potencial W
                if (!CliPotencialWebOracle.Any())
                {
                    Comun.LogText("No hay registros de cliente potencial en origen");
                }

                CliPotencialWeb cliAux;
                //Crear/Actualizar en CRM
                sW_SoloCarga.Start();
                foreach (var cliPWORA in CliPotencialWebOracle)
                {
                    try
                    {
                        cliAux = new CliPotencialWeb();
                        bool ok = CliPotencialWebCRM.TryGetValue(cliPWORA.Key, out cliAux);

                        if (ok) //Existe en CRM. Comprobar para actualizar
                        {
                            Entity clConf;
                            clConf = new Entity(CliPotencialWeb.EntityName);
                            bool compruebaUpdate = CompruebaUpdateCliPotencialWeb(cliPWORA.Value, cliAux, ref clConf);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = clConf });
                            else
                                Crm.Iguales++;

                        }
                        else //Antonio 22 ene, se te había pasado poner el create
                        {
                            var creCliPot = GetEntityFromCliPotencialWeb(cliPWORA.Value);
                            Crm.AnadirElementoEmr(new CreateRequest { Target = creCliPot });
                        }
                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion el Entity equivalente de Cliente potencial: Clave tercero relacionado: " + cliPWORA.Value.ClaveTercero + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA CLIENTE WEB: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("CLIENTE POTENCIAL WEB");
                #endregion Tratamiento Cliente Potencial W


                #region Tratamiento Suscriptores
                if (!SuscriptorOracle.Any())
                {
                    Comun.LogText("No hay registros de Suscriptor en origen");
                }

                Suscriptor susAux;
                //Crear/Actualizar en CRM
                sW_SoloCarga.Start();
                foreach (var susORA in SuscriptorOracle)
                {
                    try
                    {
                        susAux = new Suscriptor();
                        bool ok = SuscriptorCRM.TryGetValue(susORA.Key, out susAux);

                        if (ok) //Existe en CRM. Comprobar para actualizar
                        {
                            Entity ssConf;
                            ssConf = new Entity(Suscriptor.EntityName);
                            bool compruebaUpdate = CompruebaUpdateSuscriptor(susORA.Value, susAux, ref ssConf);

                            if (compruebaUpdate)
                                Crm.AnadirElementoEmr(new UpdateRequest { Target = ssConf });
                            else
                                Crm.Iguales++;

                        }
                        else //Antonio 22 ene, se te había pasado poner el create
                        {
                            var creSus = GetEntityFromSuscriptor(susORA.Value);
                            Crm.AnadirElementoEmr(new CreateRequest { Target = creSus });
                        }
                    }
                    catch (Exception ex)
                    {
                        Comun.LogText("ERROR al realizar una operacion el Entity equivalente de Suscriptores: Clave tercero relacionado: " + susORA.Value.ClaveTercero + " ::: " + ex.ToString());
                    }
                }
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA SUSCRIPTORES: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                sW_SoloCarga = new Stopwatch();
                Crm.MostrarEstadisticas("SUSCRIPTORES");
                #endregion Tratamiento Suscriptores

                LiberaYTermina();

                sW_Detalles.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO TOTAL DETALLES DE NEGOCIO: " + sW_Detalles.Elapsed.ToString() + " <-----\n\n");
            }
            catch (Exception ex)
            {
                Comun.LogText("ERROR en Lanzador de DETALLES DE NEGOCIO ::: " + ex.ToString());
                if (Oracle != null && Oracle.OraConnParaLog.State == ConnectionState.Open)
                    Oracle.OraConnParaLog.Dispose();
            }
        }



        private void BorrarDetallesNegocio()
        {
            #region EV-Conformidad
            var l = new List<Entity>();
            var q = new QueryExpression("aen_certificacion");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_idcertificado", ConditionOperator.Null));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var ents = Crm.IOS.RetrieveMultiple(q);
                l.AddRange(ents.Entities);
                if (ents.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = ents.PagingCookie;
                }
                else
                    break;
            }
            foreach (var e in l)
                Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_certificacion", e.Id) });
            #endregion


            #region Compra normas
            l = new List<Entity>();
            q = new QueryExpression("aen_normacomprada");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_idcertificado", ConditionOperator.Null));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var ents = Crm.IOS.RetrieveMultiple(q);
                l.AddRange(ents.Entities);
                if (ents.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = ents.PagingCookie;
                }
                else
                    break;
            }
            foreach (var e in l)
                Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_normacomprada", e.Id) });
            #endregion


            #region Compra libros
            l = new List<Entity>();
            q = new QueryExpression("aen_publicacionesadquiridas");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_idcertificado", ConditionOperator.Null));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var ents = Crm.IOS.RetrieveMultiple(q);
                l.AddRange(ents.Entities);
                if (ents.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = ents.PagingCookie;
                }
                else
                    break;
            }
            foreach (var e in l)
                Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_publicacionesadquiridas", e.Id) });

            #endregion


            #region Potencial web
            l = new List<Entity>();
            q = new QueryExpression("aen_potencialcliente");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_idcertificado", ConditionOperator.Null));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var ents = Crm.IOS.RetrieveMultiple(q);
                l.AddRange(ents.Entities);
                if (ents.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = ents.PagingCookie;
                }
                else
                    break;
            }
            foreach (var e in l)
                Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_potencialcliente", e.Id) });

            #endregion


            #region Suscriptores
            l = new List<Entity>();
            q = new QueryExpression("aen_suscripcionadquirida");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_idcertificado", ConditionOperator.Null));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var ents = Crm.IOS.RetrieveMultiple(q);
                l.AddRange(ents.Entities);
                if (ents.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = ents.PagingCookie;
                }
                else
                    break;
            }
            foreach (var e in l)
                Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_suscripcionadquirida", e.Id) });

            #endregion

            Crm.ProcesarUltimosEmr();
        }


        private void LeerTerceroEsOracle()
        {
            OracleDataAdapter da = new OracleDataAdapter(Oracle.QueryTercerosES, Comun.ConnStringOracle);
            DataTable dt = new DataTable();
            da.Fill(dt);
            TercerosEsOracle = new Dictionary<string, TerceroEs>();
            foreach (DataRow fila in dt.Rows)
            {
                var ter = TerceroEsFromOracle(fila);
                if (ter == null)
                {
                    //Comun.LogText("evalConformidad de Oracle no válida: " + (string)fila["aen_claveintegracion"]);
                    continue;
                }
                TercerosEsOracle.Add(ter.Aen_claveintegracion, ter);
            }
        }


        private void LeerDetalleNegocioOracle()
        {
            #region Diccionario Evaluacion de la conformidad
            OracleDataAdapter da = new OracleDataAdapter(Oracle.QueryEvaluacionConformidad, Comun.ConnStringOracle);
            DataTable dt = new DataTable();
            da.Fill(dt);
            EvaluacionesConformidadOracle = new Dictionary<string, EvaluacionConformidad>();
            foreach (DataRow fila in dt.Rows)
            {
                var evalConformidad = EvaluacionConformidadFromOracle(fila);
                if (evalConformidad == null)
                {
                    //Comun.LogText("evalConformidad de Oracle no válida: " + (string)fila["aen_claveintegracion"]);
                    continue;
                }
                EvaluacionesConformidadOracle.Add(evalConformidad.ClaveCertificado, evalConformidad);
            }
            #endregion Diccionario Evaluacion de la conformidad

            #region Diccionario Comprador Normas
            da = new OracleDataAdapter(Oracle.QueryCompradorNormas, Comun.ConnStringOracle);
            dt = new DataTable();
            da.Fill(dt);
            CompradorNormasOracle = new Dictionary<string, CompradorNormas>();
            foreach (DataRow fila in dt.Rows)
            {
                var compraNorma = CompradorNormasFromOracle(fila);
                if (compraNorma == null)
                    continue;
                CompradorNormasOracle.Add(compraNorma.Aen_ClaveNorma, compraNorma);
            }
            #endregion Diccionario Comprador Normas

            #region Diccionario Comprador Libros
            da = new OracleDataAdapter(Oracle.QueryCompradorLibros, Comun.ConnStringOracle);
            dt = new DataTable();
            da.Fill(dt);
            CompradorLibrosOracle = new Dictionary<string, CompradorLibros>();
            foreach (DataRow fila in dt.Rows)
            {
                var compraLibro = CompradorLibrosFromOracle(fila);
                if (compraLibro == null)
                    continue;
                CompradorLibrosOracle.Add(compraLibro.Aen_ClaveLibros, compraLibro);
            }
            #endregion Diccionario Comprador Libros

            #region Diccionario Cliente Potencial Web
            da = new OracleDataAdapter(Oracle.QueryClientePotencial, Comun.ConnStringOracle);
            dt = new DataTable();
            da.Fill(dt);
            CliPotencialWebOracle = new Dictionary<string, CliPotencialWeb>();
            foreach (DataRow fila in dt.Rows)
            {
                var cliPot = CliPotencialWebFromOracle(fila);
                if (cliPot == null)
                    continue;
                CliPotencialWebOracle.Add(cliPot.Aen_ClavePotencial, cliPot);
            }
            #endregion Diccionario Cliente Potencial Web

            #region Diccionario Suscriptores
            da = new OracleDataAdapter(Oracle.QuerySuscriptores, Comun.ConnStringOracle);
            dt = new DataTable();
            da.Fill(dt);
            SuscriptorOracle = new Dictionary<string, Suscriptor>();
            foreach (DataRow fila in dt.Rows)
            {
                var sus = SuscriptorFromOracle(fila);
                if (sus == null)
                    continue;
                SuscriptorOracle.Add(sus.Aen_ClaveSuscriptor, sus);
            }
            #endregion Diccionario Suscriptores
        }






        private void CachearInfoDetalleNegocio()
        {
            TercerosEsCRM = new Dictionary<string, TerceroEs>();
            EvaluacionesConformidadCRM = new Dictionary<string, EvaluacionConformidad>();
            CompradorNormasCRM = new Dictionary<string, CompradorNormas>();
            CompradorLibrosCRM = new Dictionary<string, CompradorLibros>();
            CliPotencialWebCRM = new Dictionary<string, CliPotencialWeb>();
            SuscriptorCRM = new Dictionary<string, Suscriptor>();
            Comun.LogText("Leyendo entidades de CRM...");

            //CrmServiceClient conn = new CrmServiceClient(Comun.ConnStringCrm);
            //Crm.IOS = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            //Crm.InicializarPFE();

            #region Crea Diccionario TercerosES
            var q = new QueryExpression("account");
            q.ColumnSet = new ColumnSet(NombresCamposTerceroEs.Aen_claveintegracionCRM,
                NombresCamposTerceroEs.Aen_evaluaciondelaconformidadCRM, NombresCamposTerceroEs.Aen_essuscriptorCRM, NombresCamposTerceroEs.Aen_escompradornormasCRM,
                NombresCamposTerceroEs.Aen_escompradorlibrosCRM, NombresCamposTerceroEs.Aen_espotencialclienteCRM,
                NombresCamposTerceroEs.Aen_esmiembroctcCRM,
                //NombresCamposTerceroEs.Aen_esmiembrouneCRM, NombresCamposTerceroEs.Aen_esorganismoCRM, 
                NombresCamposTerceroEs.Aen_revistaaenorCRM
                );
            q.Criteria.Conditions.AddRange(
                //new ConditionExpression(NombreCamposTercero.StatecodeCRM, ConditionOperator.Equal, 0),
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var t in entidades.Entities)
                {
                    var terceroEs = TerceroEsFromCrm(t);
                    TercerosEsCRM.Add((string)t[NombreCamposTercero.Aen_claveintegracionCRM], terceroEs);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            #endregion Crea Diccionario TercerosES


            CreateDiccionarioEvaluacionConformidad();
            CreateDiccionarioCompradorNormas();
            CreateDiccionarioCompradorLibros();
            CreateDiccionarioCliPotencialWeb();
            CreateDiccionarioSuscriptores();
        }


        private void DeleteDetalleNegocioCRM(Guid accountid, string NombreEntidad)
        {
            var queryDel = new QueryExpression(NombreEntidad);

            queryDel.ColumnSet.AddColumn(NombreEntidad + "id");
            if(NombreEntidad.Equals(Suscriptor.EntityName) || NombreEntidad.Equals(CompradorLibros.EntityName)) //aen_tercero
                queryDel.Criteria.AddCondition(new ConditionExpression(NombresCamposSuscriptor.IdTerceroCRM, ConditionOperator.Equal, accountid));
            else //aen_terceroid
                queryDel.Criteria.AddCondition(new ConditionExpression(NombresCamposCompradorNormas.IdTerceroCRM, ConditionOperator.Equal, accountid));
            EntityCollection entt = Crm.IOS.RetrieveMultiple(queryDel);

            if (entt != null && entt.Entities.Count > 0)
            {
                foreach (var e in entt.Entities)
                    Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference(NombreEntidad, e.Id) });
            }
        }


        private void LimpiarDetallesNegocioCRMAntiguos()
        {
            TerceroEs tes;

            foreach (var tCrm in TercerosEsCRM)
            {
                tes = new TerceroEs();
                bool ok = TercerosEsOracle.TryGetValue(tCrm.Key, out tes);

                if (!ok && tCrm.Value.Aen_evaluaciondelaconformidad)
                {
                    //El check del tercero tiene que ponerse a false antes de eliminar su registro relacionado.
                    var tercero = new Entity("account");
                    tercero[NombresCamposTerceroEs.AccountidCRM] = tCrm.Value.Accountid;
                    tercero[NombresCamposTerceroEs.Aen_evaluaciondelaconformidadCRM] = false;
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = tercero });
                    DeleteDetalleNegocioCRM(tCrm.Value.Accountid, EvaluacionConformidad.EntityName);
                }
                if (!ok && tCrm.Value.Aen_escompradornormas)
                {
                    //El check del tercero tiene que ponerse a false antes de eliminar su registro relacionado.
                    var tercero = new Entity("account");
                    tercero[NombresCamposTerceroEs.AccountidCRM] = tCrm.Value.Accountid;
                    tercero[NombresCamposTerceroEs.Aen_escompradornormasCRM] = false;
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = tercero });
                    DeleteDetalleNegocioCRM(tCrm.Value.Accountid, CompradorNormas.EntityName);
                }
                if (!ok && tCrm.Value.Aen_escompradorlibros)
                {
                    //El check del tercero tiene que ponerse a false antes de eliminar su registro relacionado.
                    var tercero = new Entity("account");
                    tercero[NombresCamposTerceroEs.AccountidCRM] = tCrm.Value.Accountid;
                    tercero[NombresCamposTerceroEs.Aen_escompradorlibrosCRM] = false;
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = tercero });
                    DeleteDetalleNegocioCRM(tCrm.Value.Accountid, CompradorLibros.EntityName);
                }
                if (!ok && tCrm.Value.Aen_espotencialcliente)
                {
                    //El check del tercero tiene que ponerse a false antes de eliminar su registro relacionado.
                    var tercero = new Entity("account");
                    tercero[NombresCamposTerceroEs.AccountidCRM] = tCrm.Value.Accountid;
                    tercero[NombresCamposTerceroEs.Aen_espotencialclienteCRM] = false;
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = tercero });
                    DeleteDetalleNegocioCRM(tCrm.Value.Accountid, CliPotencialWeb.EntityName);
                }
                if (!ok && tCrm.Value.Aen_essuscriptor)
                {
                    //El check del tercero tiene que ponerse a false antes de eliminar su registro relacionado.
                    var tercero = new Entity("account");
                    tercero[NombresCamposTerceroEs.AccountidCRM] = tCrm.Value.Accountid;
                    tercero[NombresCamposTerceroEs.Aen_essuscriptorCRM] = false;
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = tercero });
                    DeleteDetalleNegocioCRM(tCrm.Value.Accountid, Suscriptor.EntityName);
                }
            }
        }


        private void LiberaYTermina()
        {
            TercerosEsOracle = null; //.Clear();
            TercerosEsCRM = null; //.Clear();
            EvaluacionesConformidadOracle = null; //.Clear();
            EvaluacionesConformidadCRM = null; //.Clear();
            CompradorNormasOracle = null; //.Clear();
            CompradorNormasCRM = null; //.Clear();
            CompradorLibrosOracle = null; //.Clear();
            CompradorLibrosCRM = null; //.Clear();
            CliPotencialWebOracle = null; //.Clear();
            CliPotencialWebCRM = null; //.Clear();
            SuscriptorOracle = null; //.Clear();
            SuscriptorCRM = null; //.Clear();

            Oracle.CierraConexionOracle();
        }







        #region Operaciones Objetos TERCEROS ES
        private TerceroEs TerceroEsFromOracle(DataRow fila)
        {
            var terEs = new TerceroEs();
            var terAux = new TerceroEs();

            terEs.Aen_claveintegracion = fila[NombresCamposTerceroEs.Aen_claveintegracionORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposTerceroEs.Aen_claveintegracionORACLE];

            var ok = TercerosEsCRM.TryGetValue(terEs.Aen_claveintegracion, out terAux);
            if (ok)
                terEs.Accountid = terAux.Accountid;
            else
                terEs.Accountid = Guid.Empty;
            
            terEs.Aen_evaluaciondelaconformidad = fila[NombresCamposTerceroEs.Aen_evaluaciondelaconformidadORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_evaluaciondelaconformidadORACLE] == 1;
            terEs.Aen_espotencialcliente = fila[NombresCamposTerceroEs.Aen_espotencialclienteORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_espotencialclienteORACLE] == 1;
            terEs.Aen_essuscriptor = fila[NombresCamposTerceroEs.Aen_essuscriptorORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_essuscriptorORACLE] == 1;
            terEs.Aen_escompradornormas = fila[NombresCamposTerceroEs.Aen_escompradornormasORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_escompradornormasORACLE] == 1;
            terEs.Aen_escompradorlibros = fila[NombresCamposTerceroEs.Aen_escompradorlibrosORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_escompradorlibrosORACLE] == 1;
            terEs.Aen_esmiembroctc = fila[NombresCamposTerceroEs.Aen_esmiembroctcORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_esmiembroctcORACLE] == 1;
            //terEs.Aen_esmiembroune = fila[NombresCamposTerceroEs.Aen_esmiembrouneORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_esmiembrouneORACLE] == 1;
            //terEs.Aen_esorganismo = fila[NombresCamposTerceroEs.Aen_esorganismoORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_esorganismoORACLE] == 1;
            terEs.Aen_revistaaenor = fila[NombresCamposTerceroEs.Aen_revistaaenorORACLE] == DBNull.Value ? false : (decimal)fila[NombresCamposTerceroEs.Aen_revistaaenorORACLE] == 1;

            return terEs;
        }

        private TerceroEs TerceroEsFromCrm(Entity tercer)
        {
            var ter = new TerceroEs();
            ter.Accountid = tercer.Id;
            ter.Aen_claveintegracion = tercer.Contains(NombresCamposTerceroEs.Aen_claveintegracionCRM) ? tercer.GetAttributeValue<string>(NombresCamposTerceroEs.Aen_claveintegracionCRM) : string.Empty;
            ter.Aen_evaluaciondelaconformidad = tercer.Contains(NombresCamposTerceroEs.Aen_evaluaciondelaconformidadCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_evaluaciondelaconformidadCRM) : false;
            ter.Aen_espotencialcliente = tercer.Contains(NombresCamposTerceroEs.Aen_espotencialclienteCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_espotencialclienteCRM) : false;
            ter.Aen_essuscriptor = tercer.Contains(NombresCamposTerceroEs.Aen_essuscriptorCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_essuscriptorCRM) : false;
            ter.Aen_escompradornormas = tercer.Contains(NombresCamposTerceroEs.Aen_escompradornormasCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_escompradornormasCRM) : false;
            ter.Aen_escompradorlibros = tercer.Contains(NombresCamposTerceroEs.Aen_escompradorlibrosCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_escompradorlibrosCRM) : false;
            ter.Aen_esmiembroctc = tercer.Contains(NombresCamposTerceroEs.Aen_esmiembroctcCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_esmiembroctcCRM) : false;
            //ter.Aen_esmiembroune = tercer.Contains(NombresCamposTerceroEs.Aen_esmiembrouneCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_esmiembrouneCRM) : false;
            //ter.Aen_esorganismo = tercer.Contains(NombresCamposTerceroEs.Aen_esorganismoCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_esorganismoCRM) : false;
            ter.Aen_revistaaenor = tercer.Contains(NombresCamposTerceroEs.Aen_revistaaenorCRM) ? tercer.GetAttributeValue<bool>(NombresCamposTerceroEs.Aen_revistaaenorCRM) : false;

            return ter;
        }

        private Entity GetEntityFromTerceroES(TerceroEs tes)
        {
            Entity terEnt = new Entity("account");

            if (tes.Accountid != null && tes.Accountid != Guid.Empty)
            {
                terEnt.Id = tes.Accountid;
                terEnt["accountid"] = tes.Accountid;
            }

            if (!tes.Aen_claveintegracion.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_claveintegracionCRM] = tes.Aen_claveintegracion;
            if (!tes.Aen_evaluaciondelaconformidad.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_evaluaciondelaconformidadCRM] = tes.Aen_evaluaciondelaconformidad;
            if (!tes.Aen_espotencialcliente.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_espotencialclienteCRM] = tes.Aen_espotencialcliente;
            if (!tes.Aen_essuscriptor.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_essuscriptorCRM] = tes.Aen_essuscriptor;
            if (!tes.Aen_escompradornormas.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_escompradornormasCRM] = tes.Aen_escompradornormas;
            if (!tes.Aen_escompradorlibros.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_escompradorlibrosCRM] = tes.Aen_escompradorlibros;
            if (!tes.Aen_esmiembroctc.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_esmiembroctcCRM] = tes.Aen_esmiembroctc;
            //if (!tes.Aen_esmiembroune.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_esmiembrouneCRM] = tes.Aen_esmiembroune;
            //if (!tes.Aen_esorganismo.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_esorganismoCRM] = tes.Aen_esorganismo;
            if (!tes.Aen_revistaaenor.Equals(string.Empty)) terEnt[NombresCamposTerceroEs.Aen_revistaaenorCRM] = tes.Aen_revistaaenor;

            //Para saltar plugins de envío de datos a NEXO
            //terEnt["aen_vienedeintegracion"] = true;

            return terEnt;
        }

        private bool CompruebaUpdateTerceroES(TerceroEs tOra, TerceroEs tCrm, ref Entity updateTes)
        {
            bool actualizar = false;
            //Comprobar todos los checks. Esta en Oracle y no en CRM --> Añadir. Esta en CRM y no en ORACLE --> Eliminar de CRM el registro de entidad detalle

            if (!tOra.Aen_evaluaciondelaconformidad.Equals(tCrm.Aen_evaluaciondelaconformidad))
            {
                if (!tOra.Aen_evaluaciondelaconformidad && tCrm.Aen_evaluaciondelaconformidad)
                    DeleteDetalleNegocioCRM(tCrm.Accountid, EvaluacionConformidad.EntityName);

                actualizar = true;
            }

            if (!tOra.Aen_espotencialcliente.Equals(tCrm.Aen_espotencialcliente)) actualizar = true;
            if (!tOra.Aen_essuscriptor.Equals(tCrm.Aen_essuscriptor)) actualizar = true;
            if (!tOra.Aen_escompradornormas.Equals(tCrm.Aen_escompradornormas)) actualizar = true;
            if (!tOra.Aen_escompradorlibros.Equals(tCrm.Aen_escompradorlibros)) actualizar = true;
            if (!tOra.Aen_esmiembroctc.Equals(tCrm.Aen_esmiembroctc)) actualizar = true;
            //if (!tOra.Aen_esmiembroune.Equals(tCrm.Aen_esmiembroune)) actualizar = true;
            //if (!tOra.Aen_esorganismo.Equals(tCrm.Aen_esorganismo)) actualizar = true;
            if (!tOra.Aen_revistaaenor.Equals(tCrm.Aen_revistaaenor)) actualizar = true;

            if (actualizar)
            {
                tOra.Accountid = tCrm.Accountid;
                updateTes = GetEntityFromTerceroES(tOra);
            }
            return actualizar;
        }

        #endregion Operaciones Objetos TERCEROS ES


        #region Operaciones Objetos EVALUACION CONFORMIDAD
        private void CreateDiccionarioEvaluacionConformidad()
        {
            var q = new QueryExpression(EvaluacionConformidad.EntityName);
            q.ColumnSet = new ColumnSet(new string[] {
                NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM,
                NombresCamposEvaluacionConformidad.SubNormaSPCCRM, NombresCamposEvaluacionConformidad.IdSubexpedienteCRM,
                NombresCamposEvaluacionConformidad.SubexpedienteCRM, NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM,
                NombresCamposEvaluacionConformidad.FechaEstadoCRM,
                NombresCamposEvaluacionConformidad.CodigoCertificadoCRM,
                NombresCamposEvaluacionConformidad.ClaveTerceroCRM, NombresCamposEvaluacionConformidad.ClaveCertificadoCRM,
                "statecode" });

            q.Criteria.Conditions.AddRange(
                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression(NombresCamposEvaluacionConformidad.ClaveCertificadoCRM, ConditionOperator.NotNull),
                new ConditionExpression(NombresCamposEvaluacionConformidad.TerceroIdCRM, ConditionOperator.NotNull)
                );

            var leTerc = new LinkEntity(EvaluacionConformidad.EntityName, "account",
                NombresCamposEvaluacionConformidad.TerceroIdCRM, "accountid", JoinOperator.Inner);
            leTerc.Columns = new ColumnSet(new string[] { NombreCamposTercero.Aen_claveintegracionCRM, "accountid" });
            leTerc.LinkCriteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull)
                );

            q.LinkEntities.AddRange(leTerc);

            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var eva in entidades.Entities)
                {
                    var evco = new EvaluacionConformidad();
                    evco.CertificacionId = eva.Id;
                    evco.ClaveTercero = ((AliasedValue)eva["account1." + NombreCamposTercero.Aen_claveintegracionCRM]).Value.ToString();
                    evco.IdTercero = new Guid(((AliasedValue)eva["account1.accountid"]).Value.ToString());
                    evco.NormaCertificadaCTC = eva.Contains(NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM) : string.Empty;
                    evco.SubNormaSPC = eva.Contains(NombresCamposEvaluacionConformidad.SubNormaSPCCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.SubNormaSPCCRM) : string.Empty;
                    evco.Subexpediente = eva.Contains(NombresCamposEvaluacionConformidad.SubexpedienteCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.SubexpedienteCRM) : string.Empty;
                    evco.IdSubexpediente = eva.Contains(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM) ? eva.GetAttributeValue<int>(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM).ToString() : string.Empty;
                    evco.ClaveCertificado = eva.Contains(NombresCamposEvaluacionConformidad.ClaveCertificadoCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.ClaveCertificadoCRM) : string.Empty;
                    evco.CodigoCertificado = eva.Contains(NombresCamposEvaluacionConformidad.CodigoCertificadoCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.CodigoCertificadoCRM) : string.Empty;
                    evco.Estado = eva.Contains(NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM) ? eva.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM) : string.Empty;
                    evco.FechaEstado = eva.Contains(NombresCamposEvaluacionConformidad.FechaEstadoCRM) ?
                        eva.GetAttributeValue<DateTime>(NombresCamposEvaluacionConformidad.FechaEstadoCRM).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;

                    EvaluacionesConformidadCRM.Add(evco.ClaveCertificado, evco);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
        }

        private EvaluacionConformidad EvaluacionConformidadFromOracle(DataRow fila)
        {
            var evalConformidad = new EvaluacionConformidad();
            evalConformidad.CertificacionId = Guid.Empty;
            //evalConformidad.IdCertificado = fila[NombresCamposEvaluacionConformidad.IdCertificadoORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.IdCertificadoORACLE];
            evalConformidad.ClaveCertificado = (string)fila[NombresCamposEvaluacionConformidad.ClaveCertificadoORACLE];
            evalConformidad.CodigoCertificado = fila[NombresCamposEvaluacionConformidad.CodigoCertificadoORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.CodigoCertificadoORACLE];
            evalConformidad.NormaCertificadaCTC = fila[NombresCamposEvaluacionConformidad.NormaCertificadaCTCORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.NormaCertificadaCTCORACLE];
            evalConformidad.SubNormaSPC = fila[NombresCamposEvaluacionConformidad.SubNormaSPCORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.SubNormaSPCORACLE];
            evalConformidad.IdSubexpediente = fila[NombresCamposEvaluacionConformidad.IdSubexpedienteORACLE] == DBNull.Value ? string.Empty : ((decimal)fila[NombresCamposEvaluacionConformidad.IdSubexpedienteORACLE]).ToString();
            evalConformidad.Subexpediente = fila[NombresCamposEvaluacionConformidad.SubexpedienteORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.SubexpedienteORACLE];
            evalConformidad.Estado = fila[NombresCamposEvaluacionConformidad.EstadoORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.EstadoORACLE];
            evalConformidad.FechaEstado = fila[NombresCamposEvaluacionConformidad.FechaEstadoORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposEvaluacionConformidad.FechaEstadoORACLE]).ToString("dd/MM/yyyy");
            evalConformidad.ClaveTercero = fila[NombresCamposEvaluacionConformidad.ClaveTerceroORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposEvaluacionConformidad.ClaveTerceroORACLE];

            if (evalConformidad.ClaveTercero != string.Empty)
            {
                TerceroEs teraux = new TerceroEs();
                var okter = TercerosEsCRM.TryGetValue(evalConformidad.ClaveTercero, out teraux);
                if (okter)
                    evalConformidad.IdTercero = teraux.Accountid;
                else
                    evalConformidad.IdTercero = Guid.Empty;
            }

            EvaluacionConformidad aux = new EvaluacionConformidad();
            var ok = EvaluacionesConformidadCRM.TryGetValue(evalConformidad.ClaveCertificado, out aux);
            if (!ok)
                //evalConformidad.IdTercero = Guid.Empty;
                evalConformidad.CertificacionId = Guid.Empty;
            else
                //evalConformidad.IdTercero = aux.Accountid;
                evalConformidad.CertificacionId = aux.CertificacionId;

            return evalConformidad;
        }

        private EvaluacionConformidad EvaluacionConformidadFromCrm(Entity cert)
        {
            var ter = new EvaluacionConformidad();
            ter.CertificacionId = cert.Id;
            ter.ClaveCertificado = cert.Contains(NombresCamposEvaluacionConformidad.ClaveCertificadoCRM) ? cert.GetAttributeValue<int>(NombresCamposEvaluacionConformidad.ClaveCertificadoCRM).ToString(): string.Empty;
            ter.IdSubexpediente = cert.Contains(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM) ? cert.GetAttributeValue<int>(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM).ToString() : string.Empty;
            ter.CodigoCertificado = cert.Contains(NombresCamposEvaluacionConformidad.CodigoCertificadoCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.CodigoCertificadoCRM) : string.Empty;
            ter.NormaCertificadaCTC = cert.Contains(NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM) : string.Empty;
            ter.SubNormaSPC = cert.Contains(NombresCamposEvaluacionConformidad.SubNormaSPCCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.SubNormaSPCCRM): string.Empty;
            ter.Subexpediente = cert.Contains(NombresCamposEvaluacionConformidad.SubexpedienteCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.SubexpedienteCRM) : string.Empty;
            ter.IdSubexpediente = cert.Contains(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.IdSubexpedienteCRM) : string.Empty;
            ter.Estado = cert.Contains(NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM) : string.Empty;
            ter.FechaEstado = cert.Contains(NombresCamposEvaluacionConformidad.FechaEstadoCRM) ? cert.GetAttributeValue<DateTime>(NombresCamposEvaluacionConformidad.FechaEstadoCRM).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
            ter.ClaveTercero = cert.Contains(NombresCamposEvaluacionConformidad.ClaveTerceroCRM) ? cert.GetAttributeValue<string>(NombresCamposEvaluacionConformidad.ClaveTerceroCRM) : string.Empty;
            ter.IdTercero = cert.Contains(NombresCamposEvaluacionConformidad.TerceroIdCRM) ? ((EntityReference)cert.GetAttributeValue<EntityReference>(NombresCamposEvaluacionConformidad.TerceroIdCRM)).Id : Guid.Empty;

            return ter;
        }

        private Entity GetEntityFromEvaluacionConformidad(EvaluacionConformidad ev)
        {
            Entity entCert = new Entity(EvaluacionConformidad.EntityName);

            if (ev.CertificacionId != null && ev.CertificacionId != Guid.Empty)
            { //Update de registro
                entCert.Id = ev.CertificacionId;
                entCert[NombresCamposEvaluacionConformidad.CertificacionIdCRM] = ev.CertificacionId;
            }

            entCert[NombresCamposEvaluacionConformidad.ClaveCertificadoCRM] = ev.ClaveCertificado;
            if (!ev.NormaCertificadaCTC.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.NormaCertificadaCTCCRM] = ev.NormaCertificadaCTC;
            if (!ev.SubNormaSPC.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.SubNormaSPCCRM] = ev.SubNormaSPC;
            if (!ev.Subexpediente.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.SubexpedienteCRM] = ev.Subexpediente;
            if (!ev.IdSubexpediente.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.IdSubexpedienteCRM] = int.Parse(ev.IdSubexpediente);
            if (!ev.CodigoCertificado.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.CodigoCertificadoCRM] = ev.CodigoCertificado;
            if (!ev.Estado.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.EstadoDelCertificadoCRM] = ev.Estado;
            //if (!ev.FechaEstado.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.FechaEstadoCRM] = DateTime.ParseExact(ev.FechaEstado, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            if (!ev.FechaEstado.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(ev.FechaEstado, out DateTime f);
                if (okFecha)
                    entCert[NombresCamposEvaluacionConformidad.FechaEstadoCRM] = f;
            }

            if (ev.IdTercero != Guid.Empty) entCert[NombresCamposEvaluacionConformidad.TerceroIdCRM] = new EntityReference("account", ev.IdTercero);
            if (!ev.ClaveTercero.Equals(string.Empty)) entCert[NombresCamposEvaluacionConformidad.ClaveTerceroCRM] = ev.ClaveTercero;

            return entCert;
        }

        private bool CompruebaUpdateEvaluacionConformidad(EvaluacionConformidad evOra, EvaluacionConformidad evCrm, ref Entity updateFinal)
        {
            bool actualizar = false;

            if (!evOra.ClaveCertificado.Equals(evCrm.ClaveCertificado)) actualizar = true;
            if (!evOra.NormaCertificadaCTC.Equals(evCrm.NormaCertificadaCTC)) actualizar = true;
            if (!evOra.SubNormaSPC.Equals(evCrm.SubNormaSPC)) actualizar = true;
            if (!evOra.Subexpediente.Equals(evCrm.Subexpediente)) actualizar = true;
            if (!evOra.IdSubexpediente.Equals(evCrm.IdSubexpediente)) actualizar = true;
            if (!evOra.CodigoCertificado.Equals(evCrm.CodigoCertificado)) actualizar = true;
            if (!evOra.Estado.Equals(evCrm.Estado)) actualizar = true;
            if (!evOra.FechaEstado.Equals(evCrm.FechaEstado)) actualizar = true;

            if (actualizar)
            {
                evOra.CertificacionId = evCrm.CertificacionId;
                updateFinal = GetEntityFromEvaluacionConformidad(evOra);
            }
            return actualizar;
        }

        #endregion Operaciones Objetos EVALUACION CONFORMIDAD


        #region Operaciones Objetos COMPRADOR NORMAS
        private void CreateDiccionarioCompradorNormas()
        {
            var q = new QueryExpression(CompradorNormas.EntityName);
            q.ColumnSet = new ColumnSet(new string[] {
                NombresCamposCompradorNormas.Aen_ClaveNormaCRM, NombresCamposCompradorNormas.Aen_CantidadCRM,
                NombresCamposCompradorNormas.Aen_CodigoCTNCRM, NombresCamposCompradorNormas.Aen_DescripcionCTNCRM,
                NombresCamposCompradorNormas.Aen_CodigoarticuloCRM , NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM,
                NombresCamposCompradorNormas.Aen_TituloCRM , NombresCamposCompradorNormas.Aen_ImporteCRM ,
                NombresCamposCompradorNormas.ClaveTerceroCRM, NombresCamposCompradorNormas.IdTerceroCRM ,
                "statecode" });

            q.Criteria.Conditions.AddRange(
                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression(NombresCamposCompradorNormas.Aen_ClaveNormaCRM, ConditionOperator.NotNull) );

            var leTerc = new LinkEntity(CompradorNormas.EntityName, "account",
                NombresCamposCompradorNormas.IdTerceroCRM, "accountid", JoinOperator.Inner);
            leTerc.Columns = new ColumnSet(new string[] { NombreCamposTercero.Aen_claveintegracionCRM, "accountid" });
            leTerc.LinkCriteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull)
                );

            q.LinkEntities.AddRange(leTerc);

            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var eva in entidades.Entities)
                {
                    var evco = new CompradorNormas();
                    evco.CompradorNormasId = eva.Id;
                    evco.ClaveTercero = ((AliasedValue)eva["account1." + NombreCamposTercero.Aen_claveintegracionCRM]).Value.ToString();
                    evco.IdTercero = new Guid(((AliasedValue)eva["account1.accountid"]).Value.ToString());
                    evco.Aen_ClaveNorma = eva.Contains(NombresCamposCompradorNormas.Aen_ClaveNormaCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_ClaveNormaCRM) : string.Empty;
                    evco.Aen_CodigoCTN = eva.Contains(NombresCamposCompradorNormas.Aen_CodigoCTNCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_CodigoCTNCRM) : string.Empty;
                    evco.Aen_DescripcionCTN = eva.Contains(NombresCamposCompradorNormas.Aen_DescripcionCTNCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_DescripcionCTNCRM) : string.Empty;
                    evco.Aen_Codigoarticulo = eva.Contains(NombresCamposCompradorNormas.Aen_CodigoarticuloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_CodigoarticuloCRM) : string.Empty;
                    evco.Aen_Descripcionarticulo = eva.Contains(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) : string.Empty;
                    evco.Aen_Titulo = eva.Contains(NombresCamposCompradorNormas.Aen_TituloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_TituloCRM) : string.Empty;
                    evco.Aen_Importe = eva.Contains(NombresCamposCompradorNormas.Aen_ImporteCRM) ? eva.GetAttributeValue<decimal>(NombresCamposCompradorNormas.Aen_ImporteCRM) : decimal.Zero;
                    evco.Aen_Cantidad = eva.Contains(NombresCamposCompradorNormas.Aen_CantidadCRM) ? eva.GetAttributeValue<int>(NombresCamposCompradorNormas.Aen_CantidadCRM) : int.MinValue;

                    CompradorNormasCRM.Add(evco.Aen_ClaveNorma, evco);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
        }

        private CompradorNormas CompradorNormasFromOracle(DataRow fila)
        {
            var compraN = new CompradorNormas();
            compraN.CompradorNormasId = Guid.Empty;
            compraN.ClaveTercero = fila[NombresCamposCompradorNormas.ClaveTerceroORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.ClaveTerceroORACLE];
            compraN.Aen_ClaveNorma = fila[NombresCamposCompradorNormas.Aen_ClaveNormaORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_ClaveNormaORACLE];
            compraN.Aen_CodigoCTN = fila[NombresCamposCompradorNormas.Aen_CodigoCTNORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_CodigoCTNORACLE];
            compraN.Aen_DescripcionCTN = fila[NombresCamposCompradorNormas.Aen_DescripcionCTNORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_DescripcionCTNORACLE];
            compraN.Aen_Codigoarticulo = fila[NombresCamposCompradorNormas.Aen_CodigoarticuloORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_CodigoarticuloORACLE];
            compraN.Aen_Descripcionarticulo = fila[NombresCamposCompradorNormas.Aen_DescripcionarticuloORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_DescripcionarticuloORACLE];
            compraN.Aen_Titulo = fila[NombresCamposCompradorNormas.Aen_TituloORACLE] == DBNull.Value ? string.Empty : (string)fila[NombresCamposCompradorNormas.Aen_TituloORACLE];
            compraN.Aen_Importe = fila[NombresCamposCompradorNormas.Aen_ImporteORACLE] == DBNull.Value ? decimal.Zero : Convert.ToDecimal(fila[NombresCamposCompradorNormas.Aen_ImporteORACLE]);
            compraN.Aen_Cantidad = fila[NombresCamposCompradorNormas.Aen_CantidadORACLE] == DBNull.Value ? int.MinValue: Convert.ToInt16(fila[NombresCamposCompradorNormas.Aen_CantidadORACLE]);


            TerceroEs aux = new TerceroEs();
            var ok = TercerosEsCRM.TryGetValue(compraN.ClaveTercero, out aux);
            if (!ok)
                compraN.IdTercero = Guid.Empty;
            else
                compraN.IdTercero = aux.Accountid;

            return compraN;
        }

        private CompradorNormas CompradorNormasFromCrm(Entity cert)
        {
            var ter = new CompradorNormas();

            ter.CompradorNormasId = cert.Id;
            ter.ClaveTercero = cert.Contains(NombresCamposCompradorNormas.ClaveTerceroCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.ClaveTerceroCRM) : string.Empty;
            ter.Aen_ClaveNorma = cert.Contains(NombresCamposCompradorNormas.Aen_ClaveNormaCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_ClaveNormaCRM) : string.Empty;
            ter.IdTercero = cert.Contains(NombresCamposCompradorNormas.IdTerceroCRM) ? ((EntityReference)cert.GetAttributeValue<EntityReference>(NombresCamposCompradorNormas.IdTerceroCRM)).Id : Guid.Empty;
            ter.Aen_CodigoCTN = cert.Contains(NombresCamposCompradorNormas.Aen_CodigoCTNCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_CodigoCTNCRM) : string.Empty;
            ter.Aen_DescripcionCTN = cert.Contains(NombresCamposCompradorNormas.Aen_DescripcionCTNCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_DescripcionCTNCRM) : string.Empty;
            ter.Aen_Codigoarticulo = cert.Contains(NombresCamposCompradorNormas.Aen_CodigoarticuloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_CodigoarticuloCRM) : string.Empty;
            ter.Aen_Descripcionarticulo = cert.Contains(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) : string.Empty;
            ter.Aen_Titulo = cert.Contains(NombresCamposCompradorNormas.Aen_TituloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorNormas.Aen_TituloCRM) : string.Empty;
            ter.Aen_Cantidad = cert.Contains(NombresCamposCompradorNormas.Aen_CantidadCRM) ? cert.GetAttributeValue<int>(NombresCamposCompradorNormas.Aen_CantidadCRM) : int.MinValue;
            ter.Aen_Importe = cert.Contains(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) ? cert.GetAttributeValue<decimal>(NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM) : decimal.Zero;

            return ter;
        }

        private Entity GetEntityFromCompradorNormas(CompradorNormas ev)
        {
            Entity entCert = new Entity(CompradorNormas.EntityName);

            if (ev.CompradorNormasId != null && ev.CompradorNormasId != Guid.Empty)
            { //Update de registro
                entCert.Id = ev.CompradorNormasId;
                entCert[NombresCamposCompradorNormas.CompradorNormasId] = ev.CompradorNormasId;
            }
            if (!ev.Aen_ClaveNorma.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_ClaveNormaCRM] = ev.Aen_ClaveNorma;
            if (!ev.IdTercero.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.IdTerceroCRM] = new EntityReference("account", ev.IdTercero);
            if (!ev.ClaveTercero.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.ClaveTerceroCRM] = ev.ClaveTercero;
            if (!ev.Aen_CodigoCTN.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_CodigoCTNCRM] = ev.Aen_CodigoCTN;
            if (!ev.Aen_DescripcionCTN.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_DescripcionCTNCRM] = ev.Aen_DescripcionCTN;
            if (!ev.Aen_Codigoarticulo.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_CodigoarticuloCRM] = ev.Aen_Codigoarticulo;
            if (!ev.Aen_Descripcionarticulo.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_DescripcionarticuloCRM] = ev.Aen_Descripcionarticulo;
            if (!ev.Aen_Titulo.Equals(string.Empty)) entCert[NombresCamposCompradorNormas.Aen_TituloCRM] = ev.Aen_Titulo;
            if (!ev.Aen_Cantidad.Equals(int.MinValue)) entCert[NombresCamposCompradorNormas.Aen_CantidadCRM] = ev.Aen_Cantidad;
            if (!ev.Aen_Importe.Equals(decimal.Zero)) entCert[NombresCamposCompradorNormas.Aen_ImporteCRM] = ev.Aen_Importe;

            return entCert;
        }

        private bool CompruebaUpdateCompradorNormas(CompradorNormas cnOra, CompradorNormas cnCrm, ref Entity updateFinal)
        {
            bool actualizar = false;

            if (!cnOra.ClaveTercero.Equals(cnCrm.ClaveTercero)) actualizar = true;
            if (!cnOra.Aen_CodigoCTN.Equals(cnCrm.Aen_CodigoCTN)) actualizar = true;
            if (!cnOra.Aen_DescripcionCTN.Equals(cnCrm.Aen_DescripcionCTN)) actualizar = true;
            if (!cnOra.Aen_Codigoarticulo.Equals(cnCrm.Aen_Codigoarticulo)) actualizar = true;
            if (!cnOra.Aen_Descripcionarticulo.Equals(cnCrm.Aen_Descripcionarticulo)) actualizar = true;
            if (!cnOra.Aen_Titulo.Equals(cnCrm.Aen_Titulo)) actualizar = true;
            if (!cnOra.Aen_Importe.ToString("#,##").Equals(cnCrm.Aen_Importe.ToString("#,##")) && cnOra.Aen_Importe != 0) actualizar = true;
            if (!cnOra.Aen_Cantidad.Equals(cnCrm.Aen_Cantidad)) actualizar = true;

            if (actualizar)
            {
                cnOra.CompradorNormasId = cnCrm.CompradorNormasId;
                updateFinal = GetEntityFromCompradorNormas(cnOra);
            }

            return actualizar;
        }

        #endregion Operaciones Objetos COMPRADOR NORMAS


        #region Operaciones Objetos COMPRADOR LIBROS
        private void CreateDiccionarioCompradorLibros()
        {
            var q = new QueryExpression(CompradorLibros.EntityName);
            q.ColumnSet = new ColumnSet(new string[] {
                NombresCamposCompradorLibros.Aen_ClaveLibrosCRM, NombresCamposCompradorLibros.Aen_CantidadCRM,
                NombresCamposCompradorLibros.Aen_CodigoarticuloCRM, NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM,
                NombresCamposCompradorLibros.Aen_TituloCRM, NombresCamposCompradorLibros.Aen_ImporteCRM,
                NombresCamposCompradorLibros.ClaveTerceroCRM, NombresCamposCompradorLibros.IdTerceroCRM,
                "statecode" });

            q.Criteria.Conditions.AddRange(
                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression(NombresCamposCompradorLibros.Aen_ClaveLibrosCRM, ConditionOperator.NotNull) );

            var leTerc = new LinkEntity(EvaluacionConformidad.EntityName, "account",
                NombresCamposCompradorLibros.IdTerceroCRM, "accountid", JoinOperator.Inner);
            leTerc.Columns = new ColumnSet(new string[] { NombreCamposTercero.Aen_claveintegracionCRM, "accountid" });
            leTerc.LinkCriteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull)
                );

            q.LinkEntities.AddRange(leTerc);

            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var eva in entidades.Entities)
                {
                    var evco = new CompradorLibros();
                    evco.CompradorLibrosId = eva.Id;
                    evco.ClaveTercero = ((AliasedValue)eva["account1." + NombreCamposTercero.Aen_claveintegracionCRM]).Value.ToString();
                    evco.IdTercero = new Guid(((AliasedValue)eva["account1.accountid"]).Value.ToString());
                    evco.Aen_Codigoarticulo = eva.Contains(NombresCamposCompradorLibros.Aen_CodigoarticuloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_CodigoarticuloCRM) : string.Empty;
                    evco.Aen_ClaveLibros = eva.Contains(NombresCamposCompradorLibros.Aen_ClaveLibrosCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_ClaveLibrosCRM) : string.Empty;
                    evco.Aen_Descripcionarticulo = eva.Contains(NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM) : string.Empty;
                    evco.Aen_Titulo = eva.Contains(NombresCamposCompradorLibros.Aen_TituloCRM) ? eva.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_TituloCRM) : string.Empty;
                    evco.Aen_Cantidad = eva.Contains(NombresCamposCompradorLibros.Aen_CantidadCRM) ? eva.GetAttributeValue<int>(NombresCamposCompradorLibros.Aen_CantidadCRM) : int.MinValue;
                    evco.Aen_Importe = eva.Contains(NombresCamposCompradorLibros.Aen_ImporteCRM) ? eva.GetAttributeValue<decimal>(NombresCamposCompradorLibros.Aen_ImporteCRM) : decimal.Zero;

                    CompradorLibrosCRM.Add(evco.Aen_ClaveLibros, evco);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
        }

        private CompradorLibros CompradorLibrosFromOracle(DataRow fila)
        {
            var compraL = new CompradorLibros();
            compraL.CompradorLibrosId = Guid.Empty;
            compraL.ClaveTercero = fila[NombresCamposCompradorLibros.ClaveTerceroORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCompradorLibros.ClaveTerceroORACLE]).Trim();
            compraL.Aen_ClaveLibros = fila[NombresCamposCompradorLibros.Aen_ClaveLibrosORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCompradorLibros.Aen_ClaveLibrosORACLE]).Trim();
            compraL.Aen_Codigoarticulo = fila[NombresCamposCompradorLibros.Aen_CodigoarticuloORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCompradorLibros.Aen_CodigoarticuloORACLE]).Trim();
            compraL.Aen_Descripcionarticulo = fila[NombresCamposCompradorLibros.Aen_DescripcionarticuloORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCompradorLibros.Aen_DescripcionarticuloORACLE]).Trim();
            compraL.Aen_Titulo = fila[NombresCamposCompradorLibros.Aen_TituloORACLE]== DBNull.Value ? string.Empty : ((string)fila[NombresCamposCompradorLibros.Aen_TituloORACLE]).Trim();
            compraL.Aen_Importe = fila[NombresCamposCompradorLibros.Aen_ImporteORACLE] == DBNull.Value ? decimal.Zero : Convert.ToDecimal(fila[NombresCamposCompradorLibros.Aen_ImporteORACLE]);
            compraL.Aen_Cantidad = fila[NombresCamposCompradorLibros.Aen_CantidadORACLE] == DBNull.Value ? int.MinValue : Convert.ToInt16((fila[NombresCamposCompradorLibros.Aen_CantidadORACLE]).ToString());

            TerceroEs aux = new TerceroEs();
            var ok = TercerosEsCRM.TryGetValue(compraL.ClaveTercero, out aux);
            if (!ok)
                compraL.IdTercero = Guid.Empty;
            else
                compraL.IdTercero = aux.Accountid;

            return compraL;
        }

        private CompradorLibros CompradorLibrosFromCrm(Entity cert)
        {
            var ter = new CompradorLibros();

            ter.CompradorLibrosId = cert.Id;
            ter.ClaveTercero = cert.Contains(NombresCamposCompradorLibros.ClaveTerceroCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorLibros.ClaveTerceroCRM) : string.Empty;
            ter.Aen_ClaveLibros = cert.Contains(NombresCamposCompradorLibros.Aen_ClaveLibrosCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_ClaveLibrosCRM) : string.Empty;
            ter.IdTercero = cert.Contains(NombresCamposCompradorLibros.IdTerceroCRM) ? ((EntityReference)cert.GetAttributeValue<EntityReference>(NombresCamposCompradorLibros.IdTerceroCRM)).Id : Guid.Empty;
            ter.Aen_Codigoarticulo = cert.Contains(NombresCamposCompradorLibros.Aen_CodigoarticuloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_CodigoarticuloCRM) : string.Empty;
            ter.Aen_Descripcionarticulo = cert.Contains(NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM) : string.Empty;
            ter.Aen_Titulo = cert.Contains(NombresCamposCompradorLibros.Aen_TituloCRM) ? cert.GetAttributeValue<string>(NombresCamposCompradorLibros.Aen_TituloCRM) : string.Empty;
            ter.Aen_Importe = cert.Contains(NombresCamposCompradorLibros.Aen_ImporteCRM) ? cert.GetAttributeValue<decimal>(NombresCamposCompradorLibros.Aen_ImporteCRM) : decimal.Zero;
            ter.Aen_Cantidad = cert.Contains(NombresCamposCompradorLibros.Aen_CantidadCRM) ? cert.GetAttributeValue<int>(NombresCamposCompradorLibros.Aen_CantidadCRM) : int.MinValue;

            return ter;
        }

        private Entity GetEntityFromCompradorLibros(CompradorLibros ev)
        {
            Entity entCert = new Entity(CompradorLibros.EntityName);

            if (ev.CompradorLibrosId != null && ev.CompradorLibrosId != Guid.Empty)
            { //Update de registro
                entCert.Id = ev.CompradorLibrosId;
                entCert[NombresCamposCompradorLibros.CompradorLibrosId] = ev.CompradorLibrosId;
            }
            entCert[NombresCamposCompradorLibros.Aen_ClaveLibrosCRM] = ev.Aen_ClaveLibros;
            if (!ev.IdTercero.Equals(string.Empty)) entCert[NombresCamposCompradorLibros.IdTerceroCRM] = new EntityReference("account", ev.IdTercero);
            entCert[NombresCamposCompradorLibros.ClaveTerceroCRM] = ev.ClaveTercero;
            entCert[NombresCamposCompradorLibros.Aen_CodigoarticuloCRM] = ev.Aen_Codigoarticulo;
            entCert[NombresCamposCompradorLibros.Aen_DescripcionarticuloCRM] = ev.Aen_Descripcionarticulo;
            entCert[NombresCamposCompradorLibros.Aen_TituloCRM] = ev.Aen_Titulo;
            if (!ev.Aen_Importe.Equals(decimal.Zero)) entCert[NombresCamposCompradorLibros.Aen_ImporteCRM] = ev.Aen_Importe;
            if (!ev.Aen_Cantidad.Equals(int.MinValue)) entCert[NombresCamposCompradorLibros.Aen_CantidadCRM] = ev.Aen_Cantidad;

            return entCert;
        }

        private bool CompruebaUpdateCompradorLibros(CompradorLibros cnOra, CompradorLibros cnCrm, ref Entity updateFinal)
        {
            bool actualizar = false;

            if (!cnOra.ClaveTercero.Equals(cnCrm.ClaveTercero)) actualizar = true;
            if (!cnOra.Aen_Codigoarticulo.Equals(cnCrm.Aen_Codigoarticulo)) actualizar = true;
            if (!cnOra.Aen_Descripcionarticulo.Equals(cnCrm.Aen_Descripcionarticulo)) actualizar = true;
            if (!cnOra.Aen_Titulo.Equals(cnCrm.Aen_Titulo)) actualizar = true;
            if (!cnOra.Aen_Importe.ToString("#,##").Equals(cnCrm.Aen_Importe.ToString("#,##")) && cnOra.Aen_Importe != 0) actualizar = true;
            if (!cnOra.Aen_Cantidad.Equals(cnCrm.Aen_Cantidad)) actualizar = true;

            if (actualizar)
            {
                cnOra.CompradorLibrosId = cnCrm.CompradorLibrosId;
                updateFinal = GetEntityFromCompradorLibros(cnOra);
            }
            return actualizar;
        }

        #endregion Operaciones Objetos COMPRADOR LIBROS


        #region Operaciones Objetos POTENCIAL WEB
        private void CreateDiccionarioCliPotencialWeb()
        {
            var q = new QueryExpression(CliPotencialWeb.EntityName);
            q.ColumnSet = new ColumnSet(new string[] {
                NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM,
                NombresCamposCliPotencialWeb.ClaveTerceroCRM, NombresCamposCliPotencialWeb.IdTerceroCRM,
                NombresCamposCliPotencialWeb.Aen_PedidoCRM, NombresCamposCliPotencialWeb.Aen_ObservacionesCRM,
                NombresCamposCliPotencialWeb.Aen_EmailCRM, NombresCamposCliPotencialWeb.Aen_TituloCRM,
                NombresCamposCliPotencialWeb.Aen_FechaemisionCRM, NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM,
                "statecode" });

            q.Criteria.Conditions.AddRange(
                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression(NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM, ConditionOperator.NotNull) );

            var leTerc = new LinkEntity(CliPotencialWeb.EntityName, "account",
                NombresCamposCliPotencialWeb.IdTerceroCRM, "accountid", JoinOperator.Inner);
            leTerc.Columns = new ColumnSet(new string[] { NombreCamposTercero.Aen_claveintegracionCRM, "accountid" });
            leTerc.LinkCriteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull)
                );

            q.LinkEntities.AddRange(leTerc);

            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var eva in entidades.Entities)
                {
                    var evco = new CliPotencialWeb();
                    evco.CliPotencialWebId = eva.Id;
                    evco.ClaveTercero = ((AliasedValue)eva["account1." + NombreCamposTercero.Aen_claveintegracionCRM]).Value.ToString();
                    evco.IdTercero = new Guid(((AliasedValue)eva["account1.accountid"]).Value.ToString());
                    evco.Aen_ClavePotencial = eva.Contains(NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM) ? eva.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM) : string.Empty;
                    evco.Aen_Pedido = eva.Contains(NombresCamposCliPotencialWeb.Aen_PedidoCRM) ? eva.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_PedidoCRM) : string.Empty;
                    evco.Aen_Observaciones = eva.Contains(NombresCamposCliPotencialWeb.Aen_ObservacionesCRM) ? eva.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_ObservacionesCRM) : string.Empty;
                    evco.Aen_Email = eva.Contains(NombresCamposCliPotencialWeb.Aen_EmailCRM) ? eva.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_EmailCRM) : string.Empty;
                    evco.Aen_Titulo = eva.Contains(NombresCamposCliPotencialWeb.Aen_TituloCRM) ? eva.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_TituloCRM) : string.Empty;
                    evco.Aen_Entradadelcliente = eva.Contains(NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM) ? eva.GetAttributeValue<OptionSetValue>(NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM).Value.ToString() : string.Empty;
                    evco.Aen_Fechaemision = eva.Contains(NombresCamposCliPotencialWeb.Aen_FechaemisionCRM) ?
                        eva.GetAttributeValue<DateTime>(NombresCamposCliPotencialWeb.Aen_FechaemisionCRM).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;

                    CliPotencialWebCRM.Add(evco.Aen_ClavePotencial, evco);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
        }

        private CliPotencialWeb CliPotencialWebFromOracle(DataRow fila)
        {
            var potweb = new CliPotencialWeb();
            potweb.CliPotencialWebId = Guid.Empty;
            potweb.ClaveTercero = fila[NombresCamposCliPotencialWeb.ClaveTerceroORACLE]== DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.ClaveTerceroORACLE]).Trim();
            potweb.Aen_ClavePotencial = fila[NombresCamposCliPotencialWeb.Aen_ClavePotencialORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_ClavePotencialORACLE]).Trim();
            potweb.Aen_Pedido = fila[NombresCamposCliPotencialWeb.Aen_PedidoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_PedidoORACLE]).Trim();
            potweb.Aen_Fechaemision = fila[NombresCamposCliPotencialWeb.Aen_FechaemisionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposCliPotencialWeb.Aen_FechaemisionORACLE]).ToString("dd/MM/yyyy");
            potweb.Aen_Titulo = fila[NombresCamposCliPotencialWeb.Aen_TituloORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_TituloORACLE]).Trim();
            potweb.Aen_Observaciones = fila[NombresCamposCliPotencialWeb.Aen_ObservacionesORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_ObservacionesORACLE]).Trim();
            potweb.Aen_Email = fila[NombresCamposCliPotencialWeb.Aen_EmailORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_EmailORACLE]).Trim();
            potweb.Aen_Entradadelcliente = fila[NombresCamposCliPotencialWeb.Aen_EntradadelclienteORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposCliPotencialWeb.Aen_EntradadelclienteORACLE]).Replace(".","").Trim();

            TerceroEs aux = new TerceroEs();
            var ok = TercerosEsCRM.TryGetValue(potweb.ClaveTercero, out aux);
            if (!ok)
                potweb.IdTercero = Guid.Empty;
            else
                potweb.IdTercero = aux.Accountid;

            return potweb;
        }

        private CliPotencialWeb CliPotencialWebFromCrm(Entity cert)
        {
            var ter = new CliPotencialWeb();

            ter.CliPotencialWebId = cert.Id;
            ter.ClaveTercero = cert.Contains(NombresCamposCliPotencialWeb.ClaveTerceroCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.ClaveTerceroCRM) : string.Empty;
            ter.Aen_ClavePotencial = cert.Contains(NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM) : string.Empty;
            ter.IdTercero = cert.Contains(NombresCamposCliPotencialWeb.IdTerceroCRM) ? ((EntityReference)cert.GetAttributeValue<EntityReference>(NombresCamposCliPotencialWeb.IdTerceroCRM)).Id : Guid.Empty;
            ter.Aen_Pedido = cert.Contains(NombresCamposCliPotencialWeb.Aen_PedidoCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_PedidoCRM) : string.Empty;
            ter.Aen_Observaciones = cert.Contains(NombresCamposCliPotencialWeb.Aen_ObservacionesCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_ObservacionesCRM) : string.Empty;
            ter.Aen_Titulo = cert.Contains(NombresCamposCliPotencialWeb.Aen_TituloCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_TituloCRM) : string.Empty;
            ter.Aen_Fechaemision = cert.Contains(NombresCamposCliPotencialWeb.Aen_FechaemisionCRM) ? cert.GetAttributeValue<DateTime>(NombresCamposCliPotencialWeb.Aen_FechaemisionCRM).ToLocalTime().ToString("yyyy-MM-dd".Trim()) : string.Empty;
            ter.Aen_Email = cert.Contains(NombresCamposCliPotencialWeb.Aen_EmailCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_EmailCRM) : string.Empty;
            ter.Aen_Entradadelcliente = cert.Contains(NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM) ? cert.GetAttributeValue<string>(NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM) : string.Empty;

            return ter;
        }

        private Entity GetEntityFromCliPotencialWeb(CliPotencialWeb ev)
        {
            Entity entPotWeb = new Entity(CliPotencialWeb.EntityName);

            if (ev.CliPotencialWebId != null && ev.CliPotencialWebId != Guid.Empty)
            { //Update de registro
                entPotWeb.Id = ev.CliPotencialWebId;
                entPotWeb[NombresCamposCliPotencialWeb.CliPotencialWebId] = ev.CliPotencialWebId;
            }
            if (!ev.IdTercero.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.IdTerceroCRM] = new EntityReference("account", ev.IdTercero);
            if (!ev.ClaveTercero.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.ClaveTerceroCRM] = ev.ClaveTercero;
            if (!ev.Aen_ClavePotencial.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_ClavePotencialCRM] = ev.Aen_ClavePotencial;
            if (!ev.Aen_Pedido.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_PedidoCRM] = ev.Aen_Pedido;
            if (!ev.Aen_Observaciones.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_ObservacionesCRM] = ev.Aen_Observaciones;
            if (!ev.Aen_Titulo.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_TituloCRM] = ev.Aen_Titulo;
            if (!ev.Aen_Fechaemision.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(ev.Aen_Fechaemision, out DateTime f);
                if (okFecha)
                    entPotWeb[NombresCamposCliPotencialWeb.Aen_FechaemisionCRM] = f;
            }
            if (!ev.Aen_Email.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_EmailCRM] = ev.Aen_Email;
            if (!ev.Aen_Entradadelcliente.Equals(string.Empty)) entPotWeb[NombresCamposCliPotencialWeb.Aen_EntradadelclienteCRM] = new OptionSetValue(Convert.ToInt32(ev.Aen_Entradadelcliente));

            return entPotWeb;
        }

        private bool CompruebaUpdateCliPotencialWeb(CliPotencialWeb cnOra, CliPotencialWeb cnCrm, ref Entity updateFinal)
        {
            bool actualizar = false;

            if (!cnOra.ClaveTercero.Equals(cnCrm.ClaveTercero)) actualizar = true;
            if (!cnOra.Aen_Pedido.Equals(cnCrm.Aen_Pedido)) actualizar = true;
            if (!cnOra.Aen_Observaciones.Equals(cnCrm.Aen_Observaciones)) actualizar = true;
            if (!cnOra.Aen_Titulo.Equals(cnCrm.Aen_Titulo)) actualizar = true;
            if (!cnOra.Aen_Fechaemision.Equals(cnCrm.Aen_Fechaemision)) actualizar = true;
            if (!cnOra.Aen_Email.Equals(cnCrm.Aen_Email)) actualizar = true;
            if (!cnOra.Aen_Entradadelcliente.Equals(cnCrm.Aen_Entradadelcliente)) actualizar = true;

            if (actualizar)
            {
                cnOra.CliPotencialWebId = cnCrm.CliPotencialWebId;
                updateFinal = GetEntityFromCliPotencialWeb(cnOra);
            }

            return actualizar;
        }

        #endregion Operaciones Objetos POTENCIAL WEB


        #region Operaciones Objetos SUSCRIPTOR
        private void CreateDiccionarioSuscriptores()
        {
            var q = new QueryExpression(Suscriptor.EntityName);
            q.ColumnSet = new ColumnSet(new string[] {
                NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM,
                NombresCamposSuscriptor.ClaveTerceroCRM, NombresCamposSuscriptor.IdTerceroCRM,
                NombresCamposSuscriptor.Aen_ProductoCRM, NombresCamposSuscriptor.Aen_SituacionCRM,
                NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM, NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM,
                "statecode" });

            q.Criteria.Conditions.AddRange(
                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression(NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM, ConditionOperator.NotNull) );

            var leTerc = new LinkEntity(Suscriptor.EntityName, "account",
                NombresCamposSuscriptor.IdTerceroCRM, "accountid", JoinOperator.Inner);
            leTerc.Columns = new ColumnSet(new string[] { NombreCamposTercero.Aen_claveintegracionCRM, "accountid" });
            leTerc.LinkCriteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull)
                );

            q.LinkEntities.AddRange(leTerc);

            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var eva in entidades.Entities)
                {
                    var evco = new Suscriptor();
                    evco.SuscriptorId = eva.Id;
                    evco.Aen_ClaveSuscriptor = eva.Contains(NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM) ? eva.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM) : string.Empty;
                    evco.ClaveTercero = ((AliasedValue)eva["account1." + NombreCamposTercero.Aen_claveintegracionCRM]).Value.ToString();
                    evco.IdTercero = new Guid(((AliasedValue)eva["account1.accountid"]).Value.ToString());
                    evco.Aen_Producto = eva.Contains(NombresCamposSuscriptor.Aen_ProductoCRM) ? eva.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_ProductoCRM) : string.Empty;
                    evco.Aen_Situacion = eva.Contains(NombresCamposSuscriptor.Aen_SituacionCRM) ? eva.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_SituacionCRM) : string.Empty;
                    evco.Aen_Fechabajasuscripcion = eva.Contains(NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM) ?
                        eva.GetAttributeValue<DateTime>(NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                    evco.Aen_Fechafinsuscripcion = eva.Contains(NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM) ?
                        eva.GetAttributeValue<DateTime>(NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                    try
                    {
                        SuscriptorCRM.Add(evco.Aen_ClaveSuscriptor, evco);
                    }
                    catch (Exception e)
                    { Comun.LogText("SuscriptorCRM (clavesuscriptor): "+ evco.Aen_ClaveSuscriptor); }
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
        }

        private Suscriptor SuscriptorFromOracle(DataRow fila)
        {
            var sus = new Suscriptor();
            sus.SuscriptorId = Guid.Empty;
            sus.Aen_ClaveSuscriptor = fila[NombresCamposSuscriptor.Aen_ClaveSuscriptorORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposSuscriptor.Aen_ClaveSuscriptorORACLE]).Trim();
            sus.ClaveTercero = fila[NombresCamposSuscriptor.ClaveTerceroORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposSuscriptor.ClaveTerceroORACLE]).Trim();
            sus.Aen_Producto = fila[NombresCamposSuscriptor.Aen_ProductoORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposSuscriptor.Aen_ProductoORACLE]).Trim();
            sus.Aen_Situacion = fila[NombresCamposSuscriptor.Aen_SituacionORACLE] == DBNull.Value ? string.Empty : ((string)fila[NombresCamposSuscriptor.Aen_SituacionORACLE]).Trim();
            sus.Aen_Fechafinsuscripcion = fila[NombresCamposSuscriptor.Aen_FechafinsuscripcionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposSuscriptor.Aen_FechafinsuscripcionORACLE]).ToString("dd/MM/yyyy");
            sus.Aen_Fechabajasuscripcion = fila[NombresCamposSuscriptor.Aen_FechabajasuscripcionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposSuscriptor.Aen_FechabajasuscripcionORACLE]).ToString("dd/MM/yyyy");

            TerceroEs aux = new TerceroEs();
            var ok = TercerosEsCRM.TryGetValue(sus.ClaveTercero, out aux);
            if (!ok)
                sus.IdTercero = Guid.Empty;
            else
                sus.IdTercero = aux.Accountid;

            return sus;
        }

        private Suscriptor SuscriptorFromCrm(Entity cert)
        {
            var ter = new Suscriptor();

            ter.SuscriptorId = cert.Id;
            ter.Aen_ClaveSuscriptor = cert.Contains(NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM) ? cert.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM) : string.Empty;
            ter.ClaveTercero = cert.Contains(NombresCamposSuscriptor.ClaveTerceroCRM) ? cert.GetAttributeValue<string>(NombresCamposSuscriptor.ClaveTerceroCRM) : string.Empty;
            ter.IdTercero = cert.Contains(NombresCamposSuscriptor.IdTerceroCRM) ? ((EntityReference)cert.GetAttributeValue<EntityReference>(NombresCamposSuscriptor.IdTerceroCRM)).Id : Guid.Empty;
            ter.Aen_Producto = cert.Contains(NombresCamposSuscriptor.Aen_ProductoCRM) ? cert.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_ProductoCRM) : string.Empty;
            ter.Aen_Situacion = cert.Contains(NombresCamposSuscriptor.Aen_SituacionCRM) ? cert.GetAttributeValue<string>(NombresCamposSuscriptor.Aen_SituacionCRM) : string.Empty;
            ter.Aen_Fechafinsuscripcion = cert.Contains(NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM) ? cert.GetAttributeValue<DateTime>(NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM).ToLocalTime().ToString("yyyy-MM-dd".Trim()) : string.Empty;
            ter.Aen_Fechabajasuscripcion = cert.Contains(NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM) ? cert.GetAttributeValue<DateTime>(NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM).ToLocalTime().ToString("yyyy-MM-dd".Trim()) : string.Empty;

            return ter;
        }

        private Entity GetEntityFromSuscriptor(Suscriptor ev)
        {
            Entity entSus = new Entity(Suscriptor.EntityName);

            if (ev.SuscriptorId != null && ev.SuscriptorId != Guid.Empty)
            { //Update de registro
                entSus.Id = ev.SuscriptorId;
                entSus[Suscriptor.EntityName+"id"] = ev.SuscriptorId;
            }
            if (!ev.Aen_ClaveSuscriptor.Equals(string.Empty)) entSus[NombresCamposSuscriptor.Aen_ClaveSuscriptorCRM] = ev.Aen_ClaveSuscriptor;
            if (!ev.IdTercero.Equals(string.Empty)) entSus[NombresCamposSuscriptor.IdTerceroCRM] = new EntityReference("account", ev.IdTercero);
            if (!ev.ClaveTercero.Equals(string.Empty)) entSus[NombresCamposSuscriptor.ClaveTerceroCRM] = ev.ClaveTercero;
            if (!ev.Aen_Producto.Equals(string.Empty)) entSus[NombresCamposSuscriptor.Aen_ProductoCRM] = ev.Aen_Producto;
            if (!ev.Aen_Situacion.Equals(string.Empty)) entSus[NombresCamposSuscriptor.Aen_SituacionCRM] = ev.Aen_Situacion;
            if (!ev.Aen_Fechafinsuscripcion.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(ev.Aen_Fechafinsuscripcion, out DateTime f);
                if (okFecha)
                    entSus[NombresCamposSuscriptor.Aen_FechafinsuscripcionCRM] = f;
            }
            if (!ev.Aen_Fechabajasuscripcion.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(ev.Aen_Fechabajasuscripcion, out DateTime f);
                if (okFecha)
                    entSus[NombresCamposSuscriptor.Aen_FechabajasuscripcionCRM] = f;
            }

            return entSus;
        }

        private bool CompruebaUpdateSuscriptor(Suscriptor cnOra, Suscriptor cnCrm, ref Entity updateFinal)
        {
            bool actualizar = false;

            if (!cnOra.ClaveTercero.Equals(cnCrm.ClaveTercero)) actualizar = true;
            if (!cnOra.Aen_Producto.Equals(cnCrm.Aen_Producto)) actualizar = true;
            if (!cnOra.Aen_Situacion.Equals(cnCrm.Aen_Situacion)) actualizar = true;
            if (!cnOra.Aen_Fechafinsuscripcion.Equals(cnCrm.Aen_Fechafinsuscripcion)) actualizar = true;
            if (!cnOra.Aen_Fechabajasuscripcion.Equals(cnCrm.Aen_Fechabajasuscripcion)) actualizar = true;

            if (actualizar)
            {
                cnOra.SuscriptorId = cnCrm.SuscriptorId;
                updateFinal = GetEntityFromSuscriptor(cnOra);
            }

            return actualizar;
        }
        #endregion Operaciones Objetos SUSCRIPTOR
    }
}
