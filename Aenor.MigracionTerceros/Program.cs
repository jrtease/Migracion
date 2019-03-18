using Aenor.MigracionTerceros.Clases;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros
{
    public static class Program
    {
        #region Propiedades Globales
        static Comun comunGlobal;
        static Crm crmGlobal;
        static Oracle oracleGlobal;
        #endregion Propiedades Globales

        #region BORRAR PRUEBAS
        private static void BorrarPruebas(Crm crm, Comun co)
        {
            #region CNAES Tercero
            var quece = new QueryExpression("aen_cnaetercero");
            //quece.TopCount = 5000;

            var cenaes = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(quece);
                cenaes.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    quece.PageInfo.PageNumber++;
                    quece.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in cenaes.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("aen_cnaetercero", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion CNAES Tercero

            co.LogText("\n=============================== CNAES BORRADOS ===================================\n");

            #region DIRECCIONES
            var qdir = new QueryExpression("aen_direccion");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qdir.TopCount = 5000;

            while (true)
            {
                var entidadesdir = Crm.IOS.RetrieveMultiple(qdir);
                foreach (var t in entidadesdir.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_direccion", t.Id) });
                if (entidadesdir.MoreRecords)
                {
                    qdir.PageInfo.PageNumber++;
                    qdir.PageInfo.PagingCookie = entidadesdir.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion DIRECCIONES

            co.LogText("\n=============================== DIRECCIONES BORRADAS ===================================\n");

            #region CONTACTOS
            var qcon = new QueryExpression("contact");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("contact", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion CONTACTOS

            co.LogText("\n=============================== CONTACTOS BORRADAS ===================================\n");

            #region CERTIFICACION
            qcon = new QueryExpression("aen_certificacion");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_certificacion", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion CERTIFICACION

            co.LogText("\n=============================== CERTIFICACION BORRADAS ===================================\n");

            #region aen_normacomprada
            qcon = new QueryExpression("aen_normacomprada");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_normacomprada", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion aen_normacomprada

            co.LogText("\n=============================== aen_normacomprada BORRADAS ===================================\n");

            #region aen_publicacionesadquiridas
            qcon = new QueryExpression("aen_publicacionesadquiridas");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_publicacionesadquiridas", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion aen_publicacionesadquiridas

            co.LogText("\n=============================== aen_publicacionesadquiridas BORRADAS ===================================\n");

            #region aen_potencialcliente
            qcon = new QueryExpression("aen_potencialcliente");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_potencialcliente", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion aen_potencialcliente

            co.LogText("\n=============================== aen_potencialcliente BORRADAS ===================================\n");


            #region aen_suscripcionadquirida
            qcon = new QueryExpression("aen_suscripcionadquirida");
            //q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            //qcon.TopCount = 5000;
            while (true)
            {
                var entidadescon = Crm.IOS.RetrieveMultiple(qcon);
                foreach (var t in entidadescon.Entities)
                    crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_suscripcionadquirida", t.Id) });
                if (entidadescon.MoreRecords)
                {
                    qcon.PageInfo.PageNumber++;
                    qcon.PageInfo.PagingCookie = entidadescon.PagingCookie;
                }
                else
                    break;
            }
            crm.ProcesarUltimosEmr();
            #endregion aen_suscripcionadquirida

            co.LogText("\n=============================== aen_suscripcionadquirida BORRADAS ===================================\n");


            #region TERCEROS
            var qter = new QueryExpression("account");
            /*q.Criteria.Conditions.AddRange(
                new ConditionExpression("createdon", ConditionOperator.Today)
                );*/
            //qter.TopCount = 5000;
            var terceros = new EntityCollection();
            while (true)
            {
                var res2 = crm.GetIOS().RetrieveMultiple(qter);
                terceros.Entities.AddRange(res2.Entities);
                if (res2.MoreRecords)
                {
                    qter.PageInfo.PageNumber++;
                    qter.PageInfo.PagingCookie = res2.PagingCookie;
                }
                else
                    break;
            }
            foreach (var t in terceros.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("account", t.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion TERCEROS

            co.LogText("\n=============================== TERCEROS BORRADOS ===================================\n");
        }
        #endregion BORRAR PRUEBAS

        static void Main(string[] args)
        {
            try
            {
                comunGlobal = new Comun();
                crmGlobal = new Crm(comunGlobal);
                oracleGlobal = new Oracle(crmGlobal, comunGlobal);

            }
            catch (Exception e)
            {
                comunGlobal.LogText("ERROR en MAIN ::: " + e.ToString());
                if (oracleGlobal != null && oracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    oracleGlobal.OraConnParaLog.Dispose();
                return;
            }

            //Estas dos lineas, para borrar (PRUEBAS SOLO)
            //crmGlobal.InicializarPFE(oracleGlobal);
            //BorrarPruebas(crmGlobal, comunGlobal);

            comunGlobal.LogText("&&&&&&&&&&&&&&&&&&&&&&&&& INICIO DEL PROCESO DE CARGA &&&&&&&&&&&&&&&&&&&&&&&&&\n\n\n");

            new LanzadorTerceros().Iniciar(oracleGlobal, comunGlobal, crmGlobal);

            new LanzadorDetalleNegocio().Iniciar(oracleGlobal, comunGlobal, crmGlobal);

            new LanzadorContactos().Iniciar(oracleGlobal, comunGlobal, crmGlobal);

            new LanzadorDirecciones().Iniciar(oracleGlobal, comunGlobal, crmGlobal);

            comunGlobal.LogText("&&&&&&&&&&&&&&&&&&&&&&&&& FIN DEL PROCESO DE CARGA &&&&&&&&&&&&&&&&&&&&&&&&&\n\n\n");

            oracleGlobal.CierraConexionOracle();

        }
    }
}
