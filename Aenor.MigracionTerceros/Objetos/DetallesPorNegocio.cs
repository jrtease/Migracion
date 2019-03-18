using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionTerceros.Objetos
{
    class DetallesPorNegocio
    {
    }

    #region TERCEROS ES
    public class TerceroEs
    {
        public Guid Accountid { get; set; }
        public string Aen_claveintegracion { get; set; }
        public bool Aen_evaluaciondelaconformidad { get; set; } //este es certificación
        //Estos son los siguientes a hacer, los que no estén hay que crearlos en tercero, su entidad relacionada y por cada una hay que hacer su integración.
        //Si viene a true, se crea el registro en la entidad relacionda, y si viene a false, se elimina (no se inactiva)
        public bool Aen_espotencialcliente { get; set; }
        public bool Aen_essuscriptor { get; set; }
        public bool Aen_escompradornormas { get; set; }
        public bool Aen_escompradorlibros { get; set; }
        public bool Aen_esmiembroctc { get; set; }
        //public bool Aen_esmiembroune { get; set; }
        //public bool Aen_esorganismo { get; set; }
        public bool Aen_revistaaenor { get; set; }
    }


    public class NombresCamposTerceroEs
    {
        public static string AccountidCRM = "accountid";

        public static string Aen_claveintegracionCRM = "aen_claveintegracion";
        public static string Aen_evaluaciondelaconformidadCRM = "aen_evaluaciondelaconformidad"; //este es certificación
        public static string Aen_espotencialclienteCRM = "aen_espotencialcliente";
        public static string Aen_essuscriptorCRM = "aen_essuscriptor";
        public static string Aen_escompradornormasCRM = "aen_escompradordenormas";
        public static string Aen_escompradorlibrosCRM = "aen_escompradordelibros";
        public static string Aen_esmiembroctcCRM = "aen_esmiembroctc";
        //public static string Aen_esmiembrouneCRM = "aen_esmiembroune";
        //public static string Aen_esorganismoCRM = "aen_esorganismo";
        public static string Aen_revistaaenorCRM = "aen_revistaaenor";


        public static string Aen_claveintegracionORACLE = "aen_claveintegracion";
        public static string Aen_evaluaciondelaconformidadORACLE = "aen_evalconformidad"; // "aen_evaluaciondelaconformidad"; //este es certificación
        public static string Aen_espotencialclienteORACLE = "aen_espotencialcliente";
        public static string Aen_essuscriptorORACLE = "aen_essuscriptor";
        public static string Aen_escompradornormasORACLE = "aen_escompradordenormas";
        public static string Aen_escompradorlibrosORACLE = "aen_escompradordelibros";
        public static string Aen_esmiembroctcORACLE = "aen_esmiembroctc";
        //public static string Aen_esmiembrouneORACLE = "aen_esmiembroune";
        //public static string Aen_esorganismoORACLE = "aen_esorganismo";
        public static string Aen_revistaaenorORACLE = "aen_revistaaenor";
    }
    #endregion TERCEROS ES



    #region EVALUACION DE CONFORMIDAD
    public class EvaluacionConformidad //ES_EMPRESA_CERTIFICADA
    {
        public static string EntityName = "aen_certificacion";

        public Guid CertificacionId { get; set; } //GUID DE CRM

        public string ClaveTercero { get; set; } //aen_terceroid --> account2.aen_claveintegracion
        public Guid IdTercero { get; set; }
        public string ClaveCertificado { get; set; }
        public string NormaCertificadaCTC { get; set; }
        public string SubNormaSPC { get; set; }
        public string Subexpediente { get; set; }
        public string IdSubexpediente { get; set; }
        public string CodigoCertificado { get; set; } // = Codigo y Name del registro
        public string Estado { get; set; }
        public string FechaEstado { get; set; }
    }


    public class NombresCamposEvaluacionConformidad
    {
        public static string CertificacionIdCRM = "aen_certificacionid"; //GUID DE CRM

        public static string NormaCertificadaCTCCRM = "aen_normacertificadactc";
        public static string SubNormaSPCCRM = "aen_subnormaspc";
        public static string IdSubexpedienteCRM = "aen_idsubexpediente";
        public static string SubexpedienteCRM = "aen_subexpediente";
        public static string EstadoDelCertificadoCRM = "aen_estadodelcertificado";
        public static string FechaEstadoCRM = "aen_fechadeestado";
        public static string CodigoCertificadoCRM = "aen_name";
        public static string ClaveTerceroCRM = "aen_claveintegracion";
        public static string ClaveCertificadoCRM = "aen_idcertificado";
        public static string TerceroIdCRM = "aen_terceroid";


        public static string NormaCertificadaCTCORACLE = "aen_normacertificadactc";
        public static string SubNormaSPCORACLE = "aen_subnormaspc";
        public static string IdSubexpedienteORACLE = "aen_idsubexpediente";
        public static string SubexpedienteORACLE = "aen_subexpediente";
        public static string EstadoORACLE = "aen_estado";
        public static string FechaEstadoORACLE = "aen_fechadeestado";
        public static string CodigoCertificadoORACLE = "aen_codigodecertificado";
        public static string ClaveTerceroORACLE = "aen_claveintegracion";
        public static string ClaveCertificadoORACLE = "aen_clavecertificado";
    }
    #endregion EVALUACION DE CONFORMIDAD



    #region COMPRADOR NORMAS
    public class CompradorNormas
    {
        public static string EntityName = "aen_normacomprada";
        public Guid CompradorNormasId { get; set; }

        public string Aen_ClaveNorma { get; set; }
        public string Aen_CodigoCTN { get; set; }
        public string Aen_DescripcionCTN { get; set; }
        public string Aen_Codigoarticulo { get; set; }
        public string Aen_Descripcionarticulo { get; set; }
        public string Aen_Titulo { get; set; }
        public int Aen_Cantidad { get; set; }
        public decimal Aen_Importe { get; set; }
        public string ClaveTercero { get; set; }
        public Guid IdTercero { get; set; }
    }

    public class NombresCamposCompradorNormas
    {
        public static string CompradorNormasId = "aen_normacompradaid";

        public static string Aen_ClaveNormaCRM = "aen_clavenorma";
        public static string Aen_CodigoCTNCRM = "aen_codigoctn";
        public static string Aen_DescripcionCTNCRM = "aen_descripcionctn";
        public static string Aen_CodigoarticuloCRM = "aen_codigoarticulo";
        public static string Aen_DescripcionarticuloCRM = "aen_name";
        public static string Aen_TituloCRM = "aen_titulo";
        public static string Aen_CantidadCRM = "aen_cantidad";
        public static string Aen_ImporteCRM = "aen_importe";
        public static string ClaveTerceroCRM = "aen_claveintegracion";
        public static string IdTerceroCRM = "aen_terceroid";

        public static string Aen_ClaveNormaORACLE = "aen_clavenorma";
        public static string Aen_CodigoCTNORACLE = "aen_codigoctn";
        public static string Aen_DescripcionCTNORACLE = "aen_descripcionctn";
        public static string Aen_CodigoarticuloORACLE = "aen_codigoarticulo";
        public static string Aen_DescripcionarticuloORACLE = "aen_descripcionarticulo";
        public static string Aen_TituloORACLE = "aen_titulo";
        public static string Aen_CantidadORACLE = "aen_cantidad";
        public static string Aen_ImporteORACLE = "aen_importe";
        public static string ClaveTerceroORACLE = "aen_claveintegracion";
    }
    #endregion COMPRADOR NORMAS



    #region COMPRADOR LIBROS
    public class CompradorLibros
    {
        public static string EntityName = "aen_publicacionesadquiridas";
        public Guid CompradorLibrosId { get; set; } //GUID CRM

        public string Aen_ClaveLibros { get; set; }
        public string Aen_Codigoarticulo { get; set; }
        public string Aen_Descripcionarticulo { get; set; }
        public string Aen_Titulo { get; set; }
        public int Aen_Cantidad { get; set; }
        public decimal Aen_Importe { get; set; }
        public string ClaveTercero { get; set; }
        public Guid IdTercero { get; set; }
    }

    public class NombresCamposCompradorLibros
    {
        public static string CompradorLibrosId = "aen_publicacionesadquiridasid";

        public static string Aen_ClaveLibrosCRM = "aen_clavelibros";
        public static string Aen_CodigoarticuloCRM = "aen_codigoarticulo";
        public static string Aen_DescripcionarticuloCRM = "aen_descripcionarticulo";
        public static string Aen_TituloCRM = "aen_name"; // "aen_titulo";
        public static string Aen_CantidadCRM = "aen_cantidad";
        public static string Aen_ImporteCRM = "aen_importe";
        public static string ClaveTerceroCRM = "aen_claveintegracion";
        public static string IdTerceroCRM = "aen_tercero";

        public static string Aen_ClaveLibrosORACLE = "aen_clavelibros";
        public static string Aen_CodigoarticuloORACLE = "aen_codigoarticulo";
        public static string Aen_DescripcionarticuloORACLE = "aen_descripcionarticulo";
        public static string Aen_TituloORACLE = "aen_titulo";
        public static string Aen_CantidadORACLE = "aen_cantidad";
        public static string Aen_ImporteORACLE = "aen_importe";
        public static string ClaveTerceroORACLE = "aen_claveintegracion";
    }
    #endregion COMPRADOR LIBROS



    #region CLIENTE POTENCIAL WEB
    public class CliPotencialWeb
    {
        public static string EntityName = "aen_potencialcliente"; // "aen_clientepotencial";
        public Guid CliPotencialWebId { get; set; } //GUID CRM

        public string Aen_ClavePotencial { get; set; }
        public string Aen_Pedido { get; set; }
        public string Aen_Titulo { get; set; }
        public string Aen_Fechaemision { get; set; }
        public string Aen_Observaciones { get; set; }
        public string Aen_Email { get; set; }
        public string Aen_Entradadelcliente { get; set; }
        public string ClaveTercero { get; set; }
        public Guid IdTercero { get; set; }
    }

    public class NombresCamposCliPotencialWeb 
    {
        public static string CliPotencialWebId = "aen_potencialclienteid"; // "aen_clientepotencialid";

        public static string Aen_ClavePotencialCRM = "aen_clavepotencial"; 
        public static string Aen_PedidoCRM = "aen_pedido";
        public static string Aen_ObservacionesCRM = "aen_obsevaciones"; //"aen_observaciones", el bueno es sin la r
        public static string Aen_EmailCRM = "aen_email";
        public static string Aen_TituloCRM = "aen_titulo";
        public static string Aen_FechaemisionCRM = "aen_fechadeemision";
        public static string Aen_EntradadelclienteCRM = "aen_entradadelcliente";
        public static string ClaveTerceroCRM = "aen_claveintegracion";
        public static string IdTerceroCRM = "aen_terceroid";

        public static string Aen_ClavePotencialORACLE = "aen_clavepotencial";
        public static string Aen_PedidoORACLE = "aen_codigopedido";
        public static string Aen_ObservacionesORACLE = "aen_observaciones";
        public static string Aen_EmailORACLE = "aen_email";
        public static string Aen_TituloORACLE = "aen_titulo";
        public static string Aen_FechaemisionORACLE = "aen_fechaemision";
        public static string Aen_EntradadelclienteORACLE = "aen_entradadelcliente";
        public static string ClaveTerceroORACLE = "aen_claveintegracion";
    }
    #endregion CLIENTE POTENCIAL WEB



    #region SUSCRIPTORES
    public class Suscriptor
    {
        public static string EntityName = "aen_suscripcionadquirida";
        public Guid SuscriptorId { get; set; } //GUID CRM

        public string Aen_ClaveSuscriptor { get; set; }
        public string ClaveTercero { get; set; }
        public Guid IdTercero { get; set; }
        public string Aen_Producto { get; set; }
        public string Aen_Situacion { get; set; }
        public string Aen_Fechafinsuscripcion { get; set; }
        public string Aen_Fechabajasuscripcion { get; set; }
    }

    public class NombresCamposSuscriptor
    {
        public static string SuscriptorID = "aen_suscripcionid";

        public static string Aen_ClaveSuscriptorCRM = "aen_clavesuscriptor";
        public static string ClaveTerceroCRM = "aen_claveintegracion";
        public static string IdTerceroCRM = "aen_tercero";
        public static string Aen_ProductoCRM = "aen_name";
        public static string Aen_SituacionCRM = "aen_situacion";
        public static string Aen_FechafinsuscripcionCRM = "aen_fechafinsuscripcion";
        public static string Aen_FechabajasuscripcionCRM = "aen_fechabajasuscripcion";

        public static string Aen_ClaveSuscriptorORACLE = "aen_clavesuscriptor";
        public static string ClaveTerceroORACLE = "aen_claveintegracion";
        public static string IdTerceroORACLE = "aen_terceroid";
        public static string Aen_ProductoORACLE = "aen_producto";
        public static string Aen_SituacionORACLE = "aen_situacion";
        public static string Aen_FechafinsuscripcionORACLE = "aen_fechafinsuscripcion";
        public static string Aen_FechabajasuscripcionORACLE = "aen_fechabajasuscripcion";
    }
    #endregion SUSCRIPTORES
}
