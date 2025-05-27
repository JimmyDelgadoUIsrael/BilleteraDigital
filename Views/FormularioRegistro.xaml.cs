using BilleteraDigital.Configuraciones;
using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
namespace BilleteraDigital.Views;



public partial class FormularioRegistro : ContentPage
{
    private readonly DatabaseService _db;
    private Transaccion _transaccionExistente;
    public FormularioRegistro(DatabaseService db, Transaccion? transaccion = null)
    {
        InitializeComponent();
        _db = db;
        _transaccionExistente = transaccion;

        MontoEntry.Placeholder = $"0.00 {ConfiguracionUsuario.MonedaSeleccionada}";

        if (_transaccionExistente != null)
        {
            TipoPicker.SelectedItem = _transaccionExistente.tipo;
            DescripcionEntry.Text = _transaccionExistente.descripcion;
            MontoEntry.Text = _transaccionExistente.monto.ToString();
        }
    }

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TipoPicker.SelectedItem?.ToString()) ||
            string.IsNullOrWhiteSpace(MontoEntry.Text) ||
            !decimal.TryParse(MontoEntry.Text, out decimal monto))
        {
            await DisplayAlert("Error", "Complete todos los campos correctamente.", "OK");
            return;
        }

        try
        {
            Ubicacion? ubic = null;

            // Solo si es transacción nueva y el Check está marcado
            if (_transaccionExistente == null && chkUbicacion.IsChecked)
            {
                var permiso = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (permiso == PermissionStatus.Granted)
                {
                    var pos = await Geolocation.GetLocationAsync(
                                  new GeolocationRequest(GeolocationAccuracy.Medium));
                    if (pos != null)
                    {
                        ubic = new Ubicacion
                        {
                            Latitud = pos.Latitude,
                            Longitud = pos.Longitude
                        };
                    }
                }
            }

            if (_transaccionExistente != null)
            {
                _transaccionExistente.tipo = TipoPicker.SelectedItem.ToString();
                _transaccionExistente.descripcion = DescripcionEntry.Text;
                _transaccionExistente.monto = monto;

                await _db.ActualizarTransaccionAsync(_transaccionExistente);
            }
            else
            {
                var nueva = new Transaccion
                {
                    tipo = TipoPicker.SelectedItem.ToString(),
                    descripcion = DescripcionEntry.Text,
                    monto = monto,
                    fecha = DateTime.Now,
                    moneda = ConfiguracionUsuario.MonedaSeleccionada
                };

                await _db.GuardarTransaccionAsync(nueva, ubic); // con ubicación opcional
            }

            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
    }


    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(); // Vuelve a la página principal
    }
}