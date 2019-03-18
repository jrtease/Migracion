using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionFormacion.Objetos
{
    public class SubareaDeConocimiento
    {
        public Guid aen_subareaconocimientoid { get; set; }
        public string aen_name { get; set; }
        public int statecode { get; set; }
        public int statuscode { get; set; }
        public Guid aen_areaconocimiento { get; set; }

        
        public void SubAreaConocimientoFromOracle(DataRow fila, Dictionary<string,Guid> MaestroAreas)
        {
            aen_subareaconocimientoid = Guid.Empty;
            aen_name = fila[AenSubareadeconocimiento.PrimaryName] == DBNull.Value ? string.Empty : fila[AenSubareadeconocimiento.PrimaryName].ToString().Trim();
            statecode = fila[AenSubareadeconocimiento.StateCode] == DBNull.Value ? 1 : (fila[AenSubareadeconocimiento.StateCode].ToString().Trim().Equals('S') ? 0 : 1);
            statuscode = statecode + 1;
            //TODO Enlazar con Area de conocimiento
            //aen_areaconocimiento = ;
        }
    }
}
