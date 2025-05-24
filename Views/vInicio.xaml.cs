using BilleteraDigital.Configuraciones;
using BilleteraDigital.Utilitario;

namespace BilleteraDigital.Views;

public partial class vInicio : ContentPage
{
    private readonly DatabaseService _db;
    private List<Modelo.Transaccion> _transacciones = new();
    private List<Modelo.Transaccion> _transaccionesFiltradas = new();

    public vInicio(DatabaseService db, string correo)
    {
        InitializeComponent();
        _db = db;
        lblUsuario.Text = correo.ToUpper();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarTransaccionesAsync();
        lblMoneda.Text = $"Moneda Seleccionada: {ConfiguracionUsuario.MonedaSeleccionada}";
    }

    private async Task CargarTransaccionesAsync()
    {
        if (_db == null)
        {
            await MostrarMensaje("Error", "No se pudo cargar la base de datos.");
            return;
        }

        _transacciones = await _db.ObtenerTransaccionesAsync() ?? new();
        _transaccionesFiltradas = new(_transacciones);

        TransaccionesView.ItemsSource = _transaccionesFiltradas;
        ActualizarTotales(_transaccionesFiltradas);
    }

    private void ActualizarTotales(IEnumerable<Modelo.Transaccion> lista)
    {
        decimal ingresos = lista.Where(t => t.tipo == "Ingreso").Sum(t => t.monto);
        decimal gastos = lista.Where(t => t.tipo == "Gasto").Sum(t => t.monto);
        decimal total = ingresos - gastos;

        lblTotalIngresos.Text = $"Total Ingresos: {ingresos:C}";
        lblTotalGastos.Text = $"Total Gastos: {gastos:C}";
        lblTotal.Text = $"Total: {total:C}";
    }

    private async Task MostrarMensaje(string titulo, string mensaje) =>
        await DisplayAlert(titulo, mensaje, "Aceptar");

    private async Task NavegarAsync(Page pagina) =>
        await Navigation.PushAsync(pagina);

    private async void btnNuevo_Clicked(object sender, EventArgs e) =>
        await Navigation.PushModalAsync(new FormularioRegistro(_db));

    private async void btnEditar_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Modelo.Transaccion transaccion)
            await Navigation.PushModalAsync(new FormularioRegistro(_db, transaccion));
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is not Modelo.Transaccion transaccion) return;

        if (await DisplayAlert("Confirmar", "¿Eliminar esta transacción?", "Sí", "No"))
        {
            await _db.EliminarTransaccionAsync(transaccion);
            await CargarTransaccionesAsync();
        }
    }

    private async void btnConfig_Clicked(object sender, EventArgs e) =>
        await NavegarAsync(new vConfig(_db, new CurrencyService()));

    private void btnExport_Clicked(object sender, EventArgs e) =>
        Navigation.PushAsync(new vReporte(_db));

    private void btnNoticia_Clicked(object sender, EventArgs e) =>
        Navigation.PushAsync(new NoticiasView());

    private void OnBuscarTexto(object sender, TextChangedEventArgs e) =>
        FiltrarTransacciones();

    private void OnFiltroSeleccionado(object sender, EventArgs e) =>
        FiltrarTransacciones();

    private void FiltrarTransacciones()
    {
        string texto = BuscarTexto.Text?.ToLower() ?? "";
        string tipo = pickerFiltro.SelectedItem?.ToString();

        var resultado = _transacciones.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(tipo) && tipo != "Todos")
            resultado = resultado.Where(t => t.tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(texto))
            resultado = resultado.Where(t => t.descripcion.ToLower().Contains(texto));

        _transaccionesFiltradas = resultado.ToList();
        TransaccionesView.ItemsSource = _transaccionesFiltradas;
        ActualizarTotales(_transaccionesFiltradas);
    }
}
