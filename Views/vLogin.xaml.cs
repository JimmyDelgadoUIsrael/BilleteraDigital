using BilleteraDigital.Utilitario;
using BilleteraDigital.ViewModelo;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;




namespace BilleteraDigital.Views;

public partial class vLogin : ContentPage
{
    private readonly LoginViewModel _viewModel = new();
    private DatabaseService db;

    public vLogin()
    {
        InitializeComponent();
    }

    private async void btnIngresar_Clicked(object sender, EventArgs e)
    {
        string correo = txtCorreo.Text;
        string contrasena = txtPassword.Text;
        var usuario = await _viewModel.IniciarSesionAsync(correo, contrasena);
        if (usuario != null)
        {
            Preferences.Set("UsuarioCorreo", usuario.Correo);
            Preferences.Set("NombreUsuario", usuario.NombreUsuario);
            await DisplayAlert("Exito", "Inicio de sesion exitoso", "OK");
            await DisplayAlert("Exito", $"Bienvenido \n{usuario.NombreUsuario}", "OK");
            var db = App.Services.GetService<DatabaseService>();
            await Navigation.PushAsync(new vInicio(db, correo));
        }
        else
        {
            await DisplayAlert("Error", "Credenciales incorrectas", "OK");
        }

    }


    private async void OnCrearCuentaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vRegistroUser());
    }


    private async void OnHuellaClicked(object sender, EventArgs e)
    {
        AutenticarHuella();
    }

    private async void AutenticarHuella()
    {
        var disponible = await CrossFingerprint.Current.IsAvailableAsync();
        if (!disponible)
        {
            await DisplayAlert("No disponible", "La autenticación por huella no está disponible.", "OK");
            return;
        }

        var result = await CrossFingerprint.Current.AuthenticateAsync(
            new AuthenticationRequestConfiguration("Autenticación requerida ", "Escanea tu huella"));

        if (result.Authenticated)
        {
            var correoGuardado = Preferences.Get("UsuarioCorreo", null);

            if (!string.IsNullOrEmpty(correoGuardado))
            {
                var db = App.Services.GetService<DatabaseService>();
                await DisplayAlert("Éxito", "Autenticado correctamente", "OK");
                await Navigation.PushAsync(new vInicio(db, correoGuardado)); // puedes pasar el correo real si lo tienes
            }
            else
            {
                await DisplayAlert("Sin sesion", "No se encontro usuario vinculado a esta huella", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "La Autenticacion ha fallado", "OK");
        }
    }

    private async void OnOlvideContraseñaTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new vRestablecerContrasena());
    }

    private async void btnAcerca_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vAbout());
    }
}