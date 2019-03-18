using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionProductos.Objetos
{
    public class NormasICS
    {
        #region PROPIEDADES
        public bool Aen_Ics_Nuevo { get; set; }
        public string Aen_Identificador_Nexo { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public string Aen_Codigo_Ics { get; set; }   //PK 2 --> ICS
        public Guid Aen_Codigo_IcsGUID { get; set; }   
        public string Aen_Organismo { get; set; }
        public string Aen_Articulo { get; set; }   //PK 1 --> Normas-Productos
        public Guid Aen_ArticuloGUID { get; set; }  
        public string Aen_Codigo_Norma { get; set; }
        #endregion PROPIEDADES



        #region METODOS
        public void NormasICSFromOracle(DataRow fila, Dictionary<string, Guid> MaestroICS, Dictionary<string, Guid> MaestroVersion)
        {
            Aen_Ics_Nuevo = fila[NombresCamposNormasICS.Aen_Ics_NuevoORACLE] == DBNull.Value ? false : (fila[NombresCamposNormasICS.Aen_Ics_NuevoORACLE].Equals('I') ? true : false);
            Aen_Identificador_Nexo = fila[NombresCamposNormasICS.Aen_Identificador_NexoORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposNormasICS.Aen_Identificador_NexoORACLE].ToString();
            Aen_Fecha_Actualizacion = fila[NombresCamposNormasICS.Aen_Fecha_ActualizacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposNormasICS.Aen_Fecha_ActualizacionORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Codigo_Ics = fila[NombresCamposNormasICS.Aen_Codigo_IcsORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposNormasICS.Aen_Codigo_IcsORACLE].ToString();
            if (Aen_Codigo_Ics.Equals(string.Empty))
                Aen_Codigo_IcsGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroICS.TryGetValue(Aen_Codigo_Ics, out aux);
                Aen_Codigo_IcsGUID = aux;
            }
            Aen_Organismo = fila[NombresCamposNormasICS.Aen_OrganismoORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposNormasICS.Aen_OrganismoORACLE].ToString();
            Aen_Articulo = fila[NombresCamposNormasICS.Aen_ArticuloORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposNormasICS.Aen_ArticuloORACLE].ToString();
            if (Aen_Articulo.Equals(string.Empty))
                Aen_ArticuloGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroVersion.TryGetValue(Aen_Articulo, out aux);
                Aen_ArticuloGUID = aux;
            }
            Aen_Codigo_Norma = fila[NombresCamposNormasICS.Aen_Codigo_NormaORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposNormasICS.Aen_Codigo_NormaORACLE].ToString();
        }
        #endregion METODOS
    }


    public class NombresCamposNormasICS
    {
        public static string RelationshipName = "aen_versin_aen_ics";

        public static string EntityNameICS = "aen_ics";
        public static string EntityIDICS = "aen_icsid";
        public static string EntityNameVersion = "aen_versin";
        public static string EntityIDVersin = "aen_versinid";


        public static string Aen_Ics_NuevoORACLE = "aen_ics_nuevo";
        public static string Aen_Identificador_NexoORACLE = "aen_identificador_nexo";
        public static string Aen_Fecha_ActualizacionORACLE = "aen_fecha_actualizacion";
        public static string Aen_Codigo_IcsORACLE = "aen_codigo_ics";
        public static string Aen_OrganismoORACLE = "aen_organismo";
        public static string Aen_ArticuloORACLE = "aen_articulo";
        public static string Aen_Codigo_NormaORACLE = "aen_codigo_norma";
    }
}
