using SQLite;

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
        public string moneda { get; set; } = "USD"; // Moneda por defecto
        [Ignore]
        public string montoMoneda => $"{monto:C} {moneda}";
        public int idUsuario { get; set; } // ID del usuario al que pertenece la transacción
                                           // navegación opcional
        [Ignore] public Ubicacion? Ubicacion { get; set; }
        [Ignore]
        public string UbicacionTexto =>
    Ubicacion != null
        ? $"📍 {Ubicacion.Latitud:F4}, {Ubicacion.Longitud:F4}"
        : ""; // o "Sin ubicación"
    }
}
