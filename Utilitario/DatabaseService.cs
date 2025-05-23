using BilleteraDigital.Modelo;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleteraDigital.Utilitario
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var config = new SQLiteConnectionString(Constante.DataBasePath, Constante.Flags, true);
            _db = new SQLiteAsyncConnection(config);
            _db.CreateTableAsync<Transaccion>().Wait(); // asegúrate de haber creado la clase Transaccion
        }

        public Task<int> AgregarTransaccionAsync(Transaccion t) =>
            _db.InsertAsync(t);

        public Task<List<Transaccion>> ObtenerTransaccionesAsync() =>
            _db.Table<Transaccion>().OrderByDescending(t => t.fecha).ToListAsync();

        public Task<int> ActualizarTransaccionAsync(Transaccion t) =>
            _db.UpdateAsync(t);

        public Task<int> EliminarTransaccionAsync(Transaccion t) =>
            _db.DeleteAsync(t);

    }
}
