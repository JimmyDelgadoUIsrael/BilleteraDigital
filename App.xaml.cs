using BilleteraDigital.Utilitario;

namespace BilleteraDigital
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
<<<<<<< HEAD
=======
            var db = Services.GetRequiredService<DatabaseService>();
>>>>>>> Incorporación de login
            return new Window(new NavigationPage(new Views.vLogin()));
        }
    }
}