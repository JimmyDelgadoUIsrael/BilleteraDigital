using SQLite;

namespace BilleteraDigital.Modelo
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]

        public int id { get; set; }

        [Unique, NotNull]
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;

    }
}
