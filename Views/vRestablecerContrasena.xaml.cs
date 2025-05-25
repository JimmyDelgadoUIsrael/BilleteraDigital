using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using SQLite;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BilleteraDigital.Views;

public partial class vRestablecerContrasena : ContentPage
{
    private SQLiteAsyncConnection _database;
    public vRestablecerContrasena()
	{
		InitializeComponent();
        _database = new SQLiteAsyncConnection(Constante.DatabasePath, Constante.Flags);
    }

    private async void OnRestablecerClicked(object sender, EventArgs e)
    {
        var correo = txtCorreo.Text.Trim();
        var nuevaContrasena = txtNuevaContrasena.Text;

        if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(nuevaContrasena))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        var usuario = await _database.Table<Usuario>().Where(u => u.Correo == correo).FirstOrDefaultAsync();

        if (usuario != null)
        {
            usuario.Contrasena = nuevaContrasena;
            await _database.UpdateAsync(usuario);
            await DisplayAlert("Éxito", "Contraseña actualizada correctamente", "OK");
            await Navigation.PopAsync(); // Volver al login
        }
        else
        {
            await DisplayAlert("Error", "No se encontró un usuario con ese correo", "OK");
        }
    }
}