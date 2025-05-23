namespace BilleteraDigital.Configuraciones
{
    public static class ConfiguracionUsuario
    {
        private const string MonedaClave = "moneda_base";

        public static string MonedaSeleccionada
        {
            get => Preferences.Get(MonedaClave, "USD");
            set => Preferences.Set(MonedaClave, value);
        }
    }
}
