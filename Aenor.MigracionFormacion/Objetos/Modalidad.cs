using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aenor.MigracionFormacion.Objetos
{
    public class Modalidad
    {
        public Guid aen_modalidadid { get; set; }
        public string aen_codigomodalidad { get; set; }
        public string aen_name { get; set; }

        public void ModalidadFromOracle(DataRow fila)
        {
            aen_modalidadid = Guid.Empty;
            aen_codigomodalidad = fila[AenModalidad.AenCodigomodalidad] == DBNull.Value ? string.Empty : fila[AenModalidad.AenCodigomodalidad].ToString().Trim();
            aen_name = fila[AenModalidad.PrimaryName] == DBNull.Value ? string.Empty : fila[AenModalidad.PrimaryName].ToString().Trim();
        }
    }
}
