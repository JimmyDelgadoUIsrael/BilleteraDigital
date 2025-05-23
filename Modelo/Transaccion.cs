using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleteraDigital.Modelo
{
    public class Transaccion
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string tipo { get; set; } = string.Empty; // "Ingreso" o "Egreso"
        public decimal monto { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public DateTime fecha { get; set; } = DateTime.Now;
    }
}
