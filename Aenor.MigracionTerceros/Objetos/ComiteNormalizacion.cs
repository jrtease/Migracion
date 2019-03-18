using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Objetos
{
    public class ComiteNormalizacion
    {
        public Guid Aen_ComiteNormalizacionId { get; set; }
        public Guid Aen_ComitePadreId { get; set; }
        public Guid Aen_IndustriaAenorId { get; set; }
        //public string Aen_Name { get; set; }    //PK
        public string Aen_NameEs { get; set; }
        public string Aen_NameEn { get; set; }
        public string Aen_Codigo { get; set; }
        public Guid Aen_SectorAenorId { get; set; }
        public string Aen_TipoComite { get; set; } //ICS (a tabla ICS), UNE, ISO (a tabla comite normalizacion)
        public string StateCode { get; set; }
    }


    public class NombresCamposComite
    {
        public static string NombreEntidad = "aen_comitenormalizacion";


        //TODO COMPLETAR NOMBRE DE LOS CAMPOS
        #region CRM
        public static string Aen_ComiteNormalizacionIdCRM = "aen_comitenormalizacionid";
        public static string Aen_ComitePadreIdCRM = "aen_comitepadre";
        public static string Aen_IndustriaAenorIdCRM = "aen_industriaaenorid";
        public static string Aen_NameEsCRM = "aen_namees";
        public static string Aen_NameEnCRM = "aen_nameen";
        public static string Aen_CodigoCRM = "aen_name";
        public static string Aen_SectorAenorIdCRM = "aen_sectoraenorid";
        // public static string Aen_TipoComiteCRM = "";
        public static string StateCodeCRM = "statecode";
        public static string StatusCodeCRM = "statuscode";
        #endregion CRM



        #region Oracle
        public static string Aen_ComiteNormalizacionIdORACLE = "";
        public static string Aen_ComitePadreIdORACLE = "aen_comite_padre";
        //public static string Aen_IndustriaAenorIdORACLE = "";
        public static string Aen_NameEsORACLE = "aen_nombre_comite";
        public static string Aen_NameEnORACLE = "aen_nombre_comite_en";
        public static string Aen_CodigoORACLE = "aen_codigo_comite";
        //public static string Aen_SectorAenorIdORACLE = "";
        //public static string Aen_TipoComiteORACLE = ""; 
        //public static string StateCodeORACLE = "";
        #endregion Oracle
    }

}
