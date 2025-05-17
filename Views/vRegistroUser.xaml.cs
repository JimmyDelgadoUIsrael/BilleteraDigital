using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using SQLite;
using System.Threading.Tasks;

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
		};

		if (String.IsNullOrEmpty(nuevoUsuario.Correo) || string.IsNullOrEmpty(nuevoUsuario.Contrasena))
		{
			await DisplayAlert("Error", "Correo y contrase requeridos", "OK");
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

 
}