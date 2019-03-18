using Aenor.MigracionTerceros.Objetos;
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

namespace Aenor.MigracionTerceros.Clases
{ 
    /* Clase de mezcla de diccionarios, equivalencias, operaciones, ....*/
    public class Comun
    {
        public string ConnStringOracle, ConnStringCrm;
        public int Buffer, GradoDeParalelismo, TimeOutEnMinutos, DefaultConnectionLimit;
        public bool LogTextoWebJob;

        public Comun()
        {
            InicializarVariables();
        }


        /*
         * Funcion que comprueba campo a campo la info de terceroOracle con la de auxTerCRM y va creando la Entity account (tercero)
         * --> Key: true si es distinta y hay que machacar con  Value
         * --> Key: False si es la misma y no hay que actualizar nada
         * */
        public bool ActualizarTercero(Dictionary<string,Tercero> Tercero_CRM, Tercero terceroORACLE, Tercero auxTerCRM, ref Entity aActualizar)
        {
            bool retorno = false; //Son iguales mientras que no se pruebe lo contrario
            List<string> nombreCampoCambiante = new List<string>();

            if (!terceroORACLE.Aen_Acronimo.Equals(auxTerCRM.Aen_Acronimo))
                { retorno = true; nombreCampoCambiante.Add("ACRONIMO");}
            if (!terceroORACLE.Aen_An8.Equals(auxTerCRM.Aen_An8)
                && !(terceroORACLE.Aen_An8.Equals(string.Empty) && auxTerCRM.Aen_An8.Equals("0")))
                { retorno = true; nombreCampoCambiante.Add("AN8");}
            if (!terceroORACLE.Aen_Alumno.Equals(auxTerCRM.Aen_Alumno))
                { retorno = true; nombreCampoCambiante.Add("ALUMNO");}
            if (!terceroORACLE.Aen_Profesor.Equals(auxTerCRM.Aen_Profesor))
                { retorno = true; nombreCampoCambiante.Add("PROFESOR");}
            if (!terceroORACLE.Aen_Responsable.Equals(auxTerCRM.Aen_Responsable))
                { retorno = true; nombreCampoCambiante.Add("RESPONSABLE");}
            if (!terceroORACLE.Aen_Apellidos.Equals(auxTerCRM.Aen_Apellidos))
                { retorno = true; nombreCampoCambiante.Add("APELLIDOS");}
            if (!terceroORACLE.Emailaddress1.Equals(auxTerCRM.Emailaddress1))
                { retorno = true; nombreCampoCambiante.Add("EMAIL1");}
            if (!terceroORACLE.Parentaccountid.Equals(auxTerCRM.Parentaccountid))
                { retorno = true; nombreCampoCambiante.Add("PARENTACCOUNTID");}
            if (!terceroORACLE.Aen_Delegacionid.Equals(auxTerCRM.Aen_Delegacionid))
                { retorno = true; nombreCampoCambiante.Add("DELEGACION");}
            if (!terceroORACLE.Aen_Departamentoid.Equals(auxTerCRM.Aen_Departamentoid))
                { retorno = true; nombreCampoCambiante.Add("DEPTO");}
            if (!terceroORACLE.Aen_Paisdocumentoid.Equals(auxTerCRM.Aen_Paisdocumentoid))
                { retorno = true; nombreCampoCambiante.Add("PAISDOCUMENTO");}
            if (!terceroORACLE.Aen_Escliente.Equals(auxTerCRM.Aen_Escliente))
                { retorno = true; nombreCampoCambiante.Add("ESCLIENTE");}
            if (!terceroORACLE.Aen_Esclientecertool.Equals(auxTerCRM.Aen_Esclientecertool))
                { retorno = true; nombreCampoCambiante.Add("ESCERTOOL");}
            if (!terceroORACLE.Aen_Esclientelaboratorio.Equals(auxTerCRM.Aen_Esclientelaboratorio))
                { retorno = true; nombreCampoCambiante.Add("ESLABORATORIO");}
            if (!terceroORACLE.Aen_Esclienteweberratum.Equals(auxTerCRM.Aen_Esclienteweberratum))
                { retorno = true; nombreCampoCambiante.Add("WEBERRATUM");}
            if (!terceroORACLE.Aen_Escompradordenormas.Equals(auxTerCRM.Aen_Escompradordenormas))
                { retorno = true; nombreCampoCambiante.Add("COMPRANORMAS");}
            if (!terceroORACLE.Aen_Esempleado.Equals(auxTerCRM.Aen_Esempleado))
                { retorno = true; nombreCampoCambiante.Add("ESEMPLEADO");}
            if (!terceroORACLE.Aen_Eslibreria.Equals(auxTerCRM.Aen_Eslibreria))
                { retorno = true; nombreCampoCambiante.Add("ESLIBRERIA");}
            if (!terceroORACLE.Aen_Esmiembroctc.Equals(auxTerCRM.Aen_Esmiembroctc))
                { retorno = true; nombreCampoCambiante.Add("ESCTC");}
            if (!terceroORACLE.Aen_Esmiembroune.Equals(auxTerCRM.Aen_Esmiembroune))
                { retorno = true; nombreCampoCambiante.Add("ESMIEMBROUNE");}
            if (!terceroORACLE.Aen_Esorganismo.Equals(auxTerCRM.Aen_Esorganismo))
                { retorno = true; nombreCampoCambiante.Add("ESORGANISMO");}
            if (!terceroORACLE.Aen_Esproveedor.Equals(auxTerCRM.Aen_Esproveedor))
                { retorno = true; nombreCampoCambiante.Add("ESPROVEEDOR");}
            if (!terceroORACLE.Aen_Essuscriptor.Equals(auxTerCRM.Aen_Essuscriptor))
                { retorno = true; nombreCampoCambiante.Add("ESSUSCRIPTOR");}
            if (!terceroORACLE.Aen_Revistaaenor.Equals(auxTerCRM.Aen_Revistaaenor))
                { retorno = true; nombreCampoCambiante.Add("REVISTAAENOR");}
            if (!terceroORACLE.Statecode.Equals(auxTerCRM.Statecode))
                { retorno = true; nombreCampoCambiante.Add("STATE");}
            if (!terceroORACLE.Aen_Bloqueadocliente.Equals(auxTerCRM.Aen_Bloqueadocliente))
                { retorno = true; nombreCampoCambiante.Add("BLOQUEADOCLIENTE");}
            if (!terceroORACLE.Aen_Bloqueadoproveedor.Equals(auxTerCRM.Aen_Bloqueadoproveedor))
                { retorno = true; nombreCampoCambiante.Add("BLOQUEADOPROVEEDOR");}
            if (!terceroORACLE.Aen_Estadosolicitudclienteerp.Equals(auxTerCRM.Aen_Estadosolicitudclienteerp))
                { retorno = true; nombreCampoCambiante.Add("SOLCLIERP");}
            if (!terceroORACLE.Aen_Estadosolicitudempleadoerp.Equals(auxTerCRM.Aen_Estadosolicitudempleadoerp))
                { retorno = true; nombreCampoCambiante.Add("SOLEMPERP");}
            if (!terceroORACLE.Aen_Estadosolicitudproveedorerp.Equals(auxTerCRM.Aen_Estadosolicitudproveedorerp))
                { retorno = true; nombreCampoCambiante.Add("SOLPROVERP");}
            if (!terceroORACLE.Fax.Equals(auxTerCRM.Fax))
                { retorno = true; nombreCampoCambiante.Add("FAX");}
            if (!terceroORACLE.Aen_Fechadealta.Equals(auxTerCRM.Aen_Fechadealta))
                { retorno = true; nombreCampoCambiante.Add("FECHAALTA");}
            if (!terceroORACLE.Aen_Fechadebaja.Equals(auxTerCRM.Aen_Fechadebaja))
                { retorno = true; nombreCampoCambiante.Add("FECHABAJA");}
            if (!terceroORACLE.Aen_Identificadortercero.Equals(auxTerCRM.Aen_Identificadortercero))
                { retorno = true; nombreCampoCambiante.Add("IDENTTERCERO");}
            if (!terceroORACLE.Aen_claveintegracion.Equals(auxTerCRM.Aen_claveintegracion))
                { retorno = true; nombreCampoCambiante.Add("CLAVEINTEGRACION");}
            if (!terceroORACLE.Aen_Clienteerpid.Equals(auxTerCRM.Aen_Clienteerpid))
                { retorno = true; nombreCampoCambiante.Add("CLIENTERPID");}
            if (!terceroORACLE.Aen_Empleadoerpid.Equals(auxTerCRM.Aen_Empleadoerpid))
                { retorno = true; nombreCampoCambiante.Add("EMPLEADOERPID");}
            if (!terceroORACLE.Aen_Proveedorerpid.Equals(auxTerCRM.Aen_Proveedorerpid))
                { retorno = true; nombreCampoCambiante.Add("PROVERPID");}
            if (!terceroORACLE.Aen_Industriaaenor.Equals(auxTerCRM.Aen_Industriaaenor))
                { retorno = true; nombreCampoCambiante.Add("INDUSTRIAAENOR");}
            if ( !(terceroORACLE.Revenue.Equals(decimal.MinValue) && auxTerCRM.Revenue.Equals(decimal.MinValue)) 
                && (!terceroORACLE.Revenue.ToString("#,##").Equals(auxTerCRM.Revenue.ToString("#,##")) ))
                { retorno = true; nombreCampoCambiante.Add("REVENUE");}
            if (!terceroORACLE.Aen_loginempleado.Equals(auxTerCRM.Aen_loginempleado))
                { retorno = true; nombreCampoCambiante.Add("LOGIN");}
            if (!terceroORACLE.Aen_Nombredelcliente.Equals(auxTerCRM.Aen_Nombredelcliente))
                { retorno = true; nombreCampoCambiante.Add("NOMBRECLIENTE");}
            if (!terceroORACLE.Name.Equals(auxTerCRM.Name))
                { retorno = true; nombreCampoCambiante.Add("NAME");}
            if (!terceroORACLE.Aen_Numerodocumento.Equals(auxTerCRM.Aen_Numerodocumento))
                { retorno = true; nombreCampoCambiante.Add("NUMDOCUMENTO");}
            if (!terceroORACLE.Aen_Observaciones.Equals(auxTerCRM.Aen_Observaciones))
                { retorno = true; nombreCampoCambiante.Add("OBSERV");}
            if (!terceroORACLE.Aen_Origen.Equals(auxTerCRM.Aen_Origen))
                { retorno = true; nombreCampoCambiante.Add("ORIGEN");}
            if (!terceroORACLE.Aen_Riesgopagoaxesor.Equals(auxTerCRM.Aen_Riesgopagoaxesor))
                { retorno = true; nombreCampoCambiante.Add("RIESGOPAGOAXESOR");}
            if (!terceroORACLE.Aen_Sectoraenor.Equals(auxTerCRM.Aen_Sectoraenor))
                { retorno = true; nombreCampoCambiante.Add("SECTOR");}
            if (!terceroORACLE.Aen_Genero.Equals(auxTerCRM.Aen_Genero))
                { retorno = true; nombreCampoCambiante.Add("GENERO");}
            if (!terceroORACLE.Aen_Siglas.Equals(auxTerCRM.Aen_Siglas))
                { retorno = true; nombreCampoCambiante.Add("SIGLAS");}
            if (!terceroORACLE.Websiteurl.Equals(auxTerCRM.Websiteurl))
                { retorno = true; nombreCampoCambiante.Add("WEBSITE");}
            if (!terceroORACLE.Aen_Subtipodetercero.Equals(auxTerCRM.Aen_Subtipodetercero))
                { retorno = true; nombreCampoCambiante.Add("SUBTIPOT");}
            if (!terceroORACLE.Telephone1.Equals(auxTerCRM.Telephone1))
                { retorno = true; nombreCampoCambiante.Add("TELF1");}
            if (!terceroORACLE.Aen_Tipodocumento.Equals(auxTerCRM.Aen_Tipodocumento))
                { retorno = true; nombreCampoCambiante.Add("TIPODOC");}
            if (!terceroORACLE.Aen_Tipopersona.Equals(auxTerCRM.Aen_Tipopersona))
                { retorno = true; nombreCampoCambiante.Add("TIPOPERS");}
            if (!terceroORACLE.Aen_Observacionesmigracion.Equals(auxTerCRM.Aen_Observacionesmigracion))
                { retorno = true; nombreCampoCambiante.Add("OBSERVMIG");}
            if (!terceroORACLE.Numberofemployees.Equals(auxTerCRM.Numberofemployees)
                && !(terceroORACLE.Numberofemployees.Equals(string.Empty) && auxTerCRM.Numberofemployees.Equals("0")))
                { retorno = true; nombreCampoCambiante.Add("NUMEMP");}
            if (!terceroORACLE.Transactioncurrencyid.Equals(auxTerCRM.Transactioncurrencyid) 
                && !(terceroORACLE.Transactioncurrencyid.Equals(Guid.Empty) && auxTerCRM.TransactioncurrencyidString.Equals("Euro")))
                { retorno = true; nombreCampoCambiante.Add("TRANSACTCURRENCY");}
            if (!terceroORACLE.Aen_Webcorporativa.Equals(auxTerCRM.Aen_Webcorporativa)) { retorno = true; nombreCampoCambiante.Add("WEBCORP");}
            if (!terceroORACLE.Aen_Telefonocorporativo.Equals(auxTerCRM.Aen_Telefonocorporativo)) { retorno = true; nombreCampoCambiante.Add("TELFCORP");}
            if (!terceroORACLE.Aen_Correoelectronicocorporativo.Equals(auxTerCRM.Aen_Correoelectronicocorporativo)) { retorno = true; nombreCampoCambiante.Add("EMAILCORP");}
            if (!terceroORACLE.Aen_Condicionesdepagoid.Equals(auxTerCRM.Aen_Condicionesdepagoid)) { retorno = true; nombreCampoCambiante.Add("CONDICIONESDEPAGO");}
            if (!terceroORACLE.Aen_Formasdepagoid.Equals(auxTerCRM.Aen_Formasdepagoid)) { retorno = true; nombreCampoCambiante.Add("FORMASDEPAGO");}
            if (!terceroORACLE.Aen_Entradadelcliente.Equals(auxTerCRM.Aen_Entradadelcliente)) { retorno = true; nombreCampoCambiante.Add("ENTRADADELCLIENT");}
            if (!terceroORACLE.Aen_Evaluaciondelaconformidad.Equals(auxTerCRM.Aen_Evaluaciondelaconformidad)) { retorno = true; nombreCampoCambiante.Add("EVALCONFORMIDAD");}
            if (!terceroORACLE.Aen_Tipodocumentoempleado.Equals(auxTerCRM.Aen_Tipodocumentoempleado)) { retorno = true; nombreCampoCambiante.Add("TIPODOCEMPLEADO"); }
            if (!terceroORACLE.Aen_Numerodocumentoempleado.Equals(auxTerCRM.Aen_Numerodocumentoempleado)) { retorno = true; nombreCampoCambiante.Add("NUMDOCEMPLEADO"); }


            if (retorno)
            {
                terceroORACLE.Accountid = auxTerCRM.Accountid;
                aActualizar = terceroORACLE.GetEntityFromTercero();
            }

            return retorno;
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
                        logtextowebjob = c.Element("logtextowebjob").Value
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
