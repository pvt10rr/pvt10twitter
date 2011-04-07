// This application lets the tester be the master of the session.
//  Logger - logs details in the 'log.log' file.
///-------

// References and Dependencies
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Twitterizer;

namespace Rapid_Reporter
{
    public static class Logger
    {
        // Notice there's only one operation: record a new message.
        //  Three overloaded options to use at convenience, all end up in the same function.

        // Note: Log is active only if there's a _rrlog_.log log file available in the directory

        public static void record(string message)
        {
            record(message, "generic");
        }
        public static void record(string message, string origin)
        {
            record(message, origin, "general");
        }
        public static void record(string message, string origin, string type)
        {
            // How it works: log is logged only if the log file exists.
            //  In normal operations the user sees no log.
            //  The upside is that the log can be started even in the middle of the application operation!
            //  The downside is that there are many calls to disk even when no log is present.
            //      One option could be to try the log once a minute until the file exists.
            //      Or every 15 log writes...

            // This part will keep the Directory.GetCurrentDirectory. The rest will work with session files.
            string targetFile = Directory.GetCurrentDirectory() + @"\_rrlog_.log";
            if (File.Exists(targetFile))
            {
                try
                {
                    File.AppendAllText(targetFile, Process.GetCurrentProcess().Id + ", " + DateTime.Now + ", " + origin + ", " + type + ", " + message + "\n");
                }
                catch (Exception ex)
                {
                    // We ignore silently errors of logging
                    // Reason: On other exceptions we put out a message box. But the log is a secondary feature, we don't want it to annoy.
                    Debug.WriteLine(ex.Message);
                    return;
                }
            }
        }

        // FileErrorMessage
        //  This is called from 'catch' operations throghout the code. When a file exception is found, we come here and show a message box.
        //  Note: there's an exception in RTFNote.xaml.cs that still does not use this function.
        public static bool FileErrorMessage(Exception ex, string title, string fileName)
        {
            Logger.record("\t[FileErrorMessage]: EXCEPTION reached - (Log)", "Logger", "error");
            Logger.record("\t[FileErrorMessage]: EXCEPTION: " + ex.Message, "Logger", "error");
            // Attention: Ternary operation returns a boolean. True if retry selected, false if cancel selected.
            //  This saves us from having to use 'using System.Windows.Forms' unnecessarily elsewhere.
            return
                (DialogResult.Retry == System.Windows.Forms.MessageBox.Show(
                "Ouch! An error occured when trying to write the note into a file.\n" +
                "The file name is: " + fileName + "\n\n" +
                "Possible causes:\n" +
                " -- You don't have write permissions to the folder or file;\n" +
                " -- The file is open and locked by another program (Excel?);\n" +
                " -- Windows preview pane is holding the file blocked for editing;\n" +
                " -- (there may be other reasons).\n\n" +
                "Possible solutions:\n" +
                " -- Set write permissions to the folder or file;\n" +
                " -- Close another application that may be using the file;\n" +
                " -- Select another file in explorer.\n\n" +
                "Exception details for investigation:\n" +
                ex.Message,
                "Rapid Reporter file error: " + title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error)) ? true : false;
        }
    }
}
