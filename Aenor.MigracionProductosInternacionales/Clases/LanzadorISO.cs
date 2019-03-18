using Aenor.MigracionProductosInternacionales.Objetos;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductosInternacionales.Clases
{
    class LanzadorISO
    {
        Oracle oracle;
        Comun comun;

        int normasIsoInsertadas;
        int normasIsoYaExistentes;
        int productosIsoInsertados;
        int productosIsoYaExistentes;
        int productosIsoSinNormaExistente;
        int icsEnlazados;
        int icsYaExistentesEnlazados;
        int erroresNormaIso;
        int erroresProdIso;
        int erroresRelacionIso;

        internal void Iniciar(Comun comunGlobal, Oracle oracleGlobal)
        {
            oracle = oracleGlobal;
            comun = comunGlobal;

            normasIsoInsertadas = productosIsoInsertados = normasIsoYaExistentes = productosIsoYaExistentes = productosIsoSinNormaExistente = icsEnlazados = icsYaExistentesEnlazados = erroresNormaIso = erroresProdIso = erroresRelacionIso = 0;

            //1. Queries con registros de tablas origen
            DataTable IsoNorm = oracleGlobal.GetElementsFromQuery(Oracle.QueryISONormas);
            comun.LogText("     * Registros de Normas ISO leídas de origen : " + IsoNorm.Rows.Count);
            DataTable IsoProd = oracleGlobal.GetElementsFromQuery(Oracle.QueryISOProductos);
            comun.LogText("     * Registros de Productos ISO leídos de origen : " + IsoProd.Rows.Count);
            DataTable IsoRel = oracleGlobal.GetElementsFromQuery(Oracle.QueryISOIcsNormaVersion);
            comun.LogText("     * Registros de Relaciones ICS-Norma ISO leídas de origen : " + IsoRel.Rows.Count);


            List<StoreProcedureParam> parametros;

            //2. Leer cada tabla registro a registro para insertar en tabla destino (intermedia) de la cual se volcará 
            //  a CRM con Aenor.MigracionProductos
            #region 2.1. NORMAS
            comun.LogText("     //-------- Cargando Normas ISO.....");
            string articuloActual = string.Empty;
            foreach (DataRow fila in IsoNorm.Rows)
            {
                try
                {
                    articuloActual = fila["aen_articulo"] == DBNull.Value ? string.Empty : fila["aen_articulo"].ToString().Trim();
                    parametros = new List<StoreProcedureParam>();
                    parametros.Add(new StoreProcedureParam("pAEN_ORGANISMO", "ISO", ParameterDirection.Input, OracleDbType.Varchar2));
                    parametros.Add(new StoreProcedureParam("pAEN_ARTICULO", articuloActual, ParameterDirection.Input, OracleDbType.Varchar2));

                    bool existente = oracle.ExistElementByKey(Oracle.TABLA_NORMAS_INTERMEDIA, "aen_articulo" , articuloActual);
                    string codigo_retorno = string.Empty;

                    if (!existente)
                    {
                        parametros.Add(new StoreProcedureParam("pAEN_IDENTIFICADOR_NEXO", (fila["aen_identificador_nexo"] == DBNull.Value ? string.Empty : fila["aen_identificador_nexo"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_CODIGO_NORMA", (fila["aen_codigo_norma"] == DBNull.Value ? string.Empty : fila["aen_codigo_norma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_RAIZ_NORMA", (fila["aen_raiz_norma"] == DBNull.Value ? string.Empty : fila["aen_raiz_norma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_EDICION", (fila["aen_fecha_edicion"] == DBNull.Value ? string.Empty : ((DateTime)fila["aen_fecha_edicion"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim()), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_ANULACION", (fila["aen_fecha_anulacion"] == DBNull.Value ? string.Empty : fila["aen_fecha_anulacion"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pAEN_ESTADO", (fila["aen_estado"] == DBNull.Value ? string.Empty : fila["aen_estado"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_TITULO_NORMA_EN", (fila["aen_titulo_norma_en"] == DBNull.Value ? string.Empty : fila["aen_titulo_norma_en"].ToString().Replace("'", "''").Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_NORMA_NUEVA", "I", ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_ACTUALIZACION", DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pCODIGO_ERROR", string.Empty, ParameterDirection.Output, OracleDbType.Int16));

                        codigo_retorno = oracle.EjecutarStoredProcedure(Oracle.FC_INSERTA_NORMAS, parametros);
                        normasIsoInsertadas++;
                    }
                    else
                    {
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_EDICION", (fila["aen_fecha_edicion"] == DBNull.Value ? string.Empty : ((DateTime)fila["aen_fecha_edicion"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim()), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_ANULACION", (fila["aen_fecha_anulacion"] == DBNull.Value ? string.Empty : fila["aen_fecha_anulacion"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pAEN_ESTADO", (fila["aen_estado"] == DBNull.Value ? string.Empty : fila["aen_estado"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_TITULO_NORMA_EN", (fila["aen_titulo_norma_en"] == DBNull.Value ? string.Empty : fila["aen_titulo_norma_en"].ToString().Replace("'", "''").Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_NORMA_NUEVA", "N", ParameterDirection.Input, OracleDbType.Varchar2));
                        parametros.Add(new StoreProcedureParam("pAEN_FECHA_ACTUALIZACION", DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), ParameterDirection.Input, OracleDbType.Date));
                        parametros.Add(new StoreProcedureParam("pCODIGO_ERROR", string.Empty, ParameterDirection.Output, OracleDbType.Int16));

                        codigo_retorno = oracle.EjecutarStoredProcedure(Oracle.FC_ACTUALIZA_NORMAS, parametros);
                        normasIsoYaExistentes++;
                    }


                    //0: Insertada correctamente -1: No insertada, ya existente. Restante, error.
                    if (!codigo_retorno.Equals("0") && !codigo_retorno.Equals("-1"))
                    {
                        erroresNormaIso++;
                        comun.LogText(" *** ERROR desconocido en la insercion de NORMA ISO: " + articuloActual);
                        oracle.MandarErrorIntegracion(articuloActual, "Art NORMA ISO:" + articuloActual, Oracle.TipoNormaISO, Oracle.TipoAccionInsert, null);
                    }
                }
                catch (Exception e)
                {
                    erroresNormaIso++;
                    comun.LogText(" *** ERROR en la insercion de NORMA ISO: " + articuloActual + " ::: " + e.ToString());
                    oracle.MandarErrorIntegracion(articuloActual, "Art NORMA ISO:" + articuloActual + "::" + comun.Left(e.ToString(), 950), Oracle.TipoNormaISO, Oracle.TipoAccionInsert, null);
                }
            }
            #endregion 2.1. NORMAS


            #region 2.2. PRODUCTOS
            string codigoProductoActual = string.Empty;
            comun.LogText("     //-------- Cargando Productos ISO.....");
            foreach (DataRow fila in IsoProd.Rows)
            {
                try
                {
                    codigoProductoActual = fila["aen_codigo_producto"] == DBNull.Value ? string.Empty : fila["aen_codigo_producto"].ToString().Trim();
                    articuloActual = fila["aen_articulo"] == DBNull.Value ? string.Empty : fila["aen_articulo"].ToString().Trim();

                    //TIENE QUE EXISTIR LA NORMA QUE ENLAZA CON EL PRODUCTO!
                    if (oracle.ExistElementByKey(Oracle.TABLA_NORMAS_INTERMEDIA, "aen_articulo", articuloActual))
                    {
                        if (!oracle.ExistElementByKey(Oracle.TABLA_PRODUCTOS_INTERMEDIA, "aen_codigo_producto", codigoProductoActual))
                        {
                            parametros = new List<StoreProcedureParam>();
                            parametros.Add(new StoreProcedureParam("pAEN_ORGANISMO", "ISO", ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_ARTICULO", articuloActual, ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_IDENTIFICADOR_NEXO", (fila["aen_identificador_nexo"] == DBNull.Value ? string.Empty : fila["aen_identificador_nexo"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_CODIGO_PRODUCTO", codigoProductoActual, ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_CODIGO_NORMA", (fila["aen_codigo_norma"] == DBNull.Value ? string.Empty : fila["aen_codigo_norma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_IDIOMA", (fila["aen_idioma"] == DBNull.Value ? string.Empty : fila["aen_idioma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_SOPORTE", (fila["aen_soporte"] == DBNull.Value ? string.Empty : fila["aen_soporte"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_DOCUMENTO", (fila["aen_documento"] == DBNull.Value ? string.Empty : fila["aen_documento"].ToString().Replace("'", "''").Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_PRECIO", (fila["aen_precio"] == DBNull.Value ? string.Empty : fila["aen_precio"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Decimal));
                            parametros.Add(new StoreProcedureParam("pAEN_VENDIBLE_WEB", (fila["aen_vendible_web"] == DBNull.Value ? string.Empty : fila["aen_vendible_web"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_PATH", (fila["aen_path"] == DBNull.Value ? string.Empty : fila["aen_path"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_FECHA_DOCUMENTO", (fila["aen_fecha_documento"] == DBNull.Value ? string.Empty : ((DateTime)fila["aen_fecha_documento"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim()), ParameterDirection.Input, OracleDbType.Date));
                            parametros.Add(new StoreProcedureParam("pAEN_FECHA_ACTUALIZACION", DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), ParameterDirection.Input, OracleDbType.Date));
                            parametros.Add(new StoreProcedureParam("pAEN_NOMBRE_PRODUCTO", (fila["aen_nombre_producto"] == DBNull.Value ? string.Empty : fila["aen_nombre_producto"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pCODIGO_ERROR", string.Empty, ParameterDirection.Output, OracleDbType.Int16));

                            string codigo_retorno = oracle.EjecutarStoredProcedure(Oracle.FC_INSERTA_PRODUCTOS, parametros);
                            productosIsoInsertados++;

                            //0: Insertada correctamente -1: No insertada, ya existente. Restante, error.
                            if (!codigo_retorno.Equals("0") && !codigo_retorno.Equals("-1"))
                            {
                                erroresProdIso++;
                                comun.LogText(" *** ERROR desconocido en la insercion de PRODUCTO ISO: " + codigoProductoActual);
                                oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO ISO:" + codigoProductoActual, Oracle.TipoProductoISO, Oracle.TipoAccionInsert, null);
                            }
                        }
                        else
                        {
                            //TODO UPDATE DE PRODUCTOS ?????
                            productosIsoYaExistentes++;
                        }
                    }
                    else
                    {
                        productosIsoSinNormaExistente++;
                        erroresProdIso++;
                        comun.LogText(" *** ERROR en la insercion de PRODUCTO ISO: " + codigoProductoActual + " --> NORMA INEXISTENTE EN DESTINO:" + articuloActual);
                        oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO ISO:" + codigoProductoActual + ":: NORMA INEXISTENTE EN DESTINO: " + articuloActual , Oracle.TipoProductoISO, Oracle.TipoAccionInsert, null);
                    }
                }
                catch (Exception e)
                {
                    erroresProdIso++;
                    comun.LogText(" *** ERROR en la insercion de PRODUCTO ISO: " + codigoProductoActual + " ::: " + e.ToString());
                    oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO ISO:" + codigoProductoActual + "::" + comun.Left(e.ToString(), 950), Oracle.TipoProductoISO, Oracle.TipoAccionInsert, null);
                }
            }
            #endregion 2.2. PRODUCTOS


            #region 2.3. RELACIONES
            //icsEnlazados;
            //icsYaExistentesEnlazados;


            #endregion 2.3. RELACIONES



            comun.LogText("     ** ESTADISTICAS ** ");
            comun.LogText("     --> Normas Iso Insertadas: " + normasIsoInsertadas);
            comun.LogText("     --> Normas Iso Ya Existentes en destino: " + normasIsoYaExistentes);
            comun.LogText("     --> Productos Iso Insertados: " + productosIsoInsertados);
            comun.LogText("     --> Productos Iso Ya Existentes en destino: " + productosIsoYaExistentes);
            comun.LogText("     --> Productos Iso sin norma Existente en destino: " + productosIsoSinNormaExistente);
            comun.LogText("     --> Ics enlazados con normas Iso: " + icsEnlazados);
            comun.LogText("     --> Ics con enlace Iso ya existente en destino: " + icsYaExistentesEnlazados);
            comun.LogText("     --> Errores Norma Iso: " + erroresNormaIso);
            comun.LogText("     --> Errores Producto Iso: " + erroresProdIso);
            comun.LogText("     --> Errores Relaciones Iso: " + erroresRelacionIso);
        }
    }
}
