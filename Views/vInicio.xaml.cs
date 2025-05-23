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

        decimal totalIngreso = lista.Where(t => t.tipo == "Ingreso").Sum(t => t.monto);
        decimal totalGasto = lista.Where(t => t.tipo == "Gasto").Sum(t => t.monto);

        lblTotalIngresos.Text = $"Total Ingresos: {totalIngreso:C}";
        lblTotalGastos.Text = $"Total Gastos: {totalGasto:C}";
    }
    private async void btnNuevo_Clicked(object sender, EventArgs e)
    {
        var formulario = new FormularioRegistro(_db);
        await Navigation.PushModalAsync(new FormularioRegistro(_db));
    }

    private async void btnEditar_Clicked(object sender, EventArgs e)
    {
        var transaccion = (sender as Button)?.CommandParameter as Modelo.Transaccion;
        if (transaccion != null)
        {
            var formulario = new FormularioRegistro(_db, transaccion); // Sobrecarga con edición
            await Navigation.PushModalAsync(formulario);
        }

    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        var transaccion = (sender as Button)?.CommandParameter as Modelo.Transaccion;
        if (transaccion != null)
        {
            var confirm = await DisplayAlert("Confirmar", "¿Eliminar esta transacción?", "Sí", "No");
            if (confirm)
            {
                await _db.EliminarTransaccionAsync(transaccion);
                await CargarTransaccionesAsync();
            }
        }
    }
}