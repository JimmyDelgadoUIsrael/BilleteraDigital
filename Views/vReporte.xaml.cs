using BilleteraDigital.Utilitario;
using System.Diagnostics;

namespace BilleteraDigital.Views;

public partial class vReporte : ContentPage
{
    private readonly DatabaseService _db;
    public vReporte(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
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
        var rutaPdf = await exportador.ExportarTransaccionesAPdfAsync(transacciones);
        await DisplayAlert("Exportado", $"PDF guardado en:\n{rutaPdf}", "OK");
    }
}