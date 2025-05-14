using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleteraDigital.Modelo
{
    internal class Usuario
    {
        [PrimaryKey, AutoIncrement]

        public int id { get; set; }

        [Unique]
        public string nombreusuario { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public string rol { get; set; } = string.Empty;
        public bool estado { get; set; } = true;
    }
}
