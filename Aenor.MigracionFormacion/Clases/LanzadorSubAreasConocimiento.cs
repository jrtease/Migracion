using Aenor.MigracionFormacion.Objetos;
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

namespace Aenor.MigracionFormacion.Clases
{
    class LanzadorSubAreasConocimiento
    {
        #region PROPIEDADES
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, SubareaDeConocimiento> SubAreasConocimientoOracle, SubAreasConocimientoCRM;
        public Dictionary<string, Guid> MaestroSubAreasConocimientoCRM;
        public List<string> SubAreasADesactivar;

        #endregion PROPIEDADES


        //TODO Enlazar con Area de Conocimiento


        #region METODOS
        public Dictionary<string, Guid> Iniciar(Oracle ora, Comun com, Crm crm, Dictionary<string, Guid> MaestroAreasConocimiento)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;

                CrmGlobal.InicializarPFE(OracleGlobal);

                //1 Cargar AreasConocimiento
                LeerSubAreasConocimientoFromCRM();
                LeerSubAreasConocimientoFromOracle(MaestroAreasConocimiento);

                bool ok;
                SubareaDeConocimiento sarea;
                SubAreasADesactivar = new List<string>();
                if (SubAreasConocimientoOracle.Any())
                {
                    #region Tratamiento AreasConocimiento
                    ComunGlobal.LogText("----   Iniciando CARGA SUBAREAS DE CONOCIMIENTO  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + SubAreasConocimientoOracle.Count);
                    foreach (var sa in SubAreasConocimientoOracle)
                    {
                        ok = SubAreasConocimientoCRM.TryGetValue(sa.Key, out sarea);
                        if (!ok)
                        {
                            var ent = new Entity(AenSubareadeconocimiento.EntityName);
                            ent[AenSubareadeconocimiento.PrimaryName] = sarea.aen_name;
                            if (sarea.statecode == 1)
                                SubAreasADesactivar.Add(sarea.aen_name);
                            CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = ent });
                        }
                        else
                        {
                            bool sonIguales = SubAreasConocimientoIguales(sa.Value, sarea);
                            if (sonIguales)
                                CrmGlobal.Iguales++;
                            else
                            {
                                var ent = new Entity(AenSubareadeconocimiento.EntityName, sarea.aen_subareaconocimientoid);
                                ent[AenSubareadeconocimiento.PrimaryName] = sarea.aen_name;
                                ent[AenSubareadeconocimiento.StateCode] = new OptionSetValue(sarea.statecode);
                                ent[AenSubareadeconocimiento.StatusCode] = new OptionSetValue(sarea.statuscode);
                                CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = ent });
                            }
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();

                    CrmGlobal.MostrarEstadisticas("Subareas de Conocimiento");
                    ComunGlobal.LogText("----   FINALIZADA CARGA SUBAREAS DE CONOCIMIENTO ------");

                    #endregion Tratamiento AreasConocimiento
                }
                else
                    ComunGlobal.LogText("No hay Subareas de Conocimiento en origen, terminamos");



                //2. Devolver Maestro de AreasConocimiento
                #region Carga Maestro Areas de Conocimiento
                MaestroSubAreasConocimientoCRM = new Dictionary<string, Guid>();

                var q = new QueryExpression(AenSubareadeconocimiento.EntityName);
                q.Criteria.AddCondition(AenSubareadeconocimiento.PrimaryName, ConditionOperator.NotNull);
                q.ColumnSet = new ColumnSet(AenSubareadeconocimiento.PrimaryName);

                EntityCollection modColeccion = CrmGlobal.GetIOS().RetrieveMultiple(q);

                if (modColeccion.Entities.Count > 0)
                {
                    foreach (Entity modCRM in modColeccion.Entities)
                        MaestroSubAreasConocimientoCRM.Add(modCRM.GetAttributeValue<string>(AenSubareadeconocimiento.PrimaryName), modCRM.Id);
                }
                #endregion Carga Maestro AreasConocimiento


                //3. Desactivar las Áreas de conocimiento inactivas desde origen
                #region Desactivar areas
                if (SubAreasADesactivar.Any())
                {
                    Guid auxG = Guid.Empty;
                    foreach (string str in SubAreasADesactivar)
                    {
                        var okk = MaestroSubAreasConocimientoCRM.TryGetValue(str, out auxG);
                        if (okk)
                        {
                            Entity en = new Entity(AenSubareadeconocimiento.EntityName, auxG);
                            en[AenSubareadeconocimiento.StateCode] = new OptionSetValue(1);
                            en[AenSubareadeconocimiento.StatusCode] = new OptionSetValue(2);
                            CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = en });
                        }
                    }
                }
                #endregion Desactivar areas


                //4. Limpiar Diccionarios para liberar memoria
                SubAreasConocimientoOracle.Clear();
                SubAreasConocimientoCRM.Clear();
                SubAreasADesactivar.Clear();


                return MaestroSubAreasConocimientoCRM;
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de SubAreasConocimiento ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();

                return null;
            }
        }




        private bool SubAreasConocimientoIguales(SubareaDeConocimiento mod1, SubareaDeConocimiento mod2)
        {
            return mod1.aen_name == mod2.aen_name &&
                mod1.statecode == mod2.statecode;
        }


        private void LeerSubAreasConocimientoFromOracle(Dictionary<string,Guid> MaestroAreas)
        {
            SubAreasConocimientoOracle = new Dictionary<string, SubareaDeConocimiento>();
            string queryTot = Oracle.QuerySubareasDeConocimiento;
            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            foreach (DataRow fila in dt.Rows)
            {
                var mod = new SubareaDeConocimiento();
                mod.SubAreaConocimientoFromOracle(fila, MaestroAreas);
                if (mod != null)
                    SubAreasConocimientoOracle.Add(mod.aen_name, mod);
            }
        }


        private void LeerSubAreasConocimientoFromCRM()
        {
            var q = new QueryExpression(AenSubareadeconocimiento.EntityName);
            q.Criteria.AddCondition(AenSubareadeconocimiento.PrimaryName, ConditionOperator.NotNull);
            q.ColumnSet = new ColumnSet(AenSubareadeconocimiento.PrimaryName, AenSubareadeconocimiento.StateCode);
            var res = CrmGlobal.GetIOS().RetrieveMultiple(q);
            SubAreasConocimientoCRM = new Dictionary<string, SubareaDeConocimiento>();
            foreach (var mod in res.Entities)
            {
                SubAreasConocimientoCRM.Add(
                    (string)mod[AenSubareadeconocimiento.PrimaryName],
                    new SubareaDeConocimiento
                    {
                        aen_subareaconocimientoid = mod.Id,
                        aen_name = (string)mod[AenSubareadeconocimiento.PrimaryName],
                        statecode = mod.Contains(AenSubareadeconocimiento.StateCode) ? mod.GetAttributeValue<OptionSetValue>(AenSubareadeconocimiento.StateCode).Value : 1,
                        statuscode = mod.Contains(AenSubareadeconocimiento.StatusCode) ? mod.GetAttributeValue<OptionSetValue>(AenSubareadeconocimiento.StatusCode).Value : 2
                    });
            }
        }

        #endregion METODOS
    }
}
