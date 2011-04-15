using System.Diagnostics;
using System.Windows;

namespace Rapid_Reporter
{
    /// <summary>
    /// Interaction logic for AboutDlg.xaml
    /// The dialog shows basic information about the application, as well as an expandable credits pane.
    /// Two buttons are provided: A help button that shows the help dialog, and an Ok button to close the dialog.
    /// </summary>
    public partial class AboutDlg : Window
    {
        // Constructor
        //  We set the application name, version, and dialog title.
        public AboutDlg()
        {
            Logger.record("[AboutDlg]: Starting About Dialog. Initializing component.", "AboutDlg", "info");
            InitializeComponent();
            this.Title = System.Windows.Forms.Application.ProductName + " - Help";
            this.appName.Content = System.Windows.Forms.Application.ProductName;
            this.appVers.Content = System.Windows.Forms.Application.ProductVersion;
            Ok.Focus();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.record("[Window_Loaded]: loading dialog....", "AboutDlg", "info");
            Ok.Focus();
        }

        // There's a link line in the dialog pointing to the Rapid Reporter page: http://testing.gershon.info/reporter/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Logger.record("[RequestNavigate]: Link Pressed:" + e.Uri.AbsoluteUri, "AboutDlg", "info");
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true; // Can dismiss the event now that we dealt with it
        }

        // We provide access to the help dialog with explanations from inside the about dialog
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[Help_Click]: Showing help dialog #0 (normal help)", "AboutDlg", "info");

            // Help is:
            // 0 - Normal help inside application
            // 1 - -help help
            // 2 - -report help
            // 3 - -tohtml help

            Help help = new Help();
            help.LoadText(0);

            // The following two lines differentiate this call to the Help dialog from the other calls
            //  Other lines duplicate the ShowHelp function.
            help.Owner = this;          // About Dialog is the owner of the help, for modal matters
            help.ShowInTaskbar = false; // Rapid Reporter has an icon on the task bar (not the )

            help.ShowDialog();
            Logger.record("[Help_Click]: Help dialog displayed and exited", "AboutDlg", "info");
        }
    }
}
