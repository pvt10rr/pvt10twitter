using System.Windows;
using System.Windows.Forms;

namespace Rapid_Reporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            Logger.record("[App]: >>>> Rapid Reporter Starting", "App", "info");

            // OS Version can be analyzed with the help of:
            //  http://stackoverflow.com/questions/545666/how-to-translate-ms-windows-os-version-numbers-into-product-names-in-net
            //  http://www.eggheadcafe.com/software/aspnet/35878122/how-to-determine-windows-flavor-in-net.aspx
            Logger.record("[App]: OS  Version: " + System.Environment.OSVersion, "App", "info");

            // Framework can be analyzed with the help of:
            //  http://en.wikipedia.org/wiki/List_of_.NET_Framework_versions
            Logger.record("[App]: Env version: " + System.Environment.Version, "App", "info");

            // Locale can be analyzed with the help of:
            //  http://msdn.microsoft.com/en-us/goglobal/bb964664.aspx
            //  http://msdn.microsoft.com/en-us/goglobal/bb895996
            Logger.record("[App]: Monitor num: " + InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId, "App", "info");

            Logger.record("[App]: Current Dir: " + System.Environment.CurrentDirectory, "App", "info");
            Logger.record("[App]: CommandLine: " + System.Environment.CommandLine, "App", "info");
            Logger.record("[App]: Network  On: " + SystemInformation.Network, "App", "info");
            Logger.record("[App]: Monitor num: " + SystemInformation.MonitorCount, "App", "info");
            Logger.record("[App]: WorkingArea: " + SystemInformation.WorkingArea, "App", "info");
        }
        ~App()
        {
            Logger.record("[App]: <<<< Rapid Reporter Ending", "App", "info");
            Logger.record("[App]: <<<< ====================================", "App", "info");
        }
    }
}
