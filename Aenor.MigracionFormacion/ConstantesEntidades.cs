// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://aenorcrmdesarrollo.crm4.dynamics.com/
// Filename  : C:\Users\jose.ruizc\Desktop\Cursos.cs
// Created   : 2019-02-27 09:32:01
// *********************************************************************

namespace Aenor.MigracionFormacion
{
    /// <summary>DisplayName: Área de conocimiento, OwnershipType: UserOwned, IntroducedVersion: 1.0.0.0</summary>
    /// <remarks>Áreas de conocimiento de los cursos</remarks>
    public static class AenAreadeconocimiento
    {
        public const string EntityName = "aen_areadeconocimiento";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        /// <remarks>Identificador único de instancias de entidad</remarks>
        public const string PrimaryKey = "aen_areadeconocimientoid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        /// <remarks>El nombre de la entidad personalizada.</remarks>
        public const string PrimaryName = "aen_name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que creó el registro.</remarks>
        public const string CreatedBy = "createdby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que creó el registro.</remarks>
        public const string CreatedOnBehalfBy = "createdonbehalfby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Código de la zona horaria que estaba en uso cuando se creó el registro.</remarks>
        public const string UtcConversionTimezoneCode = "utcconversiontimezonecode";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Estado, OptionSetType: State</summary>
        /// <remarks>Estado del Área de conocimiento</remarks>
        public const string StateCode = "statecode";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se creó el registro.</remarks>
        public const string CreatedOn = "createdon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se migró el registro.</remarks>
        public const string OverriddenCreatedOn = "overriddencreatedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se modificó el registro.</remarks>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que modificó el registro.</remarks>
        public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que modificó el registro.</remarks>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Número de secuencia de la importación que creó este registro.</remarks>
        public const string ImportSequenceNumber = "importsequencenumber";
        /// <summary>Type: BigInt, RequiredLevel: None, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        /// <remarks>Número de versión</remarks>
        public const string VersionNumber = "versionnumber";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Para uso interno.</remarks>
        public const string TimezoneruleVersionNumber = "timezoneruleversionnumber";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired</summary>
        /// <remarks>Tipo de identificador de propietario</remarks>
        public const string OwnerIdType = "owneridtype";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        /// <remarks>Id. del propietario</remarks>
        public const string OwnerId = "ownerid";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Razón para el estado, OptionSetType: Status</summary>
        /// <remarks>Razón para el estado del Área de conocimiento</remarks>
        public const string StatusCode = "statuscode";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: businessunit</summary>
        /// <remarks>Identificador único de la unidad de negocio propietaria del registro</remarks>
        public const string OwningBusinessUnit = "owningbusinessunit";
        /// <summary>Parent: "AenAreadeconocimiento" Child: "AenSubareadeconocimiento" Lookup: "AenArea"</summary>
        public const string Rel1M_AenAreadeconocimientoAenSubareadeconocimientoArea = "aen_areadeconocimiento_aen_subareadeconocimiento_Area";
        /// <summary>Parent: "AenAreadeconocimiento" Child: "AenCurso" Lookup: "AenAreadeconocimiento"</summary>
        public const string Rel1M_AenAreadeconocimientoAenCursoAreadeconocimiento = "aen_areadeconocimiento_aen_curso_Areadeconocimiento";
        public enum StateCode_OptionSet
        {
            Activo = 0,
            Inactivo = 1
        }
        public enum OwnerIdType_OptionSet
        {
        }
        public enum StatusCode_OptionSet
        {
            Activo = 1,
            Inactivo = 2
        }
    }

    /// <summary>DisplayName: Convocatoria, OwnershipType: UserOwned, IntroducedVersion: 1.0</summary>
    public static class AenConvocatorias
    {
        public const string EntityName = "aen_convocatorias";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        /// <remarks>Identificador único de instancias de entidad</remarks>
        public const string PrimaryKey = "aen_convocatoriasid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 150, Format: Text</summary>
        /// <remarks>El nombre de la entidad personalizada.</remarks>
        public const string PrimaryName = "aen_name";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenAlumnosinscritos = "aen_alumnosinscritos";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Estado del campo de informe Alumnos inscritos.</remarks>
        public const string AenAlumnosinscritosState = "aen_alumnosinscritos_state";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Hora en que se actualizó el campo de informe Alumnos inscritos por última vez.</remarks>
        public const string AenAlumnosinscritosDate = "aen_alumnosinscritos_date";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Numero de alumnos asociados en preinscripciones para esta convocatoria</remarks>
        public const string AenAlumnospreinscritos = "aen_alumnospreinscritos";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Estado del campo de informe Alumnos preinscritos.</remarks>
        public const string AenAlumnospreinscritosState = "aen_alumnospreinscritos_state";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Hora en que se actualizó el campo de informe Alumnos preinscritos por última vez.</remarks>
        public const string AenAlumnospreinscritosDate = "aen_alumnospreinscritos_date";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que creó el registro.</remarks>
        public const string CreatedBy = "createdby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que creó el registro.</remarks>
        public const string CreatedOnBehalfBy = "createdonbehalfby";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Canal de venta, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        /// <remarks>Canal de venta para esta convocatoria del curso</remarks>
        public const string AenCanaldeventa = "aen_canaldeventa";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        /// <remarks>Referencia a la ciudad donde se imparte el curso</remarks>
        public const string AenCiudad = "aen_ciudad";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenCodigodelaconvocatoria = "aen_codigodelaconvocatoria";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Código de la zona horaria que estaba en uso cuando se creó el registro.</remarks>
        public const string UtcConversionTimezoneCode = "utcconversiontimezonecode";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string AenColaboraciones = "aen_colaboraciones";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string AenComunidadautonoma = "aen_comunidadautonoma";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: contact</summary>
        /// <remarks>Identificador único de Contacto asociado con Convocatoria.</remarks>
        public const string AenCoordinadorId = "aen_coordinadorid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: aen_curso</summary>
        /// <remarks>Identificador único de Curso asociado con Convocatoria.</remarks>
        public const string AenCursoId = "aen_cursoid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenDescripcion = "aen_descripcion";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenDias = "aen_dias";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenDireccindeimparticion = "aen_direccindeimparticion";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: aen_direccion</summary>
        /// <remarks>Dirección del centro donde se celebra la convocatoria</remarks>
        public const string AenDireccionincompany = "aen_direccionincompany";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string AenDuracionestimada = "aen_duracionestimada";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenEsperados = "aen_esperados";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Estado, OptionSetType: State</summary>
        /// <remarks>Estado del Convocatoria</remarks>
        public const string StateCode = "statecode";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Estado de la convocatoria, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        public const string AenEstadodelaconvocatoria = "aen_estadodelaconvocatoria";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se creó el registro.</remarks>
        public const string CreatedOn = "createdon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se migró el registro.</remarks>
        public const string OverriddenCreatedOn = "overriddencreatedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string AenFechadefin = "aen_fechadefin";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string AenFechadeinicio = "aen_fechadeinicio";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se modificó el registro.</remarks>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 200, Format: Text</summary>
        public const string AenHorario = "aen_horario";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string AenHorarioconvocatoriaespecial = "aen_horarioconvocatoriaespecial";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string AenHorariocursosincompanydistribuidor = "aen_horariocursosincompanydistribuidor";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenHorasonline = "aen_horasonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenHoraspresencial = "aen_horaspresencial";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: site</summary>
        /// <remarks>Lugar de imparticiónde la convocatoria</remarks>
        public const string AenSitio = "aen_sitio";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenMaximo = "aen_maximo";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenMediosdidacticos = "aen_mediosdidacticos";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenMinimo = "aen_minimo";
        /// <summary>Type: Picklist, RequiredLevel: ApplicationRequired, DisplayName: Modalidad curso, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        public const string AenModalidad = "aen_modalidad";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que modificó el registro.</remarks>
        public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que modificó el registro.</remarks>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenMostrarenweb = "aen_mostrarenweb";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenNumerodealumnos = "aen_numerodealumnos";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenNumerodeconvocatoria = "aen_numerodeconvocatoria";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Número de secuencia de la importación que creó este registro.</remarks>
        public const string ImportSequenceNumber = "importsequencenumber";
        /// <summary>Type: BigInt, RequiredLevel: None, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        /// <remarks>Número de versión</remarks>
        public const string VersionNumber = "versionnumber";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Para uso interno.</remarks>
        public const string TimezoneruleVersionNumber = "timezoneruleversionnumber";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenObservaciones = "aen_observaciones";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired</summary>
        /// <remarks>Tipo de identificador de propietario</remarks>
        public const string OwnerIdType = "owneridtype";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenPais = "aen_pais";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenProvincia = "aen_provincia";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Razón para el estado, OptionSetType: Status</summary>
        /// <remarks>Razón para el estado del Convocatoria</remarks>
        public const string StatusCode = "statuscode";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        /// <remarks>Id. del propietario</remarks>
        public const string OwnerId = "ownerid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account</summary>
        /// <remarks>Identificador único de Tercero asociado con Convocatoria.</remarks>
        public const string AenTerceroId = "aen_terceroid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenTitulointerno = "aen_titulointerno";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenTotalhoras = "aen_totalhoras";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: businessunit</summary>
        /// <remarks>Identificador único de la unidad de negocio propietaria del registro</remarks>
        public const string OwningBusinessUnit = "owningbusinessunit";
        /// <summary>Parent: "AenCurso" Child: "AenConvocatorias" Lookup: "AenCursoId"</summary>
        public const string RelM1_AenCursoAenConvocatorias = "aen_curso_aen_convocatorias";
        public enum AenCanaldeventa_OptionSet
        {
            EnAbierto = 1,
            OnLine = 2,
            InCompany = 3,
            Distribuidor = 4
        }
        public enum StateCode_OptionSet
        {
            Activo = 0,
            Inactivo = 1
        }
        public enum AenEstadodelaconvocatoria_OptionSet
        {
            Prevista = 277220000,
            Confirmada = 277220001,
            Impartida = 277220002,
            Anulada = 277220003
        }
        public enum AenModalidad_OptionSet
        {
            Presencial = 1,
            Online = 3,
            Semipresencial = 2
        }
        public enum OwnerIdType_OptionSet
        {
        }
        public enum StatusCode_OptionSet
        {
            Prevista = 1,
            MinAlcanzado = 277220004,
            Confirmada = 277220000,
            Impartida = 277220001,
            Facturada = 2,
            Anulada = 277220003
        }
    }

    /// <summary>DisplayName: Curso, OwnershipType: UserOwned, IntroducedVersion: 1.0.0.0</summary>
    public static class AenCurso
    {
        public const string EntityName = "aen_curso";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        /// <remarks>Identificador único de instancias de entidad</remarks>
        public const string PrimaryKey = "aen_cursoid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 200, Format: Text</summary>
        /// <remarks>El nombre de la entidad personalizada.</remarks>
        public const string PrimaryName = "aen_name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: aen_areadeconocimiento</summary>
        public const string AenAreadeconocimiento = "aen_areadeconocimiento";
        /// <summary>Type: Virtual, RequiredLevel: None, DisplayName: Areas (clasificacion web), OptionSetType: Picklist</summary>
        public const string AenAreasclasificacionweb = "aen_areasclasificacionweb";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenAsistenciaminima = "aen_asistenciaminima";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenAsistenciaminimaonline = "aen_asistenciaminimaonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenAsistenciaminimapresencial = "aen_asistenciaminimapresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenAsistenciaminimasemipresencial = "aen_asistenciaminimasemipresencial";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que creó el registro.</remarks>
        public const string CreatedBy = "createdby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que creó el registro.</remarks>
        public const string CreatedOnBehalfBy = "createdonbehalfby";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 250</summary>
        public const string AenCertificado = "aen_certificado";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        /// <remarks>Código del curso</remarks>
        public const string AenCodigo = "aen_codigo";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Código de la zona horaria que estaba en uso cuando se creó el registro.</remarks>
        public const string UtcConversionTimezoneCode = "utcconversiontimezonecode";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 500</summary>
        public const string AenColaboraciones = "aen_colaboraciones";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 3000</summary>
        public const string AenContenido = "aen_contenido";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenControlasistencia = "aen_controlasistencia";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenConvalidaciones = "aen_convalidaciones";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        /// <remarks>Id. del propietario</remarks>
        public const string OwnerId = "ownerid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: aen_curso</summary>
        public const string AenCursooriginal = "aen_cursooriginal";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenDecatalogo = "aen_decatalogo";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenDescripcion = "aen_descripcion";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 100000</summary>
        public const string AenDescripcionCodeEditor = "aen_descripcioncodeeditor";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenDiasonline = "aen_diasonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenDiaspresencial = "aen_diaspresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenDiassemipresencial = "aen_diassemipresencial";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenDirigidoa = "aen_dirigidoa";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 200</summary>
        public const string AenDisponibleonline = "aen_disponibleonline";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        /// <remarks>Se explica cómo se reparte al profesorado</remarks>
        public const string AenDistribucionhorariadelprofesorado = "aen_distribucionhorariadelprofesorado";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        /// <remarks>Indica si el canal de venta es a través de un distribuidor</remarks>
        public const string AenDistribuidor = "aen_distribuidor";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: transactioncurrency</summary>
        /// <remarks>Identificador único de la divisa asociada a la entidad.</remarks>
        public const string TransactioncurrencyId = "transactioncurrencyid";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 1000</summary>
        public const string AenDocumentacinaentregaralalumno = "aen_documentacinaentregaralalumno";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 200, Format: Text</summary>
        public const string AenDuracionyhorariodelcurso = "aen_duracionyhorariodelcurso";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenDuracionyhorarioestimadoonline = "aen_duracionyhorarioestimadoonline";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenDuracionyhorarioestimadopresencial = "aen_duracionyhorarioestimadopresencial";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenDuracionyhorarioestimadosemipresencial = "aen_duracionyhorarioestimadosemipresencial";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenElalumnorecibira = "aen_elalumnorecibira";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenEnabierto = "aen_enabierto";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 500</summary>
        public const string AenEquipopedagogico = "aen_equipopedagogico";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenEsperadoonline = "aen_esperadoonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenEsperadopresencial = "aen_esperadopresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenEsperadosemipresencial = "aen_esperadosemipresencial";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Estado, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        public const string AenEstado = "aen_estado";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Estado, OptionSetType: State</summary>
        /// <remarks>Estado del Curso</remarks>
        public const string StateCode = "statecode";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenExamen = "aen_examen";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se creó el registro.</remarks>
        public const string CreatedOn = "createdon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se migró el registro.</remarks>
        public const string OverriddenCreatedOn = "overriddencreatedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se modificó el registro.</remarks>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenHorarioonline = "aen_horarioonline";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenHorariopresencial = "aen_horariopresencial";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenHorariosemipresencial = "aen_horariosemipresencial";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 300</summary>
        public const string AenImportE = "aen_importe";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenIncompany = "aen_incompany";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenMaximoonline = "aen_maximoonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenMaximopresencial = "aen_maximopresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenMaximosemipresencial = "aen_maximosemipresencial";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenMetodologiadetrabajo = "aen_metodologiadetrabajo";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenMinimo = "aen_minimo";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que modificó el registro.</remarks>
        public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que modificó el registro.</remarks>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenMostrarenweb = "aen_mostrarenweb";
        /// <summary>Type: Decimal, RequiredLevel: None, MinValue: -100000000000, MaxValue: 100000000000, Precision: 2</summary>
        public const string AenNotaminima = "aen_notaminima";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 200</summary>
        public const string AenNumerodeasistentes = "aen_numerodeasistentes";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenNumerodehorasonline = "aen_numerodehorasonline";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenNumerodehoraspresencial = "aen_numerodehoraspresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string AenNumerodehorassemipresencial = "aen_numerodehorassemipresencial";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Número de secuencia de la importación que creó este registro.</remarks>
        public const string ImportSequenceNumber = "importsequencenumber";
        /// <summary>Type: BigInt, RequiredLevel: None, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        /// <remarks>Número de versión</remarks>
        public const string VersionNumber = "versionnumber";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Para uso interno.</remarks>
        public const string TimezoneruleVersionNumber = "timezoneruleversionnumber";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenObjetivos = "aen_objetivos";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenObligatorio = "aen_obligatorio";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenObservaciones = "aen_observaciones";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenObservacionesweb = "aen_observacionesweb";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenObservacionesaladuracion = "aen_observacionesaladuracion";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenObservacionesinternas = "aen_observacionesinternas";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenOnline = "aen_online";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string AenOrdendelcurso = "aen_ordendelcurso";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired</summary>
        /// <remarks>Tipo de identificador de propietario</remarks>
        public const string OwnerIdType = "owneridtype";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenPerfil = "aen_perfil";
        /// <summary>Type: Decimal, RequiredLevel: None, MinValue: -100000000000, MaxValue: 100000000000, Precision: 2</summary>
        public const string AenPorcentajedeasistencia = "aen_porcentajedeasistencia";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPrecioonline = "aen_precioonline";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_precioonline</summary>
        /// <remarks>Valor de Precio (On-line) en divisa base.</remarks>
        public const string AenPrecioonlineBase = "aen_precioonline_base";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPreciopresencial = "aen_preciopresencial";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_preciopresencial</summary>
        /// <remarks>Valor de Precio (Presencial) en divisa base.</remarks>
        public const string AenPreciopresencialBase = "aen_preciopresencial_base";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPreciosemipresencial = "aen_preciosemipresencial";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_preciosemipresencial</summary>
        /// <remarks>Valor de Precio (Semipresencial) en divisa base.</remarks>
        public const string AenPreciosemipresencialBase = "aen_preciosemipresencial_base";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPreciomiembroonline = "aen_preciomiembroonline";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_preciomiembroonline</summary>
        /// <remarks>Valor de Precio miembro (On-line) en divisa base.</remarks>
        public const string AenPreciomiembroonlineBase = "aen_preciomiembroonline_base";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPreciomiembropresencial = "aen_preciomiembropresencial";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_preciomiembropresencial</summary>
        /// <remarks>Valor de Precio miembro (Presencial) en divisa base.</remarks>
        public const string AenPreciomiembropresencialBase = "aen_preciomiembropresencial_base";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4</summary>
        public const string AenPreciomiembrosemipresencial = "aen_preciomiembrosemipresencial";
        /// <summary>Type: Money, RequiredLevel: None, MinValue: -922337203685477, MaxValue: 922337203685477, Precision: 4, CalculationOf: aen_preciomiembrosemipresencial</summary>
        /// <remarks>Valor de Precio miembro (Semipresencial) en divisa base.</remarks>
        public const string AenPreciomiembrosemipresencialBase = "aen_preciomiembrosemipresencial_base";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenPresencial = "aen_presencial";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenPresentacioncursointroduccion = "aen_presentacioncursointroduccion";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenProfesorado = "aen_profesorado";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenProfesoradoweb = "aen_profesoradoweb";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AenPromocionespecial = "aen_promocionespecial";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: True</summary>
        public const string AenPropiedaddeaenor = "aen_propiedaddeaenor";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 4000, Format: Text</summary>
        public const string AenPrueba = "aen_prueba";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Razón para el estado, OptionSetType: Status</summary>
        /// <remarks>Razón para el estado del Curso</remarks>
        public const string StatusCode = "statuscode";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 255</summary>
        public const string AenReconocimientos = "aen_reconocimientos";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenRequisitos = "aen_requisitos";
        /// <summary>Type: Virtual, RequiredLevel: None, DisplayName: Sectores (clasificacion web), OptionSetType: Picklist</summary>
        public const string AenSectoresclasificacionweb = "aen_sectoresclasificacionweb";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenSemipresencial = "aen_semipresencial";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string AenSoloincompany = "aen_soloincompany";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: aen_subareadeconocimiento</summary>
        public const string AenSubareadeconocimiento = "aen_subareadeconocimiento";
        /// <summary>Type: Decimal, RequiredLevel: None, MinValue: 0,0000000001, MaxValue: 100000000000, Precision: 10</summary>
        /// <remarks>Tipo de cambio de la divisa asociada a la entidad en relación con la divisa base.</remarks>
        public const string ExchangeRate = "exchangerate";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Tipologia, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        public const string AenTipologia = "aen_tipologia";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string AenTitulacionpropiadeaenor = "aen_titulacionpropiadeaenor";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: businessunit</summary>
        /// <remarks>Identificador único de la unidad de negocio propietaria del registro</remarks>
        public const string OwningBusinessUnit = "owningbusinessunit";
        /// <summary>Parent: "AenAreadeconocimiento" Child: "AenCurso" Lookup: "AenAreadeconocimiento"</summary>
        public const string RelM1_AenAreadeconocimientoAenCursoAreadeconocimiento = "aen_areadeconocimiento_aen_curso_Areadeconocimiento";
        /// <summary>Parent: "AenSubareadeconocimiento" Child: "AenCurso" Lookup: "AenSubareadeconocimiento"</summary>
        public const string RelM1_AenSubareadeconocimientoAenCursoSubareadeconocimiento = "aen_subareadeconocimiento_aen_curso_Subareadeconocimiento";
        /// <summary>Parent: "AenCurso" Child: "AenCurso" Lookup: "AenCursooriginal"</summary>
        public const string Rel1M_AenCursoAenCursoCursooriginal = "aen_curso_aen_curso_Cursooriginal";
        /// <summary>Parent: "AenCurso" Child: "AenConvocatorias" Lookup: "AenCursoId"</summary>
        public const string Rel1M_AenCursoAenConvocatorias = "aen_curso_aen_convocatorias";
        public enum AenAreasclasificacionweb_OptionSet
        {
        }
        public enum AenEstado_OptionSet
        {
            Nuevo = 277220000,
            Actualizado = 277220001,
            Sincambios = 277220002,
            VersionAntigua = 277220003
        }
        public enum StateCode_OptionSet
        {
            Activo = 0,
            Inactivo = 1
        }
        public enum OwnerIdType_OptionSet
        {
        }
        public enum StatusCode_OptionSet
        {
            Activo = 1,
            Inactivo = 2
        }
        public enum AenSectoresclasificacionweb_OptionSet
        {
        }
        public enum AenTipologia_OptionSet
        {
            Cursoestandar = 1,
            Master = 2,
            Titulacionpropia = 3,
            ActividadExtraordinaria = 4,
            Certificaciondepersonas = 5
        }
    }

    /// <summary>DisplayName: Modalidad, OwnershipType: OrganizationOwned, IntroducedVersion: 1.0.0.0</summary>
    public static class AenModalidad
    {
        public const string EntityName = "aen_modalidad";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        /// <remarks>Identificador único de instancias de entidad</remarks>
        public const string PrimaryKey = "aen_modalidadid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        /// <remarks>El nombre de la entidad personalizada.</remarks>
        public const string PrimaryName = "aen_name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que creó el registro.</remarks>
        public const string CreatedBy = "createdby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que creó el registro.</remarks>
        public const string CreatedOnBehalfBy = "createdonbehalfby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Código de la zona horaria que estaba en uso cuando se creó el registro.</remarks>
        public const string UtcConversionTimezoneCode = "utcconversiontimezonecode";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        public const string AenCodigomodalidad = "aen_codigomodalidad";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Estado, OptionSetType: State</summary>
        /// <remarks>Estado del Modalidad</remarks>
        public const string StateCode = "statecode";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se creó el registro.</remarks>
        public const string CreatedOn = "createdon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se migró el registro.</remarks>
        public const string OverriddenCreatedOn = "overriddencreatedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se modificó el registro.</remarks>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: organization</summary>
        /// <remarks>Identificador único de la organización.</remarks>
        public const string OrganizationId = "organizationid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que modificó el registro.</remarks>
        public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que modificó el registro.</remarks>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Número de secuencia de la importación que creó este registro.</remarks>
        public const string ImportSequenceNumber = "importsequencenumber";
        /// <summary>Type: BigInt, RequiredLevel: None, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        /// <remarks>Número de versión</remarks>
        public const string VersionNumber = "versionnumber";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Para uso interno.</remarks>
        public const string TimezoneruleVersionNumber = "timezoneruleversionnumber";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Razón para el estado, OptionSetType: Status</summary>
        /// <remarks>Razón para el estado del Modalidad</remarks>
        public const string StatusCode = "statuscode";
        public enum StateCode_OptionSet
        {
            Activo = 0,
            Inactivo = 1
        }
        public enum StatusCode_OptionSet
        {
            Activo = 1,
            Inactivo = 2
        }
    }

    /// <summary>DisplayName: Subárea de conocimiento, OwnershipType: UserOwned, IntroducedVersion: 1.0.0.0</summary>
    public static class AenSubareadeconocimiento
    {
        public const string EntityName = "aen_subareadeconocimiento";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        /// <remarks>Identificador único de instancias de entidad</remarks>
        public const string PrimaryKey = "aen_subareadeconocimientoid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        /// <remarks>El nombre de la entidad personalizada.</remarks>
        public const string PrimaryName = "aen_name";
        /// <summary>Type: Lookup, RequiredLevel: ApplicationRequired, Targets: aen_areadeconocimiento</summary>
        public const string AenArea = "aen_area";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que creó el registro.</remarks>
        public const string CreatedBy = "createdby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que creó el registro.</remarks>
        public const string CreatedOnBehalfBy = "createdonbehalfby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Código de la zona horaria que estaba en uso cuando se creó el registro.</remarks>
        public const string UtcConversionTimezoneCode = "utcconversiontimezonecode";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Estado, OptionSetType: State</summary>
        /// <remarks>Estado del Subárea de conocimiento</remarks>
        public const string StateCode = "statecode";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se creó el registro.</remarks>
        public const string CreatedOn = "createdon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se migró el registro.</remarks>
        public const string OverriddenCreatedOn = "overriddencreatedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        /// <remarks>Fecha y hora en que se modificó el registro.</remarks>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario delegado que modificó el registro.</remarks>
        public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        /// <remarks>Identificador único del usuario que modificó el registro.</remarks>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        /// <remarks>Número de secuencia de la importación que creó este registro.</remarks>
        public const string ImportSequenceNumber = "importsequencenumber";
        /// <summary>Type: BigInt, RequiredLevel: None, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        /// <remarks>Número de versión</remarks>
        public const string VersionNumber = "versionnumber";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -1, MaxValue: 2147483647</summary>
        /// <remarks>Para uso interno.</remarks>
        public const string TimezoneruleVersionNumber = "timezoneruleversionnumber";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired</summary>
        /// <remarks>Tipo de identificador de propietario</remarks>
        public const string OwnerIdType = "owneridtype";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        /// <remarks>Id. del propietario</remarks>
        public const string OwnerId = "ownerid";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Razón para el estado, OptionSetType: Status</summary>
        /// <remarks>Razón para el estado del Subárea de conocimiento</remarks>
        public const string StatusCode = "statuscode";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: businessunit</summary>
        /// <remarks>Identificador único de la unidad de negocio propietaria del registro</remarks>
        public const string OwningBusinessUnit = "owningbusinessunit";
        /// <summary>Parent: "AenAreadeconocimiento" Child: "AenSubareadeconocimiento" Lookup: "AenArea"</summary>
        public const string RelM1_AenAreadeconocimientoAenSubareadeconocimientoArea = "aen_areadeconocimiento_aen_subareadeconocimiento_Area";
        /// <summary>Parent: "AenSubareadeconocimiento" Child: "AenCurso" Lookup: "AenSubareadeconocimiento"</summary>
        public const string Rel1M_AenSubareadeconocimientoAenCursoSubareadeconocimiento = "aen_subareadeconocimiento_aen_curso_Subareadeconocimiento";
        public enum StateCode_OptionSet
        {
            Activo = 0,
            Inactivo = 1
        }
        public enum OwnerIdType_OptionSet
        {
        }
        public enum StatusCode_OptionSet
        {
            Activo = 1,
            Inactivo = 2
        }
    }
}
