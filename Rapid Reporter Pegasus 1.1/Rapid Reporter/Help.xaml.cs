using System.Diagnostics;
using System.Windows;

namespace Rapid_Reporter
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// Generic help dialog
    /// </summary>
    public partial class Help : Window
    {
        // Constructor
        //  We set the application name, version, and dialog title.
        public Help()
        {
            Logger.record("[Help]: Starting help dialog. Initializing component.", "HelpDlg", "info");
            InitializeComponent();
            this.Title = System.Windows.Forms.Application.ProductName + " - Help";
            this.appName.Content = System.Windows.Forms.Application.ProductName;
            this.appVers.Content = System.Windows.Forms.Application.ProductVersion;
            Ok.Focus();
        }

        // The dialog is generic.
        // We load a different text every time, and two functions are provided: One to load the text from the pre-defined list of messages,
        //  and one to load a short text, on demand. Both simply write to "this.helpApp.Text".
        public void LoadText(int helpType)
        {
            Logger.record("[LoadText i]: Loading text by type, #" + helpType.ToString(), "HelpDlg", "info");
            // Help is:
            // 0 - Normal help inside application
            // 1 - -help help
            // 2 - -report help
            // 3 - -tohtml help
            this.helpApp.Text = longstrings.helpstrings.helpstring[helpType];
        }
        public void LoadText(string helpText)
        {
            Logger.record("[LoadText s]: Loading text by text", "HelpDlg", "info");
            this.helpApp.Text = helpText;
        }

        // Loaded and Unloaded event
        // When we load the window, we focus the Ok button (to close the dialog quickly).
        // When we unload the window, we delete the help text, to leave it blank for the next time.
        //  Everytime the dialog overwrites the text, but by cleaning it we protect ourselves from a possible ugly bug of showing the wrong text.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.record("[Window_Loaded]: Loading help window", "HelpDlg", "info");
            Ok.Focus();
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.helpApp.Text = "";
            Logger.record("[Window_Unloaded]: Unloading help window", "HelpDlg", "info");
        }

        // There's a link line in the help dialog, pointing to the Rapid Reporter page: http://testing.gershon.info/reporter/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Logger.record("[RequestNavigate]: Link Pressed:" + e.Uri.AbsoluteUri, "HelpDlg", "info");
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
