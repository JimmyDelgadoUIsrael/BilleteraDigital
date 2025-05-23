using BilleteraDigital.Configuraciones;
using BilleteraDigital.Servicio;
using BilleteraDigital.Utilitario;

namespace BilleteraDigital.Views;

public partial class vConfig : ContentPage
{
    private readonly DatabaseService _db;
    private readonly CurrencyService _currencyService;

    public vConfig(DatabaseService db, CurrencyService currencyService)
    {
        InitializeComponent();
        _db = db;
        _currencyService = currencyService;
        pckModena.SelectedItem = ConfiguracionUsuario.MonedaSeleccionada;
    }

    private void btnAbout_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.vAbout());
    }


    private async void btnCambioMoneda_Clicked(object sender, EventArgs e)
    {

        var monedaSeleccionada = pckModena.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(monedaSeleccionada)) return;

        // Si cambia la moneda, conviertes todos los ingresos/egresos existentes
        var monedaAnterior = ConfiguracionUsuario.MonedaSeleccionada;
        if (monedaSeleccionada != monedaAnterior)
        {
            decimal? tasaCambio = await _currencyService.GetExchangeRateAsync(monedaAnterior, monedaSeleccionada);
            if (tasaCambio != null)
            {
                var lista = await _db.ObtenerTransaccionesAsync();
                foreach (var t in lista)
                {
                    t.monto *= tasaCambio.Value;
                    t.moneda = monedaSeleccionada;
                    await _db.GuardarTransaccionAsync(t);
                }
            }
        }

        ConfiguracionUsuario.MonedaSeleccionada = monedaSeleccionada;
        await DisplayAlert("Configuración", $"Moneda actualizada a {monedaSeleccionada}", "OK");
    }
}