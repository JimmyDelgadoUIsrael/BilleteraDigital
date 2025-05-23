using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using System.Threading.Tasks;

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
                    fecha = DateTime.Now
                };
                await _db.AgregarTransaccionAsync(nueva);
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