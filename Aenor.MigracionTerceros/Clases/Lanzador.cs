using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aenor.MigracionTerceros
{
    internal class Lanzador
    {
        public Lanzador()
        {
        }

        #region BORRAR PRUEBAS
        private static void BorrarPrueba(Crm crm)
        {
            var q = new QueryExpression("account");
            q.Criteria.Conditions.AddRange(
                new ConditionExpression("createdon", ConditionOperator.Today)
                );
            var terceros = new EntityCollection();
            while (true)
            {
                var res = crm.GetIOS().RetrieveMultiple(q);
                terceros.Entities.AddRange(res.Entities);
                if (res.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = res.PagingCookie;
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
        }
        #endregion BORRAR PRUEBAS



        internal void Iniciar()
        {
            Stopwatch sW_TODO= new Stopwatch(), sW_cacheoDatos = new Stopwatch(), sW_createUpdate = new Stopwatch(), sW_leeOra = new Stopwatch(),
                sW_cnae = new Stopwatch(), sW_emparentar = new Stopwatch(), sW_desactivar = new Stopwatch();
                sW_TODO.Start();

            var comun = new Comun();

            //TODO 0. Obtener de CRM los campos parametrizados que van a intervenir en el proceso (entidad de configuracion)


            //1. Bajar los terceros de CRM a un dictionary<string, Tercero>: solo el id_oracle si no comparamos, o bien todos los campos a sincronizar si comparamos
            //diccionarios.getAllTercerosFromCRM();
            Crm crm = new Crm();
                sW_cacheoDatos.Start();
            crm.CachearInfoCRM();
                sW_cacheoDatos.Stop();
                comun.LogText(" ----->TIEMPO CACHEO DATOS: " + sW_cacheoDatos.Elapsed.ToString() + " <-----");

            //BorrarPrueba(crm);


            //2. Leer la tabla del Oracle
            Oracle oracle = new Oracle(crm);
            oracle.CadenaConOracle = comun.ConnStringOracle;
                sW_leeOra.Start();
            oracle.LeerTerceros();
                sW_leeOra.Stop();
                comun.LogText(" ----->TIEMPO LECTURA ORACLE: " + sW_leeOra.Elapsed.ToString() + " <-----");


            //3. Por cada elemento del diccionario de oracle, comprobar contra el diccionario crm. Sino se encuentra, se crea. Si se 
            // encuentra, se comprueba campo a campo para validar si se machaca o no
            // 4.Hacer las creates/ updates con executemultiplerequest
            bool ok;
            Tercero auxTerCRM = new Tercero();
            Entity tercUpdate;
                sW_createUpdate.Start();

            foreach (var terceroORACLE in oracle.Terceros)
            {
                ok = crm.TercerosCRM.TryGetValue(terceroORACLE.Key, out auxTerCRM);

                //Guardamos los Terceros que a posteriori desactivaremos
                if (terceroORACLE.Value.Statecode.Equals("Inactivo"))
                    crm.TercerosADesactivar.Add(terceroORACLE.Value.Aen_claveintegracion);

                if (ok) // Existe. Comprobar campo a campo
                {
                    try
                    {
                        tercUpdate = new Entity("account");
                        bool ret = comun.ActualizarTercero(terceroORACLE.Value, auxTerCRM, ref tercUpdate);

                        if (ret)
                            crm.AnadirElementoEmr(new UpdateRequest { Target = tercUpdate });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR al ACTUALIZAR el Entity equivalente del Tercero " + terceroORACLE.Value.Aen_claveintegracion + " ::: " + e.ToString());
                    }
                }
                else // No existe, se crea.
                {
                    try
                    {
                        Entity newTercero = terceroORACLE.Value.GetEntityFromTercero();
                        crm.AnadirElementoEmr(new CreateRequest { Target = newTercero});
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR al CREAR el Entity equivalente del Tercero " + terceroORACLE.Value.Aen_claveintegracion + " ::: " + e.ToString());
                    }
                }

            }
            crm.ProcesarUltimosEmr();
                sW_createUpdate.Stop();
                comun.LogText(" ----->TIEMPO CREATE/UPDATE TERCEROS: " + sW_createUpdate.Elapsed.ToString() + " <-----");


            //5. Carga de CNAES (TABLA CARGADA ANTERIORMENTE? A QUE ENTIDAD VA? SE PUEDE HACER EN ANTERIOR UPDATE/CREATE?)
                sW_cnae.Start();
            oracle.LeerCNAES();
            crm.CargaCNAEs(oracle.ListaCNAEs);
                sW_cnae.Stop();
                comun.LogText(" -----> END; TIEMPO CARGA CNAES: " + sW_cnae.Elapsed.ToString() + " <-----");


            //6. Emparentar terceros pendientes
                sW_emparentar.Start();
            crm.EmparentaTerceros();
                sW_emparentar.Stop();
                comun.LogText(" -----> END; TIEMPO EMPARENTACION TERCEROS: " + sW_emparentar.Elapsed.ToString() + " <-----");


            //7. Desactivar terceros que vienen como Inactivos
                sW_desactivar.Start();
            crm.DesactivarTerceros();
                sW_desactivar.Stop();
                comun.LogText(" -----> END; TIEMPO DESACTIVAR TERCEROS INACTIVOS: " + sW_desactivar.Elapsed.ToString() + " <-----");


            sW_TODO.Stop();
                comun.LogText(" -----> END; TIEMPO TOTAL: " + sW_TODO.Elapsed.ToString() + " <-----");
        }
    }
}