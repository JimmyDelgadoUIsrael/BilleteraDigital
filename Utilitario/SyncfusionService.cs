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

            /*
             
            using var excelEngine = new ExcelEngine();
            var application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            var workbook = application.Workbooks.Create(1);
            var worksheet = workbook.Worksheets[0];
              worksheet.Name = "Transacciones";
            */
            using ExcelEngine excelEngine = new();
            Syncfusion.XlsIO.IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = "Transacciones";

            // --- TITULO BILLETERA DIGITAL ---
            worksheet.Range["A1:F1"].Merge();
            worksheet.Range["A1"].Text = "BILLETERA DIGITAL";
            worksheet.Range["A1"].CellStyle.Font.Bold = true;
            worksheet.Range["A1"].CellStyle.Font.Size = 18;
            worksheet.Range["A1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
            worksheet.Range["A1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
            worksheet.SetRowHeight(1, 30);

            // --- ENCABEZADOS (mover a fila 4 para dejar espacio para título y logo) ---
            worksheet.Range["A4"].Text = "ID";
            worksheet.Range["B4"].Text = "Tipo";
            worksheet.Range["C4"].Text = "Monto";
            worksheet.Range["D4"].Text = "Descripción";
            worksheet.Range["E4"].Text = "Fecha";
            worksheet.Range["F4"].Text = "Moneda";

            int fila = 5; // Datos empiezan en fila 5
            foreach (var t in transacciones)
            {
                worksheet.Range[$"A{fila}"].Number = t.id;
                worksheet.Range[$"B{fila}"].Text = t.tipo;

                double monto = t.tipo.ToLower().Contains("ingreso") ? (double)t.monto : -(double)t.monto;
                worksheet.Range[$"C{fila}"].Number = monto;

                worksheet.Range[$"D{fila}"].Text = t.descripcion;
                worksheet.Range[$"E{fila}"].DateTime = t.fecha;
                worksheet.Range[$"F{fila}"].Text = t.moneda;
                fila++;
            }

            worksheet.UsedRange.AutofitColumns();

            // Total (en la fila siguiente a los datos)
            worksheet.Range[$"B{fila}"].Text = "Total";
            worksheet.Range[$"C{fila}"].Formula = $"SUM(C5:C{fila - 1})";
            worksheet.Range[$"B{fila}:C{fila}"].CellStyle.Font.Bold = true;
            worksheet.Range[$"B{fila}:C{fila}"].CellStyle.NumberFormat = "\"$\"#,##0.00";

            // Crear tabla (desde encabezado hasta total)
            var rangoTabla = worksheet.Range[$"A4:F{fila}"];
            IListObject tabla = worksheet.ListObjects.Create("TablaTransacciones", rangoTabla);
            tabla.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium9;

            // Escribir resumen por tipo para gráfico
            int filaResumen = EscribirResumenPorTipo(worksheet, transacciones);

            // Solo agrega gráficos si hay datos
            if (filaResumen > 2)
            {
                AgregarGraficoMontosPorTipo(worksheet, fila);
                AgregarGraficoDonaMontosPorTipo(worksheet, fila);
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Transacciones.xlsx");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            return filePath;
        }



        private int EscribirResumenPorTipo(IWorksheet worksheet, List<Transaccion> transacciones)
        {
            double totalIngresos = transacciones
                .Where(t => t.tipo.ToLower().Contains("ingreso"))
                .Sum(t => (double)t.monto);

            double totalGastos = transacciones
                .Where(t => !t.tipo.ToLower().Contains("ingreso"))
                .Sum(t => (double)t.monto);

            double saldoNeto = totalIngresos - totalGastos;

            worksheet.Range["H1"].Text = "RESUMEN";
            worksheet.Range["H1"].CellStyle.Font.Bold = true;
            worksheet.Range["H1:I1"].Merge();

            worksheet.Range["H2"].Text = "Descripción";
            worksheet.Range["I2"].Text = "Monto";

            worksheet.Range["H3"].Text = "Total Ingresos";
            worksheet.Range["I3"].Number = totalIngresos;
            worksheet.Range["I3"].CellStyle.NumberFormat = "\"$\"#,##0.00";

            worksheet.Range["H4"].Text = "Total Gastos";
            worksheet.Range["I4"].Number = totalGastos;
            worksheet.Range["I4"].CellStyle.NumberFormat = "\"$\"#,##0.00";

            worksheet.Range["H5"].Text = "Saldo Neto";
            worksheet.Range["I5"].Number = saldoNeto;
            worksheet.Range["I5"].CellStyle.NumberFormat = "\"$\"#,##0.00";

            var rangoTabla = worksheet.Range["H2:I5"];
            IListObject tabla = worksheet.ListObjects.Create("TablaResumenFinanciero", rangoTabla);
            tabla.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium9;
            tabla.ShowTotals = false;

            worksheet.Range["H1:I5"].AutofitColumns();

            return 6;
        }

        private void AgregarGraficoDonaMontosPorTipo(IWorksheet worksheet, int filaFinal)
        {
            try
            {
                string dataRange = "H3:I4"; // Solo Ingresos y Gastos
                IChartShape chart = worksheet.Charts.Add();
                chart.ChartType = ExcelChartType.Doughnut;
                chart.DataRange = worksheet.Range[dataRange];
                chart.IsSeriesInRows = false;

                chart.ChartTitle = "Distribución de Ingresos vs Gastos";
                chart.ChartTitleArea.Bold = true;
                chart.ChartTitleArea.Size = 12;

                chart.TopRow = filaFinal + 2;
                chart.LeftColumn = 9;
                chart.BottomRow = filaFinal + 20;
                chart.RightColumn = 16;

                IChartSerie serie = chart.Series[0];
                serie.DataPoints[0].DataLabels.IsValue = true;
                serie.DataPoints[1].DataLabels.IsValue = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear gráfico de dona: " + ex.Message);
            }
        }



        private void AgregarGraficoMontosPorTipo(IWorksheet worksheet, int filaDatos)
        {
            try
            {
                string dataRange = "H3:I4"; // Solo Ingresos y Gastos

                IChartShape chart = worksheet.Charts.Add();
                chart.ChartType = ExcelChartType.Column_Clustered;
                chart.DataRange = worksheet.Range[dataRange];
                chart.IsSeriesInRows = false;

                chart.ChartTitle = "Ingresos vs Gastos";
                chart.ChartTitleArea.Bold = true;
                chart.ChartTitleArea.Size = 12;

                chart.TopRow = filaDatos + 2;
                chart.LeftColumn = 1;
                chart.BottomRow = filaDatos + 20;
                chart.RightColumn = 8;

                // Configurar etiquetas y colores
                IChartSerie serie = chart.Series[0];
                serie.DataPoints.DefaultDataPoint.DataLabels.IsValue = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear gráfico de barras: " + ex.Message);
            }
        }




        public async Task<string> ExportarTransaccionesAPdfAsync(List<Transaccion> transacciones)
        {
            using PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            // Cargar imagen del logo (desde recursos, asumiendo ruta relativa o embedded)
            // Ajusta la ruta según dónde esté ubicado el archivo en tu proyecto
            string logoPath = Path.Combine("Resource", "Images", "logotipo.png");
            PdfBitmap logo = null;

            if (File.Exists(logoPath))
            {
                using FileStream logoStream = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
                logo = new PdfBitmap(logoStream);
            }

            // Altura y posición para título y logo
            float yPosition = 10;

            // Dibujar fondo azul para el título (toda la parte superior)
            var titleBackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(0, 51, 102)); // azul oscuro
            graphics.DrawRectangle(titleBackgroundBrush, 0, yPosition, page.GetClientSize().Width, 40);

            // Dibujar título centrado
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
            string titleText = "BILLETERA DIGITAL";
            var titleSize = titleFont.MeasureString(titleText);
            float titleWidth = titleSize.Width;
            float titleHeight = titleSize.Height;

            float titleX = (page.GetClientSize().Width - titleWidth) / 2;
            float titleY = yPosition + (40 - titleHeight) / 2;
            graphics.DrawString(titleText, titleFont, PdfBrushes.White, new Syncfusion.Drawing.PointF(titleX, titleY));

            // Dibujar logo a la izquierda (si existe)
            if (logo != null)
            {
                float logoHeight = 30;
                float logoWidth = logo.Width * logoHeight / logo.Height; // mantiene proporción
                float logoX = 10;
                float logoY = yPosition + (40 - logoHeight) / 2;
                graphics.DrawImage(logo, logoX, logoY, logoWidth, logoHeight);
            }

            // Crear y configurar el grid, ajustar posición para que quede debajo del título
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

                BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(100, 149, 237)), // Azul claro
                TextBrush = PdfBrushes.White,
                Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold),
                StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle }
            };


            // Aplicar estilo a los encabezados
            foreach (PdfGridCell cell in pdfGrid.Headers[0].Cells)
            {
                cell.Style = headerStyle;
            }

            // Dibujar la tabla en el PDF con un margen superior para no sobreponer el título
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, yPosition + 50));




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
