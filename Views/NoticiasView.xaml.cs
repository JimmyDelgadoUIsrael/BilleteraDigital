using System; // Asegúrate de que System esté aquí
using System.Text.Json;
using System.Net.Http;
using System.Windows.Input; // Necesario para ICommand y Command
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel; // ¡MUY IMPORTANTE! Necesario para Launcher.OpenAsync
using BilleteraDigital.Modelo;

namespace BilleteraDigital.Views
{
    public partial class NoticiasView : ContentPage
    {
        public ICommand OpenUrlCommand { get; }

        public NoticiasView()
        {
            InitializeComponent();

            OpenUrlCommand = new Command<string>(async (url) =>
            {
                // **Paso 1: Verificar si la URL es nula o vacía**
                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        // **Paso 2: Intentar abrir la URL**
                        await Launcher.OpenAsync(url);
                    }
                    catch (Exception ex)
                    {
                        // **Paso 3: Capturar cualquier error al abrir la URL (por ejemplo, formato inválido)**
                        await DisplayAlert("Error al abrir URL", $"No se pudo abrir la URL: {url}. Detalles: {ex.Message}", "OK");
                        Console.WriteLine($"Error al abrir URL: {url}. Detalles: {ex.Message}");
                    }
                }
                else
                {
                    // **Paso 4: Mostrar una alerta si la URL no está disponible**
                    await DisplayAlert("URL no disponible", "Este artículo no tiene una URL válida para ver el contenido completo.", "OK");
                    Console.WriteLine("Intento de abrir URL nula o vacía.");
                }
            });

            LoadNewsData(); // Se llama a LoadNewsData para cargar las noticias
        }

        // ... El resto del código de LoadNewsData() debería estar aquí ...
        // ... (Tu código para LoadNewsData, que ya sabemos que funciona a veces, debería ir aquí) ...
        private async void LoadNewsData()
        {
            string apiKey = "f82ea0142aff43f8a8c6b96f9dee649f";
            string apiUrl = $"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MAUIApp/1.0"); // 👈 importante

                    var response = await client.GetAsync(apiUrl);
                    var content = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Status: {response.StatusCode}");
                    Console.WriteLine($"Response body: {content}");

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
                catch (HttpRequestException ex)
                {
                    await DisplayAlert("Error de Conexión", $"No se pudieron cargar las noticias. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error HTTP: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    await DisplayAlert("Error de Datos", $"No se pudo procesar la información. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error Inesperado", $"Ocurrió un error inesperado. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error General: {ex.Message}");
                }
            }
        }

    }
}