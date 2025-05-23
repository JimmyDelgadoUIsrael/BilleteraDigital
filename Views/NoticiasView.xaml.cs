using System; // Aseg�rate de que System est� aqu�
using System.Text.Json;
using System.Net.Http;
using System.Windows.Input; // Necesario para ICommand y Command
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel; // �MUY IMPORTANTE! Necesario para Launcher.OpenAsync
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
                // **Paso 1: Verificar si la URL es nula o vac�a**
                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        // **Paso 2: Intentar abrir la URL**
                        await Launcher.OpenAsync(url);
                    }
                    catch (Exception ex)
                    {
                        // **Paso 3: Capturar cualquier error al abrir la URL (por ejemplo, formato inv�lido)**
                        await DisplayAlert("Error al abrir URL", $"No se pudo abrir la URL: {url}. Detalles: {ex.Message}", "OK");
                        Console.WriteLine($"Error al abrir URL: {url}. Detalles: {ex.Message}");
                    }
                }
                else
                {
                    // **Paso 4: Mostrar una alerta si la URL no est� disponible**
                    await DisplayAlert("URL no disponible", "Este art�culo no tiene una URL v�lida para ver el contenido completo.", "OK");
                    Console.WriteLine("Intento de abrir URL nula o vac�a.");
                }
            });

            LoadNewsData(); // Se llama a LoadNewsData para cargar las noticias
        }

        // ... El resto del c�digo de LoadNewsData() deber�a estar aqu� ...
        // ... (Tu c�digo para LoadNewsData, que ya sabemos que funciona a veces, deber�a ir aqu�) ...
        private async void LoadNewsData()
        {
            string apiKey = "f82ea0142aff43f8a8c6b96f9dee649f"; // Tu API Key
            string apiUrl = $"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(apiUrl);
                    var newsResponse = JsonSerializer.Deserialize<NewsResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    BindingContext = newsResponse;
                }
                catch (HttpRequestException ex)
                {
                    await DisplayAlert("Error de Conexi�n", $"No se pudieron cargar las noticias. Verifica tu conexi�n a internet o la API Key. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error HTTP al cargar noticias: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    await DisplayAlert("Error de Datos", $"No se pudo procesar la informaci�n de las noticias. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error de JSON al cargar noticias: {ex.Message}");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error Inesperado", $"Ocurri� un error inesperado al cargar las noticias. Detalles: {ex.Message}", "OK");
                    Console.WriteLine($"Error General al cargar noticias: {ex.Message}");
                }
            }
        }
    }
}