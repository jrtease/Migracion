using Aenor.MigracionFormacion.Clases;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionFormacion
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
                    comunGlobal.LogText("|_________   Iniciando CARGA ENTRE:" + comunGlobal.FechaIniIncremental + " - " + comunGlobal.FechaFinIncremental + " _________|");
                    break;
            }
            #endregion Mostrar tipo carga

            sW_todo = new Stopwatch();
            sW_todo.Start();
            new LanzadorFormacion().Iniciar(oracleGlobal, comunGlobal, crmGlobal);
            sW_todo.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE CARGA: " + sW_todo.Elapsed.ToString());
        }
    }
}
