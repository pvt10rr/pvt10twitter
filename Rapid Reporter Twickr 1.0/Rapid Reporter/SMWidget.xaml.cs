// This application lets the tester be the master of the session.
///-------

// References and Dependencies
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using FlickrNet;

// Scope of the application.
// In this file we deal with the main 'Widget':
/*
 * +------------------------------------------------------------------------------------+
 * | <:  [S] +----------------------------------------------------------------------+ X |
 * |  :  [N] +----------------------------------------------------------------------+   |
 * +------------------------------------------------------------------------------------+
 */
namespace Rapid_Reporter
{
    // Controls the main Widget
    public partial class SMWidget : Window
    {

        public void flickrMessageBox()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to login to Flickr?", "Flickr Login", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {

                m_flickr.Login();
                m_flickrLoggedIn = true;
            }
            else
            {
                m_flickrLoggedIn = false;
            }
        }

        // Flickr
        FlickrAddon m_flickr = new FlickrAddon();
        bool m_flickrLoggedIn = false;
        public static bool ToggleUpload2 = false;

        // CharCounter variables
        public static string hardcodedText;
        public static int preLength;
        public static int twitterMessageLimit;
        public static int numberOfChar;
        public static int countDownNrOfChar;

        // Shows how many more characters you can type in your note
        // until it will be truncated. (If Twitter is enabled).
        public void CharCounter()
        {
            if (currentStage == sessionStartingStage.charter)
            {
                preLength = currentSession.tester.Length + 7 + TwitterAddon.hashCode.Length;
            }
            else
            {
                preLength = currentSession.tester.Length + currentSession.noteTypes[currentNoteType].Length + TwitterAddon.hashCode.Length;
            }

            hardcodedText = "[Reporter: ]  # #";
            twitterMessageLimit = 140 - preLength - hardcodedText.Length;
            numberOfChar = NoteContent.CaretIndex;
            countDownNrOfChar = twitterMessageLimit - numberOfChar;
            charCounter.Text = countDownNrOfChar.ToString();
        }

        private void ChangeAccount_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[ChangeAccount_Click]: Change Account", "SMWidget", "info");
            // string url = flickr.AuthCalcWebUrl(AuthLevel.Write);
            m_flickr.LogOut();
            FlickrInlogg.Text = "Flickr Offline  ";
        }
        
        // What happens when you press the Twitter button.
        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            // What happes if Twitter is enabled.
            if (TwitterAddon.twitter)
            {
                // Disables Twitter.
                TwitterAddon.twitter = false;
                // Changes button icon.
                TwitterIcon.Source = new BitmapImage(new Uri("icontwit_dis.png", UriKind.Relative));
                // Changes button tooltip.
                Twitter.ToolTip = "Twitter Posting Disabled";
                // Hides the Character Counter.
                charCounter.Visibility = Visibility.Hidden;
            }
            else
            {
                // What happens if Twitter is disabled.
                if (currentStage == sessionStartingStage.twitterAccount || TwitterAddon.twitterPIN != null)
                {
                    // Enables Twitter.
                    TwitterAddon.twitter = true;
                    // Changes button icon.
                    TwitterIcon.Source = new BitmapImage(new Uri("icontwit.png", UriKind.Relative));
                    // Changes button tooltip.
                    Twitter.ToolTip = "Twitter Posting Enabled";
                    // Makes the Character Counter visible.
                    charCounter.Visibility = Visibility.Visible;
                }
             }
        }

        private void Flickr_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[ToggleUpload_Click]: ToggleUpload", "SMWidget", "info");
            SMWidget.ToggleUpload2 = !SMWidget.ToggleUpload2;
            //     bool edit = false;
            //     if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift) edit = true;
            //      if (edit) this.WindowState = System.Windows.WindowState.Minimized;          // <--\

            if (SMWidget.ToggleUpload2 == true)
            {
                FlickrIcon.Source = new BitmapImage(new Uri("iconflick.png", UriKind.Relative));
                Flickr.ToolTip = "Flickr Upload Enabled";
            }
            else
                FlickrIcon.Source = new BitmapImage(new Uri("iconflick_dis.png", UriKind.Relative));
                Flickr.ToolTip = "Flickr Upload Disabled";
                // ToggleUploadIcon.IsEnabled = false;
        }

        // Session Notes variables
        private int currentNoteType = 0;        // The actual types are controlled by the Session class.
        private int prevNoteType = 0; private int nextNoteType = 0; // Used for the hints about the next note up or down.
        private int currentScreenshot = 1;      // The number of the screenshot (increases by 1). Helps putting them in order, and finding them between multiple the files.
        private string screenshotName = "";     // Attached to a Session Note.
        public string rtfNoteName = "";         // Attached to a Session Note. Public because it is used *directly* by the RTFNote

        // State Based Behaviors:
        // Command line:
        //  1) If helpCommands, then app will show help and exit
        //  2) If collectReports, then app will collect reports and exit
        //  3) If htmlReport, then app will create HTML and exit
        //  4) If workingDir, then change the working directory of the rest of the operation (don't exit).
        // Session flow:
        //  1) There are two initialization stages in application: tester and charter. Then it moves to the 'testing', or 'notes' stage.
        
        // Command line state based behavior
        private string helpCommand1 = "-help";                                                      // 1) \
        private string helpCommand2 = "/h";                                                         // 1)  > Help dialog requested
        private string helpCommand3 = "/?";                                                         // 1) /
        private string reportCommand = "-report";                                                   // 2) CSVs report requested
        private string htmlCommand = "-tohtml"; private string htmlFile = "";                       // 3) HTML translation requested, File var gets the name of the file to translate
        private string changeDirCommand = "-directory"; private string newDir = "";              // 4) Directory change requested, Dir var gets the address of the new directory
        bool CLIReport = false; bool CLIHelp = false; bool CLIHtml = false; bool changeDir = false; // These flags will define the flow ofa application
        bool prematureEnd = false;                                                                  // This flag defines if app dies after the action or not.

        // Session flow state based behavior
        enum sessionStartingStage { twitterAccount, tester, charter, notes };
        // This is used only in the beginning, in order to receive the tester name and charter text
        sessionStartingStage currentStage = sessionStartingStage.twitterAccount;

        // Timer to perform recurring actions (timing is set on windows load)
        static System.Windows.Forms.Timer recurrenceTimer = new System.Windows.Forms.Timer();

        // These two classes are external.
        //  We share classes and data all around. This coupling will be a weak spot if app gets complex.
        Session currentSession  = new Session();    // The session managing class
        RTFNote rtf = new RTFNote();                // The enhanced note window

        /** Starting Process **/
        /**********************/
        /// The application starts by asking for tester and charter information. Only then the session starts

        // Default constructor, everything is empty/default values
        public SMWidget()
        {
            
            TwitterAddon.GetUniqueKey(7); // Calling the hash code method in TwitterAddon.
            
            // Flickr
            Logger.record("[SMWidget]: App constructor. Initializing.", "SMWidget", "info");
            InitializeComponent();
            rtf.InitializeComponent();
            rtf.sm = this;
            NoteContent.Focus();

            parseCLI();
            handleCLI();
            Logger.record("[SMWidget]: App constructor initialized and CLI executed.", "SMWidget", "info");
        }

        // Prepare the session report log (adds the notes types, for example)
        private void SMWidgetForm_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.record("[SMWidgetForm_Loaded]: Form loading to windows", "SMWidget", "info");
            SMWidgetForm.Title = System.Windows.Forms.Application.ProductName;
            SetWorkingDir(currentSession.workingDir);
            StateMove(sessionStartingStage.twitterAccount);

            // Some of the actions in the tool are recurrent. We do them every 30 seconds.
            recurrenceTimer.Tick += new EventHandler(TimerEventProcessor); // this is the function called everytime the timer expires
            recurrenceTimer.Interval = 30 * 1000; // 30 times 1 second (1000 milliseconds)
            recurrenceTimer.Start();

            this.NoteContent.Focus();
        }
        // When the widget is on focus, the note taking area is always on focus. Tester can keep writing all the time
        private void SMWidgetForm_GotFocus(object sender, RoutedEventArgs e)
        {
            Logger.record("[SMWidgetForm_GotFocus]: SMWidget on focus", "SMWidget", "info");
            SMWidgetForm.NoteContent.Focus();
        }

        /** Closing Process **/
        /**********************/

        //// Existent notes were automatically saved in a persistent file, so no need to save now.
        //// Mainly, the session should be terminated (timing notes added to file too) and all windows closed.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[CloseButton_Click]: Closing Form...", "SMWidget", "info");
            this.Close();
        }
        // Closing the form can't just close the window, it has to follow the finalization process
        private void SMWidgetForm_Closed(object sender, EventArgs e)
        {
            Logger.record("[SMWidgetForm_Closed]: Exiting Application...", "SMWidget", "info");
            ExitApp();
        }
        // Before closing the window, we have to close the session and the RTF note
        private void ExitApp()
        {
            // Session
            Logger.record("[ExitApp]: Closing Session...", "SMWidget", "info");
            currentSession.CloseSession();
            // RTF Note
            Logger.record("[ExitApp]: Closing RTF Note (force = true)...", "SMWidget", "info");
            rtf.forceClose = true; // We keep the RTF open (hidden), so we have to force it out
            Logger.record("[ExitApp]: Closing RTF Note...", "SMWidget", "info");
            rtf.Close();
            // This form
           Logger.record("[ExitApp]: End of application!", "SMWidget", "info");
        }

        /** Window Event Handling **/
        /***************************/

        // Transparency:
        // Application can be set transparent, to avoid distraction from task at hand
        private void TransparencySlide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Logger.record("[TransparencySlide_ValueChanged]: Changing transparency to " + e.NewValue, "SMWidget", "config");
            SMWidgetForm.Opacity = e.NewValue;
            rtf.Opacity = Math.Min(e.NewValue+0.2,1);
        }

        // SMWidget Left Click:
        // Application can be moved around the screen, to keep it out of the way
        void SMWidget_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.record("[SMWidget_LeftButtonDown]: Window dragged on screen", "SMWidget", "info");
            this.DragMove();
        }

        /** Note Event Handling **/
        // Very important functions happen during note taking:
        //  Every submittal saves the note to disk (data is always safe)
        //  Type of note can be changed easily at all times, by pressing up/down
        private void NoteContent_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int notesLenght = currentSession.noteTypes.Length - 1;

            // Updates the Character Counter if current stage is notes and Twitter is enabled.
            if ((currentStage == sessionStartingStage.notes || currentStage == sessionStartingStage.charter) && TwitterAddon.twitter)
            {
                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    // Ignore if pressing left or right key
                }
                else
                {
                    CharCounter();
                }
            }
            
            // 1) Up/Down
            // 2) Enter
            // 3) Esc
            // TODO: Ctrl--, Ctrl-+, Ctrl-S, Crl-N

            // 1)
            // Up and Down cycles through the note types
            if ((e.Key == Key.Down || e.Key == Key.Up) && currentStage == sessionStartingStage.notes)
            {
                // Algorithm is spelled out: If at end, forward goes to beginning, if at beginning back goes to end, step by 1 otherwise.
                if (e.Key == Key.Up)
                {
                    prevNoteType = currentNoteType;

                    if (currentNoteType > 0)
                    { currentNoteType--; }
                    else
                    { currentNoteType = notesLenght; }
                    if (nextNoteType > 0)
                    { nextNoteType--; }
                    else
                    { nextNoteType = notesLenght; }
                }
                else if (e.Key == Key.Down)
                {
                    nextNoteType = currentNoteType;

                    if (currentNoteType < notesLenght)
                    { currentNoteType++; }
                    else
                    { currentNoteType = 0; }
                    if (prevNoteType < notesLenght)
                    { prevNoteType++; }
                    else
                    { prevNoteType = 0; }
                }
                Logger.record("\t[NoteContent_KeyUp]: Changing note to " + currentSession.noteTypes[currentNoteType], "SMWidget", "info");
                NoteType.Text = currentSession.noteTypes[currentNoteType] + ":";
                prevType.Text = "↓ " + currentSession.noteTypes[prevNoteType] + ":";
                nextType.Text = "↑ " + currentSession.noteTypes[nextNoteType] + ":";

                // Updates Character Counter if Twitter is enabled.
                if (TwitterAddon.twitter)
                {
                    CharCounter();
                }
            }

            // 2)
            // Enter keys accept the note into the report
            else if (e.Key == Key.Enter)
            {
                // Disables Twitter if Twitter PIN is left blank.
                if (e.Key == Key.Enter && currentStage == sessionStartingStage.twitterAccount && NoteContent.Text.Trim().Length == 0)
                {
                    TwitterAddon.twitter = false;
                    TwitterIcon.Source = new BitmapImage(new Uri("icontwit_dis.png", UriKind.Relative));
                    Twitter.ToolTip = "Twitter Posting Disabled";
                    Twitter.IsChecked = false;
                    Twitter.IsEnabled = false;
                    currentTwitterAccount.Text = "Twitter Posting Disabled  ";

                    StateMove(sessionStartingStage.tester);
                }
                // Ignore empty inputs
                if (e.Key == Key.Enter && NoteContent.Text.Trim().Length != 0)
                {
                    Logger.record("\t[NoteContent_KeyUp]: Enter pressed...", "SMWidget", "info");
                    if (currentStage == sessionStartingStage.twitterAccount)
                    {
                        currentSession.twitterAccount = NoteContent.Text.Replace("\"", "''").Replace(",", ";").Trim();
                        TwitterAddon.twitterPIN = currentSession.twitterAccount;
                        if (TwitterAddon.twitterPIN.Length != 7)
                        {
                            MessageBox.Show("Invalid pin. Please try again.", "Error");
                            StateMove(sessionStartingStage.twitterAccount);
                        }
                        else
                        {
                            TwitterAddon.TwitterOAuth();
                            currentTwitterAccount.Text = "Twitter Account: " + TwitterAddon.ScreenName + "  ";
                            StateMove(sessionStartingStage.tester);
                        }
                    }
                    else if (currentStage == sessionStartingStage.tester)
                    {
                        currentSession.tester = NoteContent.Text.Replace("\"", "''").Replace(",", ";").Trim();
                        StateMove(sessionStartingStage.charter);
                    }
                    else if (currentStage == sessionStartingStage.charter)
                    {
                        currentSession.charter = NoteContent.Text.Replace("\"", "''").Replace(",", ";").Trim();
                        StateMove(sessionStartingStage.notes);
                    }
                    else
                    {
                        // What we do when adding a note:
                        //  - 1) We add to the session notes
                        //  - 2) We add to the history context menu
                        //  - 3) We clear notes and attachments to make place for new ones
                        /*1*/
                        currentSession.UpdateNotes(currentNoteType, NoteContent.Text.Replace("\"", "''").Replace(",", ";").Trim(), screenshotName, rtfNoteName);
                        /*2*/   System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem();
                                item.Header = NoteContent.Text;
                                item.Click += delegate { GetHistory(item.Header.ToString()); };
                                NoteHistory.Items.Add(item);
                                NoteHistory.Visibility = Visibility.Visible;
                                Logger.record("\t\t[NoteContent_KeyUp]: Note added.", "SMWidget", "info");
                    }
                        /*3*/ ClearNote();
                }
            }

            // 3)
            // Escape key cancels the note, cancels the note attachments
            else if (e.Key == Key.Escape)// This was here before, seems like not needed: && currentStage == sessionStartingStage.notes)
            {
                Logger.record("[NoteContent_KeyUp]: (note aborted)", "SMWidget", "info");
                ClearNote();
            }
        }
        
        // The function below will change the visuals of the application at the different stages (tester/charter/notes state based behavior)
        private void StateMove(sessionStartingStage newStage)
        {
            Logger.record("[StateMove]: Session Stage now: " + currentStage.ToString(), "SMWidget", "info");
            currentStage = newStage;
            switch (currentStage)
            {
                case sessionStartingStage.twitterAccount:
                    NoteType.Text = "Twitter PIN:"; prevType.Text = ""; nextType.Text = "";
                    prevNoteType = 1; nextNoteType = currentSession.noteTypes.Length - 1;
                    OpenFolder.Header = "Change working folder...";
                    Logger.record("\t[StateMove]: Session Stage moving -> TwitterPIN", "SMWidget", "info");
                    TwitterAddon.TwitterLogin();
                    break;
                case sessionStartingStage.tester:
                    NoteType.Text = "Reporter:";
                    Logger.record("\t[StateMove]: Session Stage moving -> Tester", "SMWidget", "info");
                    flickrMessageBox();
                    break;
                case sessionStartingStage.charter:
                    NoteType.Text = "Charter:";
                    Logger.record("\t[StateMove]: Session Stage moving -> Charter", "SMWidget", "info");
                    break;
                case sessionStartingStage.notes:
                    NoteContent.ToolTip = (100 < currentSession.charter.Length) ? currentSession.charter.Remove(100)+"..." : currentSession.charter;
                    NoteType.Text = currentSession.noteTypes[currentNoteType] + ":";
                    prevType.Text = "↓ " + currentSession.noteTypes[prevNoteType] + ":";
                    nextType.Text = "↑ " + currentSession.noteTypes[nextNoteType] + ":";


                    // Flickr
                    if (m_flickrLoggedIn)
                    {
                        FlickrInlogg.Text = "Flickr Account: " + m_flickr.GetCurrentUser() + "  "; //currentSession.noteTypes[ReporterNoteName] 
                    }
                    else
                    {
                        FlickrInlogg.Text = "Flickr Offline  ";
                    }


                    currentSession.StartSession(); ProgressGo(90); t90.IsChecked = true;
                    ScreenShot.IsEnabled = true; RTFNoteBtn.IsEnabled = true; FlickrIcon.IsEnabled = true;
                    // Change the icon of the image of the buttons, to NOT appear disabled.
                    ScreenShotIcon.Source = new BitmapImage(new Uri("iconshot.png", UriKind.Relative));
                    RTFNoteBtnIcon.Source = new BitmapImage(new Uri("iconnotes.png", UriKind.Relative));
                    TimerMenu.IsEnabled = true;

                    ChangeAccount.IsEnabled = true; // Flickr

                    OpenFolder.Header = "Open working folder...";
                    Logger.record("\t\t[StateMove]: Session Stage moving -> Notes", "SMWidget", "info");
                    break;
                default:
                    Logger.record("\t[StateMove]: Session Stage moving -> NULL", "SMWidget", "error");
                    break;
            }
        }

        // AboutBox
        // Shows About dialog box with software info, contacts and credits
        private void AboutBox_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[AboutBox_Click]: About box invoked", "SMWidget", "info");
            AboutDlg about = new AboutDlg();
            about.Owner = this;
            about.ShowDialog();
        }

        //OpenFolder
        // Used to reach the working folder, where attachments and reports are.
        private void WorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[WorkingFolder_Click]: Opening working directory", "SMWidget", "info");
            // If we didn't start the session, allow changing the folder.
            // If session started, can only open
            if (currentStage == sessionStartingStage.notes)
            {
                Logger.record("\t[WorkingFolder_Click]: Loading explorer to " + currentSession.workingDir, "SMWidget", "info");
                Process.Start(new ProcessStartInfo(currentSession.workingDir));
            }
            else
            {
                Logger.record("\t[WorkingFolder_Click]: Loading folder picker", "SMWidget", "info");
                System.Windows.Forms.FolderBrowserDialog folderPick = new System.Windows.Forms.FolderBrowserDialog();
                if (folderPick.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Logger.record("\t\t[WorkingFolder_Click]: Folder picked, will set working folder", "SMWidget", "info");
                    SetWorkingDir(folderPick.SelectedPath);
                }
            }
            e.Handled = true;
        }

        // GetHistory:
        // Note reuse (the historyNote comes from pressing the history context menu)
        void GetHistory(string historyNote)
        {
            Logger.record("[GetHistory]: Retrieving note from history", "SMWidget", "info");
            NoteContent.Text = historyNote;
        }

        // ProgressTimer
        // Makes the progress bar progress, according to the time chosen
        private void ProgressTimer_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[ProgressTimer_Click]: Time to end: " + sender.ToString(), "SMWidget", "info");
            if (sender.ToString().Contains("1 min")) ProgressGo(1);
            if (sender.ToString().Contains("3 min")) ProgressGo(3);
            if (sender.ToString().Contains("60 min")) ProgressGo(60);
            if (sender.ToString().Contains("90 min")) ProgressGo(90);
            if (sender.ToString().Contains("120 min")) ProgressGo(120);
            if (sender.ToString().Contains("Stop")) ProgressGo(0);
        }
        private void ProgressGo(int time) // time is received in minutes
        {
            Logger.record("[ProgressGo]: Time to end: " + time + " min", "SMWidget", "info");
            ProgressBackground.Value = 0;

            Logger.record("[ProgressGo]: Hiding clock icon", "SMWidget", "info");
            timeralarm.Visibility = Visibility.Hidden;

            StopTimers();
            if (time > 0)
            {
                currentSession.duration = time * 60;
                Duration duration = new Duration(TimeSpan.FromSeconds(currentSession.duration));
                DoubleAnimation timedAnimation = new DoubleAnimation(100, duration);

                // Progress Bar Repositioning
                ////
                // In order to reposition the timer in a proportional place, we do the following calculation:
                //  The position of the progress bar should be put in the percentage elapsed time from the grand total time,
                //  where the grand total time is the elapsed time until now plus the time that was chosen as the new session end.
                //              Elapsed Time
                //      ------------------------------ == Percentage of time elapsed
                //      Elapsed Time + Additional Time
                ProgressBackground.Value = 100*(
                    ((DateTime.Now - currentSession.startingTime).TotalSeconds) /
                    (((DateTime.Now - currentSession.startingTime).TotalSeconds) + currentSession.duration)
                    );
                Logger.record("\t[ProgressGo]: Time calculation. Value: " + ProgressBackground.Value + "; Elapsed: " + (DateTime.Now - currentSession.startingTime).TotalSeconds + "; duration: " + currentSession.duration, "SMWidget", "info");
                
                // In order to reposition the timer at the beginning of the progress bar at every change, one should stop it before restarting;
                //  StopTimers();
                //
                // In order to keep the timer in its place and just speed up or slow down to meet the end at the current time, no
                //  other operation needs to be done.
                //ProgressBackground.Value = ProgressBackground.Value; // WTF? True = True? That's the best 'no other operation' possible?

                ProgressBackground.BeginAnimation(System.Windows.Controls.ProgressBar.ValueProperty, timedAnimation);
            }
        }
        // Sets the progress bar to null (stopped, not seen), and hides clock icon
        private void StopTimers()
        {
            Logger.record("[StopTimers]: Stopping timer (setting to null)", "SMWidget", "info");
            ProgressBackground.BeginAnimation(System.Windows.Controls.ProgressBar.ValueProperty, null);
            Logger.record("[StopTimers]: Hiding clock icon", "SMWidget", "info");
            timeralarm.Visibility = Visibility.Hidden;
        }
        // TimerEventProcessor
        // The actions in this fuction will happen every time the timer expires.
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            Logger.record("[TimerEventProcessor]: Will perform recurring tasks", "SMWidget", "info");
            if (100 <= ProgressBackground.Value)
            {
                Logger.record("[TimerEventProcessor]: Time's Up! Showing timer icon", "SMWidget", "info");
                timeralarm.Visibility = Visibility.Visible;
            }
        }
        // timeralarm_MouseLeftButtonDown:
        // We'll check when the progress bar reaches the end, to trigger the time's up notification
        private void timeralarm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.record("[timeralarm_MouseLeftButtonDown]: Time's Up timer acknowledged by user", "SMWidget", "info");
            t0.IsChecked = true;
            t60.IsChecked = false; t90.IsChecked = false; t120.IsChecked = false;

            ProgressGo(0);
        }
        // timer_Checked, gets timing commands from the context menu
        private void timer_Checked(object sender, RoutedEventArgs e)
        {
            Logger.record("[timer_Checked]: Timer context menu command: " + sender.ToString(), "SMWidget", "info");
            t0.IsChecked = t0.Equals(sender);
            t60.IsChecked = t60.Equals(sender);
            t90.IsChecked = t90.Equals(sender);
            t120.IsChecked = t120.Equals(sender);
        }


        /** Note Handling  **/
        /********************/

        // Screenshots:
        // Saves all screenshots in files, sets the last one to be added to the next note
        private void ScreenShot_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[ScreenShot_Click]: Capturing screen", "SMWidget", "info");
            bool edit = false;
            if(System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift) edit = true;
            if (edit) this.WindowState = System.Windows.WindowState.Minimized;          // <--\
            ScreenShot ss = new ScreenShot();                                           //     )
            AddScreenshot2Note(ss.CaptureScreenShot());                                 //    (
            Logger.record("[ScreenShot_Click]: Captured " + screenshotName + ", edit: " + edit.ToString(), "SMWidget", "info");
            if (edit)                                                                   //    (
            {                                                                           //     )
                Process paint = new Process();                                          //    (
                paint.StartInfo.FileName = "mspaint.exe";                               //     )
                paint.StartInfo.Arguments = "\"" + currentSession.workingDir + screenshotName + "\"";
                paint.Start();                                                          //     )
                this.WindowState = System.Windows.WindowState.Normal;                   // <--/
            }
        }
        // Adding attached screenshot have dedicated functions that deal with the visual
        //  clues as well
        private void AddScreenshot2Note(Bitmap bitmap)
        {
            Logger.record("[AddScreenshot2Note]: Saving screen to file", "SMWidget", "info");
            bool exDrRetry = false;

            // Name the screenshot, save to disk
            screenshotName = currentScreenshot++.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + TwitterAddon.hashCode + ".jpg"; // HASCODE ADDED!!!___<-----#########
            do
            {
                exDrRetry = false;
                try
                {
                    // Flickr
                    string strReportFileName = currentSession.GetCurrentSessionFile();

                    Graphics graphicImage = Graphics.FromImage(bitmap);
                    graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

                    StringFormat format = new System.Drawing.StringFormat(StringFormatFlags.DirectionRightToLeft);

                    graphicImage.DrawString(strReportFileName, new Font("Arial", 14, System.Drawing.FontStyle.Bold),
                        System.Drawing.Brushes.Red, new System.Drawing.Point(50, 50));



                    // Legacy
                    bitmap.Save(currentSession.workingDir + screenshotName, ImageFormat.Jpeg);
                    AutoSaveScrenshot(screenshotName);



                    // Flickr
                    string strFileTag = "#Session File: " + strReportFileName;
                    string URL = "";

                    if (ToggleUpload2)
                    {
                        if (m_flickrLoggedIn)
                        {

                            URL = m_flickr.GetUrl(m_flickr.Upload(screenshotName, "", "", currentSession.GetTags() + strFileTag));
                            currentSession.UpdateNotes("WebUrl: ", URL);

                            FlickrInlogg.Text = "Flickr Account: " + m_flickr.GetCurrentUser() + "  "; //currentSession.noteTypes[ReporterNoteName] 
                        }
                        else
                        {
                            m_flickr.Login();
                            FlickrInlogg.Text = "Flickr Account: " + m_flickr.GetCurrentUser() + "  ";
                            URL = m_flickr.GetUrl(m_flickr.Upload(screenshotName, "", "", currentSession.GetTags() + strFileTag));
                            currentSession.UpdateNotes("Flickr Link: ", URL);
                            m_flickrLoggedIn = true;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Logger.record("[AddScreenshot2Note]: EXCEPTION reached - Session Note file could not be saved (" + screenshotName + ")", "SMWidget", "error");
                    exDrRetry = Logger.FileErrorMessage(ex, "SaveToSessionNotes", screenshotName);
                }
            } while (exDrRetry);

            // Put a visual effect to remember the tester there's an image on the attachment barrel
            BevelBitmapEffect effect = new BevelBitmapEffect();
            effect.BevelWidth = 2; effect.EdgeProfile = EdgeProfile.BulgedUp;
            ScreenShot.BitmapEffect = effect;
        }

        // RTFNote:
        // Show or hide the enhanced notes window.
        // We don't change the note between toggles, we just ide it. Tester can continue to type later.
        private void RTFNote_Click(object sender, RoutedEventArgs e)
        {
            Logger.record("[RTFNote_Click]: Will toggle RTF note screen", "SMWidget", "info");
            // The RTF Note button is a toggle button - it has two states: show the note area and hide the note area
            if (RTFNoteBtn.IsChecked.Equals(true)) // Show the note are
            {
                Logger.record("\t[RTFNote_Click]: Repositioning the RTF window", "SMWidget", "info");
                // Position is either on top or bottom of SM Widget, with same width as the Widget
                rtf.Left = this.Left;
                rtf.Width = this.Width;
                if (this.Top > rtf.Height + 10)
                {
                    rtf.Top = this.Top - (rtf.Height + 10);
                }
                else
                {
                    rtf.Top = (this.Top + this.Height) + 10;
                }
                Logger.record("\t[RTFNote_Click]: Reposition:"+ rtf.Top +","+ rtf.Left +","+ rtf.Width +",", "SMWidget", "info");
                rtf.Show();
                rtf.Focus();
            }
            else // Hide the note area
            {
                Logger.record("\t[RTFNote_Click]: Hiding the RTF window", "SMWidget", "info");
                rtf.Hide();
            }
        }

        // Due to requests from users, we note all the screenshots and notes int the report.
        // This can be filtered in Excel if desired
        internal void AutoSaveNote(string filename)
        {
            Logger.record("[AutoSaveNote]: Extended note saved by user (not attached yet) (" + filename + ")", "SMWidget", "info");
            currentSession.UpdateNotes("autogenerated", "extended note saved", "", filename);
        }
        internal void AutoSaveScrenshot(string filename)
        {
            Logger.record("[AutoSaveScrenshot]: Screenshot saved by user (not attached yet) (" + filename + ")", "SMWidget", "info");
            currentSession.UpdateNotes("autogenerated", "screenshot saved", filename, "");
        }

        // ClearNote:
        // Makes space for a new note
        private void ClearNote()
        {
            Logger.record("[ClearNote]: Will delete note content and attachments indication", "SMWidget", "info");
            NoteContent.Text = "";  // New note
            screenshotName = "";    // New pic attachment
            rtfNoteName = "";       // New note attachment (RTF note area content is left intact!)
            // Clear visual effects (screenshots are all always saved anyway)
            BevelBitmapEffect effect = new BevelBitmapEffect();
            effect.BevelWidth = 0; effect.EdgeProfile = EdgeProfile.BulgedUp;
            ScreenShot.BitmapEffect = effect;
            RTFNoteBtn.BitmapEffect = effect;
            if (TwitterAddon.twitter && currentStage == sessionStartingStage.notes)
            {
                CharCounter();
            }
        }

        // Change Working Dir
        // Changes the working Directory for the session
        private void SetWorkingDir(string newPath)
        {
            Logger.record("[SetWorkingDir] Setting directory to " + newDir, "SMWidget", "info");
            if (!newPath.EndsWith(@"\")) newPath += @"\"; // Add the trailing 'slash' to the directory
            rtf.workingDir = currentSession.workingDir = newPath; // the workingDir needs to be the same for all files!
            FolderName.Header = (50 < currentSession.workingDir.Length) ? "..." + currentSession.workingDir.Substring(currentSession.workingDir.Length - 47) : currentSession.workingDir;
        }
    
        /** Command Line Handling **/
        /***************************/

        // Command line parsing
        // This function allows command repetition. However
        public void parseCLI()
        {
            Logger.record("[parseCLI]: Parsing Application's command line arguments if existent", "SMWidget", "info");
            if (Environment.GetCommandLineArgs().Length > 1) // First argument is the command path+name
            {
                Logger.record("\t[parseCLI]: Command Line Arguments found. Will parse.", "SMWidget", "info");
                ArrayList CLIArguments = new ArrayList();
                foreach (string argument in Environment.GetCommandLineArgs())
                    CLIArguments.Add(argument);

                /// Help Commands:
                if (CLIArguments.Contains(helpCommand1) || CLIArguments.Contains(helpCommand2) || CLIArguments.Contains(helpCommand3))
                {
                    Logger.record("\t\t[parseCLI]: a help command received in arguments.", "SMWidget", "info");
                    CLIHelp = true;
                    prematureEnd = true;
                    // Q: Why do we care removing the helpCommands entries from arguments?
                    // A: Because in cases where command line options are combined, or when there are duplicate commands,
                    //  or in the future we don't exit the application immediately, leaving them could have bad impact.
                    if (CLIArguments.Contains(helpCommand1)) CLIArguments.RemoveAt(CLIArguments.IndexOf(helpCommand1));
                    else if (CLIArguments.Contains(helpCommand2)) CLIArguments.RemoveAt(CLIArguments.IndexOf(helpCommand2));
                    else if (CLIArguments.Contains(helpCommand3)) CLIArguments.RemoveAt(CLIArguments.IndexOf(helpCommand3));
                }

                /// Directory Commands:
                if (CLIArguments.Contains(changeDirCommand))
                {
                    Logger.record("\t\t[parseCLI]: '" + changeDirCommand + "' command received in argument " + CLIArguments.IndexOf(changeDirCommand).ToString(), "SMWidget", "info");
                    // We DON'T use 'prematureEnd = false;' because that could cancel the '= true' of another option

                    // Q: Why do we care removing the reportCommand entry from arguments?
                    // A: Because in cases where command line options are combined, or when there are duplicate commands,
                    //  or in the future we don't exit the application immediately, leaving them could have bad impact.

                    // The next condition checks if the index of the htmlCommand is not the last argument.
                    //  The argument after htmlCommand will be regarded as the file to Parse. Even if it is not :)                    
                    if (CLIArguments.IndexOf(changeDirCommand) < CLIArguments.ToArray().Length - 1) // remove 1 because the first is the app name
                    {
                        Logger.record("\t\t\t[parseCLI]: There's another argument to serve as file name. Will proceed.", "SMWidget", "info");
                        changeDir = true;
                        newDir = CLIArguments[CLIArguments.IndexOf(changeDirCommand) + 1].ToString();
                        Logger.record("\t\t\t[parseCLI]: the folder to use is " + newDir + " in " + CLIArguments.IndexOf(htmlCommand).ToString(), "SMWidget", "info");
                        CLIArguments.RemoveAt(CLIArguments.IndexOf(changeDirCommand) + 1);
                    }
                    CLIArguments.RemoveAt(CLIArguments.IndexOf(changeDirCommand));
                }

                /// Report Commands:
                if (CLIArguments.Contains(reportCommand))
                {
                    Logger.record("\t\t[parseCLI]: '" + reportCommand + "' command received in argument " + CLIArguments.IndexOf(reportCommand).ToString(), "SMWidget", "info");
                    CLIReport = true;
                    prematureEnd = true;
                    // Q: Why do we care removing the reportCommand entry from arguments?
                    // A: Because in cases where command line options are combined, or when there are duplicate commands,
                    //  or in the future we don't exit the application immediately, leaving them could have bad impact.
                    CLIArguments.RemoveAt(CLIArguments.IndexOf(reportCommand));
                }

                /// HTML Commands:
                if (CLIArguments.Contains(htmlCommand))
                {
                    Logger.record("\t\t[parseCLI]: '" + htmlCommand + "' command received in argument " + CLIArguments.IndexOf(htmlCommand).ToString(), "SMWidget", "info");
                    prematureEnd = true;
                    // Q: Why do we care removing the reportCommand entry from arguments?
                    // A: Because in cases where command line options are combined, or when there are duplicate commands,
                    //  or in the future we don't exit the application immediately, leaving them could have bad impact.

                    // The next condition checks if the index of the htmlCommand is not the last argument.
                    //  The argument after htmlCommand will be regarded as the file to Parse. Even if it is not :)                    
                    if (CLIArguments.IndexOf(htmlCommand) < CLIArguments.ToArray().Length - 1) // remove 1 because the first is the app name
                    {
                        Logger.record("\t\t\t[parseCLI]: There's another argument to serve as file name. Will proceed.", "SMWidget", "info");
                        CLIHtml = true;
                        htmlFile = CLIArguments[CLIArguments.IndexOf(htmlCommand) + 1].ToString();
                        Logger.record("\t\t\t[parseCLI]: the file to parse is " + htmlFile + " in " + CLIArguments.IndexOf(htmlCommand).ToString(), "SMWidget", "info");
                        CLIArguments.RemoveAt(CLIArguments.IndexOf(htmlCommand) + 1);
                    }
                    CLIArguments.RemoveAt(CLIArguments.IndexOf(htmlCommand));
                }

                /// Copy of the remaining arguments to note types
                if (CLIArguments.ToArray().Length > 1)
                {
                    currentSession.noteTypes = new string[Math.Max(CLIArguments.ToArray().Length - 1, 2)];
                    int i = 1;
                    for (; i < CLIArguments.ToArray().Length; i++)
                    {
                        Logger.record("\t\t[parseCLI]: Copying " + i.ToString() + " arguments to the new note types", "SMWidget", "config");
                        currentSession.noteTypes[i - 1] = CLIArguments[i].ToString().Replace(",", ".").Trim();
                        Logger.record("\t\t[parseCLI]: Lenght of noteTypes: " + currentSession.noteTypes.Length, "SMWidget", "config");
                    }
                    // The 'if' statement below is used to protect against arguments that provide only one note type. 'i == 2' in that case.
                    if (2 == i) currentSession.noteTypes[1] = CLIArguments[1].ToString().Replace(",", ".").Trim();
                }
            }
        }

        // Command line redirection
        // We don't do it in the command line arguments parsing, because we don't want to repeat an operation if the parameter is repeated
        public void handleCLI()
        {
            Logger.record("[handleCLI]: Handling Application's command line arguments that were parse (if existent)", "SMWidget", "info");

            // The order of the conditions is extremely important, as some actions will have impact on others.
            //  For example changing the directory will have impact on the report commands only if it is done before them.

            // For help function:
            // Help is:
            // 0 - Normal help inside application
            // 1 - -help help
            // 2 - -report help
            // 3 - -tohtml help
            if (CLIHelp)
            {
                ShowHelp(1);
            }
            if (changeDir)
            {
                Logger.record("\t[handleCLI]: Change directory Requested. Will modify variable to: " + newDir, "SMWidget", "info");
                if (Directory.Exists(newDir))
                {
                    Logger.record("\t\t[handleCLI]: Change directory will be executed, directory exists", "SMWidget", "info");
                    SetWorkingDir(newDir);
                }
                else
                {
                    ShowHelp("The folder named [" + newDir + "] provided is not a valid existent folder.\nWill use current directory for session files.");
                    SetWorkingDir(Directory.GetCurrentDirectory());
                }
            }
            if (CLIReport)
            {
                Logger.record("\t[handleCLI]: Report Gathering Requested. Will join files...", "SMWidget", "info");
                ShowHelp(2);
                currentSession.CollectReport();
            }
            else if (CLIHtml)
            {
                Logger.record("\t[handleCLI]: HTML Transform Requested. Will create file from: " + htmlFile, "SMWidget", "info");
                if (File.Exists(currentSession.workingDir + htmlFile) && htmlFile.EndsWith(".csv", true, null))
                {
                    Logger.record("\t\t[handleCLI]: HTML Transform will be executed, file exists", "SMWidget", "info");
                    ShowHelp(3);
                    currentSession.CSV2HTML(htmlFile);
                }
                else
                {
                    ShowHelp("The file name [" + htmlFile + "] provided is not a valid existent file");
                }
            }

            // Some command line arguments will exit the application before it starts
            if (prematureEnd)
            {
                Logger.record("\t[handleCLI]: The CLI arguments require a premature end. App will close...", "SMWidget", "info");
                this.Close();
                // The following line will appear in cases where the Close() command did not finish the app.
                Logger.record("\t[handleCLI]: The CLI arguments require a premature end. this.Close() executed.", "SMWidget", "info");
            }
        }
        
        // Popping the Help Dialog
        //  Some of the lines below are duplicated in the About dialog code
        public void ShowHelp(int helpType)
        {
            Logger.record("\t\t[ShowHelp i]: Showing help dialog #" + helpType.ToString(), "SMWidget", "info");

            // Help is:
            // 0 - Normal help inside application
            // 1 - -help help
            // 2 - -report help
            // 3 - -tohtml help
            ShowHelp(longstrings.helpstrings.helpstring[helpType]);
        }
        public void ShowHelp(string helpText)
        {
            Logger.record("\t\t[ShowHelp s]: Showing help dialog >" + helpText, "SMWidget", "info");

            Help help = new Help();
            help.LoadText(helpText);

            help.ShowInTaskbar = true; // In this cases we don't show the main window, so at least we show this one
            help.ShowDialog();
        }
    }
}
