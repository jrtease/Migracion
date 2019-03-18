using Aenor.MigracionProductos.Clases;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Aenor.MigracionProductos
{
    class Program
    {
        #region Propiedades Globales
        static Comun comunGlobal;
        static Crm crmGlobal;
        static Oracle oracleGlobal;

        static Stopwatch sW_todo;
        #endregion Propiedades Globales

        #region BORRAR PRUEBAS
        private static void BorrarPruebas(Crm crm, Comun co)
        {
            #region BORRAR PRODUCTOS
            var queprod = new QueryExpression("product");
            //quece.TopCount = 5000;

            var prods = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(queprod);
                prods.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    queprod.PageInfo.PageNumber++;
                    queprod.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in prods.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("product", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion BORRAR PRODuCTOS
            co.LogText("\n=============================== PRODUCTOS  BORRADOS ===================================\n");
            
            #region BORRAR ICS
            var queics = new QueryExpression("aen_ics");
            //quece.TopCount = 5000;

            var icss = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(queics);
                icss.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    queics.PageInfo.PageNumber++;
                    queics.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in icss.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("aen_ics", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion BORRAR ICS
            co.LogText("\n=============================== ICS  BORRADOS ===================================\n");

            #region BORRAR VERSION
            var quever = new QueryExpression("aen_versin");
            //quece.TopCount = 5000;

            var vers = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(quever);
                vers.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    quever.PageInfo.PageNumber++;
                    quever.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in vers.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("aen_versin", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion BORRAR VERSION
            co.LogText("\n=============================== VERSION  BORRADAS ===================================\n");

            #region BORRAR NORMA RAIZ
            var quenor = new QueryExpression("aen_norma");
            //quece.TopCount = 5000;

            var norm = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(quenor);
                norm.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    quenor.PageInfo.PageNumber++;
                    quenor.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in norm.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("aen_norma", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion BORRAR VERSION
            co.LogText("\n=============================== NORMAS  BORRADAS ===================================\n");

            #region BORRAR COMITES
            var quecom = new QueryExpression("aen_comitetecnicodenormalizacion");
            //quece.TopCount = 5000;

            var com = new EntityCollection();
            while (true)
            {
                var res1 = crm.GetIOS().RetrieveMultiple(quecom);
                com.Entities.AddRange(res1.Entities);
                if (res1.MoreRecords)
                {
                    quecom.PageInfo.PageNumber++;
                    quecom.PageInfo.PagingCookie = res1.PagingCookie;
                }
                else
                    break;
            }
            foreach (var ce in com.Entities)
            {
                crm.AnadirElementoEmr(new DeleteRequest()
                {
                    Target = new EntityReference("aen_comitetecnicodenormalizacion", ce.Id)
                });
            }
            crm.ProcesarUltimosEmr();
            #endregion BORRAR COMITES
            co.LogText("\n=============================== COMITES  BORRADAS ===================================\n");
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

            #region Mostrar tipo carga
            switch (comunGlobal.FechaFinIncremental)
            {
                case "0":
                    comunGlobal.LogText("|_________   Iniciando CARGA INICIAL _________|");
                    break;
                default:
                    comunGlobal.LogText("|_________   Iniciando CARGA ENTRE:" + comunGlobal.FechaIniIncremental + " - " + comunGlobal.FechaFinIncremental +  " _________|");
                    break;
            }
            #endregion Mostrar tipo carga

            sW_todo = new Stopwatch();
            sW_todo.Start();
                new LanzadorNormas().Iniciar(oracleGlobal, comunGlobal, crmGlobal);
            sW_todo.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE CARGA: " + sW_todo.Elapsed.ToString());
        }
    }
}
