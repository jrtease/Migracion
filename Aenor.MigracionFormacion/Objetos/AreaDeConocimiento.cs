using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionFormacion.Objetos
{
    public class AreaDeConocimiento
    {

        public Guid aen_areaconocimientoid { get; set; }
        public string aen_name { get; set; }
        public int statecode { get; set; }
        public int statuscode { get; set; }



        public void AreaConocimientoFromOracle(DataRow fila)
        {
            aen_areaconocimientoid = Guid.Empty;
            aen_name = fila[AenAreadeconocimiento.PrimaryName] == DBNull.Value ? string.Empty : fila[AenAreadeconocimiento.PrimaryName].ToString().Trim();
            statecode = fila[AenAreadeconocimiento.StateCode] == DBNull.Value ? 1 : (fila[AenAreadeconocimiento.StateCode].ToString().Trim().Equals('S') ? 0 : 1);
            statuscode = statecode + 1;
        }
    }
}
