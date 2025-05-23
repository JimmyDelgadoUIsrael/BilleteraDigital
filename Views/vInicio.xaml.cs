
using BilleteraDigital.Configuraciones;
using BilleteraDigital.Utilitario;

namespace BilleteraDigital.Views;

public partial class vInicio : ContentPage
{
    private readonly DatabaseService _db;
    public vInicio(DatabaseService db, String correo)
    {
        InitializeComponent();
        _db = db;
        lblUsuario.Text = correo.ToUpper();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarTransaccionesAsync();

        string monedaGuardada = ConfiguracionUsuario.MonedaSeleccionada;
        lblMoneda.Text = "Modena Selecionada: " + monedaGuardada;

    }
    private async Task CargarTransaccionesAsync()
    {
        if (_db == null)
        {
            await DisplayAlert("Error", "No se pudo cargar la base de datos. El servicio no está disponible.", "Aceptar");
            return;
        }

        var lista = await _db.ObtenerTransaccionesAsync();

        if (lista == null)
        {
            await DisplayAlert("Advertencia", "No se pudieron cargar las transacciones.", "Aceptar");
            return;
        }

        TransaccionesView.ItemsSource = lista;

        decimal totalIngreso = lista.Where(t => t.tipo == "Ingreso").Sum(t => t.monto);
        decimal totalGasto = lista.Where(t => t.tipo == "Gasto").Sum(t => t.monto);
        decimal total = totalIngreso - totalGasto;

        lblTotalIngresos.Text = $"Total Ingresos: {totalIngreso:C}";
        lblTotalGastos.Text = $"Total Gastos: {totalGasto:C}";
        lblTotal.Text = $"Total: {total:C}";

        if (total <= 0)
        {
            DisplayAlert(Title, "No tienes dinero disponible", "Aceptar");
        }
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
            var formulario = new FormularioRegistro(_db, transaccion); // Sobrecarga con edici�n
            await Navigation.PushModalAsync(formulario);
        }

    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        var transaccion = (sender as Button)?.CommandParameter as Modelo.Transaccion;
        if (transaccion != null)
        {
            var confirm = await DisplayAlert("Confirmar", "Eliminar esta transacciín?", "Sí", "No");
            if (confirm)
            {
                await _db.EliminarTransaccionAsync(transaccion);
                await CargarTransaccionesAsync();
            }
        }
    }

    private void btnNoticia_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NoticiasView());
    }

    private async void btnConfig_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vConfig(_db, new CurrencyService()));
    }

    private void btnExport_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vReporte(_db));
    }
}