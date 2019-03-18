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
    class LanzadorIEC
    {
        Oracle oracle;
        Comun comun;

        int normasIecInsertadas;
        int normasIecYaExistentes;
        int productosIecInsertados;
        int productosIecYaExistentes;
        int productosIecSinNormaExistente;
        int erroresNormasIec;
        int erroresProdIec;

        internal void Iniciar(Comun comunGlobal, Oracle oracleGlobal)
        {
            oracle = oracleGlobal;
            comun = comunGlobal;

            normasIecInsertadas = productosIecInsertados = normasIecYaExistentes = productosIecYaExistentes = productosIecSinNormaExistente = erroresNormasIec = erroresProdIec = 0;

            //1. Queries con registros de tablas origen
            DataTable IecNorm = oracleGlobal.GetElementsFromQuery(Oracle.QueryIECNormas);
            comun.LogText("     * Registros de Normas Iec leídas de origen : " + IecNorm.Rows.Count);
            DataTable IecProd = oracleGlobal.GetElementsFromQuery(Oracle.QueryIECProductos);
            comun.LogText("     * Registros de Productos Iec leídos de origen : " + IecProd.Rows.Count);


            List<StoreProcedureParam> parametros;


            //2. Leer cada tabla registro a registro para insertar en tabla destino (intermedia) de la cual se volcará 
            //  a CRM con Aenor.MigracionProductos
            #region 2.1. NORMAS
            comun.LogText("     //-------- Cargando Normas IEC.....");
            string articuloActual = string.Empty;
            foreach (DataRow fila in IecNorm.Rows)
            {
                try
                {
                    articuloActual = fila["aen_articulo"] == DBNull.Value ? string.Empty : fila["aen_articulo"].ToString().Trim();
                    parametros = new List<StoreProcedureParam>();
                    parametros.Add(new StoreProcedureParam("pAEN_ORGANISMO", "IEC", ParameterDirection.Input, OracleDbType.Varchar2));
                    parametros.Add(new StoreProcedureParam("pAEN_ARTICULO", articuloActual, ParameterDirection.Input, OracleDbType.Varchar2));

                    bool existente = oracle.ExistElementByKey(Oracle.TABLA_NORMAS_INTERMEDIA, "aen_articulo", articuloActual);
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
                        normasIecInsertadas++;
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
                        normasIecYaExistentes++;
                    }


                    //0: Insertada correctamente -1: No insertada, ya existente. Restante, error.
                    if (!codigo_retorno.Equals("0") && !codigo_retorno.Equals("-1"))
                    {
                        erroresNormasIec++;
                        comun.LogText(" *** ERROR desconocido en la insercion de NORMA IEC: " + articuloActual);
                        oracle.MandarErrorIntegracion(articuloActual, "Art NORMA IEC:" + articuloActual, Oracle.TipoNormaIEC, Oracle.TipoAccionInsert, null);
                    }
                }
                catch (Exception e)
                {
                    erroresNormasIec++;
                    comun.LogText(" *** ERROR en la insercion de NORMA IEC: " + articuloActual + " ::: " + e.ToString());
                    oracle.MandarErrorIntegracion(articuloActual, "Art NORMA IEC:" + articuloActual + "::" + comun.Left(e.ToString(), 950), Oracle.TipoNormaIEC, Oracle.TipoAccionInsert, null);
                }
            }
            #endregion 2.1. NORMAS


            #region 2.2. PRODUCTOS
            string codigoProductoActual = string.Empty;
            comun.LogText("     //-------- Cargando Productos IEC.....");
            foreach (DataRow fila in IecProd.Rows)
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
                            parametros.Add(new StoreProcedureParam("pAEN_ORGANISMO", "IEC", ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_ARTICULO", articuloActual, ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_IDENTIFICADOR_NEXO", (fila["aen_identificador_nexo"] == DBNull.Value ? string.Empty : fila["aen_identificador_nexo"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_CODIGO_PRODUCTO", codigoProductoActual, ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_CODIGO_NORMA", (fila["aen_codigo_norma"] == DBNull.Value ? string.Empty : fila["aen_codigo_norma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_IDIOMA", (fila["aen_idioma"] == DBNull.Value ? string.Empty : fila["aen_idioma"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_SOPORTE", (fila["aen_soporte"] == DBNull.Value ? string.Empty : fila["aen_soporte"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_DOCUMENTO", (fila["aen_documento"] == DBNull.Value ? string.Empty : fila["aen_documento"].ToString().Replace("'", "''").Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_PRECIO", (fila["aen_precio"] == DBNull.Value ? string.Empty : fila["aen_precio"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Decimal));
                            parametros.Add(new StoreProcedureParam("pAEN_VENDIBLE_WEB", "S", ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_PATH", (fila["aen_path"] == DBNull.Value ? string.Empty : fila["aen_path"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pAEN_FECHA_DOCUMENTO", (fila["aen_fecha_documento"] == DBNull.Value ? string.Empty : ((DateTime)fila["aen_fecha_documento"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim()), ParameterDirection.Input, OracleDbType.Date));
                            parametros.Add(new StoreProcedureParam("pAEN_FECHA_ACTUALIZACION", DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), ParameterDirection.Input, OracleDbType.Date));
                            parametros.Add(new StoreProcedureParam("pAEN_NOMBRE_PRODUCTO", (fila["aen_nombre_producto"] == DBNull.Value ? string.Empty : fila["aen_nombre_producto"].ToString().Trim()), ParameterDirection.Input, OracleDbType.Varchar2));
                            parametros.Add(new StoreProcedureParam("pCODIGO_ERROR", string.Empty, ParameterDirection.Output, OracleDbType.Int16));

                            string codigo_retorno = oracle.EjecutarStoredProcedure(Oracle.FC_INSERTA_PRODUCTOS, parametros);
                            productosIecInsertados++;

                            //0: Insertada correctamente -1: No insertada, ya existente. Restante, error.
                            if (!codigo_retorno.Equals("0") && !codigo_retorno.Equals("-1"))
                            {
                                erroresProdIec++;
                                comun.LogText(" *** ERROR desconocido en la insercion de PRODUCTO IEC: " + codigoProductoActual);
                                oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO IEC:" + codigoProductoActual, Oracle.TipoProductoIEC, Oracle.TipoAccionInsert, null);
                            }
                        }
                        else
                        {
                            //TODO UPDATE DE PRODUCTOS ?????
                            productosIecYaExistentes++;
                        }
                    }
                    else
                    {
                        erroresProdIec++;
                        productosIecSinNormaExistente++;
                        comun.LogText(" *** ERROR en la insercion de PRODUCTO IEC: " + codigoProductoActual + " --> NORMA INEXISTENTE EN DESTINO:" + articuloActual);
                        oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO IEC:" + codigoProductoActual + ":: NORMA INEXISTENTE EN DESTINO: " + articuloActual, Oracle.TipoProductoIEC, Oracle.TipoAccionInsert, null);
                    }
                }
                catch (Exception e)
                {
                    erroresProdIec++;
                    comun.LogText(" *** ERROR en la insercion de PRODUCTO IEC: " + codigoProductoActual + " ::: " + e.ToString());
                    oracle.MandarErrorIntegracion(codigoProductoActual, "Cod PRODUCTO IEC:" + codigoProductoActual + "::" + comun.Left(e.ToString(), 950), Oracle.TipoProductoIEC, Oracle.TipoAccionInsert, null);
                }
            }
            #endregion 2.2. PRODUCTOS




            comun.LogText("     ** ESTADISTICAS ** ");
            comun.LogText("     --> Normas Iec Insertadas: " + normasIecInsertadas);
            comun.LogText("     --> Normas Iec Ya Existentes en destino: " + normasIecYaExistentes);
            comun.LogText("     --> Productos Iec Insertados: " + productosIecInsertados);
            comun.LogText("     --> Productos Iec Ya Existentes en destino: " + productosIecYaExistentes);
            comun.LogText("     --> Productos Iec sin norma Existente en destino: " + productosIecSinNormaExistente);
            comun.LogText("     --> Errores Norma Iec: " + erroresNormasIec);
            comun.LogText("     --> Errores Producto Iec: " + erroresProdIec);
        }
    }
}
