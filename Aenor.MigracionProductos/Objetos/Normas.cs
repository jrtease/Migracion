using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductos.Objetos
{
    public class Normas
    {
        #region NORMAS (Norma Versión)
        public enum FormatoEspecial
        {
            Fotografias = (int)277220000,
            Tomos = (int)277220001,
            NoVendibleWEB = (int)277220002,
            Vacio = int.MinValue
        }

        public enum RazonEstado
        {
            Envigor = (int)1,
            Inactiva = (int)2,
            Anulada = (int)277220000,
            Vacio = int.MinValue
        }
        
        public enum Ambito
        {
            Nacional = (int)277220000,
            Europea = (int)277220001,
            Internacional = (int)277220002,
            Vacio = int.MinValue
        }

        public enum TipoNorma
        {
            NormaUNE = (int)277220000,
            NormaISO = (int)277220001,
            NormaASTM = (int)277220002,
            NormaIEC = (int)277220003,
            NormaIEEE = (int)277220004,
            Vacio = int.MinValue
        }

        
        public Guid Aen_versinGUID { get; set; }
        public bool Aen_Es_Ratificada {get; set;}
        public bool Aen_Royalty_Une { get; set; }
        public bool Aen_Royalty_Organismo { get; set; }
        public bool Aen_Norma_Nueva { get; set; } //String?¿  --> I ?
        public string Aen_Identificador_Nexo { get; set; }
        public string Aen_Fecha_Edicion { get; set; }
        public string Aen_Fecha_Anulacion { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public int Aen_Nu_Paginas { get; set; }
        public string Aen_Grupo_Precio { get; set; }
        public string Aen_Organismo { get; set; }
        public Guid Aen_OrganismoGUID { get; set; }
        public string Aen_OrganismoCI{ get; set; }
        public string Aen_Articulo { get; set; }   //PK 1
        public string Aen_Organismo_Norma { get; set; }
        public Guid Aen_Organismo_NormaGUID { get; set; }
        public string Aen_Organismo_NormaCI { get; set; }
        public FormatoEspecial Aen_Formato_Especial { get; set; }
        public string Aen_Organismo_Internacional { get; set; }
        public Guid Aen_Organismo_InternacionalGUID { get; set; }
        public string Aen_Organismo_InternacionalCI { get; set; }
        public string Aen_Organismo_Grupo { get; set; }
        public RazonEstado Aen_Estado { get; set; }
        public string Aen_Codigo_Norma { get; set; }
        public string Aen_Raiz_Norma { get; set; }
        public Guid Aen_Raiz_NormaGUID { get; set; }
        public Ambito Aen_Ambito_Norma { get; set; }
        public string Aen_Codigo_Comite { get; set; }
        public Guid Aen_Codigo_ComiteGUID { get; set; }
        public string Aen_Titulo_Norma_ES { get; set; }
        public string Aen_Titulo_Norma_EN { get; set; }
        public TipoNorma Aen_TipoNorma { get; set; }
        #endregion NORMAS

        #region METODOS
        public void NormasFromOracle(DataRow fila, Dictionary<string,Guid> MaestroTercerosCRM, Dictionary<string, Guid> MaestroComitesCRM, Dictionary<string, Guid> RaicesNormasCRM)
        {
            Aen_versinGUID = Guid.Empty;
            Aen_Es_Ratificada = fila[NombreCamposNormas.Aen_Es_RatificadaORACLE] == DBNull.Value ? false : (fila[NombreCamposNormas.Aen_Es_RatificadaORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Royalty_Une = fila[NombreCamposNormas.Aen_Royalty_UneORACLE] == DBNull.Value ? false : (fila[NombreCamposNormas.Aen_Royalty_UneORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Royalty_Organismo = fila[NombreCamposNormas.Aen_Royalty_OrganismoORACLE] == DBNull.Value ? false : (fila[NombreCamposNormas.Aen_Royalty_OrganismoORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Norma_Nueva = fila[NombreCamposNormas.Aen_Norma_NuevaORACLE] == DBNull.Value ? false : (fila[NombreCamposNormas.Aen_Norma_NuevaORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Identificador_Nexo = fila[NombreCamposNormas.Aen_Identificador_NexoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Identificador_NexoORACLE].ToString();
            Aen_Fecha_Edicion = fila[NombreCamposNormas.Aen_Fecha_EdicionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombreCamposNormas.Aen_Fecha_EdicionORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Fecha_Anulacion = fila[NombreCamposNormas.Aen_Fecha_AnulacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombreCamposNormas.Aen_Fecha_AnulacionORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Fecha_Actualizacion = fila[NombreCamposNormas.Aen_Fecha_ActualizacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombreCamposNormas.Aen_Fecha_ActualizacionORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Nu_Paginas = fila[NombreCamposNormas.Aen_Nu_PaginasORACLE] == DBNull.Value ? int.MinValue : Convert.ToInt16(fila[NombreCamposNormas.Aen_Nu_PaginasORACLE]);
            Aen_Grupo_Precio = fila[NombreCamposNormas.Aen_Grupo_PrecioORACLE] == DBNull.Value ? string.Empty: fila[NombreCamposNormas.Aen_Grupo_PrecioORACLE].ToString();
            Aen_Organismo = fila[NombreCamposNormas.Aen_OrganismoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_OrganismoORACLE].ToString();
            Aen_OrganismoCI = fila[NombreCamposNormas.Aen_OrganismoCIORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_OrganismoCIORACLE].ToString();
            if (Aen_OrganismoCI.Equals(string.Empty))
                Aen_OrganismoGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroTercerosCRM.TryGetValue(Aen_OrganismoCI, out aux);
                Aen_OrganismoGUID = aux;
            }
            Aen_Articulo = fila[NombreCamposNormas.Aen_ArticuloORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_ArticuloORACLE].ToString();
            Aen_Organismo_Norma = fila[NombreCamposNormas.Aen_Organismo_NormaORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Organismo_NormaORACLE].ToString();
            Aen_Organismo_NormaCI = fila[NombreCamposNormas.Aen_Organismo_NormaCIORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Organismo_NormaCIORACLE].ToString();
            if (Aen_Organismo_NormaCI.Equals(string.Empty))
                Aen_Organismo_NormaGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroTercerosCRM.TryGetValue(Aen_Organismo_NormaCI, out aux);
                Aen_Organismo_NormaGUID = aux;
            }
            Aen_Formato_Especial = fila[NombreCamposNormas.Aen_Formato_EspecialORACLE] == DBNull.Value ? FormatoEspecial.Vacio : AsignaFormatoPicklist(fila[NombreCamposNormas.Aen_Formato_EspecialORACLE].ToString().Trim());
            Aen_Organismo_Internacional = fila[NombreCamposNormas.Aen_Organismo_InternacionalORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Organismo_InternacionalORACLE].ToString();
            Aen_Organismo_InternacionalCI = fila[NombreCamposNormas.Aen_Organismo_InternacionalCIORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Organismo_InternacionalCIORACLE].ToString();
            if (Aen_Organismo_InternacionalCI.Equals(string.Empty))
                Aen_Organismo_InternacionalGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroTercerosCRM.TryGetValue(Aen_Organismo_InternacionalCI, out aux);
                Aen_Organismo_InternacionalGUID = aux;
            }
            Aen_Organismo_Grupo = fila[NombreCamposNormas.Aen_Organismo_GrupoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Organismo_GrupoORACLE].ToString();
            Aen_Estado = fila[NombreCamposNormas.Aen_EstadoORACLE] == DBNull.Value ? RazonEstado.Vacio : AsignaEstadoPicklist(fila[NombreCamposNormas.Aen_EstadoORACLE].ToString().Replace(" ","").Trim());
            Aen_Codigo_Norma = fila[NombreCamposNormas.Aen_Codigo_NormaORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Codigo_NormaORACLE].ToString();
            Aen_Raiz_Norma = fila[NombreCamposNormas.Aen_Raiz_NormaORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Raiz_NormaORACLE].ToString();
            if (Aen_Raiz_Norma.Equals(string.Empty))
                Aen_Raiz_NormaGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                RaicesNormasCRM.TryGetValue(Aen_Raiz_Norma, out aux);
                Aen_Raiz_NormaGUID = aux;
            }
            Aen_Ambito_Norma = fila[NombreCamposNormas.Aen_Ambito_NormaORACLE] == DBNull.Value ? Ambito.Vacio : AsignaAmbitoPicklist(fila[NombreCamposNormas.Aen_Ambito_NormaORACLE].ToString().Trim());
            Aen_Codigo_Comite = fila[NombreCamposNormas.Aen_Codigo_ComiteORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Codigo_ComiteORACLE].ToString();
            if (Aen_Codigo_Comite.Equals(string.Empty))
                Aen_Codigo_ComiteGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroComitesCRM.TryGetValue(Aen_Codigo_Comite, out aux);
                Aen_Codigo_ComiteGUID = aux;
            }
            Aen_Titulo_Norma_ES = fila[NombreCamposNormas.Aen_Titulo_Norma_ESORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Titulo_Norma_ESORACLE].ToString();
            Aen_Titulo_Norma_EN = fila[NombreCamposNormas.Aen_Titulo_Norma_ENORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormas.Aen_Titulo_Norma_ENORACLE].ToString();
            Aen_TipoNorma = fila[NombreCamposNormas.Aen_TipoNormaORACLE] == DBNull.Value ? TipoNorma.Vacio : AsignaTipoNormaPicklist(fila[NombreCamposNormas.Aen_TipoNormaORACLE].ToString().Trim());
        }

        public void VersionFromCRM(Entity nrm)
        {
            Aen_versinGUID = nrm.Id;
            Aen_Es_Ratificada = nrm.Contains(NombreCamposNormas.Aen_Es_RatificadaCRM) ? nrm.GetAttributeValue<bool>(NombreCamposNormas.Aen_Es_RatificadaCRM): false;
            Aen_Royalty_Une = nrm.Contains(NombreCamposNormas.Aen_Royalty_UneCRM) ? nrm.GetAttributeValue<bool>(NombreCamposNormas.Aen_Royalty_UneCRM) : false;
            Aen_Royalty_Organismo = nrm.Contains(NombreCamposNormas.Aen_Royalty_OrganismoCRM) ? nrm.GetAttributeValue<bool>(NombreCamposNormas.Aen_Royalty_OrganismoCRM) : false;
            Aen_Norma_Nueva = false;
            Aen_Identificador_Nexo = nrm.Contains(NombreCamposNormas.Aen_Identificador_NexoCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Identificador_NexoCRM).ToString() : string.Empty;
            Aen_Fecha_Edicion = nrm.Contains(NombreCamposNormas.Aen_Fecha_EdicionCRM) ? nrm.GetAttributeValue<DateTime>(NombreCamposNormas.Aen_Fecha_EdicionCRM).ToLocalTime().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim() : string.Empty;
            Aen_Fecha_Anulacion = nrm.Contains(NombreCamposNormas.Aen_Fecha_AnulacionCRM) ? nrm.GetAttributeValue<DateTime>(NombreCamposNormas.Aen_Fecha_AnulacionCRM).ToLocalTime().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim() : string.Empty;
            Aen_Nu_Paginas = nrm.Contains(NombreCamposNormas.Aen_Nu_PaginasCRM) ? nrm.GetAttributeValue<int>(NombreCamposNormas.Aen_Nu_PaginasCRM) : int.MinValue;
            Aen_Grupo_Precio = nrm.Contains(NombreCamposNormas.Aen_Grupo_PrecioCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Grupo_PrecioCRM).ToString()  : string.Empty;
            Aen_OrganismoGUID = nrm.Contains(NombreCamposNormas.Aen_OrganismoCRM) ? ((EntityReference)nrm.GetAttributeValue<EntityReference>(NombreCamposNormas.Aen_OrganismoCRM)).Id : Guid.Empty;
            Aen_Organismo = string.Empty;
            Aen_Organismo_InternacionalGUID = nrm.Contains(NombreCamposNormas.Aen_Organismo_InternacionalCRM) ? ((EntityReference)nrm.GetAttributeValue<EntityReference>(NombreCamposNormas.Aen_Organismo_InternacionalCRM)).Id : Guid.Empty;
            Aen_Organismo_Internacional = string.Empty;
            Aen_Organismo_NormaGUID = nrm.Contains(NombreCamposNormas.Aen_Organismo_NormaCRM) ? ((EntityReference)nrm.GetAttributeValue<EntityReference>(NombreCamposNormas.Aen_Organismo_NormaCRM)).Id : Guid.Empty;
            Aen_Organismo_Norma = string.Empty;
            Aen_Articulo = nrm.Contains(NombreCamposNormas.Aen_ArticuloCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_ArticuloCRM).ToString() : string.Empty;
            Aen_Formato_Especial = nrm.Contains(NombreCamposNormas.Aen_Formato_EspecialCRM) ? (FormatoEspecial)nrm.GetAttributeValue<OptionSetValue>(NombreCamposNormas.Aen_Formato_EspecialCRM).Value : FormatoEspecial.Vacio;
            Aen_Organismo_Grupo = nrm.Contains(NombreCamposNormas.Aen_Organismo_GrupoCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Organismo_GrupoCRM).ToString() : string.Empty;
            Aen_Estado = nrm.Contains(NombreCamposNormas.Aen_EstadoCRM) ? (RazonEstado)(nrm.GetAttributeValue<OptionSetValue>(NombreCamposNormas.Aen_EstatusCRM).Value) : RazonEstado.Vacio;
            Aen_Codigo_Norma = nrm.Contains(NombreCamposNormas.Aen_Codigo_NormaCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Codigo_NormaCRM).ToString() : string.Empty;
            Aen_Raiz_NormaGUID = nrm.Contains(NombreCamposNormas.Aen_Raiz_NormaCRM) ? ((EntityReference)nrm.GetAttributeValue<EntityReference>(NombreCamposNormas.Aen_Raiz_NormaCRM)).Id : Guid.Empty;
            Aen_Raiz_Norma = string.Empty;
            Aen_Ambito_Norma = nrm.Contains(NombreCamposNormas.Aen_Ambito_NormaCRM) ? (Ambito)nrm.GetAttributeValue<OptionSetValue>(NombreCamposNormas.Aen_Ambito_NormaCRM).Value : Ambito.Vacio;
            Aen_Codigo_ComiteGUID = nrm.Contains(NombreCamposNormas.Aen_Codigo_ComiteCRM) ? ((EntityReference)nrm.GetAttributeValue<EntityReference>(NombreCamposNormas.Aen_Codigo_ComiteCRM)).Id : Guid.Empty;
            Aen_Codigo_Comite = string.Empty;
            Aen_Titulo_Norma_ES = nrm.Contains(NombreCamposNormas.Aen_Titulo_Norma_ESCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Titulo_Norma_ESCRM): string.Empty ;
            Aen_Titulo_Norma_EN = nrm.Contains(NombreCamposNormas.Aen_Titulo_Norma_ENCRM) ? nrm.GetAttributeValue<string>(NombreCamposNormas.Aen_Titulo_Norma_ENCRM) : string.Empty;
            Aen_TipoNorma = nrm.Contains(NombreCamposNormas.Aen_TipoNormaCRM) ? (TipoNorma)(nrm.GetAttributeValue<OptionSetValue>(NombreCamposNormas.Aen_TipoNormaCRM).Value) : TipoNorma.Vacio;
        }

        public Entity GetEntity()
        {
            Entity aux = new Entity(NombreCamposNormas.EntityNameVersion);
            if (!Aen_versinGUID.Equals(Guid.Empty))
            {
                aux.Id = Aen_versinGUID;
                aux[NombreCamposNormas.EntityIDVersion] = Aen_versinGUID;
            }
            aux[NombreCamposNormas.Aen_Es_RatificadaCRM] = Aen_Es_Ratificada;
            aux[NombreCamposNormas.Aen_Royalty_UneCRM] = Aen_Royalty_Une;
            aux[NombreCamposNormas.Aen_Royalty_OrganismoCRM] = Aen_Royalty_Organismo;
            aux[NombreCamposNormas.Aen_Identificador_NexoCRM] = Aen_Identificador_Nexo;
            if (!Aen_Fecha_Edicion.Equals(string.Empty))
            { 
                var okFecha = DateTime.TryParse(Aen_Fecha_Edicion, out DateTime f);
                if (okFecha)
                    aux[NombreCamposNormas.Aen_Fecha_EdicionCRM] = f;
                else
                    aux[NombreCamposNormas.Aen_Fecha_EdicionCRM] = null;
            }
            else
                aux[NombreCamposNormas.Aen_Fecha_EdicionCRM] = null;
            if (!Aen_Fecha_Anulacion.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(Aen_Fecha_Anulacion, out DateTime f);
                if (okFecha)
                    aux[NombreCamposNormas.Aen_Fecha_AnulacionCRM] = f;
                else
                    aux[NombreCamposNormas.Aen_Fecha_AnulacionCRM] = null;
            }
            else
                aux[NombreCamposNormas.Aen_Fecha_AnulacionCRM] = null;
            if (!Aen_Nu_Paginas.Equals(int.MinValue))    aux[NombreCamposNormas.Aen_Nu_PaginasCRM] = Aen_Nu_Paginas;
            aux[NombreCamposNormas.Aen_Grupo_PrecioCRM] = Aen_Grupo_Precio;
            aux[NombreCamposNormas.Aen_OrganismoCRM] = Aen_OrganismoGUID.Equals(Guid.Empty) ? null : new EntityReference("account", Aen_OrganismoGUID);
            aux[NombreCamposNormas.Aen_ArticuloCRM] = Aen_Articulo;
            aux[NombreCamposNormas.Aen_Organismo_NormaCRM] = Aen_Organismo_NormaGUID.Equals(Guid.Empty) ? null : new EntityReference("account", Aen_Organismo_NormaGUID);
            aux[NombreCamposNormas.Aen_Formato_EspecialCRM] = Aen_Formato_Especial.Equals(FormatoEspecial.Vacio) ? null : new OptionSetValue((Int32)Aen_Formato_Especial);
            aux[NombreCamposNormas.Aen_Organismo_InternacionalCRM] = Aen_Organismo_InternacionalGUID.Equals(Guid.Empty) ? null : new EntityReference("account", Aen_Organismo_InternacionalGUID);
            aux[NombreCamposNormas.Aen_Organismo_GrupoCRM] = Aen_Organismo_Grupo;
            if (!Aen_Estado.Equals(RazonEstado.Inactiva))
            {
                aux[NombreCamposNormas.Aen_EstatusCRM] = Aen_Estado.Equals(RazonEstado.Vacio) ? null : new OptionSetValue((Int32)Aen_Estado);
                aux[NombreCamposNormas.Aen_EstadoCRM] = new OptionSetValue(0);
            }
            else
            {
                aux[NombreCamposNormas.Aen_EstatusCRM] = new OptionSetValue(2);
                aux[NombreCamposNormas.Aen_EstadoCRM] = new OptionSetValue(1);
            }
            aux[NombreCamposNormas.Aen_Codigo_NormaCRM] = Aen_Codigo_Norma;
            aux[NombreCamposNormas.Aen_Raiz_NormaCRM] = Aen_Raiz_NormaGUID.Equals(Guid.Empty) ? null : new EntityReference(NombreCamposNormas.EntityNameRaiz, Aen_Raiz_NormaGUID);
            aux[NombreCamposNormas.Aen_Ambito_NormaCRM] = Aen_Ambito_Norma.Equals(Ambito.Vacio) ? null : new OptionSetValue((Int32)Aen_Ambito_Norma);
            aux[NombreCamposNormas.Aen_Codigo_ComiteCRM] = Aen_Codigo_ComiteGUID.Equals(Guid.Empty) ? null : new EntityReference(NombresCamposComiteTecnico.EntityName, Aen_Codigo_ComiteGUID);
            aux[NombreCamposNormas.Aen_Titulo_Norma_ESCRM] = Aen_Titulo_Norma_ES;
            aux[NombreCamposNormas.Aen_Titulo_Norma_ENCRM] = Aen_Titulo_Norma_EN;
            aux[NombreCamposNormas.Aen_TipoNormaCRM] = Aen_TipoNorma.Equals(TipoNorma.Vacio) ? null : new OptionSetValue((Int32)Aen_TipoNorma);

            return aux;
        }

        public bool VersionesIguales(Normas auxVersCRM, ref Entity versionUpdate)
        {
            bool res = false;
            if (!Aen_Es_Ratificada.Equals(auxVersCRM.Aen_Es_Ratificada))
                res = true;
            if (!Aen_Royalty_Une.Equals(auxVersCRM.Aen_Royalty_Une))
                res = true;
            if (!Aen_Royalty_Organismo.Equals(auxVersCRM.Aen_Royalty_Organismo))
                res = true;
            if (!Aen_Identificador_Nexo.Equals(auxVersCRM.Aen_Identificador_Nexo))
                res = true;
            if (!Aen_Fecha_Edicion.Equals(auxVersCRM.Aen_Fecha_Edicion))
                res = true;
            if (!Aen_Fecha_Anulacion.Equals(auxVersCRM.Aen_Fecha_Anulacion))
                res = true;
            if (!Aen_Nu_Paginas.Equals(auxVersCRM.Aen_Nu_Paginas))
                res = true;
            if (!Aen_Grupo_Precio.Equals(auxVersCRM.Aen_Grupo_Precio))
                res = true;
            if (!Aen_OrganismoGUID.Equals(auxVersCRM.Aen_OrganismoGUID))
                res = true;
            if (!Aen_Articulo.Equals(auxVersCRM.Aen_Articulo))
                res = true;
            if (!Aen_Organismo_NormaGUID.Equals(auxVersCRM.Aen_Organismo_NormaGUID))
                res = true;
            if (!Aen_Formato_Especial.Equals(auxVersCRM.Aen_Formato_Especial))
                res = true;
            if (!Aen_Organismo_InternacionalGUID.Equals(auxVersCRM.Aen_Organismo_InternacionalGUID))
                res = true;
            if (!Aen_Organismo_Grupo.Equals(auxVersCRM.Aen_Organismo_Grupo))
                res = true;
            if (!Aen_Estado.Equals(auxVersCRM.Aen_Estado))
                res = true;
            if (!Aen_Codigo_Norma.Equals(auxVersCRM.Aen_Codigo_Norma))
                res = true;
            if (!Aen_Raiz_NormaGUID.Equals(auxVersCRM.Aen_Raiz_NormaGUID))
                res = true;
            if (!Aen_Ambito_Norma.Equals(auxVersCRM.Aen_Ambito_Norma))
                res = true;
            if (!Aen_Codigo_ComiteGUID.Equals(auxVersCRM.Aen_Codigo_ComiteGUID))
                res = true;
            if (!Aen_Titulo_Norma_ES.Equals(auxVersCRM.Aen_Titulo_Norma_ES))
                res = true;
            if (!Aen_Titulo_Norma_EN.Equals(auxVersCRM.Aen_Titulo_Norma_EN))
                res = true;
            if (!Aen_TipoNorma.Equals(auxVersCRM.Aen_TipoNorma))
                res = true;

            if (res)
            {
                Aen_versinGUID = auxVersCRM.Aen_versinGUID;
                versionUpdate = GetEntity();
            }
            return res;
        }

        private FormatoEspecial AsignaFormatoPicklist(string auxt)
        {
            FormatoEspecial aux;
            switch (auxt)
            {
                case "Fotografias":
                case "Fotografías":
                    aux = FormatoEspecial.Fotografias;
                    break;
                case "Tomos":
                    aux = FormatoEspecial.Tomos;
                    break;
                case "No Vendible Web":
                    aux = FormatoEspecial.NoVendibleWEB;
                    break;
                default:
                    aux = FormatoEspecial.Vacio;
                    break;
            }
            return aux;
        }

        private RazonEstado AsignaEstadoPicklist(string auxt)
        {
            RazonEstado aux;
            switch (auxt)
            {
                case "Envigor":
                    aux = RazonEstado.Envigor;
                    break;
                case "Anulada":
                    aux = RazonEstado.Anulada;
                    break;
                default:
                    aux = RazonEstado.Vacio;
                    break;
            }
            return aux;
        }

        private Ambito AsignaAmbitoPicklist(string auxt)
        {
            Ambito aux;
            switch (auxt)
            {
                case "Europea":
                    aux = Ambito.Europea;
                    break;
                case "Internacional":
                    aux = Ambito.Internacional;
                    break;
                case "Nacional":
                    aux = Ambito.Nacional;
                    break;
                default:
                    aux = Ambito.Vacio;
                    break;
            }
            return aux;
        }

        private TipoNorma AsignaTipoNormaPicklist(string v)
        {
            TipoNorma aux;
            switch (v)
            {
                case "NormaUNE":
                    aux = TipoNorma.NormaUNE;
                    break;
                case "NormaISO":
                    aux = TipoNorma.NormaISO;
                    break;
                case "NormaASTM":
                    aux = TipoNorma.NormaASTM;
                    break;
                case "NormaIEC":
                    aux = TipoNorma.NormaIEC;
                    break;
                case "NormaIEEE":
                    aux = TipoNorma.NormaIEEE;
                    break;
                default:
                    aux = TipoNorma.Vacio;
                    break;
            }
            return aux;
        }
        #endregion METODOS
    }


    public class NombreCamposNormas
    {
        public static string EntityNameRaiz = "aen_norma";
        public static string EntityIDRaiz = "aen_normaid";
        public static string Aen_CodigoRaizNorma = "aen_name";


        //TODAS PARA ENTIDAD NORMA VERSION MENOS LAS INDICADAS EN COMENTARIO
        public static string EntityNameVersion = "aen_versin";
        public static string EntityIDVersion = EntityNameVersion+"id";

        public static string Aen_Es_RatificadaCRM = "aen_ratificada";
        public static string Aen_Royalty_UneCRM = "aen_royaltyune";
        public static string Aen_Royalty_OrganismoCRM = "aen_royaltyorganismo";
        public static string Aen_Identificador_NexoCRM = "aen_identificadornexo";
        public static string Aen_Fecha_EdicionCRM = "aen_fechadeedicion";
        public static string Aen_Fecha_AnulacionCRM = "aen_fechadeanulacion";
        public static string Aen_Nu_PaginasCRM = "aen_numeropaginas";
        public static string Aen_Grupo_PrecioCRM = "aen_grupoprecio";
        public static string Aen_OrganismoCRM = "aen_organismo";
        public static string Aen_ArticuloCRM = "aen_articulo";
        public static string Aen_Organismo_NormaCRM = "aen_organismonorma";
        public static string Aen_Formato_EspecialCRM = "aen_formatoespecial";
        public static string Aen_Organismo_InternacionalCRM = "aen_organismointernacional";
        public static string Aen_Organismo_GrupoCRM = "aen_grupoorganismo";
        public static string Aen_EstadoCRM = "statecode";
        public static string Aen_EstatusCRM = "statuscode";
        public static string Aen_Codigo_NormaCRM = "aen_name";
        public static string Aen_Raiz_NormaCRM = "aen_norma";
        public static string Aen_Ambito_NormaCRM = "aen_ambito";
        public static string Aen_Codigo_ComiteCRM = "aen_codigoctn";
        public static string Aen_Titulo_Norma_ESCRM = "aen_titulonormaes";
        public static string Aen_Titulo_Norma_ENCRM = "aen_titulonormaen";
        public static string Aen_TipoNormaCRM = "aen_tipo";



        public static string Aen_Es_RatificadaORACLE = "aen_es_ratificada";
        public static string Aen_Royalty_UneORACLE = "aen_royalty_une";
        public static string Aen_Royalty_OrganismoORACLE = "aen_royalty_organismo";
        public static string Aen_Norma_NuevaORACLE = "aen_norma_nueva";
        public static string Aen_Identificador_NexoORACLE = "aen_identificador_nexo";
        public static string Aen_Fecha_EdicionORACLE = "aen_fecha_edicion";
        public static string Aen_Fecha_AnulacionORACLE = "aen_fecha_anulacion";
        public static string Aen_Fecha_ActualizacionORACLE = "aen_fecha_actualizacion";
        public static string Aen_Nu_PaginasORACLE = "aen_nu_paginas";
        public static string Aen_Grupo_PrecioORACLE = "aen_grupo_precio";
        public static string Aen_OrganismoORACLE = "aen_organismo";
        public static string Aen_OrganismoCIORACLE = "aen_claveintegracion_org";
        public static string Aen_ArticuloORACLE = "aen_articulo";
        public static string Aen_Organismo_NormaORACLE = "aen_organismo_norma";
        public static string Aen_Organismo_NormaCIORACLE = "aen_claveintegracion_org_n";
        public static string Aen_Formato_EspecialORACLE = "aen_formato_especial";
        public static string Aen_Organismo_InternacionalORACLE = "aen_organismo_internacional";
        public static string Aen_Organismo_InternacionalCIORACLE = "aen_claveintegracion_org_i";
        public static string Aen_Organismo_GrupoORACLE = "aen_organismo_grupo";
        public static string Aen_EstadoORACLE = "aen_estado";
        public static string Aen_Codigo_NormaORACLE = "aen_codigo_norma";
        public static string Aen_Raiz_NormaORACLE = "aen_raiz_norma";
        public static string Aen_Ambito_NormaORACLE = "aen_ambito_norma";
        public static string Aen_Codigo_ComiteORACLE = "aen_codigo_comite";
        public static string Aen_Titulo_Norma_ESORACLE = "aen_titulo_norma_es";
        public static string Aen_Titulo_Norma_ENORACLE = "aen_titulo_norma_en";
        public static string Aen_TipoNormaORACLE = "aen_tipo";
    }
}
