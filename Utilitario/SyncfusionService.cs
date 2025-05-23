using BilleteraDigital.Modelo;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.XlsIO;



namespace BilleteraDigital.Utilitario
{
    public class SyncfusionService
    {

        public async Task<string> ExportarTransaccionesAExcelAsync(List<Transaccion> transacciones)
        {
            // Crea el motor de Excel
            using var excelEngine = new ExcelEngine();
            var application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            // Crea un libro nuevo y hoja
            var workbook = application.Workbooks.Create(1);
            var worksheet = workbook.Worksheets[0];
            worksheet.Name = "Transacciones";

            // Escribe encabezados
            worksheet.Range["A1"].Text = "ID";
            worksheet.Range["B1"].Text = "Tipo";
            worksheet.Range["C1"].Text = "Monto";
            worksheet.Range["D1"].Text = "Descripción";
            worksheet.Range["E1"].Text = "Fecha";
            worksheet.Range["F1"].Text = "Moneda";

            // Escribe datos
            int fila = 2;
            foreach (var t in transacciones)
            {
                worksheet.Range[$"A{fila}"].Number = t.id;
                worksheet.Range[$"B{fila}"].Text = t.tipo;
                worksheet.Range[$"C{fila}"].Number = (double)t.monto;
                worksheet.Range[$"D{fila}"].Text = t.descripcion;
                worksheet.Range[$"E{fila}"].DateTime = t.fecha;
                worksheet.Range[$"F{fila}"].Text = t.moneda;
                fila++;
            }

            // Opcional: ajusta ancho de columnas automáticamente
            worksheet.UsedRange.AutofitColumns();

            // Guarda archivo en memoria
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Guarda en archivo físico (ejemplo, para MAUI usa Path apropiado)
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Transacciones.xlsx");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            return filePath; // Retorna ruta para abrir o compartir
        }

        public async Task<string> ExportarTransaccionesAPdfAsync(List<Transaccion> transacciones)
        {
            using PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            // Crear y configurar el grid
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = transacciones.Select(t => new
            {
                ID = t.id,
                Tipo = t.tipo,
                Monto = t.monto.ToString("F2"),
                Descripción = t.descripcion,
                Fecha = t.fecha.ToString("dd/MM/yyyy"),
                Moneda = t.moneda
            }).ToList();

            // Estilo para el encabezado
            PdfGridCellStyle headerStyle = new PdfGridCellStyle
            {
                BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(200, 0, 0)), // Color rojo
                TextBrush = PdfBrushes.White,
                Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
            };

            // Aplicar estilo a los encabezados
            foreach (PdfGridCell cell in pdfGrid.Headers[0].Cells)
            {
                cell.Style = headerStyle;
            }

            // Dibujar la tabla en el PDF
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 0));

            // Guardar en un MemoryStream
            using MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            // Guardar archivo en disco (seguro para MAUI)
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "Transacciones.pdf");
            using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);

            return filePath;
        }

    }
}
