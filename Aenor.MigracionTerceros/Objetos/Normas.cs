using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Objetos
{
    public class Normas
    {
        public string Aen_Organismo { get; set; }
        public string Aen_Articulo { get; set; }
        public string Aen_Identificador_Nexo { get; set; }
        public string Aen_Codigo_Norma { get; set; }
        public string Aen_Organismo_Norma { get; set; }
        public string Aen_Raiz_Norma { get; set; }
        public string Aen_Ambito_Norma { get; set; }
        public string Aen_Codigo_Comite { get; set; }
        public string Aen_Fecha_Edicion { get; set; }
        public string Aen_Fecha_Anulacion { get; set; }
        public string Aen_Estado { get; set; }
        public string Aen_Formato_Especial { get; set; }
        public string Aen_Es_Ratificada { get; set; }
        public string Aen_Royalty_Une { get; set; }
        public string Aen_Organismo_Internacional { get; set; }
        public string Aen_Royalty_Organismo { get; set; }
        public string Aen_Titulo_Norma_Es { get; set; }
        public string Aen_Titulo_Norma_En { get; set; }
        public string Aen_Organismo_Grupo { get; set; }
        public string Aen_Grupo_Precio { get; set; }
        public string Aen_Norma_Nueva { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }

        //EXCLUSIVO NORMAS_ICS
        //public string Aen_Organismo { get; set; }
        //public string Aen_Articulo { get; set; }
        //public string Aen_Identificador_Nexo { get; set; }
        //public string Aen_Codigo_Norma { get; set; }
        //public string Aen_Fecha_Actualizacion { get; set; }
        public string Aen_Codigo_Ics { get; set; }
        public string Aen_Estado_Ics { get; set; }
    }




    public class Norma_Producto
    {
        public string Aen_Organismo { get; set; }
        public string Aen_Articulo { get; set; }
        public string Aen_Identificador_Nexo { get; set; }
        public string Aen_Codigo_Producto { get; set; }
        public string Aen_Codigo_Norma { get; set; }
        public string Aen_Idioma { get; set; }
        public string Aen_Soporte { get; set; }
        public string Aen_Documento { get; set; }
        public string Aen_Precio { get; set; }
        public string Aen_Vendible_Web { get; set; }
        public string Aen_Path { get; set; }
        public string Aen_Url_Organismo { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public string Aen_Fecha_Documento { get; set; }
    }

}
