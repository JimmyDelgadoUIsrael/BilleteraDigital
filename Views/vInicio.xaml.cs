namespace BilleteraDigital.Views;

public partial class vInicio : ContentPage
{
	public vInicio( string correo)
	{
		InitializeComponent();
		lblBienvenida.Text = correo;
	}

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
		await Navigation.PopToRootAsync();
    }

    
}