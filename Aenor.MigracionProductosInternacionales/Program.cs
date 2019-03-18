using Aenor.MigracionProductosInternacionales.Clases;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductosInternacionales
{
    class Program
    {
        #region VARIABLES GLOBALES
        static Comun comunGlobal;
        static Oracle oracleGlobal;
        #endregion VARIABLES GLOBALES

        static void Main(string[] args)
        {
            #region INICIALIZAR GLOBALES
            comunGlobal = new Comun();
            comunGlobal.InicializarVariables();
            oracleGlobal = new Oracle(comunGlobal);

            Stopwatch sW_iso, sW_iec, sW_astm, sW_ieee;
            #endregion INICIALIZAR GLOBALES


            #region EJEMPLOS
            //string pAEN_ORGANISMO = "ISO";
            //string pAEN_ARTICULO = "001900NISOX007100";
            //string pAEN_IDENTIFICADOR_NEXO = "001900";
            //string pAEN_CODIGO_NORMA = "ISO 12261:1996";
            //string pAEN_RAIZ_NORMA = "ISO 12261";
            //string pAEN_FECHA_EDICION = "24/10/1996";
            //string pAEN_FECHA_ANULACION = "";
            //string pAEN_ESTADO = "Anulada";
            //string pAEN_TITULO_NORMA_EN = "Aerospace  Screws, pan head, interna offset cruciform ribbed or unribbed drive, pitch diameter shank, long length MJ threads, metallic material, coated or uncoated, strength classes less than or equal to 1 100 MPa -- Dimensions";
            //string pAEN_NORMA_NUEVA = "I";
            //string pAEN_FECHA_ACTUALIZACION = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            //string res = string.Empty;
            //string strParams = string.Empty;
            //using (OracleConnection connection = new OracleConnection(oracleGlobal.ConnStringOracle))
            //{
            //    connection.Open();
            //    OracleCommand command = new OracleCommand();
            //    command.Connection = connection;

            //    command.BindByName = true;
            //    //command.CommandType = CommandType.Text;
            //    command.CommandType = CommandType.StoredProcedure;

            //    //command.CommandText = "DECLARE RetVal NUMBER; BEGIN RetVal := YINYANG.PCRM_CARGA_DATOS_NORMASINT.fc_INSERTA_NORMASINT('ISO','001900NISOX007100','001900','','','','','','','',''); END;";
            //    //string paramet = "'"+pAEN_ORGANISMO+"','"+pAEN_ARTICULO + "', '" +pAEN_IDENTIFICADOR_NEXO + "', '" +pAEN_CODIGO_NORMA + "', '" +pAEN_RAIZ_NORMA + "', TO_DATE('" +pAEN_FECHA_EDICION + "'), TO_DATE('" +
            //    //   pAEN_FECHA_EDICION + "'), '" +pAEN_ESTADO + "', '" +pAEN_TITULO_NORMA_EN + "', '" +pAEN_NORMA_NUEVA + "', TO_DATE('" +pAEN_FECHA_ACTUALIZACION + "')";
            //    //command.CommandText = "DECLARE RetVal NUMBER; BEGIN RetVal := " + Oracle.FC_INSERTA_NORMASINT + "("+paramet+"); END;";
            //    //command.CommandText = Oracle.FC_INSERTA_NORMASINT + "(" + paramet + ")";
            //    //command.CommandText = Oracle.FC_INSERTA_NORMASINT + "(:AEN_ORGANISMO, :AEN_ARTICULO, :AEN_IDENTIFICADOR_NEXO, :AEN_CODIGO_NORMA, :AEN_RAIZ_NORMA, :AEN_FECHA_EDICION, :AEN_FECHA_ANULACION, :AEN_ESTADO, :AEN_TITULO_NORMA_EN, :AEN_NORMA_NUEVA, :AEN_FECHA_ACTUALIZACION)";
            //    //command.CommandText = Oracle.FC_INSERTA_NORMASINT + "()";
            //    //command.CommandText = "BEGIN :rv:= "+Oracle.FC_INSERTA_NORMASINT + "(" +
            //    //command.CommandText = Oracle.FC_INSERTA_NORMASINT + "(" +
            //    //command.CommandText = @"INSERT INTO YINYANG.TCRM_NORMASINT (
            //    //AEN_ORGANISMO, AEN_ARTICULO, AEN_IDENTIFICADOR_NEXO, AEN_CODIGO_NORMA, AEN_RAIZ_NORMA, AEN_FECHA_EDICION, AEN_FECHA_ANULACION, AEN_ESTADO, AEN_TITULO_NORMA_EN, AEN_NORMA_NUEVA, AEN_FECHA_ACTUALIZACION)
            //    //VALUES(
            //    //:pAEN_ORGANISMO, :pAEN_ARTICULO, :pAEN_IDENTIFICADOR_NEXO, :pAEN_CODIGO_NORMA, :pAEN_RAIZ_NORMA, :pAEN_FECHA_EDICION, :pAEN_FECHA_ANULACION, :pAEN_ESTADO, :pAEN_TITULO_NORMA_EN, :pAEN_NORMA_NUEVA, :pAEN_FECHA_ACTUALIZACION)";

            //    //command.Parameters.Add("rv", OracleDbType.Int32, ParameterDirection.ReturnValue);

            //    //command.Parameters.Add("pAEN_ORGANISMO", OracleDbType.Varchar2).Value = pAEN_ORGANISMO;
            //    //command.Parameters.Add("pAEN_ARTICULO", OracleDbType.Varchar2).Value = pAEN_ARTICULO;
            //    //command.Parameters.Add("pAEN_IDENTIFICADOR_NEXO", OracleDbType.Varchar2).Value = pAEN_IDENTIFICADOR_NEXO;
            //    //command.Parameters.Add("pAEN_CODIGO_NORMA", OracleDbType.Varchar2).Value = pAEN_CODIGO_NORMA;
            //    //command.Parameters.Add("pAEN_RAIZ_NORMA", OracleDbType.Varchar2).Value = pAEN_RAIZ_NORMA;
            //    //var okFecha = DateTime.TryParse(pAEN_FECHA_EDICION, out DateTime f1);
            //    //if (okFecha)
            //    //    command.Parameters.Add("pAEN_FECHA_EDICION", OracleDbType.Date).Value = f1;
            //    //else
            //    //    command.Parameters.Add("pAEN_FECHA_EDICION", OracleDbType.Date).Value = null;
            //    //okFecha = DateTime.TryParse(pAEN_FECHA_ANULACION, out DateTime f2);
            //    //if (okFecha)
            //    //    command.Parameters.Add("pAEN_FECHA_ANULACION", OracleDbType.Date).Value = f2;
            //    //else
            //    //    command.Parameters.Add("pAEN_FECHA_ANULACION", OracleDbType.Date).Value = null;
            //    //command.Parameters.Add("pAEN_ESTADO", OracleDbType.Varchar2).Value = pAEN_ESTADO;
            //    //command.Parameters.Add("pAEN_TITULO_NORMA_EN", OracleDbType.Varchar2).Value = pAEN_TITULO_NORMA_EN;
            //    //command.Parameters.Add("pAEN_NORMA_NUEVA", OracleDbType.Varchar2).Value = pAEN_NORMA_NUEVA;
            //    //okFecha = DateTime.TryParse(pAEN_FECHA_ACTUALIZACION, out DateTime f3);
            //    //if (okFecha)
            //    //    command.Parameters.Add("pAEN_FECHA_ACTUALIZACION", OracleDbType.Date).Value = f3;
            //    //else
            //    //    command.Parameters.Add("pAEN_FECHA_ACTUALIZACION", OracleDbType.Date).Value = null;


            //    //command.CommandText = "declare ret number; begin ret:=YINYANG.func_prueba('ISO'); end;";
            //    command.CommandText = "YINYANG.PROC_PRUEBA";
            //    command.Parameters.Add("organismo", OracleDbType.Varchar2, ParameterDirection.Input).Value = "ISO";

            //    try
            //    {
            //        var x = command.ExecuteNonQuery();
            //        res = x.ToString();
            //    }
            //    catch (OracleException ex)
            //    {
            //        comunGlobal.LogText(ex.ToString());
            //    }

            //    connection.Close();
            //    connection.Dispose();
            //}
            #endregion EJEMPLOS


            #region PROCESA ISO
            comunGlobal.LogText("__________ VOLCADO DE NORMAS ISO __________");
            sW_iso = new Stopwatch();
            sW_iso.Start();
            new LanzadorISO().Iniciar(comunGlobal, oracleGlobal);
            sW_iso.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE VOLCADO ISO: " + sW_iso.Elapsed.ToString());
            #endregion PROCESA ISO

            comunGlobal.LogText("***********************************************************************");

            #region PROCESA IEC
            comunGlobal.LogText("__________ VOLCADO DE NORMAS IEC __________");
            sW_iec = new Stopwatch();
            sW_iec.Start();
            new LanzadorIEC().Iniciar(comunGlobal, oracleGlobal);
            sW_iec.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE VOLCADO IEC: " + sW_iec.Elapsed.ToString());
            #endregion PROCESA IEC

            comunGlobal.LogText("***********************************************************************");

            #region PROCESA ASTM
            comunGlobal.LogText("__________ VOLCADO DE NORMAS ASTM __________");
            sW_astm = new Stopwatch();
            sW_astm.Start();
            new LanzadorASTM().Iniciar(comunGlobal, oracleGlobal);
            sW_astm.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE VOLCADO ASTM: " + sW_astm.Elapsed.ToString());
            #endregion PROCESA ASTM

            comunGlobal.LogText("***********************************************************************");

            #region PROCESA IEEE
            comunGlobal.LogText("__________ VOLCADO DE NORMAS IEEE __________");
            sW_ieee = new Stopwatch();
            sW_ieee.Start();
            new LanzadorIEEE().Iniciar(comunGlobal, oracleGlobal);
            sW_ieee.Stop();
            comunGlobal.LogText("|_________/====> TIEMPO DE VOLCADO IEEE: " + sW_ieee.Elapsed.ToString());
            #endregion PROCESA IEEE

            oracleGlobal.CierraConexionOracle();
        }
    }
}
