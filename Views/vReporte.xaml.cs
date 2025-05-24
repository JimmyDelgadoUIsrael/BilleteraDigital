using BilleteraDigital.Utilitario;
using System.Collections.ObjectModel;
using System.Diagnostics;



namespace BilleteraDigital.Views;

public partial class vReporte : ContentPage
{
    private readonly DatabaseService _db;
    public ObservableCollection<TransaccionResumen> TransaccionesResumen { get; set; } = new();


    public vReporte(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var transacciones = await _db.ObtenerTransaccionesAsync();

        // Agrupar por tipo (Ingreso/Egreso)
        var resumen = transacciones
            .GroupBy(t => t.tipo)
            .Select(g => new TransaccionResumen
            {
                Tipo = g.Key,
                Monto = g.Sum(t => t.monto)
            })
            .ToList();

        TransaccionesResumen.Clear();
        foreach (var item in resumen)
            TransaccionesResumen.Add(item);

    }


    public class TransaccionResumen
    {
        public string Tipo { get; set; } = string.Empty; // "Ingreso" o "Egreso"
        public decimal Monto { get; set; }
    }



    private async void btnExporteExcel_Clicked(object sender, EventArgs e)
    {
        try
        {
            // 1. Obtener transacciones
            var transacciones = await _db.ObtenerTransaccionesAsync();

            if (transacciones == null || transacciones.Count == 0)
            {
                await DisplayAlert("Sin datos", "No hay transacciones para exportar.", "OK");
                return;
            }

            // 2. Instanciar el servicio
            var exportador = new SyncfusionService();

            // 3. Exportar a Excel
            var rutaArchivo = await exportador.ExportarTransaccionesAExcelAsync(transacciones);

            // 4. Mostrar mensaje o abrir archivo (Windows/Android)
#if WINDOWS
            Process.Start("explorer.exe", rutaArchivo);
#elif ANDROID
        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(rutaArchivo)
        });
#endif

            await DisplayAlert("Éxito", $"Archivo Excel guardado en:\n{rutaArchivo}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Hubo un problema al exportar: {ex.Message}", "OK");
        }
    }

    private async void btnExportePdf_Clicked(object sender, EventArgs e)
    {
        var transacciones = await _db.ObtenerTransaccionesAsync();
        var exportador = new SyncfusionService();
        //var rutaPdf = await exportador.ExportarTransaccionesAPdfAsync(transacciones);
        var rutaPdf = await exportador.ExportarTransaccionesAPdfAsync(transacciones);
        // Abrir el archivo directamente
        if (File.Exists(rutaPdf))
        {
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(rutaPdf)
            });
        }
        else
        {
            await DisplayAlert("Error", "No se pudo encontrar el archivo PDF.", "OK");
        }
    }
}