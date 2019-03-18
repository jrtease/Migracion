using Aenor.MigracionTerceros.Clases;
using Aenor.MigracionTerceros.Objetos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aenor.MigracionTerceros.Clases
{
    public class LanzadorDirecciones
    {
        #region Propiedades
        public Crm Crm { get; set; }
        public Comun Comun { get; set; }
        public Oracle Oracle { get; set; }
        public Dictionary<string, Direccion> DireccionesOracle, DireccionesCRM;
        public Dictionary<string, Guid> Paises, Provincias, TipoDeVia, Terceros, Contactos;
        public Dictionary<Guid, string> ProvinciasNombres;
        public HashSet<string> DireccionesDesactivar;

        public string ClaveIntegracionActual = "";
        #endregion Propiedades


        public void Iniciar(Oracle oracleGlobal, Comun comunGlobal, Crm crmGlobal)
        {  
            try
            {
                //Aquí no hay que hacer new, el lanzador orquestador lo pasará a las properties
                Comun = comunGlobal;
                Comun.LogText("----Iniciando sincronización direcciones------");
                Crm = crmGlobal;
                DireccionesDesactivar = new HashSet<string>();

                Oracle = oracleGlobal;
                Crm.InicializarPFE(oracleGlobal);

                Stopwatch sW_Dir = new Stopwatch(), sW_SoloCarga = new Stopwatch();
                sW_Dir.Start();

                //BorrarDirecciones();

                LeerEntidades();
                LeerDireccionesOracle();
                if (!DireccionesOracle.Any())
                {
                    Comun.LogText("No hay direcciones en origen, terminamos");
                    return;
                }


                sW_SoloCarga.Start();
                foreach (var dirOra in DireccionesOracle)
                {
                    try
                    {
                        ClaveIntegracionActual = dirOra.Key;
                        bool ok = DireccionesCRM.TryGetValue(dirOra.Key, out Direccion dirCRM);
                        if (ok && DireccionesIguales(dirOra.Value, dirCRM))
                        {
                            Crm.Iguales++;
                            continue;
                        }
                        var dir = GetEntity(dirOra.Value);
                        if (ok)
                        {
                            dir["aen_direccionid"] = dirCRM.Aen_DireccionId;
                            Crm.AnadirElementoEmr(new UpdateRequest { Target = dir });
                        }
                        else
                        {
                            if (dirOra.Value.StateCode == "Inactivo")
                            {
                                dir["statecode"] = new OptionSetValue(0);
                                dir["statuscode"] = new OptionSetValue(1);
                                DireccionesDesactivar.Add(dirOra.Value.Aen_ClaveIntegracion);
                            }
                            Crm.AnadirElementoEmr(new CreateRequest { Target = dir });
                        }

                    }
                    catch (Exception e)
                    {
                        Comun.EscribirExcepcion(e, "Error al procesar Direccion: " + dirOra.Key);
                    }
                }   
                Crm.ProcesarUltimosEmr();
                sW_SoloCarga.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO SOLO CARGA: " + sW_SoloCarga.Elapsed.ToString() + " <-----\n\n");
                DesactivarRecienCreadasInactivas();
                sW_Dir.Stop();
                comunGlobal.LogText(" -----> END; TIEMPO TOTAL: " + sW_Dir.Elapsed.ToString() + " <-----\n\n");
                Crm.MostrarEstadisticas("DIRECCIONES");
            }
            catch (Exception ex)
            {
                //Aquí no podemos mandar clave al log de Oracle, puesto que no hay clave integración
                //Si hay error aquí es por motivo más genérico
                Comun.EscribirExcepcion(ex, "Error");
                if (ClaveIntegracionActual != "")
                    Oracle.MandarErrorIntegracion(ClaveIntegracionActual, ex.Message,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                if (Oracle != null && Oracle.OraConnParaLog.State == ConnectionState.Open)
                    Oracle.OraConnParaLog.Dispose();
            }
            finally
            {
                if (Oracle != null && Oracle.OraConnParaLog.State == ConnectionState.Open)
                    Oracle.OraConnParaLog.Dispose();
            }
        }

        private void DesactivarRecienCreadasInactivas()
        {
            var q = new QueryExpression("aen_direccion");
            q.ColumnSet = new ColumnSet("aen_claveintegracion");
            q.Criteria.AddCondition(
                new ConditionExpression("aen_claveintegracion", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                {
                    var claveInt = (string)e["aen_claveintegracion"];
                    if (DireccionesDesactivar.Contains(claveInt))
                    {
                        var dir = new Entity("aen_direccion");
                        dir["aen_direccionid"] = e.Id;
                        dir["statecode"] = new OptionSetValue(1);
                        dir["statuscode"] = new OptionSetValue(2);
                        Crm.AnadirElementoEmr(new UpdateRequest { Target = dir });
                    }
                }
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Crm.ProcesarUltimosEmr();
        }

        #region Funciones Lectura
        public void LeerEntidades()
        {
            Terceros = new Dictionary<string, Guid>();
            Contactos = new Dictionary<string, Guid>();
            Paises = new Dictionary<string, Guid>();
            Provincias = new Dictionary<string, Guid>();
            ProvinciasNombres = new Dictionary<Guid, string>();
            TipoDeVia = new Dictionary<string, Guid>();
            DireccionesCRM = new Dictionary<string, Direccion>();
            Comun.LogText("Leyendo entidades de CRM...");

            //var q = new QueryExpression("aen_comunidadautonoma");
            //q.ColumnSet = new ColumnSet("aen_name");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_name", ConditionOperator.NotNull));
            //var ents = Crm.IOS.RetrieveMultiple(q);
            var q = new QueryExpression("aen_pais");
            q.ColumnSet = new ColumnSet("aen_codigopais");
            q.Criteria.Conditions.Add(new ConditionExpression("aen_codigopais", ConditionOperator.NotNull));
            var ents = Crm.IOS.RetrieveMultiple(q);
            foreach (var e in ents.Entities)
                Paises.Add((string)e["aen_codigopais"], e.Id);
            Comun.LogText("Cacheados países: " + Paises.Count);
            q = new QueryExpression("aen_provincia");
            q.ColumnSet = new ColumnSet("aen_codigoprovincia", "aen_name");
            q.Criteria.Conditions.Add(new ConditionExpression("aen_codigoprovincia", ConditionOperator.NotNull));
            ents = Crm.IOS.RetrieveMultiple(q);
            //Parallel.ForEach(ents.Entities, e => Provincias.Add((string)e["aen_codigoprovincia"], e.Id));
            foreach (var e in ents.Entities)
            {
                Provincias.Add((string)e["aen_codigoprovincia"], e.Id);
                ProvinciasNombres.Add(e.Id, e.Contains("aen_name") ? (string)e["aen_name"] : null);
            }
            Comun.LogText("Cacheados provincias: " + Provincias.Count);

            //q = new QueryExpression("aen_localidad");
            //q.ColumnSet = new ColumnSet("aen_name");
            //q.Criteria.Conditions.Add(new ConditionExpression("aen_name", ConditionOperator.NotNull));
            //ents = Crm.IOS.RetrieveMultiple(q);
            q = new QueryExpression("aen_tipodevia");
            q.ColumnSet = new ColumnSet("aen_tipodevia");
            q.Criteria.Conditions.Add(new ConditionExpression("aen_tipodevia", ConditionOperator.NotNull));
            ents = Crm.IOS.RetrieveMultiple(q);
            foreach (var e in ents.Entities)
                TipoDeVia.Add(((string)e["aen_tipodevia"]).ToUpper(), e.Id);
            Comun.LogText("Cacheados tipos de vía: " + TipoDeVia.Count);

            q = new QueryExpression("account");
            q.ColumnSet = new ColumnSet(NombreCamposTercero.Aen_claveintegracionCRM);
            q.Criteria.Conditions.AddRange(
                new ConditionExpression(NombreCamposTercero.Aen_claveintegracionCRM, ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var t in entidades.Entities)
                    Terceros.Add((string)t[NombreCamposTercero.Aen_claveintegracionCRM], t.Id);
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }

            Comun.LogText("Cacheados terceros: " + Terceros.Count);
            q = new QueryExpression("contact");
            q.ColumnSet = new ColumnSet("aen_claveintegracion");
            q.Criteria.Conditions.AddRange(
                //new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression("aen_claveintegracion", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                    Contactos.Add((string)e["aen_claveintegracion"], e.Id);
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Comun.LogText("Cacheados contactos: " + Contactos.Count);

            q = new QueryExpression("aen_direccion");
            q.ColumnSet = new ColumnSet("aen_direccionid", "aen_claveintegracion",
                    "aen_terceroid", "aen_codigopostal", "aen_comunidadautonoma",
                    "aen_contactoid", "aen_email", "aen_fax",
                    "aen_localidad", "aen_name", "aen_nombrecompleto",
                    "aen_numerodevia","aen_observaciones","aen_paisid",
                    "aen_codigopais","aen_provinciaid","aen_razonsocial",
                    "aen_restodireccion","aen_telefono1","aen_telefono2",
                    "aen_tipodedireccion","aen_tipodeviaid","statecode",
                    "aen_observacionesmigracion","aen_origen", "aen_nombrevia", "aen_identificadordireccion"
                    );
            q.Criteria.Conditions.AddRange(
                //new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                new ConditionExpression("aen_claveintegracion", ConditionOperator.NotNull));
            q.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1 };
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var e in entidades.Entities)
                    DireccionesCRM.Add((string)e["aen_claveintegracion"], DireccionFromCRM(e));
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Comun.LogText("Cacheadas direcciones: " + DireccionesCRM.Count);
        }

        public void LeerDireccionesOracle()
        {
            //try
            //{
            //aen_claveintegracion es la clave única de verdad
            //aen_identificadordireccion no puede ser pk porque es la clave original del sistema origen, y como son varios sistemas puede que se repita
            OracleDataAdapter da = new OracleDataAdapter(Oracle.QueryDirecciones, Comun.ConnStringOracle);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DireccionesOracle = new Dictionary<string, Direccion>();
            foreach (DataRow fila in dt.Rows)
            {
                var direccion = DireccionFromOracle(fila);
                if (direccion == null)
                {
                    //Comun.LogText("Dirección de Oracle no válida: " + (string)fila["aen_claveintegracion"]);
                    continue;
                }
                DireccionesOracle.Add(direccion.Aen_ClaveIntegracion, direccion);
            }
            //}
            //catch (Exception e)
            //{
            //    Comun.LogText("Error leyendo direcciones de Oracle: " + e.Message);
            //}
        }
        #endregion Funciones Lectura

        #region GET
        private Entity GetEntity(Direccion dir)
        {
            var d = new Entity("aen_direccion");
            if (dir.Aen_DireccionId != Guid.Empty)
                d["aen_direccionid"] = dir.Aen_DireccionId;
            d["aen_claveintegracion"] = dir.Aen_ClaveIntegracion;
            d["aen_codigopostal"] = dir.Aen_CodigoPostal == "" ? null : dir.Aen_CodigoPostal;
            d["aen_comunidadautonoma"] = dir.Aen_Comunidadautonoma == "" ? null : dir.Aen_Comunidadautonoma;
            d["aen_localidad"] = dir.Aen_Localidad == "" ? null : dir.Aen_Localidad;
            d["aen_email"] = dir.Aen_Email == "" ? null : dir.Aen_Email;
            d["aen_fax"] = dir.Aen_Fax == "" ? null : dir.Aen_Fax;
            //d["aen_identificadordireccion"] = dir.Aen_IdentificadorDireccion == "" ? null : dir.Aen_IdentificadorDireccion;
            if(dir.Aen_Descripcion != "")
                d["aen_name"] = dir.Aen_Descripcion ;
            else
                d["aen_name"] = dir.Aen_Name == "" ? null : dir.Aen_Name;
            d["aen_nombrecompleto"] = dir.Aen_Nombrecompleto == "" ? null : dir.Aen_Nombrecompleto;
            d["aen_numerodevia"] = dir.Aen_NumeroDeVia == "" ? null : dir.Aen_NumeroDeVia;
            d["aen_nombrevia"] = dir.Aen_NombreDeVia == "" ? null : dir.Aen_NombreDeVia;
            d["aen_observaciones"] = dir.Aen_Observaciones == "" ? null : dir.Aen_Observaciones;
            d["aen_observacionesmigracion"] = dir.Aen_Observacionesmigracion == "" ? null : dir.Aen_Observacionesmigracion;
            d["aen_paisid"] = dir.Aen_PaisId == Guid.Empty ? null : new EntityReference("aen_pais", dir.Aen_PaisId);
            d["aen_codigopais"] = dir.Aen_Codigopais == "" ? null : dir.Aen_Codigopais;
            d["aen_provinciaid"] = dir.Aen_ProvinciaId == Guid.Empty ? null : new EntityReference("aen_provincia", dir.Aen_ProvinciaId);
            d["aen_provincia"] = dir.Aen_ProvinciaId == Guid.Empty ? null : ProvinciasNombres[dir.Aen_ProvinciaId];
            d["aen_razonsocial"] = dir.Aen_RazonSocial; // dir.Aen_RazonSocial == "" ? null : dir.Aen_RazonSocial;
            d["aen_restodireccion"] = dir.Aen_RestoDireccion == "" ? null : dir.Aen_RestoDireccion;
            d["aen_telefono1"] = dir.Aen_Telefono1 == "" ? null : dir.Aen_Telefono1;
            d["aen_telefono2"] = dir.Aen_Telefono2 == "" ? null : dir.Aen_Telefono2;
            d["aen_terceroid"] = dir.Aen_TerceroId == Guid.Empty ? null : new EntityReference("account", dir.Aen_TerceroId);
            d["aen_contactoid"] = dir.Aen_Contacto == Guid.Empty ? null : new EntityReference("contact", dir.Aen_Contacto);
            d["aen_tipodedireccion"] = dir.Aen_TipoDeDireccion == "" ? null : GetOptionSetValueCol(dir.Aen_TipoDeDireccion);
            d["aen_tipodeviaid"] = dir.Aen_TipoDeViaId == Guid.Empty ? null : new EntityReference("aen_tipodevia", dir.Aen_TipoDeViaId);
            d["statecode"] = new OptionSetValue(dir.StateCode == "Activo" ? 0 : 1);
            d["statuscode"] = new OptionSetValue(dir.StateCode == "Activo" ? 1 : 2);
            d["aen_origen"] = new OptionSetValue(Convert.ToInt32(dir.Aen_Origen));
            d["aen_identificadordireccion"] = dir.Aen_Identificadordireccion == "" ? null : dir.Aen_Identificadordireccion;

            //Para saltar plugins de envío de datos a NEXO
            //d["aen_vienedeintegracion"] = true;

            return d;
        }

        private string GetElemsOptionSetValueCol(OptionSetValueCollection col)
        {
            //Dada una colección de optionset para campos de optset multiple, devuelve su
            //representacion en la forma: elem1;elem2;elem3 ... así luego comparamos la cadena del tirón
            var cadena = "";
            var elems = col.Select(e => e.Value).ToList();
            elems.Sort();
            elems.ForEach(e => cadena += e + ";");
            cadena = cadena.Substring(0, cadena.Length - 1);
            return cadena;
        }

        private string GetElemsListaCol(string listaElems)
        {
            //Ordena una string tipo elem1;elem2;elemx, siendo los elem enteros
            var cadena = "";
            if (string.IsNullOrEmpty(listaElems))
                return "";
            listaElems = listaElems.Replace(".", "");
            var lista = listaElems.Split(';').Select(e => int.Parse(e)).ToList();
            lista.Sort();
            lista.ForEach(e => cadena += e + ";");
            cadena = cadena.Substring(0, cadena.Length - 1);
            return cadena;
        }

        private OptionSetValueCollection GetOptionSetValueCol(string valores)
        {
            var col = new OptionSetValueCollection();
            foreach (var val in valores.Split(';'))
                col.Add(new OptionSetValue(int.Parse(val)));
            return col;
        }
        #endregion GET

        #region Crear Direcciones
        private Direccion DireccionFromCRM(Entity ent)
        {
            var direccion = new Direccion();
            direccion.Aen_DireccionId = ent.Id;
            direccion.Aen_ClaveIntegracion = (string)ent["aen_claveintegracion"];
            direccion.Aen_CodigoPostal = ent.Contains("aen_codigopostal") ? ((string)ent["aen_codigopostal"]).Trim() : "";
            direccion.Aen_Fax = ent.Contains("aen_fax") ? ((string)ent["aen_fax"]).Trim() : "";
            direccion.Aen_Email = ent.Contains("aen_email") ? ((string)ent["aen_email"]).Trim() : "";
            direccion.Aen_Name = ent.Contains("aen_name") ? ((string)ent["aen_name"]).Trim() : "";
            direccion.Aen_Nombrecompleto = ent.Contains("aen_nombrecompleto") ? ((string)ent["aen_nombrecompleto"]).Trim() : "";
            direccion.Aen_NumeroDeVia = ent.Contains("aen_numerodevia") ? ((string)ent["aen_numerodevia"]).Trim() : "";
            direccion.Aen_NombreDeVia = ent.Contains("aen_nombrevia") ? ((string)ent["aen_nombrevia"]).Trim() : "";
            direccion.Aen_Observaciones = ent.Contains("aen_observaciones") ? ((string)ent["aen_observaciones"]).Trim() : "";
            direccion.Aen_Observacionesmigracion = ent.Contains("aen_observacionesmigracion") ? ((string)ent["aen_observacionesmigracion"]).Trim() : "";
            direccion.Aen_PaisId = ent.Contains("aen_paisid") ? ((EntityReference)ent["aen_paisid"]).Id : Guid.Empty;
            direccion.Aen_Codigopais = ent.Contains("aen_codigopais") ? (string)ent["aen_codigopais"] : "";
            direccion.Aen_ProvinciaId = ent.Contains("aen_provinciaid") ? ((EntityReference)ent["aen_provinciaid"]).Id : Guid.Empty;
            direccion.Aen_RazonSocial = ent.Contains("aen_razonsocial") ? (bool)ent["aen_razonsocial"] : false;
            direccion.Aen_RestoDireccion = ent.Contains("aen_restodireccion") ? ((string)ent["aen_restodireccion"]).Trim() : "";
            direccion.Aen_Telefono1 = ent.Contains("aen_telefono1") ? ((string)ent["aen_telefono1"]).Trim() : "";
            direccion.Aen_Telefono2 = ent.Contains("aen_telefono2") ? ((string)ent["aen_telefono2"]).Trim() : "";
            direccion.Aen_TerceroId = ent.Contains("aen_terceroid") ? ((EntityReference)ent["aen_terceroid"]).Id : Guid.Empty;
            direccion.Aen_Contacto = ent.Contains("aen_contactoid") ? ((EntityReference)ent["aen_contactoid"]).Id : Guid.Empty;
            direccion.Aen_TipoDeDireccion = ent.Contains("aen_tipodedireccion") ? GetElemsOptionSetValueCol((OptionSetValueCollection)ent["aen_tipodedireccion"]) : "";
            direccion.Aen_TipoDeViaId = ent.Contains("aen_tipodeviaid") ? ((EntityReference)ent["aen_tipodeviaid"]).Id : Guid.Empty;
            direccion.StateCode = ((OptionSetValue)ent["statecode"]).Value == 0 ? "Activo" : "Inactivo";
            direccion.Aen_Origen = ent.Contains("aen_origen") ? ((OptionSetValue)ent["aen_origen"]).Value.ToString() : "";
            direccion.Aen_Comunidadautonoma = ent.Contains("aen_comunidadautonoma") ? (string)ent["aen_comunidadautonoma"] : "";
            direccion.Aen_Localidad = ent.Contains("aen_localidad") ? (string)ent["aen_localidad"] : "";
            direccion.Aen_Identificadordireccion = ent.Contains("aen_identificadordireccion") ? (string)ent["aen_identificadordireccion"] : "";
            return direccion;
        }

        private Direccion DireccionFromOracle(DataRow fila)
        {
            //si no se encuentra en crm o no viene tercero de oracle, o statecode, la dirección no es válida
            //si no se encuentra país, provincia, tipovia, lo mismo, si viene vacío de origen sí dejamos
            var ok = false;
            var direccion = new Direccion();
            direccion.Aen_DireccionId = Guid.Empty;
            direccion.Aen_ClaveIntegracion = (string)fila["aen_claveintegracion"];
            var terceroId = Guid.Empty;
            if (DBNull.Value != fila["aen_claveintegracionparent"])
            {
                ok = Terceros.TryGetValue((string)fila["aen_claveintegracionparent"], out terceroId);
                if (!ok)
                {
                    var texto = "Dirección de Oracle " + (string)fila["aen_claveintegracion"] +
                        " no válida por no encontrarse Tercero en CRM " + (string)fila["aen_claveintegracionparent"];
                    Comun.LogText(texto);
                    Oracle.MandarErrorIntegracion((string)fila["aen_claveintegracion"], texto,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                    return null;
                }
            }

            direccion.Aen_TerceroId = terceroId;
            var contactoId = Guid.Empty;
            if (DBNull.Value != fila["aen_claveintegracioncontacto"])
                ok = Contactos.TryGetValue((string)fila["aen_claveintegracioncontacto"], out contactoId);
            direccion.Aen_Contacto = contactoId;
            direccion.StateCode = (string)fila["statecode"];
            //if (direccion.StateCode != "Activo" && direccion.StateCode != "Inactivo")
            //{
            //    Comun.LogText("Dirección de Oracle " + (string)fila["aen_claveintegracion"] +
            //        " no válida por tener Statecode '" + direccion.StateCode + "'");
            //    return null;
            //}
            var paisId = Guid.Empty;
            direccion.Aen_Codigopais = string.Empty;

            if (DBNull.Value != fila["aen_paisid"])
            {
                direccion.Aen_Codigopais = (string)fila["aen_paisid"];
                ok = Paises.TryGetValue((string)fila["aen_paisid"], out paisId);
                if (!ok)
                {
                    var texto = "Dirección de Oracle " + (string)fila["aen_claveintegracion"] +
                        " no válida por tener país '" + (string)fila["aen_paisid"] + "' no encontrado en CRM";
                    Comun.LogText(texto);
                    Oracle.MandarErrorIntegracion((string)fila["aen_claveintegracion"], texto,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                    //return null;
                }
            }
            direccion.Aen_PaisId = paisId;
            var provinciaId = Guid.Empty;
            if (DBNull.Value != fila["aen_provinciaid"])
            {
                ok = Provincias.TryGetValue((string)fila["aen_provinciaid"], out provinciaId);
                if (!ok)
                {
                    var texto = "Dirección de Oracle " + (string)fila["aen_claveintegracion"] +
                        " no válida por tener provincia '" + (string)fila["aen_provinciaid"] + "' no encontrado en CRM";
                    Comun.LogText(texto);
                    Oracle.MandarErrorIntegracion((string)fila["aen_claveintegracion"], texto,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                    //return null;
                }
            }
            direccion.Aen_ProvinciaId = provinciaId;
            var tipoViaId = Guid.Empty;
            if (DBNull.Value != fila["aen_tipodeviaid"])
            {
                var tipoVia = ((string)fila["aen_tipodeviaid"]).Trim().ToUpper();
                ok = TipoDeVia.TryGetValue(tipoVia, out tipoViaId);
                if (!ok)
                {
                    var texto = "Dirección de Oracle " + (string)fila["aen_claveintegracion"] +
                        " no válida por tener tipo de vía '" + (string)fila["aen_tipodeviaid"] + "' no encontrado en CRM";
                    Comun.LogText(texto);
                    Oracle.MandarErrorIntegracion((string)fila["aen_claveintegracion"], texto,
                        Oracle.TipoEntidadDireccion, Oracle.TipoAccionValidacion, null);
                    //return null;
                }
            }
            direccion.Aen_TipoDeViaId = tipoViaId;
            direccion.Aen_CodigoPostal = DBNull.Value != fila["aen_codigopostal"] ? ((string)fila["aen_codigopostal"]).Trim() : "";
            direccion.Aen_Fax = DBNull.Value != fila["aen_fax"] ? ((string)fila["aen_fax"]).Trim() : "";
            direccion.Aen_Email = DBNull.Value != fila["aen_email"] ? ((string)fila["aen_email"]).Trim() : "";
            direccion.Aen_Name = DBNull.Value != fila["aen_name"] ? ((string)fila["aen_name"]).Trim() : "";
            direccion.Aen_Nombrecompleto = DBNull.Value != fila["aen_name"] ? ((string)fila["aen_name"]).Trim() : "";
            direccion.Aen_NumeroDeVia = DBNull.Value != fila["aen_numerodevia"] ? ((string)fila["aen_numerodevia"]).Trim() : "";
            direccion.Aen_NombreDeVia = DBNull.Value != fila["aen_nombrevia"] ? ((string)fila["aen_nombrevia"]).Trim() : "";
            direccion.Aen_Observaciones = DBNull.Value != fila["aen_observaciones"] ? ((string)fila["aen_observaciones"]).Trim() : "";
            direccion.Aen_Observacionesmigracion = DBNull.Value != fila["aen_observacionesmigracion"] ? ((string)fila["aen_observacionesmigracion"]).Trim() : "";
            direccion.Aen_RazonSocial = DBNull.Value != fila["aen_razonsocial"] ? (int)(decimal)fila["aen_razonsocial"] == 1 : false;
            direccion.Aen_RestoDireccion = DBNull.Value != fila["aen_restodireccion"] ? ((string)fila["aen_restodireccion"]).Trim() : "";
            direccion.Aen_Telefono1 = DBNull.Value != fila["aen_telefono1"] ? ((string)fila["aen_telefono1"]).Trim() : "";
            direccion.Aen_Telefono2 = DBNull.Value != fila["aen_telefono2"] ? ((string)fila["aen_telefono2"]).Trim() : "";
            //TODO Esperamos los tipos de dir en Oracle en el formato elem1;elem2;elemx, los ordenamos para facilitar comparación
            //Nos dice Marisa que en principio vendría en dos filas, pero que para la integración inicial no ocurrirá
            direccion.Aen_TipoDeDireccion = DBNull.Value != fila["aen_tipodedireccion"] ? GetElemsListaCol((string)fila["aen_tipodedireccion"]) : "";
            direccion.Aen_Origen = DBNull.Value != fila["aen_origen"] ? (fila["aen_origen"]).ToString().Replace(".", "").Trim() : "";
            direccion.Aen_Descripcion = DBNull.Value != fila["aen_descripcion"] ? (string)fila["aen_descripcion"] : "";
            direccion.Aen_Comunidadautonoma = DBNull.Value != fila["aen_comunidadautonoma"] ? (string)fila["aen_comunidadautonoma"] : "";
            direccion.Aen_Localidad = DBNull.Value != fila["aen_localidad"] ? (string)fila["aen_localidad"] : "" ;
            direccion.Aen_Identificadordireccion = DBNull.Value != fila["aen_identificadordirec"] ? (fila["aen_identificadordirec"]).ToString() : "";

            return direccion;
        }

        private bool DireccionesIguales(Direccion dirOra, Direccion dirCRM)
        {
            //Lo hacemos por líneas, no de un tirón toda la comparación, para poder depurar
            var ig = dirOra.Aen_ClaveIntegracion == dirCRM.Aen_ClaveIntegracion;
            ig = ig && dirOra.Aen_CodigoPostal == dirCRM.Aen_CodigoPostal;
            ig = ig && dirOra.Aen_Email == dirCRM.Aen_Email;
            ig = ig && dirOra.Aen_Fax == dirCRM.Aen_Fax;

            if (dirOra.Aen_Descripcion != "")
                ig = ig && dirOra.Aen_Descripcion.Trim() == dirCRM.Aen_Name.Trim();
            else
                ig = ig && dirOra.Aen_Name.Trim() == dirCRM.Aen_Name.Trim();

            ig = ig && (dirOra.Aen_Nombrecompleto == dirCRM.Aen_Nombrecompleto); // || dirOra.Aen_Name == dirCRM.Aen_Nombrecompleto);
            ig = ig && dirOra.Aen_NumeroDeVia == dirCRM.Aen_NumeroDeVia;
            ig = ig && dirOra.Aen_NombreDeVia == dirCRM.Aen_NombreDeVia;
            ig = ig && dirOra.Aen_Observaciones == dirCRM.Aen_Observaciones;
            ig = ig && dirOra.Aen_PaisId == dirCRM.Aen_PaisId;
            ig = ig && dirOra.Aen_ProvinciaId == dirCRM.Aen_ProvinciaId;
            ig = ig && dirOra.Aen_RazonSocial == dirCRM.Aen_RazonSocial;
            ig = ig && dirOra.Aen_RestoDireccion == dirCRM.Aen_RestoDireccion;
            ig = ig && dirOra.Aen_Telefono1 == dirCRM.Aen_Telefono1;
            ig = ig && dirOra.Aen_Telefono2 == dirCRM.Aen_Telefono2;
            ig = ig && dirOra.Aen_TerceroId == dirCRM.Aen_TerceroId;
            ig = ig && dirOra.Aen_Contacto == dirCRM.Aen_Contacto;
            //ig = ig && EsIgualMultSel(dirOra.Aen_TipoDeDireccion,dirCRM.Aen_TipoDeDireccion);
            //Ya estarán ordenados porque nos hemos molestado en ello al cargarlos, podemos comparar el chorro directamente
            ig = ig && dirOra.Aen_TipoDeDireccion == dirCRM.Aen_TipoDeDireccion;
            ig = ig && dirOra.Aen_TipoDeViaId == dirCRM.Aen_TipoDeViaId;
            ig = ig && dirOra.StateCode == dirCRM.StateCode;
            ig = ig && dirOra.Aen_Comunidadautonoma == dirCRM.Aen_Comunidadautonoma;
            ig = ig && dirOra.Aen_Observacionesmigracion == dirCRM.Aen_Observacionesmigracion;
            ig = ig && dirOra.Aen_Codigopais == dirCRM.Aen_Codigopais;
            ig = ig && dirOra.Aen_Localidad == dirCRM.Aen_Localidad;
            ig = ig && dirOra.Aen_Origen == dirCRM.Aen_Origen;
            ig = ig && dirOra.Aen_Identificadordireccion == dirCRM.Aen_Identificadordireccion;
            //ig = ig && (dirOra.Aen_Descripcion == dirCRM.Aen_Name && dirOra.Aen_Descripcion != "");


            return ig;
        }
        #endregion Crear Direcciones


        private void BorrarDirecciones()
        {
            var q = new QueryExpression("aen_direccion");
            q.Criteria.AddCondition(new ConditionExpression("createdon", ConditionOperator.Today));
            while (true)
            {
                var entidades = Crm.IOS.RetrieveMultiple(q);
                foreach (var t in entidades.Entities)
                    Crm.AnadirElementoEmr(new DeleteRequest { Target = new EntityReference("aen_direccion", t.Id) });
                if (entidades.MoreRecords)
                {
                    q.PageInfo.PageNumber++;
                    q.PageInfo.PagingCookie = entidades.PagingCookie;
                }
                else
                    break;
            }
            Crm.ProcesarUltimosEmr();
        }
    }
}
