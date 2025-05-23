using BilleteraDigital.ViewModelo;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using System.Threading.Tasks;

namespace BilleteraDigital.Views;

public partial class vLogin : ContentPage
{
    private readonly LoginViewModel _viewModel = new();
    public vLogin()
    {
        InitializeComponent();
    }

    private  async void btnIngresar_Clicked(object sender, EventArgs e)
    {
        string correo = txtCorreo.Text;
        string contrasena = txtPassword.Text;

        bool valido = await _viewModel.IniciarSesionAsync(correo, contrasena);

        if (valido)
        {
            await DisplayAlert("Exito", "Inicio de sesion exitoso", "OK");

            await Navigation.PushAsync(new vInicio(correo));
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
            new AuthenticationRequestConfiguration("Autenticación requerida", "Escanea tu huella"));

        if (result.Authenticated)
        {
            await DisplayAlert("Éxito", "Autenticado correctamente", "OK");
            await Navigation.PushAsync(new vInicio("correo@ejemplo.com")); // puedes pasar el correo real si lo tienes
        }
        else
        {
            await DisplayAlert("Error", "La autenticación ha fallado", "OK");
        }
    }
  }