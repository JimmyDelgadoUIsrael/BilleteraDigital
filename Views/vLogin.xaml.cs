namespace BilleteraDigital.Views;

public partial class vLogin : ContentPage
{
    public vLogin()
    {
        InitializeComponent();
    }

    private void btnIngresar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnRegistar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnAbout_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.vAbout());

    }
}