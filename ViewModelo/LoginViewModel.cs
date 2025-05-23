using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilleteraDigital.Utilitario;
using SQLite;
using BilleteraDigital.Modelo;

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

        public async Task<bool> IniciarSesionAsync(string correo, string contrasena)
        {
            var user = await _db.Table<Usuario>()
                .Where(u => u.Correo == correo && u.Contrasena == contrasena)
                .FirstOrDefaultAsync();
            return user != null;
        }

        public async Task RegistrarUsuarioAsync(string correo, string contrasena)
        {
            await _db.InsertAsync(new Usuario
            {
                Correo = correo,
                Contrasena = contrasena
            });
        }
    }
}
