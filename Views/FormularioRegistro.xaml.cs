using BilleteraDigital.Modelo;
using BilleteraDigital.Utilitario;
using System.Threading.Tasks;

namespace BilleteraDigital.Views;

public partial class FormularioRegistro : ContentPage
{
    private readonly DatabaseService _db;
    public FormularioRegistro(DatabaseService db)
	{
		InitializeComponent();
        _db = db;
    }

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        if(string.IsNullOrWhiteSpace(TipoPicker.SelectedItem?.ToString()) ||
            string.IsNullOrWhiteSpace(MontoEntry.Text) ||
            !decimal.TryParse(MontoEntry.Text, out decimal monto))
        {
            await DisplayAlert("Error", "Complete todos los campos correctamente.", "OK");
            return;
        }

        var nueva = new Transaccion
        {
            tipo = TipoPicker.SelectedItem.ToString(),
            monto = monto,
            descripcion = DescripcionEntry.Text,
            fecha = DateTime.Now
        };

        await _db.AgregarTransaccionAsync(nueva);
        await Navigation.PopModalAsync(); // Vuelve a la página principal

    }

    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(); // Vuelve a la página principal
    }
}