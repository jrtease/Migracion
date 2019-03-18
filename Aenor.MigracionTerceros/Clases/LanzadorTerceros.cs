using Aenor.MigracionTerceros.Objetos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Aenor.MigracionTerceros.Clases
{
    internal class LanzadorTerceros
    {
        public LanzadorTerceros()
        {
        }

        internal void Iniciar(Oracle oracleGlobal, Comun comunGlobal, Crm crmGlobal)
        {
            Stopwatch sW_TODO= new Stopwatch(), sW_cacheoDatos = new Stopwatch(), sW_createUpdate = new Stopwatch(), sW_leeOra = new Stopwatch(),
                sW_cnae = new Stopwatch(), sW_emparentar = new Stopwatch(), sW_desactivar = new Stopwatch(), sW_terguid = new Stopwatch(),
                sW_borrado = new Stopwatch();
                sW_TODO.Start();

            try
            {
                crmGlobal.InicializarPFE(oracleGlobal);
                //TODO 0. Obtener de CRM los campos parametrizados que van a intervenir en el proceso (entidad de configuracion)


                //1. Bajar los terceros de CRM a un dictionary<string, Tercero>: solo el id_oracle si no comparamos, o bien todos los campos a sincronizar si comparamos
                //diccionarios.getAllTercerosFromCRM();
                    sW_cacheoDatos.Start();
                crmGlobal.CachearInfoCRM();
                crmGlobal.CargaDiccionarioGuidsTercero();
                    sW_cacheoDatos.Stop();
                    comunGlobal.LogText(" ----->TIEMPO CACHEO DATOS: " + sW_cacheoDatos.Elapsed.ToString() + " <-----\n\n");


                //2. Leer la tabla del Oracle
                    sW_leeOra.Start();
                oracleGlobal.LeerTerceros();
                    sW_leeOra.Stop();
                    comunGlobal.LogText(" ----->TIEMPO LECTURA ORACLE: " + sW_leeOra.Elapsed.ToString() + " <-----\n\n");


                //3. Por cada elemento del diccionario de oracle, comprobar contra el diccionario crm. Sino se encuentra, se crea. Si se 
                //   encuentra, se comprueba campo a campo para validar si se machaca o no
                //4. Hacer las creates/ updates con PFE Core
                    sW_createUpdate.Start();
                crmGlobal.InicializarPFE(oracleGlobal);
                #region Tratamiento de TERCEROS
                bool ok;
                Tercero auxTerCRM = new Tercero();
                Entity tercUpdate;
                

                foreach (var terceroORACLE in oracleGlobal.Terceros)
                {
                    ok = crmGlobal.TercerosCRM.TryGetValue(terceroORACLE.Key, out auxTerCRM);

                    if (ok) // Existe. Comprobar campo a campo
                    {
                        try
                        {
                            tercUpdate = new Entity("account");
                            bool ret = comunGlobal.ActualizarTercero(crmGlobal.TercerosCRM,terceroORACLE.Value, auxTerCRM, ref tercUpdate);

                            if (ret)
                                crmGlobal.AnadirElementoEmr(new UpdateRequest { Target = tercUpdate });
                            else
                                crmGlobal.Iguales++;
                        }
                        catch (Exception e)
                        {
                            comunGlobal.LogText("ERROR al ACTUALIZAR el Entity equivalente del Tercero " + terceroORACLE.Value.Aen_claveintegracion + " ::: " + e.ToString());
                        }
                    }
                    else // No existe, se crea.
                    {
                        try
                        {
                            //Guardamos los Terceros que a posteriori desactivaremos (en create no se puede mandar registro inactivo)
                            if (terceroORACLE.Value.Statecode.Equals("Inactivo"))
                                crmGlobal.TercerosADesactivar.Add(terceroORACLE.Value.Aen_claveintegracion);

                            Entity newTercero = terceroORACLE.Value.GetEntityFromTercero();
                            crmGlobal.AnadirElementoEmr(new CreateRequest { Target = newTercero});
                        }
                        catch (Exception e)
                        {
                            comunGlobal.LogText("ERROR al CREAR el Entity equivalente del Tercero " + terceroORACLE.Value.Aen_claveintegracion + " ::: " + e.ToString());
                        }
                    }

                }
                crmGlobal.ProcesarUltimosEmr();
                #endregion Tratamiento de TERCEROS
                    sW_createUpdate.Stop();
                crmGlobal.MostrarEstadisticas("TERCEROS");
                    comunGlobal.LogText(" ----->TIEMPO CREATE/UPDATE TERCEROS: " + sW_createUpdate.Elapsed.ToString() + " <-----\n\n");
            

                //5. Carga de diccionario de terceros <Clave integración , Guid registro>
                    sW_terguid.Start();
                crmGlobal.CargaDiccionarioGuidsTercero();
                    sW_terguid.Stop();
                    comunGlobal.LogText(" -----> END; TIEMPO CARGA DICCIONARIO GUIDS TERCERO: " + sW_terguid.Elapsed.ToString() + " <-----\n\n");



                //6. Carga de CNAES (TABLA CARGADA ANTERIORMENTE? A QUE ENTIDAD VA? SE PUEDE HACER EN ANTERIOR UPDATE/CREATE?)
                    sW_cnae.Start();
                oracleGlobal.LeerCNAES();
                crmGlobal.CargaCNAEs(oracleGlobal.ListaCNAEs);
                //crmGlobal.EliminaOldCNAEs();
                crmGlobal.MostrarEstadisticas("CNAES");
                    sW_cnae.Stop();
                    comunGlobal.LogText(" -----> END; TIEMPO ACTUALIZACION CNAES (CREAR Y ELIMINAR LOS QUE NO VIENEN): " + sW_cnae.Elapsed.ToString() + " <-----\n\n");


                //7. Emparentar terceros pendientes
                    sW_emparentar.Start();
                crmGlobal.EmparentaTerceros();
                    crmGlobal.MostrarEstadisticas("EMPARENTACION");
                    sW_emparentar.Stop();
                    comunGlobal.LogText(" -----> END; TIEMPO EMPARENTACION TERCEROS: " + sW_emparentar.Elapsed.ToString() + " <-----\n\n");


                //8. Desactivar terceros que vienen como Inactivos
                    sW_desactivar.Start();
                crmGlobal.DesactivarTerceros();
                    crmGlobal.MostrarEstadisticas("DESACTIVACION TERCEROS");
                    sW_desactivar.Stop();
                    comunGlobal.LogText(" -----> END; TIEMPO DESACTIVAR TERCEROS INACTIVOS: " + sW_desactivar.Elapsed.ToString() + " <-----\n\n");


                //Liberamos diccionarios
                crmGlobal.LiberaDiccionarios();
                oracleGlobal.Terceros.Clear();//.Clear();

                sW_TODO.Stop();
                    comunGlobal.LogText(" -----> END; TIEMPO TOTAL: " + sW_TODO.Elapsed.ToString() + " <-----\n\n");


                //var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                //long totalBytesOfMemoryUsed = currentProcess.WorkingSet64 / 1048576;
                //comunGlobal.LogText("\n\n############################# TOP MEMORIA AL FINAL DE TERCEROS: " + totalBytesOfMemoryUsed.ToString() + " #############################\n\n");
            }
            catch (Exception e)
            {
                comunGlobal.LogText("ERROR en Lanzador de TERCEROS ::: " + e.ToString());
                if (oracleGlobal != null && oracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    oracleGlobal.OraConnParaLog.Dispose();
            }
        }
    }
}