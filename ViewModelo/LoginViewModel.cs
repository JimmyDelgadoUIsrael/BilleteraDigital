using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using SQLite;

namespace BilleteraDigital.ViewModelo
{
    public class LoginViewModel
    {
        private SQLiteAsyncConnection _db;

        public LoginViewModel()
        {
            _db = new SQLiteAsyncConnection(Constante.DatabasePath, Constante.Flags);
            _db.CreateTableAsync<Usuario>();
        }

        public async Task<Usuario> IniciarSesionAsync(string correo, string contrasena)
        {
            var user = await _db.Table<Usuario>()
                .Where(u => u.Correo == correo && u.Contrasena == contrasena)
                .FirstOrDefaultAsync();
            return user;
        }


        public async Task RegistrarUsuarioAsync(string correo, string contrasena, string nombreUsuario)
        {
            await _db.InsertAsync(new Usuario
            {
                Correo = correo,
                Contrasena = contrasena,
                NombreUsuario = nombreUsuario

            });
        }
    }
}