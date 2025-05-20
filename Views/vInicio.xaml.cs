using BilleteraDigital.Utilitario;
using System.Threading.Tasks;

namespace BilleteraDigital.Views;

public partial class vInicio : ContentPage
{
	private readonly DatabaseService _db;
    public vInicio(DatabaseService db)
	{
		InitializeComponent();
        _db = db;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarTransaccionesAsync();
    }
    private async Task CargarTransaccionesAsync()
    {
        var lista = await _db.ObtenerTransaccionesAsync();
        TransaccionesView.ItemsSource = lista;
    }
    private async void btnNuevo_Clicked(object sender, EventArgs e)
    {
        var formulario = new FormularioRegistro(_db);
        await Navigation.PushModalAsync(new FormularioRegistro(_db));
    }
}