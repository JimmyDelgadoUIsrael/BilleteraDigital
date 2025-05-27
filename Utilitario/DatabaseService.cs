using BilleteraDigital.Modelo;
using SQLite;

namespace BilleteraDigital.Utilitario
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var config = new SQLiteConnectionString(Constante.DatabasePath, Constante.Flags, true);
            _db = new SQLiteAsyncConnection(config);
            _db.CreateTableAsync<Transaccion>().Wait();
            _db.CreateTableAsync<Ubicacion>();
        }

        public Task<int> AgregarTransaccionAsync(Transaccion t) =>
            _db.InsertAsync(t);

        public async Task<List<Transaccion>> ObtenerTransaccionesAsync()
        {
            var lista = await _db.Table<Transaccion>().ToListAsync();

            foreach (var t in lista)
                t.Ubicacion = await _db.Table<Ubicacion>()
                                       .Where(u => u.TransaccionId == t.id)
                                       .FirstOrDefaultAsync();

            return lista;
        }

        public Task<int> ActualizarTransaccionAsync(Transaccion t) =>
            _db.UpdateAsync(t);

        public Task<int> EliminarTransaccionAsync(Transaccion t) =>
            _db.DeleteAsync(t);

        public async Task<int> GuardarTransaccionAsync(Transaccion t, Ubicacion? ubicacion = null)
        {
            if (t.id != 0)
            {
                // Actualiza la transacción existente
                await _db.UpdateAsync(t);

                // También puedes actualizar la ubicación si existe (opcional)
                if (ubicacion != null)
                {
                    var existente = await _db.Table<Ubicacion>()
                                             .FirstOrDefaultAsync(u => u.TransaccionId == t.id);
                    if (existente != null)
                    {
                        // Actualiza coordenadas
                        existente.Latitud = ubicacion.Latitud;
                        existente.Longitud = ubicacion.Longitud;
                        await _db.UpdateAsync(existente);
                    }
                    else
                    {
                        // Agrega nueva ubicación si no había
                        ubicacion.TransaccionId = t.id;
                        await _db.InsertAsync(ubicacion);
                    }
                }

                return t.id;
            }
            else
            {
                // Inserta nueva transacción
                await _db.InsertAsync(t);

                // Si hay ubicación, la guarda vinculada
                if (ubicacion != null)
                {
                    ubicacion.TransaccionId = t.id;
                    await _db.InsertAsync(ubicacion);
                }

                return t.id;
            }
        }

        public async Task<Usuario> ObtenerUsuarioPorCorreoAsync(string correo)
        {
            var connection = new SQLiteAsyncConnection(Constante.DatabasePath, Constante.Flags);
            await connection.CreateTableAsync<Usuario>();
            return await connection.Table<Usuario>().Where(u => u.Correo == correo).FirstOrDefaultAsync();
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            return await _db.Table<Usuario>().ToListAsync();
        }

        public async Task EliminarTodoAsync()
        {
            await _db.DeleteAllAsync<Transaccion>();
            await _db.DeleteAllAsync<Usuario>();
            await _db.DeleteAllAsync<Ubicacion>();
            Preferences.Remove("UsuarioCorreo");
            Preferences.Remove("NombreUsuario");
        }
    }
}
