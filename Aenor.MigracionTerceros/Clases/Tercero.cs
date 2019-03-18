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

namespace Aenor.MigracionTerceros
{

    //[DataContract]
    public class Tercero
    {
        #region Propiedades
        public Guid Accountid { get; set; }
        public string Aen_Acronimo { get; set; }
        public bool Aen_Alumno { get; set; }
        public bool Aen_Profesor { get; set; }
        public bool Aen_Responsable { get; set; }
        public string Aen_Apellidos { get; set; }
        public Guid Aen_Contactofacturacionid { get; set; }
        public Guid Primarycontactid { get; set; }
        public string Emailaddress1 { get; set; }
        public Guid Parentaccountid { get; set; }
        public string ParentaccountidSTR { get; set; }
        public Guid Aen_Delegacionid { get; set; }
        public Guid Aen_Departamentoid { get; set; }
        public Guid Aen_Paisdocumentoid { get; set; }
        public Guid Transactioncurrencyid { get; set; }
        public bool Aen_Escertificaciondeproductos{ get; set; }
        public bool Aen_Escertificaciondesistemas { get; set; }
        public bool Aen_Escertificadopequeocomercio { get; set; }
        public bool Aen_Escliente { get; set; }
        public bool Aen_Esclientecertool { get; set; }
        public bool Aen_Esclientelaboratorio { get; set; }
        public bool Aen_Esclienteweb { get; set; }
        public bool Aen_Escompradordenormas { get; set; }
        public bool Aen_Esempleado { get; set; }
        public bool Aen_Eslibreria { get; set; }
        public bool Aen_Esmiembroctc { get; set; }
        public bool Aen_Esmiembroune { get; set; }
        public bool Aen_Esorganismo { get; set; }
        public bool Aen_Espotencialclienteweb { get; set; }
        public bool Aen_Esclienteweberratum { get; set; }
        public bool Aen_Esproveedor { get; set; }
        public bool Aen_Essuscriptor { get; set; }
        public bool Aen_Revistaaenor { get; set; }
        public string Statecode { get; set; }
        public string Aen_Estadoclienteerp { get; set; }
        public string Aen_Estadoempleadoerp { get; set; }
        public string Aen_Estadoproveedorerp { get; set; }
        public string Aen_Estadosolicitudclienteerp { get; set; }
        public string Aen_Estadosolicitudempleadoerp { get; set; }
        public string Aen_Estadosolicitudproveedorerp { get; set; }  
        public decimal? Aen_Facturacionaenor { get; set; }
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
        public decimal Revenue_Base { get; set; }
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
        public string Aen_Tipoproveedor { get; set; }
        public string Aen_Observacionesmigracion { get; set; }
        public string Numberofemployees { get; set; }
        #endregion Propiedades

        #region Métodos
        public Tercero()
        {
        }


        public void TerceroFromCRM(Entity ter)
        {
            this.Accountid = ter.Id;

            this.Aen_Acronimo = ter.Contains(NombreCamposTercero.Aen_AcronimoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_AcronimoCRM).Trim() : string.Empty;
            this.Aen_Alumno = ter.Contains(NombreCamposTercero.Aen_AlumnoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_AlumnoCRM) : false;
            this.Aen_Profesor = ter.Contains(NombreCamposTercero.Aen_ProfesorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_ProfesorCRM) : false;
            this.Aen_Responsable = ter.Contains(NombreCamposTercero.Aen_ResponsableCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_ResponsableCRM) : false;
            this.Aen_Apellidos = ter.Contains(NombreCamposTercero.Aen_ApellidosCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ApellidosCRM).Trim() : string.Empty;
            this.Aen_Contactofacturacionid = ter.Contains(NombreCamposTercero.Aen_ContactofacturacionidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_ContactofacturacionidCRM).Id : Guid.Empty;
            this.Primarycontactid = ter.Contains(NombreCamposTercero.PrimarycontactidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.PrimarycontactidCRM).Id : Guid.Empty;
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
            this.Aen_Escertificaciondeproductos = ter.Contains(NombreCamposTercero.Aen_EscertificaciondeproductosCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscertificaciondeproductosCRM) : false;
            this.Aen_Escertificaciondesistemas = ter.Contains(NombreCamposTercero.Aen_EscertificaciondesistemasCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscertificaciondesistemasCRM) : false;
            this.Aen_Escertificadopequeocomercio = ter.Contains(NombreCamposTercero.Aen_EscertificadopequeocomercioCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscertificadopequeocomercioCRM) : false;
            this.Aen_Escliente = ter.Contains(NombreCamposTercero.Aen_EsclienteCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclienteCRM) : false;
            this.Aen_Esclientecertool = ter.Contains(NombreCamposTercero.Aen_EsclientecertoolCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclientecertoolCRM) : false;
            this.Aen_Esclientelaboratorio = ter.Contains(NombreCamposTercero.Aen_EsclientelaboratorioCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclientelaboratorioCRM) : false;
            this.Aen_Esclienteweb = ter.Contains(NombreCamposTercero.Aen_EsclientewebCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclientewebCRM) : false;
            this.Aen_Escompradordenormas = ter.Contains(NombreCamposTercero.Aen_EscompradordenormasCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EscompradordenormasCRM) : false;
            this.Aen_Esempleado = ter.Contains(NombreCamposTercero.Aen_EsempleadoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsempleadoCRM) : false;
            this.Aen_Eslibreria = ter.Contains(NombreCamposTercero.Aen_EslibreriaCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EslibreriaCRM) : false;
            this.Aen_Esmiembroctc = ter.Contains(NombreCamposTercero.Aen_EsmiembroctcCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsmiembroctcCRM) : false;
            this.Aen_Esmiembroune = ter.Contains(NombreCamposTercero.Aen_EsmiembrouneCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsmiembrouneCRM) : false;
            this.Aen_Esorganismo = ter.Contains(NombreCamposTercero.Aen_EsorganismoCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsorganismoCRM) : false;
            this.Aen_Espotencialclienteweb = ter.Contains(NombreCamposTercero.Aen_EspotencialclientewebCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EspotencialclientewebCRM) : false;
            this.Aen_Esclienteweberratum = ter.Contains(NombreCamposTercero.Aen_EsclienteweberratumCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsclienteweberratumCRM) : false;
            this.Aen_Esproveedor = ter.Contains(NombreCamposTercero.Aen_EsproveedorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EsproveedorCRM) : false;
            this.Aen_Essuscriptor = ter.Contains(NombreCamposTercero.Aen_EssuscriptorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_EssuscriptorCRM) : false;
            this.Aen_Revistaaenor = ter.Contains(NombreCamposTercero.Aen_RevistaaenorCRM) ? ter.GetAttributeValue<bool>(NombreCamposTercero.Aen_RevistaaenorCRM) : false;
            this.Statecode = ter.Contains(NombreCamposTercero.StatecodeORACLE) ? ((ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.StatecodeORACLE)).Value == 0 ? "Activo" : "Inactivo") : string.Empty;
            this.Aen_Estadoclienteerp = ter.Contains(NombreCamposTercero.Aen_EstadoclienteerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadoclienteerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadoempleadoerp = ter.Contains(NombreCamposTercero.Aen_EstadoempleadoerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadoempleadoerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadoproveedorerp = ter.Contains(NombreCamposTercero.Aen_EstadoproveedorerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadoproveedorerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudclienteerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudempleadoerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM).Value.ToString() : string.Empty;
            this.Aen_Estadosolicitudproveedorerp = ter.Contains(NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM).Value.ToString() : string.Empty;
            this.Aen_Facturacionaenor = ter.Contains(NombreCamposTercero.Aen_FacturacionaenorCRM) ? ter.GetAttributeValue<decimal>(NombreCamposTercero.Aen_FacturacionaenorCRM) : decimal.MinValue;
            this.Fax = ter.Contains(NombreCamposTercero.FaxCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.FaxCRM).Trim() : string.Empty;
            this.Aen_Fechadealta = ter.Contains(NombreCamposTercero.Aen_FechadealtaCRM) ? ter.GetAttributeValue<DateTime>(NombreCamposTercero.Aen_FechadealtaCRM).ToString("yyyy-MM-dd".Trim()) : string.Empty;
            this.Aen_Fechadebaja = ter.Contains(NombreCamposTercero.Aen_FechadebajaCRM) ? ter.GetAttributeValue<DateTime>(NombreCamposTercero.Aen_FechadebajaCRM).ToString("yyyy-MM-dd").Trim() : string.Empty;
            this.Aen_Identificadortercero = ter.Contains(NombreCamposTercero.Aen_IdentificadorterceroCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_IdentificadorterceroCRM).Trim() : string.Empty;
            this.Aen_claveintegracion = ter.Contains(NombreCamposTercero.Aen_claveintegracionCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_claveintegracionCRM).Trim() : string.Empty;
            this.Aen_Clienteerpid = ter.Contains(NombreCamposTercero.Aen_ClienteerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ClienteerpidCRM).Trim() : string.Empty;
            this.Aen_Empleadoerpid = ter.Contains(NombreCamposTercero.Aen_EmpleadoerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_EmpleadoerpidCRM).Trim() : string.Empty;
            this.Aen_Proveedorerpid = ter.Contains(NombreCamposTercero.Aen_ProveedorerpidCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ProveedorerpidCRM).Trim() : string.Empty;
            this.Aen_Industriaaenor = ter.Contains(NombreCamposTercero.Aen_IndustriaaenorCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.Aen_IndustriaaenorCRM).Id : Guid.Empty;
            this.Revenue = ter.Contains(NombreCamposTercero.RevenueCRM) ? ter.GetAttributeValue<Money>(NombreCamposTercero.RevenueCRM).Value : decimal.MinValue;
            this.Revenue_Base = ter.Contains(NombreCamposTercero.Revenue_BaseCRM) ? ter.GetAttributeValue<Money>(NombreCamposTercero.Revenue_BaseCRM).Value : decimal.MinValue;
            this.Aen_loginempleado = ter.Contains(NombreCamposTercero.Aen_loginempleadoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_loginempleadoCRM).Trim() : string.Empty;
            this.Aen_Nombredelcliente = ter.Contains(NombreCamposTercero.Aen_NombredelclienteCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_NombredelclienteCRM).Trim() : string.Empty;
            this.Name = ter.Contains(NombreCamposTercero.NameCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.NameCRM).Trim() : string.Empty;
            this.Aen_Numerodocumento = ter.Contains(NombreCamposTercero.Aen_NumerodocumentoCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_NumerodocumentoCRM).Trim() : string.Empty;
            this.Aen_Observaciones = ter.Contains(NombreCamposTercero.Aen_ObservacionesCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ObservacionesCRM).Trim() : string.Empty;
            this.Aen_Origen = ter.Contains(NombreCamposTercero.Aen_OrigenCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_OrigenCRM).Value.ToString() : string.Empty;
            this.Transactioncurrencyid = ter.Contains(NombreCamposTercero.TransactioncurrencyidCRM) ? ter.GetAttributeValue<EntityReference>(NombreCamposTercero.TransactioncurrencyidCRM).Id : Guid.Empty;
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
            this.Aen_Tipoproveedor = ter.Contains(NombreCamposTercero.Aen_TipoproveedorCRM) ? ter.GetAttributeValue<OptionSetValue>(NombreCamposTercero.Aen_TipoproveedorCRM).Value.ToString() : string.Empty;
            this.Aen_Observacionesmigracion = ter.Contains(NombreCamposTercero.Aen_ObservacionesmigracionCRM) ? ter.GetAttributeValue<string>(NombreCamposTercero.Aen_ObservacionesmigracionCRM).Trim() : string.Empty;
            this.Numberofemployees = ter.Contains(NombreCamposTercero.NumberofemployeesCRM) ? ter.GetAttributeValue<int>(NombreCamposTercero.NumberofemployeesCRM).ToString() : string.Empty;
        }


        public Entity GetEntityFromTercero()
        {
            Entity ter = new Entity("account");
            
                if (!Aen_claveintegracion.Equals(string.Empty)) ter[NombreCamposTercero.Aen_claveintegracionCRM] = Aen_claveintegracion;
                if (!Aen_Acronimo.Equals(string.Empty)) ter[NombreCamposTercero.Aen_AcronimoCRM] = Aen_Acronimo;
                ter[NombreCamposTercero.Aen_AlumnoCRM] = Aen_Alumno;
                ter[NombreCamposTercero.Aen_ProfesorCRM] = Aen_Profesor;
                ter[NombreCamposTercero.Aen_ResponsableCRM] = Aen_Responsable;
                if (!Aen_Apellidos.Equals(string.Empty)) ter[NombreCamposTercero.Aen_ApellidosCRM] = Aen_Apellidos;
                //if (!Aen_Contactofacturacionid.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_ContactofacturacionidCRM] = new EntityReference("contact", Aen_Contactofacturacionid);
                //if (!Primarycontactid.Equals(Guid.Empty)) ter[NombreCamposTercero.PrimarycontactidCRM] = new EntityReference("contact", Primarycontactid);
                if (!Emailaddress1.Equals(string.Empty)) ter[NombreCamposTercero.Emailaddress1CRM] = Emailaddress1;
                if (!Parentaccountid.Equals(Guid.Empty))
                    ter[NombreCamposTercero.ParentaccountidCRM] = new EntityReference("account", Parentaccountid);
                else
                { 
                    if(!string.IsNullOrEmpty(ParentaccountidSTR))
                        Crm.TercerosAEmparentar.Add(new KeyValuePair<string, string>(Aen_claveintegracion,ParentaccountidSTR));
                }
                if (!Aen_Delegacionid.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_DelegacionidCRM] = new EntityReference("aen_Delegacion", Aen_Delegacionid);
                if (!Aen_Departamentoid.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_DepartamentoidCRM] = new EntityReference("aen_Departamento", Aen_Departamentoid);
                if (!Aen_Paisdocumentoid.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_PaisdocumentoidCRM] = new EntityReference("aen_pais", Aen_Paisdocumentoid);
                ter[NombreCamposTercero.TransactioncurrencyidCRM] = new EntityReference("transactioncurrency", Transactioncurrencyid);
                ter[NombreCamposTercero.Aen_EscertificaciondeproductosCRM] = Aen_Escertificaciondeproductos;
                ter[NombreCamposTercero.Aen_EscertificaciondesistemasCRM] = Aen_Escertificaciondesistemas;
                ter[NombreCamposTercero.Aen_EscertificadopequeocomercioCRM] = Aen_Escertificadopequeocomercio;
                ter[NombreCamposTercero.Aen_EsclienteCRM] = Aen_Escliente;
                ter[NombreCamposTercero.Aen_EsclientecertoolCRM] = Aen_Esclientecertool;
                ter[NombreCamposTercero.Aen_EsclientelaboratorioCRM] = Aen_Esclientelaboratorio;
                ter[NombreCamposTercero.Aen_EsclientewebCRM] = Aen_Esclienteweb;
                ter[NombreCamposTercero.Aen_EscompradordenormasCRM] = Aen_Escompradordenormas;
                ter[NombreCamposTercero.Aen_EsempleadoCRM] = Aen_Esempleado;
                ter[NombreCamposTercero.Aen_EslibreriaCRM] = Aen_Eslibreria;
                ter[NombreCamposTercero.Aen_EsmiembroctcCRM] = Aen_Esmiembroctc;
                ter[NombreCamposTercero.Aen_EsmiembrouneCRM] = Aen_Esmiembroune;
                ter[NombreCamposTercero.Aen_EsorganismoCRM] = Aen_Esorganismo;
                ter[NombreCamposTercero.Aen_EspotencialclientewebCRM] = Aen_Espotencialclienteweb;
                ter[NombreCamposTercero.Aen_EsclienteweberratumCRM] = Aen_Esclienteweberratum;
                ter[NombreCamposTercero.Aen_EsproveedorCRM] = Aen_Esproveedor;
                ter[NombreCamposTercero.Aen_EssuscriptorCRM] = Aen_Essuscriptor;
                ter[NombreCamposTercero.Aen_RevistaaenorCRM] = Aen_Revistaaenor;
                //ter["statecodeCRM] = Statecode.Equals("Activo") ? false : true;
                //TODO Revisar tipo de los campos aen_EstadoXXX (Que viene de oracle, para saber a que/ como convertir)
                //if (!Aen_Estadoclienteerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadoclienteerpCRM] = new OptionSetValue(Aen_Estadoclienteerp.Equals("Activo")? 1: 0);
                //if (!Aen_Estadoempleadoerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadoempleadoerpCRM] = new OptionSetValue(Aen_Estadoclienteerp.Equals("Activo") ? 1 : 0);
                //if (!Aen_Estadoproveedorerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadoproveedorerpCRM] = new OptionSetValue(Aen_Estadoclienteerp.Equals("Activo") ? 1 : 0);
                if (!Aen_Estadosolicitudclienteerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadosolicitudclienteerpCRM] = new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudclienteerp));
                if (!Aen_Estadosolicitudempleadoerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadosolicitudempleadoerpCRM] = new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudempleadoerp));
                if (!Aen_Estadosolicitudproveedorerp.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EstadosolicitudproveedorerpCRM] = new OptionSetValue(Convert.ToInt32(Aen_Estadosolicitudproveedorerp));
                if (!Aen_Facturacionaenor.Value.Equals(decimal.MinValue))
                {
                    Money factu = new Money(Aen_Facturacionaenor.Value);
                    ter[NombreCamposTercero.Aen_FacturacionaenorCRM] = factu;
                    ter[NombreCamposTercero.Aen_Facturacionaenor_BaseCRM] = factu;
                }
                if (!Fax.Equals(string.Empty)) ter[NombreCamposTercero.FaxCRM] = Fax;
                if (!Aen_Fechadealta.Equals(string.Empty)) ter[NombreCamposTercero.Aen_FechadealtaCRM] = DateTime.ParseExact(Aen_Fechadealta, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); 
                if (!Aen_Fechadebaja.Equals(string.Empty)) ter[NombreCamposTercero.Aen_FechadebajaCRM] = DateTime.ParseExact(Aen_Fechadebaja, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); 
                if (!Aen_Identificadortercero.Equals(string.Empty)) ter[NombreCamposTercero.Aen_IdentificadorterceroCRM] = Aen_Identificadortercero;
                if (!Aen_Clienteerpid.Equals(string.Empty)) ter[NombreCamposTercero.Aen_ClienteerpidCRM] = Aen_Clienteerpid;
                if (!Aen_Empleadoerpid.Equals(string.Empty)) ter[NombreCamposTercero.Aen_EmpleadoerpidCRM] = Aen_Empleadoerpid;
                if (!Aen_Proveedorerpid.Equals(string.Empty)) ter[NombreCamposTercero.Aen_ProveedorerpidCRM] = Aen_Proveedorerpid;
                if (!Aen_Industriaaenor.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_IndustriaaenorCRM] = new EntityReference("aen_industriaaenor", Aen_Industriaaenor);
                if (!Revenue.Equals(decimal.MinValue))
                {
                    Money reven = new Money(Revenue);
                    ter[NombreCamposTercero.RevenueCRM] = reven;
                    ter[NombreCamposTercero.Revenue_BaseCRM] = reven;
                }
                if (!Aen_loginempleado.Equals(string.Empty)) ter[NombreCamposTercero.Aen_loginempleadoCRM] = Aen_loginempleado;
                if (!Aen_Nombredelcliente.Equals(string.Empty)) ter[NombreCamposTercero.Aen_NombredelclienteCRM] = Aen_Nombredelcliente;
                if (!Name.Equals(string.Empty)) ter[NombreCamposTercero.NameCRM] = Name;
                if (!Aen_Numerodocumento.Equals(string.Empty)) ter[NombreCamposTercero.Aen_NumerodocumentoCRM] = Aen_Numerodocumento;
                if (!Aen_Observaciones.Equals(string.Empty)) ter[NombreCamposTercero.Aen_ObservacionesCRM] = Aen_Observaciones;
                if (!Aen_Origen.Equals(string.Empty)) ter[NombreCamposTercero.Aen_OrigenCRM] = new OptionSetValue(Convert.ToInt32(Aen_Origen));
                if (!Aen_Riesgopagoaxesor.Equals(string.Empty)) ter[NombreCamposTercero.Aen_RiesgopagoaxesorCRM] = new OptionSetValue(Convert.ToInt32(Aen_Riesgopagoaxesor));
                if (!Aen_Sectoraenor.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_SectoraenorCRM] = new EntityReference("aen_sectoraenor", Aen_Sectoraenor);
                if (!Aen_Genero.Equals(string.Empty)) ter[NombreCamposTercero.Aen_GeneroCRM] = new OptionSetValue(Convert.ToInt32(Aen_Genero));
                if (!Aen_Siglas.Equals(string.Empty)) ter[NombreCamposTercero.Aen_SiglasCRM] = Aen_Siglas;
                if (!Websiteurl.Equals(string.Empty)) ter[NombreCamposTercero.WebsiteurlCRM] = Websiteurl;
                if (!Aen_Subtipodetercero.Equals(Guid.Empty)) ter[NombreCamposTercero.Aen_SubtipodeterceroCRM] = new EntityReference("aen_subtipotercero", Aen_Subtipodetercero);
                if (!Telephone1.Equals(string.Empty)) ter[NombreCamposTercero.Telephone1CRM] = Telephone1;
                if (!Aen_Tipodocumento.Equals(string.Empty)) ter[NombreCamposTercero.Aen_TipodocumentoCRM] = new OptionSetValue(Convert.ToInt32(Aen_Tipodocumento));
                if (!Aen_Tipopersona.Equals(string.Empty)) ter[NombreCamposTercero.Aen_TipopersonaCRM] = new OptionSetValue(Convert.ToInt32(Aen_Tipopersona));
                if (!Aen_Tipoproveedor.Equals(string.Empty)) ter[NombreCamposTercero.Aen_TipoproveedorCRM] = new OptionSetValue(Convert.ToInt32(Aen_Tipoproveedor));
                if (!Aen_Observacionesmigracion.Equals(string.Empty)) ter[NombreCamposTercero.Aen_ObservacionesmigracionCRM] = Aen_Observacionesmigracion;
                if (!Numberofemployees.Equals(string.Empty)) ter[NombreCamposTercero.NumberofemployeesCRM] = Convert.ToInt32(Numberofemployees);

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
