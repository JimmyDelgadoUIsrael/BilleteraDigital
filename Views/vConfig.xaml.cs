using BilleteraDigital.Configuraciones;
using BilleteraDigital.Utilitario;
using System.Collections.ObjectModel;

namespace BilleteraDigital.Views;

public partial class vConfig : ContentPage
{
    private readonly DatabaseService _db;
    private readonly CurrencyService _currencyService;
    public ObservableCollection<string> Monedas { get; set; } = new ObservableCollection<string>();


    public vConfig(DatabaseService db, CurrencyService currencyService)
    {
        InitializeComponent();
        _db = db;
        _currencyService = currencyService;

        BindingContext = this;

        _ = CargarMonedasAsync();
    }

    private void btnAbout_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.vAbout());
    }

    private async Task CargarMonedasAsync()
    {
        var monedasApi = await _currencyService.GetAvailableCurrenciesAsync();
        Monedas.Clear();

        foreach (var moneda in monedasApi)
            Monedas.Add(moneda);

        // Seleccionar la moneda guardada (si existe en la lista)
        if (Monedas.Contains(ConfiguracionUsuario.MonedaSeleccionada))
            pckModena.SelectedItem = ConfiguracionUsuario.MonedaSeleccionada;
    }

    private async void btnCambioMoneda_Clicked(object sender, EventArgs e)
    {

        var monedaSeleccionada = pckModena.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(monedaSeleccionada)) return;

        var monedaAnterior = ConfiguracionUsuario.MonedaSeleccionada;
        if (monedaSeleccionada != monedaAnterior)
        {
            decimal? tasaCambio = await _currencyService.GetExchangeRateAsync(monedaAnterior, monedaSeleccionada);
            if (tasaCambio != null)
            {
                lblTasaCambio.Text = $"Tasa de cambio {monedaAnterior} -> {monedaSeleccionada}: {tasaCambio.Value:F4}";

                var lista = await _db.ObtenerTransaccionesAsync();
                foreach (var t in lista)
                {
                    t.monto *= tasaCambio.Value;
                    t.moneda = monedaSeleccionada;
                    await _db.GuardarTransaccionAsync(t);
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la tasa de cambio.", "OK");
                return;
            }
        }
        else
        {
            lblTasaCambio.Text = "La moneda seleccionada es la misma.";
        }

        ConfiguracionUsuario.MonedaSeleccionada = monedaSeleccionada;
        await DisplayAlert("Configuración", $"Moneda actualizada a {monedaSeleccionada}", "OK");

    }
    private async void btnEliminarDatos_Clicked(object sender, EventArgs e)
    {
        bool confirmado = await DisplayAlert(
            "Confirmar",
            "¿Estás seguro de que deseas eliminar TODOS los datos?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmado)
            return;

        try
        {
            await _db.EliminarTodoAsync();
            await DisplayAlert("Éxito", "Todos los datos han sido eliminados.", "OK");
            await Navigation.PushAsync(new vLogin());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
}