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

        [Unique, NotNull]
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        
    }
}
