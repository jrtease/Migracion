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
    public class NormasProductos
    {
        #region PROPIEDADES
        public Guid Productoid { get; set; }
        public bool Aen_Vendible_Web { get; set; }
        public bool Aen_Producto_Nuevo { get; set; }
        public bool Aen_Documento_Mod { get; set; }
        public Guid Aen_IdiomaGUID { get; set; }
        public string Aen_Idioma { get; set; }
        public string Aen_Identificador_Nexo { get; set; }
        public string Aen_Fecha_Documento { get; set; }
        public string Aen_Fecha_Actualizacion { get; set; }
        public decimal Aen_Precio { get; set; }
        public Guid Aen_SoporteGUID { get; set; }
        public string Aen_Soporte { get; set; }
        public Guid Aen_OrganismoGUID { get; set; }
        public Guid Aen_ArticuloGUID { get; set; }
        public string Aen_Articulo { get; set; }
        public string Aen_Documento { get; set; }
        public string Aen_Path { get; set; }
        public string Aen_Url_Organismo { get; set; }
        public string Aen_Codigo_Producto { get; set; }   //PK 1
        public string Aen_Nombre_Producto { get; set; }
        #endregion PROPIEDADES


        #region METODOS
        public void NormasProductosFromOracle(DataRow fila, Dictionary<string, Guid> MaestroIdiomaCRM, Dictionary<string, Guid> MaestroSoporteCRM, Dictionary<string, Guid> MaestroVersionesCRM)
        {
            Productoid = Guid.Empty;
            Aen_Vendible_Web = fila[NombreCamposNormasProductos.Aen_Vendible_WebORACLE] == DBNull.Value ? false : (fila[NombreCamposNormasProductos.Aen_Vendible_WebORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Producto_Nuevo = fila[NombreCamposNormasProductos.Aen_Producto_NuevoORACLE] == DBNull.Value ? false : (fila[NombreCamposNormasProductos.Aen_Producto_NuevoORACLE].ToString().Trim().Equals("I") ? true : false);
            Aen_Documento_Mod = fila[NombreCamposNormasProductos.Aen_Documento_ModORACLE] == DBNull.Value ? false : (fila[NombreCamposNormasProductos.Aen_Documento_ModORACLE].ToString().Trim().Equals("S") ? true : false);
            Aen_Idioma = fila[NombreCamposNormasProductos.Aen_IdiomaORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_IdiomaORACLE].ToString();
            if (Aen_Idioma.Equals(string.Empty))
                Aen_IdiomaGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroIdiomaCRM.TryGetValue(Aen_Idioma, out aux);
                Aen_IdiomaGUID = aux;
            }
            Aen_Identificador_Nexo = fila[NombreCamposNormasProductos.Aen_Identificador_NexoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_Identificador_NexoORACLE].ToString();
            Aen_Fecha_Documento = fila[NombreCamposNormasProductos.Aen_Fecha_DocumentoORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombreCamposNormasProductos.Aen_Fecha_DocumentoORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Fecha_Actualizacion = fila[NombreCamposNormasProductos.Aen_Fecha_ActualizacionORACLE] == DBNull.Value ? string.Empty : ((DateTime)fila[NombreCamposNormasProductos.Aen_Fecha_ActualizacionORACLE]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            Aen_Precio = fila[NombreCamposNormasProductos.Aen_PrecioORACLE] == DBNull.Value ? decimal.MinValue : Convert.ToDecimal(fila[NombreCamposNormasProductos.Aen_PrecioORACLE]); ;
            Aen_Soporte = fila[NombreCamposNormasProductos.Aen_SoporteORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_SoporteORACLE].ToString();
            if (Aen_Soporte.Equals(string.Empty))
                Aen_SoporteGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroSoporteCRM.TryGetValue(Aen_Soporte, out aux);
                Aen_SoporteGUID = aux;
            }
            Aen_Articulo = fila[NombreCamposNormasProductos.Aen_ArticuloORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_ArticuloORACLE].ToString();
            if (Aen_Articulo.Equals(string.Empty))
                Aen_ArticuloGUID = Guid.Empty;
            else
            {
                Guid aux = Guid.Empty;
                MaestroVersionesCRM.TryGetValue(Aen_Articulo, out aux);
                Aen_ArticuloGUID = aux;
            }
            Aen_Documento = fila[NombreCamposNormasProductos.Aen_DocumentoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_DocumentoORACLE].ToString();
            Aen_Path = fila[NombreCamposNormasProductos.Aen_PathORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_PathORACLE].ToString();
            Aen_Url_Organismo = fila[NombreCamposNormasProductos.Aen_Url_OrganismoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_Url_OrganismoORACLE].ToString();
            Aen_Codigo_Producto = fila[NombreCamposNormasProductos.Aen_Codigo_ProductoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_Codigo_ProductoORACLE].ToString();
            Aen_Nombre_Producto = fila[NombreCamposNormasProductos.Aen_Nombre_ProductoORACLE] == DBNull.Value ? string.Empty : fila[NombreCamposNormasProductos.Aen_Nombre_ProductoORACLE].ToString();
        }

        public void NormasProductosFromCRM(Entity ent)
        {
            Productoid = ent.Id;
            Aen_Vendible_Web = ent.Contains(NombreCamposNormasProductos.Aen_Vendible_WebCRM) ? ent.GetAttributeValue<bool>(NombreCamposNormasProductos.Aen_Vendible_WebCRM) : false;
            Aen_Documento_Mod = ent.Contains(NombreCamposNormasProductos.Aen_Documento_ModCRM) ? ent.GetAttributeValue<bool>(NombreCamposNormasProductos.Aen_Documento_ModCRM) : false;
            Aen_IdiomaGUID = ent.Contains(NombreCamposNormasProductos.Aen_IdiomaCRM) ? ent.GetAttributeValue<EntityReference>(NombreCamposNormasProductos.Aen_IdiomaCRM).Id : Guid.Empty;
            Aen_Fecha_Documento = ent.Contains(NombreCamposNormasProductos.Aen_Fecha_DocumentoCRM) ? ent.GetAttributeValue<DateTime>(NombreCamposNormasProductos.Aen_Fecha_DocumentoCRM).ToLocalTime().ToString("dd-MM-yyyy".Trim()) : string.Empty;
            Aen_Precio = ent.Contains(NombreCamposNormasProductos.Aen_PrecioCRM) ? ent.GetAttributeValue<Money>(NombreCamposNormasProductos.Aen_PrecioCRM).Value : decimal.MinValue;
            Aen_SoporteGUID = ent.Contains(NombreCamposNormasProductos.Aen_SoporteCRM) ? ent.GetAttributeValue<EntityReference>(NombreCamposNormasProductos.Aen_SoporteCRM).Id : Guid.Empty;
            Aen_ArticuloGUID = ent.Contains(NombreCamposNormasProductos.Aen_ArticuloCRM) ? ent.GetAttributeValue<EntityReference>(NombreCamposNormasProductos.Aen_ArticuloCRM).Id : Guid.Empty;
            Aen_Documento = ent.Contains(NombreCamposNormasProductos.Aen_DocumentoCRM) ? ent.GetAttributeValue<string>(NombreCamposNormasProductos.Aen_DocumentoCRM) : string.Empty;
            Aen_Path = ent.Contains(NombreCamposNormasProductos.Aen_PathCRM) ? ent.GetAttributeValue<string>(NombreCamposNormasProductos.Aen_PathCRM) : string.Empty;
            Aen_Url_Organismo = ent.Contains(NombreCamposNormasProductos.Aen_Url_OrganismoCRM) ? ent.GetAttributeValue<string>(NombreCamposNormasProductos.Aen_Url_OrganismoCRM) : string.Empty;
            Aen_Codigo_Producto = ent.Contains(NombreCamposNormasProductos.Aen_Codigo_ProductoCRM) ? ent.GetAttributeValue<string>(NombreCamposNormasProductos.Aen_Codigo_ProductoCRM) : string.Empty;
            Aen_Nombre_Producto = ent.Contains(NombreCamposNormasProductos.Aen_Nombre_ProductoCRM) ? ent.GetAttributeValue<string>(NombreCamposNormasProductos.Aen_Nombre_ProductoCRM) : string.Empty;
        }

        public Entity GetEntity(Guid TipoNorma, Guid UomID, Guid SUomID)
        {
            Entity e = new Entity(NombreCamposNormasProductos.EntityName);
            if (Productoid != Guid.Empty)
            {
                e.Id = Productoid;
                e[NombreCamposNormasProductos.EntityId] = Productoid;
            }
            e[NombreCamposNormasProductos.Aen_Vendible_WebCRM] = Aen_Vendible_Web;
            e[NombreCamposNormasProductos.Aen_Documento_ModCRM] = Aen_Documento_Mod;
            e[NombreCamposNormasProductos.Aen_IdiomaCRM] = (Aen_IdiomaGUID.Equals(Guid.Empty)) ? null : new EntityReference("aen_idioma", Aen_IdiomaGUID);
            if (!Aen_Fecha_Documento.Equals(string.Empty))
            {
                var okFecha = DateTime.TryParse(Aen_Fecha_Documento, out DateTime f);
                if (okFecha)
                    e[NombreCamposNormasProductos.Aen_Fecha_DocumentoCRM] = f;
            }
            if (!Aen_Precio.Equals(decimal.MinValue))
            {
                Money reven = new Money(Convert.ToDecimal(Aen_Precio));
                e[NombreCamposNormasProductos.Aen_PrecioCRM] = reven;
            }
            else
            {
                //Money reven = new Money();
                e[NombreCamposNormasProductos.Aen_PrecioCRM] = null;
            }
            e[NombreCamposNormasProductos.Aen_SoporteCRM] = (Aen_SoporteGUID.Equals(Guid.Empty)) ? null : new EntityReference("aen_formato", Aen_SoporteGUID);
            e[NombreCamposNormasProductos.Aen_ArticuloCRM] = (Aen_ArticuloGUID.Equals(Guid.Empty)) ? null : new EntityReference(NombreCamposNormas.EntityNameVersion, Aen_ArticuloGUID);
            e[NombreCamposNormasProductos.Aen_DocumentoCRM] = Aen_Documento.Equals(string.Empty) ? string.Empty : Aen_Documento;
            e[NombreCamposNormasProductos.Aen_PathCRM] = Aen_Path.Equals(string.Empty) ? string.Empty : Aen_Path;
            e[NombreCamposNormasProductos.Aen_Url_OrganismoCRM] = Aen_Url_Organismo.Equals(string.Empty) ? string.Empty : Aen_Url_Organismo;
            e[NombreCamposNormasProductos.Aen_Codigo_ProductoCRM] = Aen_Codigo_Producto.Equals(string.Empty) ? string.Empty : Aen_Codigo_Producto;
            e[NombreCamposNormasProductos.Aen_Nombre_ProductoCRM] = Aen_Nombre_Producto.Equals(string.Empty) ? string.Empty : Aen_Nombre_Producto;
            e["defaultuomid"] = new EntityReference("uom", UomID);
            e["defaultuomscheduleid"] = new EntityReference("uomschedule", SUomID);
            e["aen_tipodeproducto"] = new EntityReference("aen_tipodeproducto", TipoNorma);
            e["aen_integracion"] = true;
            e["quantitydecimal"] = 2;

            return e;
        }

        public bool NormasProductosIguales(NormasProductos auxNPCRM, ref Entity npUpdate, Guid Tiponorma, Guid UOM, Guid SUOM)
        {
            bool res = false;

            if (!Aen_Vendible_Web.Equals(auxNPCRM.Aen_Vendible_Web))
                res = true;
            if (!Aen_Documento_Mod.Equals(auxNPCRM.Aen_Documento_Mod))
                res = true;
            if (!Aen_IdiomaGUID.Equals(auxNPCRM.Aen_IdiomaGUID))
                res = true;
            if (!(Aen_Precio.Equals(decimal.MinValue) && auxNPCRM.Aen_Precio.Equals(decimal.MinValue))
                && (!Aen_Precio.ToString("#,##").Equals(auxNPCRM.Aen_Precio.ToString("#,##"))))
                res = true;
            if (!Aen_SoporteGUID.Equals(auxNPCRM.Aen_SoporteGUID))
                res = true;
            if (!Aen_ArticuloGUID.Equals(auxNPCRM.Aen_ArticuloGUID))
                res = true;
            if (!Aen_Documento.Equals(auxNPCRM.Aen_Documento))
                res = true;
            if (!Aen_Path.Equals(auxNPCRM.Aen_Path))
                res = true;
            if (!Aen_Url_Organismo.Equals(auxNPCRM.Aen_Url_Organismo))
                res = true;
            if (!Aen_Codigo_Producto.Equals(auxNPCRM.Aen_Codigo_Producto))
                res = true;
            if (!Aen_Nombre_Producto.Equals(auxNPCRM.Aen_Nombre_Producto))
                res = true;

            if (res)
            {
                Productoid = auxNPCRM.Productoid;
                npUpdate = GetEntity(Tiponorma, UOM, SUOM);
            }   

            return res;
        }
        #endregion METODOS
    }


    public class NombreCamposNormasProductos
    {
        public static string EntityName = "product";
        public static string EntityId = EntityName + "id";

        public static string Aen_Vendible_WebCRM = "aen_vendibleweb";
        public static string Aen_Documento_ModCRM = "aen_documentomodificado";
        public static string Aen_IdiomaCRM = "aen_idioma";
        public static string Aen_Fecha_DocumentoCRM = "overriddencreatedon";
        public static string Aen_PrecioCRM = "price";
        public static string Aen_SoporteCRM = "aen_formato";
        public static string Aen_ArticuloCRM = "aen_version";
        //public static string Aen_Codigo_NormaCRM = "aen_norma";
        public static string Aen_DocumentoCRM = "aen_documento";
        public static string Aen_PathCRM = "aen_ruta";
        public static string Aen_Url_OrganismoCRM = "producturl";
        public static string Aen_Codigo_ProductoCRM = "productnumber";
        public static string Aen_Nombre_ProductoCRM = "name";



        public static string Aen_Vendible_WebORACLE = "aen_vendible_web";
        public static string Aen_Producto_NuevoORACLE = "aen_producto_nuevo";
        public static string Aen_Documento_ModORACLE = "aen_documento_mod";
        public static string Aen_IdiomaORACLE = "aen_idioma";
        public static string Aen_Identificador_NexoORACLE = "aen_identificador_nexo";
        public static string Aen_Fecha_DocumentoORACLE = "aen_fecha_documento";
        public static string Aen_Fecha_ActualizacionORACLE = "aen_fecha_actualizacion";
        public static string Aen_PrecioORACLE = "aen_precio";
        public static string Aen_SoporteORACLE = "aen_soporte";
        public static string Aen_ArticuloORACLE = "aen_articulo";
        //public static string Aen_Codigo_NormaORACLE = "aen_codigo_norma";
        public static string Aen_DocumentoORACLE = "aen_documento";
        public static string Aen_PathORACLE = "aen_path";
        public static string Aen_Url_OrganismoORACLE = "aen_url_organismo";
        public static string Aen_Codigo_ProductoORACLE = "aen_codigo_producto";
        public static string Aen_Nombre_ProductoORACLE = "aen_nombre_producto";
    }
}
