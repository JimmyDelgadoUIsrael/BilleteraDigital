using BilleteraDigital.Modelo;
using System.Text.Json;
using System.Windows.Input; // Necesario para ICommand y Command

namespace BilleteraDigital.Views
{
    public partial class NoticiasView : ContentPage
    {
        public ICommand OpenUrlCommand { get; }

        public NoticiasView()
        {
            InitializeComponent();
            CategoriaPicker.SelectedIndex = 0;

            // Opcional: disparar el evento para cargar noticias inmediatamente
            OnCategoriaChanged(CategoriaPicker, null);

            OpenUrlCommand = new Command<string>(async (url) =>
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        await Launcher.OpenAsync(url);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error al abrir URL", $"No se pudo abrir la URL: {url}. Detalles: {ex.Message}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("URL no disponible", "Este artículo no tiene una URL válida para ver el contenido completo.", "OK");
                }
            });

            // Cargar noticias por defecto (economía)
            LoadNewsData("economía");
        }


        // ... El resto del código de LoadNewsData() debería estar aquí ...
        // ... (Tu código para LoadNewsData, que ya sabemos que funciona a veces, debería ir aquí) ...
        private void OnCategoriaChanged(object sender, EventArgs e)
        {
            if (CategoriaPicker.SelectedIndex != -1)
            {
                string categoriaSeleccionada = CategoriaPicker.Items[CategoriaPicker.SelectedIndex];
                LoadNewsData(categoriaSeleccionada);
            }
        }
        private async void LoadNewsData(string categoria)
        {
            string apiKey = "f82ea0142aff43f8a8c6b96f9dee649f";
            string apiUrl = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(categoria)}&language=es&sortBy=publishedAt&apiKey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MAUIApp/1.0");

                    var response = await client.GetAsync(apiUrl);
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var newsResponse = JsonSerializer.Deserialize<NewsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        BindingContext = newsResponse;
                    }
                    else
                    {
                        await DisplayAlert("Error de API", $"Código de estado: {response.StatusCode}\nContenido: {content}", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Detalles: {ex.Message}", "OK");
                }
            }
        }


    }
}