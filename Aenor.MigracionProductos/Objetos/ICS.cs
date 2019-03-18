using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Aenor.MigracionProductos.Objetos
{
    public class ICS
    {
        #region PROPIEDADES
        public Guid Aen_ICSGUID { get; set; }
        public bool Aen_Ics_Activo { get; set; }
        public bool Aen_Ics_Nuevo { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public string Aen_Codigo_Ics { get; set; }   //PK 1
        public string Aen_Codigo_Ics_PadreSTR { get; set; }
        public Guid Aen_Codigo_Ics_Padre { get; set; }
        public string Aen_Descripcion_Ics { get; set; }
        public string Aen_Descripcion_IcsEN { get; set; }
        #endregion PROPIEDADES



        #region METODOS
        public void ICSFromOracle(DataRow fila, Dictionary<string, Guid> DicICS)
        {
            Aen_Ics_Activo = fila[NombresCamposICS.Aen_Ics_ActivoORACLE] == DBNull.Value ? false : (fila[NombresCamposICS.Aen_Ics_ActivoORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Ics_Nuevo = fila[NombresCamposICS.Aen_Ics_NuevoORACLE] == DBNull.Value ? false : (fila[NombresCamposICS.Aen_Ics_NuevoORACLE].ToString().Trim().Equals("I") ? true : false);
            Aen_Fecha_Actualizacion = fila[NombresCamposICS.Aen_Fecha_ActualizacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombresCamposICS.Aen_Fecha_ActualizacionORACLE]).ToString("dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            Aen_Codigo_Ics = fila[NombresCamposICS.Aen_Codigo_IcsORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposICS.Aen_Codigo_IcsORACLE].ToString().Trim();
            Aen_Descripcion_Ics = fila[NombresCamposICS.Aen_Descripcion_IcsORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposICS.Aen_Descripcion_IcsORACLE].ToString().Trim();
            Aen_Descripcion_IcsEN = fila[NombresCamposICS.Aen_Descripcion_IcsENORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposICS.Aen_Descripcion_IcsENORACLE].ToString().Trim();
            Aen_Codigo_Ics_PadreSTR = fila[NombresCamposICS.Aen_Codigo_Ics_PadreORACLE] == DBNull.Value ? string.Empty : fila[NombresCamposICS.Aen_Codigo_Ics_PadreORACLE].ToString().Trim();
            if (Aen_Codigo_Ics_PadreSTR.Equals(string.Empty))
                Aen_Codigo_Ics_Padre = Guid.Empty;
            else
            {
                Guid asigna = Guid.Empty;
                bool ok = DicICS.TryGetValue(Aen_Codigo_Ics_PadreSTR, out asigna);
                Aen_Codigo_Ics_Padre = asigna;
            }
        }

        public void ICSFromCRM(Entity icCRM)
        {
            Aen_ICSGUID = icCRM.Id;
            Aen_Codigo_Ics = icCRM.Contains(NombresCamposICS.Aen_Codigo_IcsCRM) ? icCRM.GetAttributeValue<string>(NombresCamposICS.Aen_Codigo_IcsCRM) : string.Empty;
            Aen_Codigo_Ics_Padre = icCRM.Contains(NombresCamposICS.Aen_Codigo_Ics_PadreCRM) ? icCRM.GetAttributeValue<EntityReference>(NombresCamposICS.Aen_Codigo_Ics_PadreCRM).Id : Guid.Empty;
            Aen_Descripcion_Ics = icCRM.Contains(NombresCamposICS.Aen_Descripcion_IcsCRM) ? icCRM.GetAttributeValue<string>(NombresCamposICS.Aen_Descripcion_IcsCRM) : string.Empty;
            Aen_Descripcion_IcsEN = icCRM.Contains(NombresCamposICS.Aen_Descripcion_IcsENCRM) ? icCRM.GetAttributeValue<string>(NombresCamposICS.Aen_Descripcion_IcsENCRM) : string.Empty;
            Aen_Ics_Activo = icCRM.Contains(NombresCamposICS.Aen_Ics_ActivoCRM) ? (icCRM.GetAttributeValue<OptionSetValue>(NombresCamposICS.Aen_Ics_ActivoCRM).Value == 0 ? true : false): false;
        }

        public Entity GetEntity()
        {
            Entity i = new Entity(NombresCamposICS.EntityName);
            if (!Aen_ICSGUID.Equals(Guid.Empty))
            {
                i.Id = Aen_ICSGUID;
                i[NombresCamposICS.EntityId] = Aen_ICSGUID;
            }
            if (!Aen_Codigo_Ics.Equals(string.Empty)) i[NombresCamposICS.Aen_Codigo_IcsCRM] = Aen_Codigo_Ics;
            if (!Aen_Descripcion_Ics.Equals(string.Empty)) i[NombresCamposICS.Aen_Descripcion_IcsCRM] = Aen_Descripcion_Ics;
            if (!Aen_Descripcion_IcsEN.Equals(string.Empty)) i[NombresCamposICS.Aen_Descripcion_IcsENCRM] = Aen_Descripcion_IcsEN;
            i[NombresCamposICS.Aen_Codigo_Ics_PadreCRM] = !Aen_Codigo_Ics_Padre.Equals(Guid.Empty) ? new EntityReference(NombresCamposICS.EntityName, Aen_Codigo_Ics_Padre) : null;

            if (Aen_Ics_Activo)
            {
                i[NombresCamposICS.Aen_Ics_ActivoCRM] = new OptionSetValue(0);
                i["statuscode"] = new OptionSetValue(1);
            }
            else
            {
                i[NombresCamposICS.Aen_Ics_ActivoCRM] = new OptionSetValue(1);
                //i["statuscode"] = new OptionSetValue(2);
            }

            return i;
        }

        public bool ICSIguales(ICS auxICSCRM, ref Entity icsUpdate)
        {
            bool res = false;

            if (!Aen_Codigo_Ics.Equals(auxICSCRM.Aen_Codigo_Ics))
                res = true;
            if (!Aen_Codigo_Ics_Padre.Equals(auxICSCRM.Aen_Codigo_Ics_Padre))
                res = true;
            if (!Aen_Descripcion_Ics.Equals(auxICSCRM.Aen_Descripcion_Ics))
                res = true;
            if (!Aen_Descripcion_IcsEN.Equals(auxICSCRM.Aen_Descripcion_IcsEN))
                res = true;
            if (!Aen_Ics_Activo.Equals(auxICSCRM.Aen_Ics_Activo))
                res = true;

            if (res)
            {
                Aen_ICSGUID = auxICSCRM.Aen_ICSGUID;
                icsUpdate = GetEntity();
            }

            return res;
        }
        #endregion METODOS
    }


    public class NombresCamposICS
    {
        public static string EntityName = "aen_ics";
        public static string EntityId = EntityName + "id";

        public static string Aen_Codigo_IcsCRM = "aen_name";
        public static string Aen_Codigo_Ics_PadreCRM = "aen_icspadre";
        public static string Aen_Descripcion_IcsCRM = "aen_descripciones";
        public static string Aen_Descripcion_IcsENCRM = "aen_descripcionen";
        public static string Aen_Ics_ActivoCRM = "statecode";


        public static string Aen_Ics_ActivoORACLE = "aen_ics_activo";
        public static string Aen_Ics_NuevoORACLE = "aen_ics_nuevo";
        public static string Aen_Fecha_ActualizacionORACLE = "aen_fecha_actualizacion";
        public static string Aen_Codigo_IcsORACLE = "aen_codigo_ics";
        public static string Aen_Codigo_Ics_PadreORACLE = "aen_codigo_ics_padre";
        public static string Aen_Descripcion_IcsORACLE = "aen_descripcion_ics";
        public static string Aen_Descripcion_IcsENORACLE = "aen_descripcion_ics_en";
    }
}
