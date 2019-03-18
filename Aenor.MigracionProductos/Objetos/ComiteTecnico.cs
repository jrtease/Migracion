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
    public class ComiteTecnico
    {
        public enum Organismo
        {
            ISO = (int)277220000,
            UNE = (int)277220001,
            ASTM = (int)277220002,
            IEEC = (int)277220003,
            VACIO = int.MinValue
        }

        #region PROPIEDADES
        public Guid Aen_ComiteGUID { get; set; }
        public bool Aen_Comite_Nuevo { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public Organismo Aen_Organismo { get; set; }
        public string Aen_Codigo_Comite { get; set; }
        public string Aen_Codigo_PadreSTR { get; set; }
        public Guid Aen_Codigo_Padre { get; set; }
        public string Aen_Nombre_Comite { get; set; }   //PK 1
        public string Aen_Nombre_Comite_EN { get; set; }
        #endregion PROPIEDADES



        #region METODOS
        public void ComiteFromOracle(DataRow fila, Dictionary<string, Guid> DicComites)
        {
            Aen_ComiteGUID = Guid.Empty;
            Aen_Comite_Nuevo = fila[NombresCamposComiteTecnico.Aen_Comite_NuevoORACLE] == DBNull.Value ? false : (fila[NombresCamposComiteTecnico.Aen_Comite_NuevoORACLE].ToString().Trim().Equals('S') ? true : false);
            Aen_Fecha_Actualizacion = fila[NombresCamposComiteTecnico.Aen_Fecha_ActualizacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposComiteTecnico.Aen_Fecha_ActualizacionORACLE]).ToString("dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            Aen_Codigo_Comite = fila[NombresCamposComiteTecnico.Aen_Codigo_ComiteORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposComiteTecnico.Aen_Codigo_ComiteORACLE].ToString().Trim();
            Aen_Nombre_Comite = fila[NombresCamposComiteTecnico.Aen_Nombre_ComiteORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposComiteTecnico.Aen_Nombre_ComiteORACLE].ToString().Trim();
            Aen_Nombre_Comite_EN = fila[NombresCamposComiteTecnico.Aen_Nombre_Comite_ENORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposComiteTecnico.Aen_Nombre_Comite_ENORACLE].ToString().Trim();
            Aen_Organismo = fila[NombresCamposComiteTecnico.Aen_OrganismoORACLE] == DBNull.Value ? Organismo.VACIO : AsignaOrganismoPicklist(fila[NombresCamposComiteTecnico.Aen_OrganismoORACLE].ToString().Trim().ToUpper());

            Aen_Codigo_PadreSTR = fila[NombresCamposComiteTecnico.Aen_Codigo_PadreORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposComiteTecnico.Aen_Codigo_PadreORACLE].ToString().Trim();
            if (Aen_Codigo_PadreSTR.Equals(string.Empty))
                Aen_Codigo_Padre = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                var ok = DicComites.TryGetValue(Aen_Codigo_PadreSTR, out aux);
                Aen_Codigo_Padre = aux;
            }
        }

        public void ComiteFromCRM(Entity com)
        {
            Aen_ComiteGUID = com.Id;
            Aen_Codigo_Comite = com.Contains(NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM) ? com.GetAttributeValue<string>(NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM) : string.Empty;
            Aen_Nombre_Comite = com.Contains(NombresCamposComiteTecnico.Aen_Nombre_ComiteCRM) ? com.GetAttributeValue<string>(NombresCamposComiteTecnico.Aen_Nombre_ComiteCRM) : string.Empty;
            Aen_Nombre_Comite_EN = com.Contains(NombresCamposComiteTecnico.Aen_Nombre_Comite_ENCRM) ? com.GetAttributeValue<string>(NombresCamposComiteTecnico.Aen_Nombre_Comite_ENCRM) : string.Empty;
            Aen_Codigo_Padre = com.Contains(NombresCamposComiteTecnico.Aen_Codigo_PadreCRM) ? (com.GetAttributeValue<EntityReference>(NombresCamposComiteTecnico.Aen_Codigo_PadreCRM)).Id : Guid.Empty;
            Aen_Organismo = com.Contains(NombresCamposComiteTecnico.Aen_OrganismoCRM) ? (Organismo)(com.GetAttributeValue<OptionSetValue>(NombresCamposComiteTecnico.Aen_OrganismoCRM).Value) : Organismo.VACIO;
        }

        public Entity GetEntity()
        {
            Entity cm = new Entity(NombresCamposComiteTecnico.EntityName);
            if (!Aen_ComiteGUID.Equals(Guid.Empty))
            {
                cm.Id = Aen_ComiteGUID;
                cm[NombresCamposComiteTecnico.EntityId] = Aen_ComiteGUID;
            }
            cm[NombresCamposComiteTecnico.Aen_Codigo_ComiteCRM] = Aen_Codigo_Comite;
            cm[NombresCamposComiteTecnico.Aen_Nombre_ComiteCRM] = Aen_Nombre_Comite;
            cm[NombresCamposComiteTecnico.Aen_Nombre_Comite_ENCRM] = Aen_Nombre_Comite_EN;
            cm[NombresCamposComiteTecnico.Aen_Codigo_PadreCRM] = !Aen_Codigo_Padre.Equals(Guid.Empty) ? new EntityReference(NombresCamposComiteTecnico.EntityName, Aen_Codigo_Padre) : null;
            cm[NombresCamposComiteTecnico.Aen_OrganismoCRM] = Aen_Organismo.Equals(Organismo.VACIO) ? null : cm[NombresCamposComiteTecnico.Aen_OrganismoCRM] = new OptionSetValue((Int32)Aen_Organismo);

            return cm;
        }

        public bool ComitesIguales(ComiteTecnico auxContCRM, ref Entity comiteUpdate)
        {
            bool res = false;

            if(!Aen_Codigo_Comite.Equals(auxContCRM.Aen_Codigo_Comite))
                res = true;
            if (!Aen_Codigo_Padre.Equals(auxContCRM.Aen_Codigo_Padre))
                res = true;
            if (!Aen_Nombre_Comite.Equals(auxContCRM.Aen_Nombre_Comite))
                res = true;
            if (!Aen_Nombre_Comite_EN.Equals(auxContCRM.Aen_Nombre_Comite_EN))
                res = true;
            if (!Aen_Organismo.Equals(auxContCRM.Aen_Organismo))
                res = true;

            if (res)
            {
                Aen_ComiteGUID = auxContCRM.Aen_ComiteGUID;
                comiteUpdate = GetEntity();
            }

            return res;
        }

        private Organismo AsignaOrganismoPicklist(string auxt)
        {
            Organismo aux;
            switch (auxt)
            {
                case "ISO":
                    aux = Organismo.ISO;
                    break;
                case "UNE":
                    aux = Organismo.UNE;
                    break;
                case "ASTM":
                    aux = Organismo.ASTM;
                    break;
                case "IEEC":
                    aux = Organismo.IEEC;
                    break;
                default:
                    aux = Organismo.VACIO;
                    break;
            }
            return aux;
        }
        #endregion METODOS
    }
    


    public class NombresCamposComiteTecnico
    {
        public static string EntityName = "aen_comitetecnicodenormalizacion";
        public static string EntityId = EntityName + "id";

        public static string Aen_OrganismoCRM = "aen_organismo";
        public static string Aen_Codigo_ComiteCRM = "aen_name";
        public static string Aen_Codigo_PadreCRM = "aen_comitepadre";
        public static string Aen_Nombre_ComiteCRM = "aen_nombrees";
        public static string Aen_Nombre_Comite_ENCRM = "aen_nombreen";



        public static string Aen_Comite_NuevoORACLE = "aen_comite_nuevo";
        public static string Aen_Fecha_ActualizacionORACLE = "aen_fecha_actualizacion";
        public static string Aen_OrganismoORACLE = "aen_organismo";
        public static string Aen_Codigo_ComiteORACLE = "aen_codigo_comite";
        public static string Aen_Codigo_PadreORACLE = "aen_comite_padre";
        public static string Aen_Nombre_ComiteORACLE = "aen_nombre_comite";
        public static string Aen_Nombre_Comite_ENORACLE = "aen_nombre_comite_en";
    }
}
