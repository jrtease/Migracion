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
    class LanzadorModalidades
    {
        #region PROPIEDADES
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, Modalidad> ModalidadesOracle, ModalidadesCRM;
        public Dictionary<string, Guid> MaestroModalidadesCRM;

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

                //1 Cargar Modalidades
                LeerModalidadesFromCRM();
                LeerModalidadesFromOracle();

                bool ok;
                Modalidad modalidad;
                if (ModalidadesOracle.Any())
                {
                    #region Tratamiento Modalidades
                    ComunGlobal.LogText("----   Iniciando CARGA MODALIDADES  ------");
                    ComunGlobal.LogText("****   Registros a cotejar: " + ModalidadesOracle.Count);
                    foreach (var mod in ModalidadesOracle)
                    {
                        ok = ModalidadesCRM.TryGetValue(mod.Key, out modalidad);
                        if (!ok)
                        {
                            var ent = new Entity(AenModalidad.EntityName);
                            ent[AenModalidad.PrimaryName] = modalidad.aen_name;
                            ent[AenModalidad.AenCodigomodalidad] = modalidad.aen_codigomodalidad;
                            CrmGlobal.AnadirElementoEmr(new CreateRequest { Target = ent });
                        }
                        else
                        {
                            bool sonIguales = ModalidadesIguales(mod.Value, modalidad);
                            if (sonIguales)
                                CrmGlobal.Iguales++;
                            else
                            {
                                var ent = new Entity(AenModalidad.EntityName, modalidad.aen_modalidadid);
                                ent[AenModalidad.PrimaryName] = modalidad.aen_name;
                                ent[AenModalidad.AenCodigomodalidad] = modalidad.aen_codigomodalidad;
                                CrmGlobal.AnadirElementoEmr(new UpdateRequest { Target = ent });
                            }
                        }
                    }
                    CrmGlobal.ProcesarUltimosEmr();

                    CrmGlobal.MostrarEstadisticas("MODALIDADES");
                    ComunGlobal.LogText("----   FINALIZADA CARGA MODALIDADES  ------");

                    #endregion Tratamiento Modalidades
                }
                else
                    ComunGlobal.LogText("No hay MODALIDADES en origen, terminamos");



                //2. Devolver Maestro de Modalidades
                #region Carga Maestro Modalidades
                MaestroModalidadesCRM = new Dictionary<string, Guid>();

                var q = new QueryExpression(AenModalidad.EntityName);
                q.Criteria.AddCondition(AenModalidad.AenCodigomodalidad, ConditionOperator.NotNull);
                q.ColumnSet = new ColumnSet(AenModalidad.AenCodigomodalidad, AenModalidad.PrimaryName);

                EntityCollection modColeccion = CrmGlobal.GetIOS().RetrieveMultiple(q);

                if (modColeccion.Entities.Count > 0)
                {
                    foreach (Entity modCRM in modColeccion.Entities)
                        MaestroModalidadesCRM.Add(modCRM.GetAttributeValue<string>(AenModalidad.AenCodigomodalidad), modCRM.Id);
                }
                #endregion Carga Maestro Modalidades


                //3. Limpiar Diccionarios para liberar memoria
                ModalidadesOracle.Clear();
                ModalidadesCRM.Clear();


                return MaestroModalidadesCRM;
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de MODALIDADES ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();

                return null;
            }
        }




        private bool ModalidadesIguales(Modalidad mod1, Modalidad mod2)
        {
            return mod1.aen_codigomodalidad == mod2.aen_codigomodalidad &&
                mod1.aen_name == mod2.aen_name;
        }


        private void LeerModalidadesFromOracle()
        {
            ModalidadesOracle = new Dictionary<string, Modalidad>();
            string queryTot = Oracle.QueryModalidades;
            OracleDataAdapter oda = new OracleDataAdapter(queryTot, ComunGlobal.ConnStringOracle);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            foreach (DataRow fila in dt.Rows)
            {
                var mod = new Modalidad();
                mod.ModalidadFromOracle(fila);
                if (mod != null)
                    ModalidadesOracle.Add(mod.aen_codigomodalidad, mod);
            }
        }


        private void LeerModalidadesFromCRM()
        {
            var q = new QueryExpression(AenModalidad.EntityName);
            q.Criteria.AddCondition(AenModalidad.AenCodigomodalidad, ConditionOperator.NotNull);
            q.ColumnSet = new ColumnSet(AenModalidad.AenCodigomodalidad, AenModalidad.PrimaryName);
            var res = CrmGlobal.GetIOS().RetrieveMultiple(q);
            ModalidadesCRM = new Dictionary<string, Modalidad>();
            foreach (var mod in res.Entities)
            {
                ModalidadesCRM.Add(
                    (string)mod[AenModalidad.AenCodigomodalidad],
                    new Modalidad
                    {
                        aen_modalidadid = mod.Id,
                        aen_codigomodalidad = (string)mod[AenModalidad.AenCodigomodalidad],
                        aen_name = mod.Contains(AenModalidad.PrimaryName) ? ComunGlobal.Left((string)mod[AenModalidad.PrimaryName], 100) : ""
                    });
            }
        }

        #endregion METODOS
    }
}
