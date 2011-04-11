using System.Windows;

namespace Rapid_Reporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Logger.record("[App]: >>>> Rapid Reporter Starting", "App", "info");
        }
        ~App()
        {
            Logger.record("[App]: <<<< Rapid Reporter Ending", "App", "info");
            Logger.record("[App]: <<<< ====================================", "App", "info");
        }
    }
}
