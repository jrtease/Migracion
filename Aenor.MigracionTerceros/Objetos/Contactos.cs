using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aenor.MigracionTerceros.Clases;
using Microsoft.Xrm.Sdk;

namespace Aenor.MigracionTerceros.Objetos
{


    public class Contactos
    {
        #region Propiedades
        public Guid Contactid { get; set; }

        public string Aen_ClaveIntegracion { get; set; }
        public Guid Aen_ClaveIntegracionParent { get; set; } //Parentcustomerid
        public Guid Aen_CargoprincipalId { get; set; }
        public string Aen_CargoprincipalId_str { get; set; }
        public string Aen_Numerodocumento { get; set; }
        public string Aen_Tipodocumento { get; set; }
        public string Aen_Tratamiento { get; set; }
        public string Aen_Observaciones { get; set; }
        public string Aen_Observacionesmigracion { get; set; }
        public string Donotsendmm { get; set; }
        public string Emailaddress1 { get; set; }
        public string Firstname { get; set; }
        public string Gendercode { get; set; }
        public string Lastname { get; set; }
        public string Mobilephone { get; set; }
        public string Statecode { get; set; }
        public string Telephone1 { get; set; }
        public string Aen_Origen { get; set; }
        public string Aen_Identificadorcontacto { get; set; }
        #endregion Propiedades



        public void ContactoFromCRM(Entity contCRM)
        {
            this.Contactid = contCRM.Id;

            this.Aen_ClaveIntegracion = contCRM.Contains(NombreCamposContacto.Aen_ClaveIntegracionCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_ClaveIntegracionCRM).Trim() : string.Empty;
            this.Aen_ClaveIntegracionParent = contCRM.Contains(NombreCamposContacto.Aen_ClaveIntegracionParentCRM) ? contCRM.GetAttributeValue<EntityReference>(NombreCamposContacto.Aen_ClaveIntegracionParentCRM).Id : Guid.Empty;
            this.Aen_CargoprincipalId = contCRM.Contains(NombreCamposContacto.Aen_CargoprincipalIdCRM) ? contCRM.GetAttributeValue<EntityReference>(NombreCamposContacto.Aen_CargoprincipalIdCRM).Id : Guid.Empty;
            this.Aen_Numerodocumento = contCRM.Contains(NombreCamposContacto.Aen_NumerodocumentoCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_NumerodocumentoCRM).Trim() : string.Empty;
            this.Aen_Tipodocumento = contCRM.Contains(NombreCamposContacto.Aen_TipodocumentoCRM) ? contCRM.GetAttributeValue<OptionSetValue>(NombreCamposContacto.Aen_TipodocumentoCRM).Value.ToString().Trim() : string.Empty;
            this.Aen_Tratamiento = contCRM.Contains(NombreCamposContacto.Aen_TratamientoCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_TratamientoCRM).Trim() : string.Empty;
            this.Aen_Observaciones = contCRM.Contains(NombreCamposContacto.Aen_ObservacionesCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_ObservacionesCRM).Trim() : string.Empty;
            this.Aen_Observacionesmigracion = contCRM.Contains(NombreCamposContacto.Aen_ObservacionesmigracionCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_ObservacionesmigracionCRM).Trim() : string.Empty;
            this.Donotsendmm = contCRM.Contains(NombreCamposContacto.DonotsendmmCRM) ? (contCRM.GetAttributeValue<Boolean>(NombreCamposContacto.DonotsendmmCRM) == true)?"1": "0" : string.Empty;
            this.Emailaddress1 = contCRM.Contains(NombreCamposContacto.Emailaddress1CRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Emailaddress1CRM).Trim() : string.Empty;
            this.Firstname = contCRM.Contains(NombreCamposContacto.FirstnameCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.FirstnameCRM).Trim() : string.Empty;
            this.Gendercode = contCRM.Contains(NombreCamposContacto.GendercodeCRM) ? contCRM.GetAttributeValue<OptionSetValue>(NombreCamposContacto.GendercodeCRM).Value.ToString().Trim() : string.Empty;
            this.Lastname = contCRM.Contains(NombreCamposContacto.LastnameCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.LastnameCRM).Trim() : string.Empty;
            this.Mobilephone = contCRM.Contains(NombreCamposContacto.MobilephoneCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.MobilephoneCRM).Trim() : string.Empty;
            this.Statecode = contCRM.Contains(NombreCamposContacto.StatecodeCRM) ? (contCRM.GetAttributeValue<OptionSetValue>(NombreCamposContacto.StatecodeCRM).Value == 0 ? "Activo" : "Inactivo") : string.Empty;
            this.Telephone1 = contCRM.Contains(NombreCamposContacto.Telephone1CRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Telephone1CRM).Trim() : string.Empty;
            this.Aen_Origen = contCRM.Contains(NombreCamposContacto.Aen_OrigenCRM) ? contCRM.GetAttributeValue<OptionSetValue>(NombreCamposContacto.Aen_OrigenCRM).Value.ToString().Trim() : string.Empty;
            this.Aen_Identificadorcontacto = contCRM.Contains(NombreCamposContacto.Aen_IdentificadorcontactoCRM) ? contCRM.GetAttributeValue<string>(NombreCamposContacto.Aen_IdentificadorcontactoCRM).ToString().Trim() : string.Empty;
        }



        public void ContactoFromOracle(DataRow fila, Dictionary<string,Guid> Mterceros)
        {
            this.Aen_ClaveIntegracion = fila[NombreCamposContacto.Aen_ClaveIntegracionORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_ClaveIntegracionORACLE].ToString().Trim();
            this.Aen_CargoprincipalId_str = fila[NombreCamposContacto.Aen_CargoprincipalIdORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_CargoprincipalIdORACLE].ToString().Trim();

            if (fila[NombreCamposContacto.Aen_ClaveIntegracionParentORACLE] == DBNull.Value)
                this.Aen_ClaveIntegracionParent = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                bool ret = Mterceros.TryGetValue(fila[NombreCamposContacto.Aen_ClaveIntegracionParentORACLE].ToString().Trim(), out aux);
                if (ret)
                    this.Aen_ClaveIntegracionParent = aux;
            }

            this.Aen_Numerodocumento = fila[NombreCamposContacto.Aen_NumerodocumentoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_NumerodocumentoORACLE].ToString().Trim();
            this.Aen_Observaciones = fila[NombreCamposContacto.Aen_ObservacionesORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_ObservacionesORACLE].ToString().Trim();
            this.Aen_Observacionesmigracion = fila[NombreCamposContacto.Aen_ObservacionesmigracionORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_ObservacionesmigracionORACLE].ToString().Trim();
            this.Aen_Origen = fila[NombreCamposContacto.Aen_OrigenORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_OrigenORACLE].ToString().Replace(".", "").Trim();
            this.Aen_Tipodocumento = fila[NombreCamposContacto.Aen_TipodocumentoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_TipodocumentoORACLE].ToString().Replace(".", "").Trim();
            this.Aen_Tratamiento = fila[NombreCamposContacto.Aen_TratamientoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_TratamientoORACLE].ToString().Trim();
            this.Donotsendmm = fila[NombreCamposContacto.DonotsendmmORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposContacto.DonotsendmmORACLE]).Trim();
            this.Emailaddress1 = fila[NombreCamposContacto.Emailaddress1ORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Emailaddress1ORACLE].ToString().Trim();
            this.Firstname = fila[NombreCamposContacto.FirstnameORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.FirstnameORACLE].ToString().Trim();
            this.Gendercode = fila[NombreCamposContacto.GendercodeORACLE] == DBNull.Value ? string.Empty : Convert.ToString(fila[NombreCamposContacto.GendercodeORACLE]).Trim();
            this.Lastname = fila[NombreCamposContacto.LastnameORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.LastnameORACLE].ToString().Trim();
            this.Mobilephone = fila[NombreCamposContacto.MobilephoneORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.MobilephoneORACLE].ToString().Trim();
            this.Statecode = fila[NombreCamposContacto.StatecodeORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.StatecodeORACLE].ToString().Trim();
            this.Telephone1 = fila[NombreCamposContacto.Telephone1ORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Telephone1ORACLE].ToString().Trim();
            this.Aen_Identificadorcontacto = fila[NombreCamposContacto.Aen_IdentificadorcontactoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposContacto.Aen_IdentificadorcontactoORACLE].ToString().Trim();
        }



        public Entity GetEntityFromContacto()
        {
            Entity ent = new Entity("contact");

            ent[NombreCamposContacto.Aen_ClaveIntegracionCRM] = Aen_ClaveIntegracion;
            ent[NombreCamposContacto.Aen_CargoprincipalIdCRM] = Aen_CargoprincipalId.Equals(Guid.Empty) ? null : new EntityReference("aen_tiposdecargo", Aen_CargoprincipalId);
            ent[NombreCamposContacto.Aen_NumerodocumentoCRM] = Aen_Numerodocumento.Equals(string.Empty) ? string.Empty : Aen_Numerodocumento;
            ent[NombreCamposContacto.Aen_TipodocumentoCRM] = Aen_Tipodocumento.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Tipodocumento));
            ent[NombreCamposContacto.Aen_TratamientoCRM] = Aen_Tratamiento.Equals(string.Empty) ? null : Aen_Tratamiento;
            ent[NombreCamposContacto.Aen_ObservacionesCRM] = Aen_Observaciones.Equals(string.Empty) ? string.Empty : Aen_Observaciones;
            ent[NombreCamposContacto.Aen_ObservacionesmigracionCRM] = Aen_Observacionesmigracion.Equals(string.Empty) ? string.Empty : Aen_Observacionesmigracion;
            if (!Donotsendmm.Equals(string.Empty))
            {
                if (Donotsendmm == "1")
                    ent[NombreCamposContacto.DonotsendmmCRM] = true;
                else if (Donotsendmm == "0")
                    ent[NombreCamposContacto.DonotsendmmCRM] = false;
            }
            else
                ent[NombreCamposContacto.DonotsendmmCRM] = false;
            ent[NombreCamposContacto.Emailaddress1CRM] = Emailaddress1.Equals(string.Empty) ? string.Empty: Emailaddress1;
            ent[NombreCamposContacto.FirstnameCRM] = Firstname.Equals(string.Empty) ? string.Empty : Firstname;
            ent[NombreCamposContacto.LastnameCRM] = Lastname.Equals(string.Empty) ? string.Empty : Lastname;
            ent[NombreCamposContacto.GendercodeCRM] = Gendercode.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Gendercode));
            ent[NombreCamposContacto.MobilephoneCRM] = Mobilephone.Equals(string.Empty) ? string.Empty : Mobilephone;
            ent[NombreCamposContacto.Telephone1CRM] = Telephone1.Equals(string.Empty) ? string.Empty : Telephone1;
            ent[NombreCamposContacto.Aen_OrigenCRM] = Aen_Origen.Equals(string.Empty) ? null : new OptionSetValue(Convert.ToInt32(Aen_Origen));
            ent[NombreCamposContacto.Aen_IdentificadorcontactoCRM] = Aen_Identificadorcontacto.Equals(string.Empty) ? null : Aen_Identificadorcontacto;

            if (Contactid != Guid.Empty)
            { 
                ent["contactid"] = Contactid;
                ent.Id = Contactid;
            }

            if (Statecode.Equals("Activo"))
            {
                ent[NombreCamposContacto.StatecodeCRM] = new OptionSetValue(0);
                ent[NombreCamposContacto.StatuscodeCRM] = new OptionSetValue(1);
            }
            else if (Statecode.Equals("Inactivo") && Contactid != Guid.Empty && Contactid != null)
            {
                ent[NombreCamposContacto.StatecodeCRM] = new OptionSetValue(1);
                ent[NombreCamposContacto.StatuscodeCRM] = new OptionSetValue(2);
            }

            //Para saltar plugins de envío de datos a NEXO
            //ent["aen_vienedeintegracion"] = true;

            return ent;
        }


        public bool ContactosIguales(Contactos contCRM, ref Entity contSalida)
        {
            bool retorna = false;

            if (!Aen_ClaveIntegracionParent.Equals(contCRM.Aen_ClaveIntegracionParent))
                retorna = true;
            if (!Aen_CargoprincipalId.Equals(contCRM.Aen_CargoprincipalId))
                retorna = true;
            if (!Aen_Numerodocumento.Equals(contCRM.Aen_Numerodocumento))
                retorna = true;
            if (!Aen_Tipodocumento.Equals(contCRM.Aen_Tipodocumento))
                retorna = true;
            if (!Aen_Tratamiento.Equals(contCRM.Aen_Tratamiento))
                retorna = true;
            if (!Aen_Observaciones.Equals(contCRM.Aen_Observaciones))
                retorna = true;
            if (!Aen_Observacionesmigracion.Equals(contCRM.Aen_Observacionesmigracion))
                retorna = true;
            if (!Donotsendmm.Equals(contCRM.Donotsendmm) && Donotsendmm != "2")
                retorna = true;
            if (!Emailaddress1.Equals(contCRM.Emailaddress1))
                retorna = true;
            if (!Firstname.Equals(contCRM.Firstname))
                retorna = true;
            if (!Gendercode.Equals(contCRM.Gendercode))
                retorna = true;
            if (!Lastname.Equals(contCRM.Lastname))
                retorna = true;
            if (!Mobilephone.Equals(contCRM.Mobilephone))
                retorna = true;
            if (!Statecode.Equals(contCRM.Statecode))
                retorna = true;
            if (!Telephone1.Equals(contCRM.Telephone1))
                retorna = true;
            if (!Aen_Origen.Equals(contCRM.Aen_Origen))
                retorna = true;
            if (!Aen_Identificadorcontacto.Equals(contCRM.Aen_Identificadorcontacto))
                retorna = true;
            //var decimalString = auxTerCRM.Revenue_Base.ToString("#,####");

            if (retorna)
            {
                Contactid = contCRM.Contactid;
                contSalida = GetEntityFromContacto();
            }

            return retorna;
        }
    }
}
