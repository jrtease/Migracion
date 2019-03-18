using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aenor.MigracionTerceros.Clases;

namespace Aenor.MigracionTerceros.Objetos
{
    public class Direccion
    {
        public Guid Aen_DireccionId { get; set; }
        public string Aen_ClaveIntegracion { get; set; }
        public string Aen_ClaveIntegracionParent { get; set; }
        public Guid Aen_TerceroId { get; set; }
        public string Aen_CodigoPostal { get; set; }
        public string Aen_Comunidadautonoma { get; set; }
        public Guid Aen_Contacto { get; set; } //A partir de aen_claveintegracioncontacto
        public string Aen_Email { get; set; }
        public string Aen_Fax { get; set; }
        public string Aen_Localidad { get; set; }
        public string Aen_Name { get; set; }
        public string Aen_Nombrecompleto { get; set; }
        public string Aen_NumeroDeVia { get; set; }
        public string Aen_NombreDeVia { get; set; }
        public string Aen_Observaciones { get; set; }
        public Guid Aen_PaisId { get; set; }
        public string Aen_Codigopais { get; set; }
        public Guid Aen_ProvinciaId { get; set; }
        public bool Aen_RazonSocial { get; set; }
        public string Aen_RestoDireccion { get; set; }
        public string Aen_Telefono1 { get; set; }
        public string Aen_Telefono2 { get; set; }
        public string Aen_TipoDeDireccion { get; set; }
        public Guid Aen_TipoDeViaId { get; set; }
        public string StateCode { get; set; }
        public string Aen_Observacionesmigracion { get; set; }
        public string Aen_Origen { get; set; }
        public string Aen_Descripcion { get; set; }
        public string Aen_Identificadordireccion { get; set; }
    }
}
