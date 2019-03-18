using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aenor.MigracionFormacion.Clases
{ 
    /* Clase de mezcla de diccionarios, equivalencias, operaciones, ....*/
    public class Comun
    {
        public string ConnStringOracle, ConnStringCrm;
        public int Buffer, GradoDeParalelismo, TimeOutEnMinutos, DefaultConnectionLimit;
        public bool LogTextoWebJob;
        public string FechaIniIncremental, FechaFinIncremental;

        public Comun()
        {
            InicializarVariables();
        }


        public void InicializarVariables()
        {
            XDocument xmlDoc = XDocument.Load(Path.Combine(
                Environment.CurrentDirectory, @"Config.xml"));
            var q = from c in xmlDoc.Descendants("config")
                    select new
                    {
                        oracle = c.Element("oracle").Value,
                        crm = c.Element("crm").Value,
                        buffer = c.Element("buffer").Value,
                        gradodeparalelismo = c.Element("gradoparalelismo").Value,
                        timeoutenminutos = c.Element("timeoutminutos").Value,
                        defaultconnectionlimit = c.Element("defaultconnectionlimit").Value,
                        logtextowebjob = c.Element("logtextowebjob").Value,
                        fechainiincremental = c.Element("fechainiincremental").Value,
                        fechafinincremental = c.Element("fechafinincremental").Value,
                    };
            foreach (var p in q)
            {
                ConnStringOracle = p.oracle;
                ConnStringCrm = p.crm;
                Buffer = int.Parse(p.buffer);
                GradoDeParalelismo = int.Parse(p.gradodeparalelismo);
                TimeOutEnMinutos = int.Parse(p.timeoutenminutos);
                DefaultConnectionLimit = int.Parse(p.defaultconnectionlimit);
                LogTextoWebJob = bool.Parse(p.logtextowebjob);
                FechaIniIncremental = p.fechainiincremental;
                FechaFinIncremental = p.fechafinincremental;
            }
            //Uso de multihilo
            System.Net.ServicePointManager.DefaultConnectionLimit = DefaultConnectionLimit;
        }

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
            if (LogTextoWebJob)
                Console.WriteLine(message);
            else
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

        public void EscribirExcepcion(Exception ex, string entradilla)
        {
            var mensaje = "";
            if (ex.InnerException != null && ex.InnerException.Message != null)
                mensaje = ex.Message + "; ex interna: " + ex.InnerException.Message;
            else
                mensaje = ex.Message;
            LogText(entradilla + " - " + mensaje + Environment.NewLine +
                (ex is FaultException<OrganizationServiceFault> ? "Detalle: " + ((FaultException<OrganizationServiceFault>)ex).Detail.Message + Environment.NewLine : "") +
                "Pila: " + ex.StackTrace);
        }

    }
}
