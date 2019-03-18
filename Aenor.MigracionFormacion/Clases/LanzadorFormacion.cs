using Aenor.MigracionFormacion.Objetos;
using Aenor.MigracionFormacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Aenor.MigracionFormacion.Clases
{
    public class LanzadorFormacion
    {
        public Crm CrmGlobal { get; set; }
        public Comun ComunGlobal { get; set; }
        public Oracle OracleGlobal { get; set; }

        public Dictionary<string, Curso> CursosCRM, CursosOracle;

        public Dictionary<string, Guid> CodigosModalidades;
        public Dictionary<string, Guid> CodigosAreas;
        public Dictionary<string, Guid> CodigosSubareas;
        public Dictionary<string, Guid> CodigosCursos;

        public void Iniciar(Oracle ora, Comun com, Crm crm)
        {
            try
            {
                OracleGlobal = ora;
                ComunGlobal = com;
                CrmGlobal = crm;

                ComunGlobal.LogText("*************************************************************************");

                //1. Cargar Modalidades
                CodigosModalidades = new LanzadorModalidades().Iniciar(OracleGlobal, ComunGlobal, CrmGlobal);
                ComunGlobal.LogText("*************************************************************************");


                //2. Áreas de conocimiento
                CodigosAreas = new LanzadorAreasConocimiento().Iniciar(OracleGlobal, ComunGlobal, CrmGlobal);
                ComunGlobal.LogText("*************************************************************************");


                //3. Subáreas de conocimiento
                CodigosSubareas = new LanzadorSubAreasConocimiento().Iniciar(OracleGlobal, ComunGlobal, CrmGlobal, CodigosAreas);
                ComunGlobal.LogText("*************************************************************************");


                //4. Cursos



                //5. Convocatorias

                //TODO INSCRIPCIONES: Ver cómo cruzar con tercero, coordinador (contact), sitio (sala) y dirección


                ComunGlobal.LogText("----   FIN CARGA   ------");
            }
            catch (Exception e)
            {
                ComunGlobal.LogText("ERROR en Lanzador de MODALIDADES ::: " + e.ToString());
                if (OracleGlobal != null && OracleGlobal.OraConnParaLog.State == ConnectionState.Open)
                    OracleGlobal.OraConnParaLog.Dispose();
            }
        }




        private void LeerCursosFromOracle()
        {

        }

        private void LeerCursosFromCRM()
        {

        }
    }
}

