using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using SQLite;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BilleteraDigital.Views;

public partial class vRegistroUser : ContentPage
{
	private SQLiteAsyncConnection _database;
	public vRegistroUser()
	{
		InitializeComponent();
		_database = new SQLiteAsyncConnection(Constante.DatabasePath, Constante.Flags);
		_database.CreateTableAsync<Usuario>();
	}
	
	private async void OnRegistrarClicked(object sender, EventArgs e)
	{
		var nuevoUsuario = new Usuario
		{
			Correo = txtCorreo.Text.Trim(),
			Contrasena = txtPassword.Text,
			NombreUsuario = txtNombreUsuario.Text.Trim(),

		};

		if (string.IsNullOrEmpty(nuevoUsuario.NombreUsuario) ||
			string.IsNullOrEmpty(nuevoUsuario.Correo) ||
			string.IsNullOrEmpty(nuevoUsuario.Contrasena))

        {
            await DisplayAlert("Error", "Correo y contraseña requeridos", "OK");
			return;
		}

		if (!EscorreoValido(nuevoUsuario.Correo))
		{
			await DisplayAlert("Error", "Por favor ingresa un correo valido", "OK");
			return;
		}
		try
		{
			await _database.InsertAsync(nuevoUsuario);
			await DisplayAlert("Exito", "Usuario registrado correctamente", "OK");
			await Navigation.PopAsync();
		}

		catch (Exception ex) 
		{
			await DisplayAlert("Error", $"No se pudo regsitrar: {ex.Message}", "OK");
		}
	}

    private async void OnAtras(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new vLogin());
    }

	private bool EscorreoValido(string correo)
	{
		if (string.IsNullOrWhiteSpace(correo))
			return false;

		try
		{
			var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
			return regex.IsMatch(correo);
		}
		catch
		{
			return false;
		}
	}

}