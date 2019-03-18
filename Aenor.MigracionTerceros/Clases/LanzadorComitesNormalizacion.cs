using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Aenor.MigracionTerceros.Clases
{
    public class ComiteNormalizacion
    {
        public Guid Aen_ComiteNormalizacionId { get; set; }
        public Guid Aen_ComitePadreId { get; set; }
        public Guid Aen_IndustriaAenorId { get; set; }
        public string Aen_Name { get; set; }
        public string Aen_NameEs { get; set; }
        public string Aen_NameEn { get; set; }
        public Guid Aen_SectorAenorId { get; set; }
        public string Aen_TipoComite { get; set; }
        public string StateCode { get; set; }
    }

    public class LanzadorComitesNormalizacion
    {
        public Crm Crm { get; set; }
        public Comun Comun { get; set; }
        public Oracle Oracle { get; set; }
        public Dictionary<string, ComiteNormalizacion> ComitesNormalizacionOracle,
            ComitesNormalizacionCRM;
        public Dictionary<string, Guid> Industrias, Sectores;

        public Dictionary<Guid, Guid> ComitesHijosPadreCRM;
        public Dictionary<string, Guid> ComitesHijosPadreCRMNombres;

        public HashSet<string> ComitesDesactivar;


        public string ClaveIntegracionActual = "";

        private const string QueryComitesNormalizacion = @"";

        public void Iniciar(Oracle oracleGlobal, Comun comunGlobal, Crm crmGlobal)
        {
            try
            {
                //Aquí no hay que hacer new, el lanzador orquestador lo pasará a las properties
                Comun = comunGlobal;
                Comun.LogText("----Iniciando sincronización comités normalizacion------");
                Crm = crmGlobal;
                ComitesDesactivar = new HashSet<string>();

                Oracle = oracleGlobal;
                Crm.InicializarPFE(oracleGlobal);

                LeerEntidades();
                LeerComitesOracle();
                if (!ComitesNormalizacionOracle.Any())
                {
                    Comun.LogText("No hay comités en origen, terminamos");
                    return;
                }


                foreach (var comiteOra in ComitesNormalizacionOracle)
                {
                    try
                    {
                        ClaveIntegracionActual = comiteOra.Key;
                        bool ok = ComitesNormalizacionCRM.TryGetValue(comiteOra.Key, out ComiteNormalizacion comiteCRM);
                        if (ok && ComitesIguales(comiteOra.Value, comiteCRM))
                        {
                            Crm.Iguales++;
                            continue;
                        }
                        var comite = GetEntity(comiteOra.Value);
                        if (ok)
                        {
                            comite["aen_comitenormalizacionid"] = comiteCRM.Aen_ComiteNormalizacionId;
                            Crm.AnadirElementoEmr(new UpdateRequest { Target = comite });
                        }
                        else
                        {
                            if (comiteOra.Value.StateCode == "Inactivo")
                            {
                                comite["statecode"] = new OptionSetValue(0);
                                comite["statuscode"] = new OptionSetValue(1);
                                ComitesDesactivar.Add(comiteOra.Value.Aen_Name);
                            }
                            Crm.AnadirElementoEmr(new CreateRequest { Target = comite });
                        }

                    }
                    catch (Exception e)
                    {
                        Comun.EscribirExcepcion(e, "Error al procesar Comité: " + comiteOra.Key);
                    }
                }
                Crm.ProcesarUltimosEmr();

                SincronizarEmparentamiento();
                DesactivarRecienCreadosInactivas();
                Crm.MostrarEstadisticas("COMITES");
            }
            catch (Exception ex)
            {
                Comun.EscribirExcepcion(ex, "Error");
                if (ClaveIntegracionActual != "")
                    Oracle.MandarErrorIntegracion(ClaveIntegracionActual, ex.Message,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                if (Oracle != null && Oracle.OraConnParaLog.State == ConnectionState.Open)
                    Oracle.OraConnParaLog.Dispose();
            }
            finally
            {
                if (Oracle != null && Oracle.OraConnParaLog.State == ConnectionState.Open)
                    Oracle.OraConnParaLog.Dispose();
            }
        }

        private void LeerComitesOracle()
        {
            //TODO Meter la QueryComitesNormalizacion en la clase Oracle junto al resto
            OracleDataAdapter da = new OracleDataAdapter(QueryComitesNormalizacion, Comun.ConnStringOracle);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ComitesNormalizacionOracle = new Dictionary<string, ComiteNormalizacion>();
            foreach (DataRow fila in dt.Rows)
            {
                var comite = ComiteNormalizacionFromOracle(fila);
                if (comite == null)
                {
                    //Comun.LogText("Dirección de Oracle no válida: " + (string)fila["aen_claveintegracion"]);
                    continue;
                }
                ComitesNormalizacionOracle.Add(comite.Aen_Name, comite);
            }
        }

        private void LeerEntidades()
        {
            ComitesNormalizacionCRM = new Dictionary<string, ComiteNormalizacion>();
            Industrias = new Dictionary<string, Guid>();
            Sectores = new Dictionary<string, Guid>();
            Comun.LogText("Leyendo entidades de CRM...");

            var q = new QueryExpression("aen_industriaaenor");
            q.ColumnSet = new ColumnSet("aen_codigo");
            q.Criteria.Conditions.Add(new ConditionExpression("aen_codigo", ConditionOperator.NotNull));
            var ents = Crm.IOS.RetrieveMultiple(q);
            foreach (var e in ents.Entities)
                Industrias.Add((string)e["aen_codigo"], e.Id);

            q = new QueryExpression("aen_sectoraenor");
            q.ColumnSet = new ColumnSet("aen_codigo");
            q.Criteria.Conditions.Add(new ConditionExpression("aen_codigo", ConditionOperator.NotNull));
            ents = Crm.IOS.RetrieveMultiple(q);
            foreach (var e in ents.Entities)
                Sectores.Add((string)e["aen_codigo"], e.Id);

            q = new QueryExpression("aen_comitenormalizacion");
            q.ColumnSet = new ColumnSet("aen_comitenormalizacionid", "aen_name",
                "aen_namees", "aen_nameen", "aen_sectoraenorid", "aen_industriaaenorid",
                "statecode");
            q.Criteria.Conditions.AddRange(
                new ConditionExpression("aen_name", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                    ComitesNormalizacionCRM.Add((string)e["aen_name"], ComiteNormalizacionFromCRM(e));
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Comun.LogText("Cacheados comités normalización: " + ComitesNormalizacionCRM.Count);
        }

        private void SincronizarEmparentamiento()
        {
            var q = new QueryExpression("aen_comitenormalizacion");
            q.ColumnSet = new ColumnSet("aen_name", "aen_comitepadre");
            q.Criteria.Conditions.AddRange(
                new ConditionExpression("aen_name", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                {
                    ComitesHijosPadreCRM.Add(e.Id,
                        e.Contains("aen_comitepadre") ? ((EntityReference)e["aen_comitepadre"]).Id : Guid.Empty);
                    ComitesHijosPadreCRMNombres.Add((string)e["aen_name"],
                        e.Contains("aen_comitepadre") ? ((EntityReference)e["aen_comitepadre"]).Id : Guid.Empty);
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }

            foreach (var c in ComitesNormalizacionOracle)
            {
                var idComitePadreOra = c.Value.Aen_ComitePadreId;
                var idComitepadreCrm = ComitesHijosPadreCRMNombres[c.Key];
                if (idComitepadreCrm != idComitePadreOra)
                {
                    var comite = new Entity("aen_comitenormalizacion");
                    comite["aen_comitepadre"] = idComitePadreOra == Guid.Empty ? null :
                        new EntityReference("aen_comitenormalizacion", idComitepadreCrm);
                    Crm.AnadirElementoEmr(new UpdateRequest { Target = comite });
                }
            }
            Crm.ProcesarUltimosEmr();
        }

        //TODO Implementar estas dos funciones
        private ComiteNormalizacion ComiteNormalizacionFromCRM(Entity e)
        {
            throw new NotImplementedException();
        }

        private ComiteNormalizacion ComiteNormalizacionFromOracle(DataRow fila)
        {
            throw new NotImplementedException();
        }

        private Entity GetEntity(ComiteNormalizacion comite)
        {
            var c = new Entity("aen_comitenormalizacion");
            if (comite.Aen_ComiteNormalizacionId != Guid.Empty)
                c["aen_comitenormalizacionid"] = comite.Aen_ComiteNormalizacionId;
            c["aen_industriaaenorid"] = comite.Aen_IndustriaAenorId == Guid.Empty ? null : new EntityReference("aen_comitenormalizacion", comite.Aen_IndustriaAenorId);
            c["aen_name"] = comite.Aen_Name == "" ? null : comite.Aen_Name;
            c["aen_namees"] = comite.Aen_NameEs == "" ? null : comite.Aen_NameEs;
            c["aen_nameen"] = comite.Aen_NameEn == "" ? null : comite.Aen_NameEn;
            c["aen_tipocomite"] = comite.Aen_TipoComite == "" ? null : new OptionSetValue(int.Parse(comite.Aen_TipoComite)); //TODO Este campo probablemente haya que transformarlo
            c["statecode"] = new OptionSetValue(comite.StateCode == "Activo" ? 0 : 1);
            c["statuscode"] = new OptionSetValue(comite.StateCode == "Activo" ? 1 : 2);
            return c;
        }

        private void DesactivarRecienCreadosInactivas()
        {
            var q = new QueryExpression("aen_comitenormalizacion");
            q.ColumnSet = new ColumnSet("aen_name");
            q.Criteria.AddCondition(
                new ConditionExpression("aen_name", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                {
                    var claveInt = (string)e["aen_name"];
                    if (ComitesDesactivar.Contains(claveInt))
                    {
                        var dir = new Entity("aen_comitenormalizacion");
                        dir["aen_comitenormalizacionid"] = e.Id;
                        dir["statecode"] = new OptionSetValue(1);
                        dir["statuscode"] = new OptionSetValue(2);
                        Crm.AnadirElementoEmr(new UpdateRequest { Target = dir });
                    }
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Crm.ProcesarUltimosEmr();
        }

        private bool ComitesIguales(ComiteNormalizacion comiteOra, ComiteNormalizacion comiteCRM)
        {
            var ig = comiteOra.Aen_Name == comiteCRM.Aen_Name; //Esta es la clave de mapeo, es un código
            ig = ig && comiteOra.Aen_IndustriaAenorId == comiteCRM.Aen_IndustriaAenorId;
            ig = ig && comiteOra.Aen_Name == comiteCRM.Aen_Name;
            ig = ig && comiteOra.Aen_NameEs == comiteCRM.Aen_NameEs;
            ig = ig && comiteOra.Aen_NameEn == comiteCRM.Aen_NameEn;
            ig = ig && comiteOra.Aen_SectorAenorId == comiteCRM.Aen_SectorAenorId;
            ig = ig && comiteOra.Aen_TipoComite == comiteCRM.Aen_TipoComite;
            ig = ig && comiteOra.StateCode == comiteCRM.StateCode;
            return ig;
        }
    }
}
