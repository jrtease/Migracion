using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aenor.MigracionProductosInternacionales.Clases
{ 
    /* Clase de cosas comunes: Conexiones, lectura de configuracion ....*/
    public class Comun
    {
        public string OracleConnString { get; set; }
        public string OracleEsquema { get; set; }

        [JsonIgnore]
        public DateTime FechaYHoraProcesoInicio { get; set; }

        [JsonIgnore]
        public int Errores { get; set; }

        [JsonIgnore]
        public int ProcesadosISO { get; set; }

        [JsonIgnore]
        public int ProcesadosASTM { get; set; }

        [JsonIgnore]
        public int ProcesadosIEC { get; set; }

        [JsonIgnore]
        public int ProcesadosIEEE { get; set; }

        [JsonIgnore]
        public string RutaLog { get; set; }

        [JsonIgnore]
        public string ResumenContadores
        {
            get { return "Normas ISO cargadas: " + ProcesadosISO + Environment.NewLine +
                    "Normas IEC cargadas: " + ProcesadosIEC + Environment.NewLine +
                    "Normas ASTM cargadas: " + ProcesadosASTM + Environment.NewLine +
                    "Normas IEEE cargadas: " + ProcesadosIEEE + Environment.NewLine + ", errores: " + Errores; }
        }

        //public string FechaIniIncremental, FechaFinIncremental;


        public void InicializarVariables()
        {
            var c = JsonConvert.DeserializeObject<Comun>(File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, @"config.json")));

            //Variables almacenadas
            this.OracleConnString = c.OracleConnString;
            this.OracleEsquema = c.OracleEsquema;
            
            //Resto
            this.ProcesadosISO = 0;
            this.ProcesadosIEC = 0;
            this.ProcesadosASTM = 0;
            this.ProcesadosIEEE = 0;
            this.Errores = 0;
            this.FechaYHoraProcesoInicio = DateTime.Now;
            this.RutaLog = Path.Combine(
                Environment.CurrentDirectory, "Logs",
                "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
        }

        //public void ActualizarConfig()
        //{
        //    this.FechaUltimaLectura = this.FechaYHoraProcesoInicio;
        //    string configLines = JsonConvert.SerializeObject(this, Formatting.Indented);
        //    File.WriteAllText("config.json", configLines);
        //}

        public string Left(string texto, int longitud)
        {
            if (string.IsNullOrEmpty(texto))
                return "";
            if (longitud > texto.Length)
                return texto;
            else
                return texto.Substring(0, longitud);
        }

        public void LogText(string message)
        {
            var rutaLog = Path.Combine(Environment.CurrentDirectory, "Logs",
                    "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            if (!File.Exists(rutaLog))
            {
                FileStream f = File.Create(rutaLog);
                f.Close();
            }
            using (StreamWriter sw = File.AppendText(rutaLog))
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss - ") + message);
        }
    }
}
