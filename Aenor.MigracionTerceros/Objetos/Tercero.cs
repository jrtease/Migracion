using Aenor.MigracionTerceros.Clases;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Objetos
{
    public class Tercero
    {
        #region Propiedades
        public Guid Accountid { get; set; }
        public string Aen_An8 { get; set; }
        public string Aen_Acronimo { get; set; }
        public bool Aen_Alumno { get; set; }
        public bool Aen_Profesor { get; set; }
        public bool Aen_Responsable { get; set; }
        public string Aen_Apellidos { get; set; }
        public string Emailaddress1 { get; set; }
        public Guid Parentaccountid { get; set; }
        public string ParentaccountidSTR { get; set; }
        public Guid Aen_Delegacionid { get; set; }
        public Guid Aen_Departamentoid { get; set; }
        public Guid Aen_Paisdocumentoid { get; set; }
        public bool Aen_Escliente { get; set; }
        public bool Aen_Esclientecertool { get; set; }
        public bool Aen_Esclientelaboratorio { get; set; }
        public bool Aen_Esclienteweberratum { get; set; }
        public bool Aen_Escompradordenormas { get; set; }
        public bool Aen_Esempleado { get; set; }
        public bool Aen_Eslibreria { get; set; }
        public bool Aen_Esmiembroctc { get; set; }
        public bool Aen_Esmiembroune { get; set; }
        public bool Aen_Esorganismo { get; set; }
        public bool Aen_Espotencialcliente { get; set; }
        public bool Aen_Esproveedor { get; set; }
        public bool Aen_Essuscriptor { get; set; }
        public bool Aen_Revistaaenor { get; set; }
        public string Statecode { get; set; }
        public string Aen_Bloqueadocliente { get; set; }
        public string Aen_Bloqueadoproveedor { get; set; }
        public string Aen_Estadosolicitudclienteerp { get; set; }
        public string Aen_Estadosolicitudempleadoerp { get; set; }
        public string Aen_Estadosolicitudproveedorerp { get; set; }
        public string Fax { get; set; }
        public string Aen_Fechadealta { get; set; }
        public string Aen_Fechadebaja { get; set; }
        public string Aen_Identificadortercero { get; set; }
        public string Aen_claveintegracion { get; set; }
        public string Aen_Clienteerpid { get; set; }
        public string Aen_Empleadoerpid { get; set; }
        public string Aen_Proveedorerpid { get; set; }
        public Guid Aen_Industriaaenor { get; set; }
        public decimal Revenue { get; set; }
        public string Aen_loginempleado { get; set; }
        public string Aen_Nombredelcliente { get; set; }
        public string Name { get; set; }
        public string Aen_Numerodocumento { get; set; }
        public string Aen_Observaciones { get; set; }
        public string Aen_Origen { get; set; }
        public string Aen_Riesgopagoaxesor { get; set; }
        public Guid Aen_Sectoraenor { get; set; }
        public string Aen_Genero { get; set; }
        public string Aen_Siglas { get; set; }
        public string Websiteurl { get; set; }
        public Guid Aen_Subtipodetercero { get; set; }
        public string Telephone1 { get; set; }
        public string Aen_Tipodocumento { get; set; }
        public string Aen_Tipopersona { get; set; }
        //public Guid Aen_Tipoproveedor { get; set; }
        public string Aen_Observacionesmigracion { get; set; }
        public string Numberofemployees { get; set; }
        public Guid Transactioncurrencyid { get; set; }
        public string TransactioncurrencyidString { get; set; }
        public string Aen_Webcorporativa { get; set; }
        public string Aen_Telefonocorporativo { get; set; }
        public string Aen_Correoelectronicocorporativo { get; set; }
        public Guid Aen_Condicionesdepagoid { get; set; }
        public Guid Aen_Formasdepagoid { get; set; }
        public string Aen_Entradadelcliente { get; set; }
        public bool Aen_Evaluaciondelaconformidad { get; set; }
        public bool Aen_Escompradordelibros { get; set; }
        public string Aen_Tipodocumentoempleado { get; set; }
        public string Aen_Numerodocumentoempleado { get; set; }
        #endregion Propiedades


        #region Métodos
        public Tercero()
        {
        }

        public void TerceroFromCRM(Entity ter)
        {
            this.Accountid = ter.Id;

            this.Aen_Acronimo = ter.Contains(NombreCamposTercero.Aen_AcronimoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_AcronimoCRM).Trim() : string.Empty;
            this.Aen_An8 = ter.Contains(NombreCamposTercero.Aen_An8CRM) ? ter.GetAttributeValue<int>(NombreCamposTercero.Aen_An8CRM).ToString().Trim() : string.Empty;
            this.Aen_Alumno = ter.Contains(NombreCamposTercero.Aen_AlumnoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_AlumnoCRM) : false;
            this.Aen_Profesor = ter.Contains(NombreCamposTercero.Aen_ProfesorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_ProfesorCRM) : false;
            this.Aen_Responsable = ter.Contains(NombreCamposTercero.Aen_ResponsableCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_ResponsableCRM) : false;
            this.Aen_Apellidos = ter.Contains(NombreCamposTercero.Aen_ApellidosCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ApellidosCRM).Trim() : string.Empty;
            this.Emailaddress1 = ter.Contains(NombreCamposTercero.Emailaddress1CRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Emailaddress1CRM).Trim() : string.Empty;
            this.Parentaccountid = ter.Contains(NombreCamposTercero.ParentaccountidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.ParentaccountidCRM).Id : Guid.Empty;
            //if(this.Parentaccountid != Guid.Empty)
            //{
            //    Entity par = Crm.IOS.Retrieve("account", this.Parentaccountid, new Microsoft.Xrm.Sdk.Query.ColumnSet(new string[] { "aen_claveintegracion" }));
            //    if (par.Contains("aen_claveintegracion") && !string.IsNullOrEmpty(par.GetAttributeValue<string>("aen_claveintegracion")))
            //        this.ParentaccountidSTR = par.GetAttributeValue<string>("aen_claveintegracion");
            //}
            this.Aen_Delegacionid = ter.Contains(NombreCamposTercero.Aen_DelegacionidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_DelegacionidCRM).Id : Guid.Empty;
            this.Aen_Departamentoid = ter.Contains(NombreCamposTercero.Aen_DepartamentoidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_DepartamentoidCRM).Id : Guid.Empty;
            this.Aen_Escliente = ter.Contains(NombreCamposTercero.Aen_EsclienteCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclienteCRM) : false;
            this.Aen_Esclientecertool = ter.Contains(NombreCamposTercero.Aen_EsclientecertoolCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclientecertoolCRM) : false;
            this.Aen_Esclientelaboratorio = ter.Contains(NombreCamposTercero.Aen_EsclientelaboratorioCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclientelaboratorioCRM) : false;
            this.Aen_Esclienteweberratum = ter.Contains(NombreCamposTercero.Aen_EsclienteweberratumCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclienteweberratumCRM) : false;
            this.Aen_Escompradordenormas = ter.Contains(NombreCamposTercero.Aen_EscompradordenormasCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscompradordenormasCRM) : false;
            this.Aen_Esempleado = ter.Contains(NombreCamposTercero.Aen_EsempleadoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsempleadoCRM) : false;
            this.Aen_Eslibreria = ter.Contains(NombreCamposTercero.Aen_EslibreriaCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EslibreriaCRM) : false;
            this.Aen_Esmiembroctc = ter.Contains(NombreCamposTercero.Aen_EsmiembroctcCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsmiembroctcCRM) : false;
            this.Aen_Esmiembroune = ter.Contains(NombreCamposTercero.Aen_EsmiembrouneCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsmiembrouneCRM) : false;
            this.Aen_Esorganismo = ter.Contains(NombreCamposTercero.Aen_EsorganismoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsorganismoCRM) : false;
            this.Aen_Espotencialcliente = ter.Contains(NombreCamposTercero.Aen_EspotencialclienteCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EspotencialclienteCRM) : false;
            this.Aen_Esproveedor = ter.Contains(NombreCamposTercero.Aen_EsproveedorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsproveedorCRM) : false;
            this.Aen_Essuscriptor = ter.Contains(NombreCamposTercero.Aen_EssuscriptorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EssuscriptorCRM) : false;
            this.Aen_Revistaaenor = ter.Contains(NombreCamposTercero.Aen_RevistaaenorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_RevistaaenorCRM) : false;
            this.Statecode = ter.Contains(NombreCamposTercero.StatecodeORACLE) ? ((ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.StatecodeORACLE)).Value == 0 ? "Activo" : "Inactivo") : string.Empty;
            this.Aen_Bloqueadocliente = ter.Contains(NombreCamposTercero.Aen_BloqueadoclienteCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_BloqueadoclienteCRM).Value.ToString() : string.Empty;
            this.Aen_Bloqueadoproveedor = ter.Contains(NombreCamposTercero.Aen_BloqueadoproveedorCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_BloqueadoproveedorCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudclienteerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudempleadoerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudproveedorerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM).Value.ToString() : string.Empty;
            this.Fax = ter.Contains(NombreCamposTercero.FaxCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.FaxCRM).Trim() : string.Empty;
            this.Aen_Fechadealta = ter.Contains(NombreCamposTercero.Aen_FechadealtaCRM) ? ter.GetAttributeValue<DateTime>(NombreCamposTercero.Aen_FechadealtaCRM).ToLocalTime().ToString("dd/MM/yy").Trim() : string.Empty;
            this.Aen_Fechadebaja = ter.Contains(NombreCamposTercero.Aen_FechadebajaCRM) ? ter.GetAttributeValue<DateTime>(NombreCamposTercero.Aen_FechadebajaCRM).ToLocalTime().ToString("dd/MM/yy").Trim() : string.Empty;
            this.Aen_Identificadortercero = ter.Contains(NombreCamposTercero.Aen_IdentificadorterceroCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_IdentificadorterceroCRM).Trim() : string.Empty;
            this.Aen_claveintegracion = ter.Contains(NombreCamposTercero.Aen_claveintegracionCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_claveintegracionCRM).Trim() : string.Empty;
            this.Aen_Clienteerpid = ter.Contains(NombreCamposTercero.Aen_ClienteerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ClienteerpidCRM).Trim() : string.Empty;
            this.Aen_Empleadoerpid = ter.Contains(NombreCamposTercero.Aen_EmpleadoerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_EmpleadoerpidCRM).Trim() : string.Empty;
            this.Aen_Proveedorerpid = ter.Contains(NombreCamposTercero.Aen_ProveedorerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ProveedorerpidCRM).Trim() : string.Empty;
            this.Aen_Industriaaenor = ter.Contains(NombreCamposTercero.Aen_IndustriaaenorCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_IndustriaaenorCRM).Id : Guid.Empty;
            this.Revenue = ter.Contains(NombreCamposTercero.RevenueCRM) ? ter.GetAttributeValue<Money>(NombreCamposTercero.RevenueCRM).Value : decimal.MinValue;
            this.Aen_loginempleado = ter.Contains(NombreCamposTercero.Aen_loginempleadoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_loginempleadoCRM).Trim() : string.Empty;
            this.Aen_Nombredelcliente = ter.Contains(NombreCamposTercero.Aen_NombredelclienteCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_NombredelclienteCRM).Trim() : string.Empty;
            this.Name = ter.Contains(NombreCamposTercero.NameCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.NameCRM).Trim() : string.Empty;
            this.Aen_Numerodocumento = ter.Contains(NombreCamposTercero.Aen_NumerodocumentoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_NumerodocumentoCRM).Trim() : string.Empty;
            this.Aen_Observaciones = ter.Contains(NombreCamposTercero.Aen_ObservacionesCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ObservacionesCRM).Trim() : string.Empty;
            this.Aen_Origen = ter.Contains(NombreCamposTercero.Aen_OrigenCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_OrigenCRM).Value.ToString() : string.Empty;
            this.Aen_Paisdocumentoid= ter.Contains(NombreCamposTercero.Aen_PaisdocumentoidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_PaisdocumentoidCRM).Id : Guid.Empty;
            this.Aen_Riesgopagoaxesor = ter.Contains(NombreCamposTercero.Aen_RiesgopagoaxesorCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_RiesgopagoaxesorCRM).Value.ToString() : string.Empty;
            this.Aen_Sectoraenor = ter.Contains(NombreCamposTercero.Aen_SectoraenorCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_SectoraenorCRM).Id : Guid.Empty;
            this.Aen_Genero = ter.Contains(NombreCamposTercero.Aen_GeneroCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_GeneroCRM).Value.ToString() : string.Empty;
            this.Aen_Siglas = ter.Contains(NombreCamposTercero.Aen_SiglasCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_SiglasCRM).Trim() : string.Empty;
            this.Websiteurl = ter.Contains(NombreCamposTercero.WebsiteurlCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.WebsiteurlCRM).Trim() : string.Empty;
            this.Aen_Subtipodetercero = ter.Contains(NombreCamposTercero.Aen_SubtipodeterceroCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_SubtipodeterceroCRM).Id : Guid.Empty;
            this.Telephone1 = ter.Contains(NombreCamposTercero.Telephone1CRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Telephone1CRM).Trim() : string.Empty;
            this.Aen_Tipodocumento = ter.Contains(NombreCamposTercero.Aen_TipodocumentoCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_TipodocumentoCRM).Value.ToString() : string.Empty;
            this.Aen_Tipopersona = ter.Contains(NombreCamposTercero.Aen_TipopersonaCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_TipopersonaCRM).Value.ToString() : string.Empty;
            //this.Aen_Tipoproveedor = ter.Contains(NombreCamposTercero.Aen_TipoproveedorCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_TipoproveedorCRM).Value.ToString() : string.Empty;
            this.Aen_Observacionesmigracion = ter.Contains(NombreCamposTercero.Aen_ObservacionesmigracionCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ObservacionesmigracionCRM).Trim() : string.Empty;
            this.Numberofemployees = ter.Contains(NombreCamposTercero.NumberofemployeesCRM) ? ter.GetAttributeValue<int>(NombreCamposTercero.NumberofemployeesCRM).ToString() : string.Empty;
            this.Transactioncurrencyid = ter.Contains(NombreCamposTercero.TransactioncurrencyidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.TransactioncurrencyidCRM).Id : Guid.Empty;
            this.TransactioncurrencyidString = ter.Contains(NombreCamposTercero.TransactioncurrencyidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.TransactioncurrencyidCRM).Name : string.Empty;
            this.Aen_Webcorporativa = ter.Contains(NombreCamposTercero.Aen_WebcorporativaCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_WebcorporativaCRM).Trim() : string.Empty;
            this.Aen_Telefonocorporativo = ter.Contains(NombreCamposTercero.Aen_TelefonocorporativoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_TelefonocorporativoCRM).Trim(): string.Empty ;
            this.Aen_Correoelectronicocorporativo = ter.Contains(NombreCamposTercero.Aen_CorreoelectronicocorporativoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_CorreoelectronicocorporativoCRM).Trim(): string.Empty;
            this.Aen_Condicionesdepagoid = ter.Contains(NombreCamposTercero.Aen_CondicionesdepagoidCRM) ? (ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_CondicionesdepagoidCRM)).Id: Guid.Empty;
            this.Aen_Formasdepagoid = ter.Contains(NombreCamposTercero.Aen_FormasdepagoidCRM) ? (ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_FormasdepagoidCRM)).Id : Guid.Empty;
            this.Aen_Entradadelcliente = ter.Contains(NombreCamposTercero.Aen_EntradadelclienteCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EntradadelclienteCRM).Value.ToString() : string.Empty;
            this.Aen_Evaluaciondelaconformidad = ter.Contains(NombreCamposTercero.Aen_EvaluaciondelaconformidadCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EvaluaciondelaconformidadCRM) : false;
            this.Aen_Escompradordelibros = ter.Contains(NombreCamposTercero.Aen_EscompradordelibrosCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscompradordelibrosCRM) : false;
            this.Aen_Tipodocumentoempleado = ter.Contains(NombreCamposTercero.Aen_TipodocumentoempleadoCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_TipodocumentoempleadoCRM).Value.ToString() : string.Empty;
            this.Aen_Numerodocumentoempleado = ter.Contains(NombreCamposTercero.Aen_NumerodocumentoempleadoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_NumerodocumentoempleadoCRM) : string.Empty;
        }

        public Entity GetEntityFromTercero()
        {
            Entity ter = new Entity("account");

            ter[NombreCamposTercero.Aen_claveintegracionCRM] = Aen_claveintegracion;
            ter[NombreCamposTercero.Aen_AcronimoCRM] = Aen_Acronimo.Equals(string.Empty) ? string.Empty : Aen_Acronimo;
            if (!Aen_An8.Equals(string.Empty)) ter[NombreCamposTercero.Aen_An8CRM] = Convert.ToInt32(Aen_An8);
            else ter[NombreCamposTercero.Aen_An8CRM] = default(int);
            ter[NombreCamposTercero.Aen_AlumnoCRM] = Aen_Alumno;
            ter[NombreCamposTercero.Aen_ProfesorCRM] = Aen_Profesor;
            ter[NombreCamposTercero.Aen_ResponsableCRM] = Aen_Responsable;
            ter[NombreCamposTercero.Aen_ApellidosCRM] = Aen_Apellidos.Equals(string.Empty) ? string.Empty : Aen_Apellidos;
            ter[NombreCamposTercero.Emailaddress1CRM] = Emailaddress1.Equals(string.Empty) ? string.Empty : Emailaddress1;
            if (!Parentaccountid.Equals(Guid.Empty)) //Emparentable (viene por update)
                ter[NombreCamposTercero.ParentaccountidCRM] = new EntityReference("account", Parentaccountid);
            else
            {
                if (!string.IsNullOrEmpty(ParentaccountidSTR)) //Emparentable (viene por create)
                    Crm.TercerosAEmparentar.Add(new KeyValuePair<string, string>(Aen_claveintegracion, ParentaccountidSTR));
                else //viene a vacío, se pone a null
                    ter[NombreCamposTercero.ParentaccountidCRM] = null;
            }
            ter[NombreCamposTercero.Aen_DelegacionidCRM] = Aen_Delegacionid.Equals(Guid.Empty) ? null : new EntityReference("aen_delegacion", Aen_Delegacionid);
            ter[NombreCamposTercero.Aen_DepartamentoidCRM] = Aen_Departamentoid.Equals(Guid.Empty) ? null : new EntityReference("aen_departamento", Aen_Departamentoid);
            ter[NombreCamposTercero.Aen_PaisdocumentoidCRM] = Aen_Paisdocumentoid.Equals(Guid.Empty) ? null : new EntityReference("aen_pais", Aen_Paisdocumentoid);
            ter[NombreCamposTercero.Aen_EsclienteCRM] = Aen_Escliente;
            ter[NombreCamposTercero.Aen_EsclientecertoolCRM] = Aen_Esclientecertool;
            ter[NombreCamposTercero.Aen_EsclientelaboratorioCRM] = Aen_Esclientelaboratorio;
            ter[NombreCamposTercero.Aen_EsclienteweberratumCRM] = Aen_Esclienteweberratum;
            ter[NombreCamposTercero.Aen_EscompradordenormasCRM] = Aen_Escompradordenormas;
            ter[NombreCamposTercero.Aen_EsempleadoCRM] = Aen_Esempleado;
            ter[NombreCamposTercero.Aen_EslibreriaCRM] = Aen_Eslibreria;
            ter[NombreCamposTercero.Aen_EsmiembroctcCRM] = Aen_Esmiembroctc;
            ter[NombreCamposTercero.Aen_EsmiembrouneCRM] = Aen_Esmiembroune;
            ter[NombreCamposTercero.Aen_EsorganismoCRM] = Aen_Esorganismo;
            ter[NombreCamposTercero.Aen_EspotencialclienteCRM] = Aen_Espotencialcliente;
            ter[NombreCamposTercero.Aen_EsproveedorCRM] = Aen_Esproveedor;
            ter[NombreCamposTercero.Aen_EssuscriptorCRM] = Aen_Essuscriptor;
            ter[NombreCamposTercero.Aen_RevistaaenorCRM] = Aen_Revistaaenor;
            ter[NombreCamposTercero.Aen_BloqueadoclienteCRM] = Aen_Bloqueadocliente.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt16(Aen_Bloqueadocliente));
            ter[NombreCamposTercero.Aen_BloqueadoproveedorCRM] = Aen_Bloqueadoproveedor.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt16(Aen_Bloqueadoproveedor));
            ter[NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM] = Aen_Estadosolicitudclienteerp.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudclienteerp));
            ter[NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM] = Aen_Estadosolicitudempleadoerp.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudempleadoerp));
            ter[NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM] = Aen_Estadosolicitudproveedorerp.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudproveedorerp));
            ter[NombreCamposTercero.FaxCRM] = Fax.Equals(string.Empty) ? string.Empty : Fax;
            if (!Aen_Fechadealta.Equals(string.Empty)) ter[NombreCamposTercero.Aen_FechadealtaCRM] = DateTime.ParseExact(Aen_Fechadealta, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture);
            else ter[NombreCamposTercero.Aen_FechadealtaCRM] = null;
            if (!Aen_Fechadebaja.Equals(string.Empty)) ter[NombreCamposTercero.Aen_FechadebajaCRM] = DateTime.ParseExact(Aen_Fechadebaja, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture);
            else ter[NombreCamposTercero.Aen_FechadebajaCRM] = null;
            ter[NombreCamposTercero.Aen_IdentificadorterceroCRM] = Aen_Identificadortercero.Equals(string.Empty) ? string.Empty : Aen_Identificadortercero;
            ter[NombreCamposTercero.Aen_ClienteerpidCRM] = Aen_Clienteerpid.Equals(string.Empty) ? string.Empty : Aen_Clienteerpid;
            ter[NombreCamposTercero.Aen_EmpleadoerpidCRM] = Aen_Empleadoerpid.Equals(string.Empty) ? string.Empty : Aen_Empleadoerpid;
            ter[NombreCamposTercero.Aen_ProveedorerpidCRM] = Aen_Proveedorerpid.Equals(string.Empty) ? string.Empty : Aen_Proveedorerpid;
            ter[NombreCamposTercero.Aen_IndustriaaenorCRM] = Aen_Industriaaenor.Equals(Guid.Empty) ? null : new EntityReference("aen_industriaaenor", Aen_Industriaaenor);
            if (!Revenue.Equals(decimal.MinValue))
            {
                Money reven = new Money(Convert.ToDecimal(Revenue));
                ter[NombreCamposTercero.RevenueCRM] = reven;
            }
            else
            {
                //Money reven = new Money();
                ter[NombreCamposTercero.RevenueCRM] = null;
            }
            ter[NombreCamposTercero.Aen_loginempleadoCRM] = Aen_loginempleado.Equals(string.Empty) ? string.Empty : Aen_loginempleado;
            ter[NombreCamposTercero.Aen_NombredelclienteCRM] = Aen_Nombredelcliente.Equals(string.Empty) ? string.Empty : Aen_Nombredelcliente;
            ter[NombreCamposTercero.NameCRM] = Name.Equals(string.Empty) ? string.Empty : Name;
            ter[NombreCamposTercero.Aen_NumerodocumentoCRM] = Aen_Numerodocumento.Equals(string.Empty) ? string.Empty : Aen_Numerodocumento;
            ter[NombreCamposTercero.Aen_ObservacionesCRM] = Aen_Observaciones.Equals(string.Empty) ? string.Empty : Aen_Observaciones;
            ter[NombreCamposTercero.Aen_OrigenCRM] = Aen_Origen.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Origen));
            ter[NombreCamposTercero.Aen_RiesgopagoaxesorCRM] = Aen_Riesgopagoaxesor.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Riesgopagoaxesor));
            ter[NombreCamposTercero.Aen_SectoraenorCRM] = Aen_Sectoraenor.Equals(Guid.Empty) ? null : new EntityReference("aen_sectoraenor", Aen_Sectoraenor);
            ter[NombreCamposTercero.Aen_GeneroCRM] = Aen_Genero.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Genero));
            ter[NombreCamposTercero.Aen_SiglasCRM] = Aen_Siglas.Equals(string.Empty) ? string.Empty : Aen_Siglas;
            ter[NombreCamposTercero.WebsiteurlCRM] = Websiteurl.Equals(string.Empty) ? string.Empty : Websiteurl;
            ter[NombreCamposTercero.Aen_SubtipodeterceroCRM] = Aen_Subtipodetercero.Equals(Guid.Empty) ? null : new EntityReference("aen_subtipotercero", Aen_Subtipodetercero);
            ter[NombreCamposTercero.Telephone1CRM] = Telephone1.Equals(string.Empty) ? string.Empty : Telephone1;
            ter[NombreCamposTercero.Aen_TipodocumentoCRM] = Aen_Tipodocumento.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Tipodocumento));
            ter[NombreCamposTercero.Aen_TipopersonaCRM] = Aen_Tipopersona.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Tipopersona));
            //ter[NombreCamposTercero.Aen_TipoproveedorCRM] = Aen_Tipoproveedor.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Tipoproveedor));
            ter[NombreCamposTercero.Aen_ObservacionesmigracionCRM] = Aen_Observacionesmigracion.Equals(string.Empty) ? string.Empty : Aen_Observacionesmigracion;

            if (!Numberofemployees.Equals(string.Empty)) ter[NombreCamposTercero.NumberofemployeesCRM] = Convert.ToInt32(Numberofemployees);
            else ter[NombreCamposTercero.NumberofemployeesCRM] = default(int);

            if (Accountid != Guid.Empty)
            {
                ter["accountid"] = Accountid;
                ter.Id = Accountid;
            }

            if (Statecode.Equals("Activo"))
            {
                ter[NombreCamposTercero.StatecodeCRM] = new OptionSetValue(0);
                ter[NombreCamposTercero.StatuscodeCRM] = new OptionSetValue(1);
            }
            else if(Statecode.Equals("Inactivo") && Accountid != Guid.Empty && Accountid != null)
            {
                ter[NombreCamposTercero.StatecodeCRM] = new OptionSetValue(1);
                ter[NombreCamposTercero.StatuscodeCRM] = new OptionSetValue(2);
            }

            if (!Transactioncurrencyid.Equals(Guid.Empty))
            {
                EntityReference curr = new EntityReference();
                curr.Id = Transactioncurrencyid;
                curr.LogicalName = "transactioncurrency";
                ter.Attributes.Add(NombreCamposTercero.TransactioncurrencyidCRM, curr);
            }
            else
            {
                Guid defaultCurrency = Guid.Empty;
                bool ooook = Crm.MaestroTransactionCurrency.TryGetValue("EUR", out defaultCurrency);
                ter.Attributes.Add(NombreCamposTercero.TransactioncurrencyidCRM, new EntityReference("transactioncurrency", defaultCurrency));
            }
            ter[NombreCamposTercero.Aen_WebcorporativaCRM] = Aen_Webcorporativa.Equals(string.Empty) ? string.Empty : Aen_Webcorporativa;
            ter[NombreCamposTercero.Aen_TelefonocorporativoCRM] = Aen_Telefonocorporativo.Equals(string.Empty) ? string.Empty : Aen_Telefonocorporativo;
            ter[NombreCamposTercero.Aen_CorreoelectronicocorporativoCRM] = Aen_Correoelectronicocorporativo.Equals(string.Empty) ? string.Empty : Aen_Correoelectronicocorporativo;
            ter[NombreCamposTercero.Aen_CondicionesdepagoidCRM] = Aen_Condicionesdepagoid.Equals(Guid.Empty) ? null : new EntityReference("aen_condicionesdepago", Aen_Condicionesdepagoid);
            ter[NombreCamposTercero.Aen_FormasdepagoidCRM] = Aen_Formasdepagoid.Equals(Guid.Empty) ? null : new EntityReference("aen_formadepago", Aen_Formasdepagoid);
            ter[NombreCamposTercero.Aen_EntradadelclienteCRM] = Aen_Entradadelcliente.Equals(string.Empty) ? null: new OptionSetValue(Convert.ToInt32(Aen_Entradadelcliente));
            ter[NombreCamposTercero.Aen_EvaluaciondelaconformidadCRM] = Aen_Evaluaciondelaconformidad;
            ter[NombreCamposTercero.Aen_EscompradordelibrosCRM] = Aen_Escompradordelibros;
            ter[NombreCamposTercero.Aen_TipodocumentoempleadoCRM] = Aen_Tipodocumentoempleado.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Tipodocumentoempleado));
            ter[NombreCamposTercero.Aen_NumerodocumentoempleadoCRM] = Aen_Numerodocumentoempleado.Equals(string.Empty) ? string.Empty : Aen_Numerodocumentoempleado;

            //Para saltar plugins de envío de datos a NEXO
            //ter["aen_vienedeintegracion"] = true;

            return ter;
        }




        #region Mas limpieza datos
        //public T asignaValorLimpio<T>(T origen)
        //{
        //    T res = default(T);

        //    if (origen.GetType() == typeof(string))
        //    {
        //        res = origen.ToString().Trim();
        //    }

        //    return res;
        //}

        //public dynamic asignaValorLimpio(dynamic valor)
        //{
        //    dynamic res = null;

        //    // ELSEs si vienen de Oracle

        //    if (valor.GetType() == typeof(string))
        //    {
        //            res = valor.ToString().Trim();
        //    }
        //    else if (valor.GetType() == typeof(int) || valor.GetType() == typeof(bool))
        //    {
        //            res = valor;
        //    }
        //    else if (valor.GetType() == typeof(decimal))
        //    {
        //        // tratamiento decimal
        //    }
        //    else if (valor.GetType() == typeof(DateTime))
        //    {
        //        // tratamiento DateTime
        //    }
        //    else if (valor.GetType() == typeof(OptionSetValue))
        //    {
        //        // tratamiento OptionSetValue
        //    }
        //    else if (valor.GetType() == typeof(EntityReference))
        //    {
        //        // tratamiento EntityReference
        //    }
        //    // Completar comportamiento según tipo de dato
        //    return res;
        //}
        #endregion Métodos limpieza datos

        #endregion Métodos

    }
}
