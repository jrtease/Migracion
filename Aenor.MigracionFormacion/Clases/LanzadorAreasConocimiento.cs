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
    class LanzadorAreasConocimiento
    {
        #region PROPIEDADES
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, AreaDeConocimiento> AreasConocimientoOracle, AreasConocimientoCRM;
        public Dictionary<string, Guid> MaestroAreasConocimientoCRM;
        public List<string> AreasADesactivar;

        #endregion PROPIEDADES





        #region METODOS
        public Dictionary<string, Guid> Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;

                CrmGlobal.InicializarPFE(OracleGlobal);

                //1 Cargar AreasConocimiento
                LeerAreasConocimientoFromCRM();
                LeerAreasConocimientoFromOracle();

                bool ok;
                AreaDeConocimiento area;
                AreasADesactivar = new List<string>();
                if (AreasConocimientoOracle.Any())
                {
                    #region Tratamiento AreasConocimiento
                    ComunGlobal.LogText("----   Iniciando CARGA AREAS DE CONOCIMIENTO  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + AreasConocimientoOracle.Count);
                    foreach (var a in AreasConocimientoOracle)
                    {
                        ok = AreasConocimientoCRM.TryGetValue(a.Key, out area);
                        if (!ok)
                        {
                            var ent = new Entity(AenAreadeconocimiento.EntityName);
                            ent[AenAreadeconocimiento.PrimaryName] = area.aen_name;
                            if (area.statecode == 1)
                                AreasADesactivar.Add(area.aen_name);
                            CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = ent });
                        }
                        else
                        {
                            bool sonIguales = AreasConocimientoIguales(a.Value, area);
                            if (sonIguales)
                                CrmGlobal.Iguales++;
                            else
                            {
                                var ent = new Entity(AenAreadeconocimiento.EntityName, area.aen_areaconocimientoid);
                                ent[AenAreadeconocimiento.PrimaryName] = area.aen_name;
                                ent[AenAreadeconocimiento.StateCode] = new OptionSetValue(area.statecode);
                                ent[AenAreadeconocimiento.StatusCode] = new OptionSetValue(area.statuscode);
                                CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = ent });
                            }
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();

                    CrmGlobal.MostrarEstadisticas("Areas de Conocimiento");
                    ComunGlobal.LogText("----   FINALIZADA CARGA AREAS DE CONOCIMIENTO ------");

                    #endregion Tratamiento AreasConocimiento
                }
                else
                    ComunGlobal.LogText("No hay Areas de Conocimiento en origen, terminamos");



                //2. Devolver Maestro de AreasConocimiento
                #region Carga Maestro Areas de Conocimiento
                MaestroAreasConocimientoCRM = new Dictionary<string, Guid>();

                var q = new QueryExpression(AenAreadeconocimiento.EntityName);
                q.Criteria.AddCondition(AenAreadeconocimiento.PrimaryName, ConditionOperator.NotNull);
                q.ColumnSet = new ColumnSet(AenAreadeconocimiento.PrimaryName);

                EntityCollection modColeccion = CrmGlobal.GetIOS().RetrieveMultiple(q);

                if (modColeccion.Entities.Count > 0)
                {
                    foreach (Entity modCRM in modColeccion.Entities)
                        MaestroAreasConocimientoCRM.Add(modCRM.GetAttributeValue<string>(AenAreadeconocimiento.PrimaryName), modCRM.Id);
                }
                #endregion Carga Maestro AreasConocimiento


                //3. Desactivar las Áreas de conocimiento inactivas desde origen
                #region Desactivar areas
                if (AreasADesactivar.Any())
                {
                    Guid auxG = Guid.Empty;
                    foreach (string str in AreasADesactivar)
                    {
                        var okk = MaestroAreasConocimientoCRM.TryGetValue(str, out auxG);
                        if (okk)
                        {
                            Entity en = new Entity(AenAreadeconocimiento.EntityName, auxG);
                            en[AenAreadeconocimiento.StateCode] = new OptionSetValue(1);
                            en[AenAreadeconocimiento.StatusCode] = new OptionSetValue(2);
                            CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = en});
                        }     
                    }
                }
                #endregion Desactivar areas


                //4. Limpiar Diccionarios para liberar memoria
                AreasConocimientoOracle.Clear();
                AreasConocimientoCRM.Clear();
                AreasADesactivar.Clear();


                return MaestroAreasConocimientoCRM;
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de AreasConocimiento ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();

                return null;
            }
        }




        private bool AreasConocimientoIguales(AreaDeConocimiento mod1, AreaDeConocimiento mod2)
        {
            return mod1.aen_name == mod2.aen_name &&
                mod1.statecode == mod2.statecode;
        }


        private void LeerAreasConocimientoFromOracle()
        {
            AreasConocimientoOracle = new Dictionary<string, AreaDeConocimiento>();
            string queryTot = Oracle.QueryAreasDeConocimiento;
            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            foreach (DataRow fila in dt.Rows)
            {
                var mod = new AreaDeConocimiento();
                mod.AreaConocimientoFromOracle(fila);
                if (mod != null)
                    AreasConocimientoOracle.Add(mod.aen_name, mod);
            }
        }


        private void LeerAreasConocimientoFromCRM()
        {
            var q = new QueryExpression(AenAreadeconocimiento.EntityName);
            q.Criteria.AddCondition(AenAreadeconocimiento.PrimaryName, ConditionOperator.NotNull);
            q.ColumnSet = new ColumnSet(AenAreadeconocimiento.PrimaryName, AenAreadeconocimiento.StateCode);
            var res = CrmGlobal.GetIOS().RetrieveMultiple(q);
            AreasConocimientoCRM = new Dictionary<string, AreaDeConocimiento>();
            foreach (var mod in res.Entities)
            {
                AreasConocimientoCRM.Add(
                    (string)mod[AenAreadeconocimiento.PrimaryName],
                    new AreaDeConocimiento
                    {
                        aen_areaconocimientoid = mod.Id,
                        aen_name = (string)mod[AenAreadeconocimiento.PrimaryName],
                        statecode = mod.Contains(AenAreadeconocimiento.StateCode) ? mod.GetAttributeValue<OptionSetValue>(AenAreadeconocimiento.StateCode).Value : 1,
                        statuscode = mod.Contains(AenAreadeconocimiento.StatusCode) ? mod.GetAttributeValue<OptionSetValue>(AenAreadeconocimiento.StatusCode).Value : 2
                    });
            }
        }

        #endregion METODOS
    }
}
