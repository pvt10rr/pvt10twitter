﻿// This application lets the tester be the master of the session.
//  Session Class - manages session details
///-------

// References and Dependencies
using System;
using System.IO;
using System.Threading;
using System.Windows;
using NUnit.Framework;


namespace Rapid_Reporter
{
    public class Session
    {

        public StreamWriter m_fileTags;
        private string m_strTags = "";
        

        /** Variables **/
        /***************/
        
        // This is configurable from inside the application:
        // Session characteristics:
        public DateTime startingTime;   // Time started, starts when moving from 'charter' to 'notes'.
        public int duration = 90 * 60;  // Duration, in seconds (default is 90 min, can be changed in runtime).
        private string columnHeaders = "Time,Reporter,Type,Content,Screenshot,RTF Note"; // Consider adding sequencial number?

        // Session data:
        public string charter = "";     // Session objective. Configured in runtime.
        public string tester = "";      // Tester's name. Configured in runtime.
        //public string flickr = "";      // Flickr account.
        // The types of comments. This can be overriden from command line, so every person can use his own terminology or language
        public string[] noteTypes = new string[7] {"Setup", "Note", "Test", "Check", "Bug", "Question", "Next Time" };

        // Session files:
        public string workingDir = Directory.GetCurrentDirectory() + @"\";  // File to write the session to
        private string sessionFile = null;      // File to write the session to
        private string sessionFileFull = null;  // workingDir + sessionFile
        public string sessionNote = "";         // Latest note only

        // Session State Based Behavior:
        //  The application iterates: tester, charter, notes.
        //  This is done in this way in case we have to add more stages... But the stages are not moved by  number or placement, they're chosen directly.
        public enum sessionStartingStage { tester, charter, notes }; // Tester == tester's name. Charter == session charter. Notes == all the notes of different note types.
        public sessionStartingStage currentStage = sessionStartingStage.tester; // This is used only in the beginning, in order to receive the tester name and charter text

        /** Sessions **/
        /**************/


        public string GetCurrentSessionFile()
        {
            return sessionFile;
        }


        // Start Session and Close Session prepare/finalize the log file
        public void StartSession()
        {
            FlickrAddon.GetUniqueKey(10);
            Logger.record("[StartSession]: Session configuration starting", "Session", "info");

            startingTime = DateTime.Now; // The time the session started is used for many things, like knowing the session file name
            sessionFile = startingTime.ToString("yyyyMMdd_HHmmss") + "_" + FlickrAddon.hashcode + ".csv";  //HASCODE VARIABLE ADDED__________<-------######
            sessionFileFull = workingDir + sessionFile; // All files should be written to a working directory -- be it current or not.
            SaveToSessionNotes(columnHeaders + "\n"); // Headers of the notes table
            UpdateNotes("(Rapid Reporter version)", System.Windows.Forms.Application.ProductVersion);
            UpdateNotes("Session Reporter", tester);
            UpdateNotes("Hashcode", FlickrAddon.hashcode);  //HASCODE VARIABLE ADDED__________<-------#######
            UpdateNotes("Session Charter", charter);
        }

        public string GetTags()
        {
            return m_strTags;
        }


        public void CloseSession() // Not closing directly, we first finalize the session
        {
            Logger.record("[CloseSession]: Session closing...", "Session", "info");

            // Why this if? We will only add the 'end session' note if we were past the charter step.
            if (!String.Equals(charter, ""))
            {
                TimeSpan duration = DateTime.Now - startingTime;
                UpdateNotes("Session End. Duration",
                    duration.Hours.ToString().PadLeft(2, '0') + ":" + duration.Minutes.ToString().PadLeft(2, '0') + ":" + duration.Seconds.ToString().PadLeft(2, '0'));
            }
            // This condition possibly could have been done by: if (currentStage == sessionStartingStage.notes).
            // I wonder why I did it the wrong way?
            // TODO: Use the second, more elegant comparison. Make a few tests to make sure it's making sense.

            Logger.record("[CloseSession]: ...Session closed", "Session", "info");
        }

        /** Notes **/
        /***********/
        // Notes are always saved on file, not only when program exists (so no data loss in case of crash)

        // UpdateNotes: There are two overloads: One receives all strings (custom messages), the other an int (typed messages)
        public void UpdateNotes(int type, string note, string screenshot, string RTFNote)
        {
            UpdateNotes(noteTypes[type], note, screenshot, RTFNote);
            Logger.record("[UpdateNotes isss]: Note added to session log. Attachments: (" + (screenshot.Length > 0).ToString() + " | " + (RTFNote.Length > 0).ToString() + ")", "Session", "info");
        }
        public void UpdateNotes(string type, string note, string screenshot = "", string RTFNote = "")
        {

            if (type != "(Rapid Reporter version)" && type != "Session End. Duration" && type != "WebUrl")
            {
                m_strTags += "#" + type + " " + note + ",";
            }

            sessionNote = DateTime.Now + "," + tester + "," + type + ",\"" + note + "\"," + screenshot + "," + RTFNote + "\n";
            SaveToSessionNotes(sessionNote);
            Logger.record("[UpdateNotes ss]: Note added to session log (" + screenshot + ", " + RTFNote + ")", "Session", "info");
        }
        // Save all notes on file, after every single note
        private void SaveToSessionNotes(string note)
        {
            Logger.record("[SaveToSessionNotes]: File will be updated and saved to " + sessionFile, "Session", "info");
            bool exDrRetry = false;

            do
            { exDrRetry = false;
                try
                {
                    File.AppendAllText(sessionFileFull, note);
                }
                catch (Exception ex)
                {
                    Logger.record("\t[SaveToSessionNotes]: EXCEPTION reached - Session Note file could not be saved (" + sessionFile + ")", "Session", "error");
                    exDrRetry = Logger.FileErrorMessage(ex, "SaveToSessionNotes", sessionFile);
                }
            } while (exDrRetry);
        }

        /** Reporting **/
        /***************/

        // General note on writign the reports to disk:
        //  Some bug reports seem to indicate that we are holding the file while trying to write to it.
        //  This is the reason of adding 150 milliseconds of delay between writes.
        //  I'll only know how dumb an idea is it after continuing (or not) to receive exception reports from users.

        // When a report collection is requested:
        public void CollectReport()
        {
            Logger.record("[CollectReport]: Report building", "Session", "info");

            bool exDrRetry = false;
            string report_prefix = "report_";
            string reportFile = report_prefix + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            string reportFileFull = workingDir + reportFile;
            String[] files = Directory.GetFiles(workingDir, "*.csv");
            Array.Sort(files);

            do
            { exDrRetry = false;
                try
                {
                    File.AppendAllText(reportFileFull, columnHeaders + "\n"); Thread.Sleep(150);
                    foreach (string file in files)
                    {
                        // Skip over other report files
                        if (file.Contains(report_prefix)) continue;
                        // Skip over files that don't end like ########_######.csv.
                      //  if (!( System.Text.RegularExpressions.Regex.IsMatch(file, ".*\\d{8}_\\d{6}.csv$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))) continue;

                        StreamReader sr = new StreamReader(file);
                        sr.ReadLine(); // We skip the first line (with the headers);

                        string oneFile = sr.ReadToEnd();
                        File.AppendAllText(reportFileFull, oneFile); Thread.Sleep(150);
                        Logger.record("\t[CollectReport]: Another file concatenated into a report: " + file, "Session", "info");
                        // TODO: Remove the Thread.Sleep(150) parts?
                    }
                }
                catch (Exception ex)
                {
                    Logger.record("[CollectReport]: EXCEPTION reached - Session Report file could not be saved (" + reportFile + ")", "Session", "error");
                    exDrRetry = Logger.FileErrorMessage(ex, "CollectReport", reportFile);
                }
            } while (exDrRetry);
            Logger.record("[CollectReport]: Report built, done.", "Session", "info");
            MessageBox.Show("Rapid Reporter has finished the report consolidation process.\nFile generated: " + reportFile, "Rapid Reporter -report consolidation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Transforming a CSV into a .HTML
        public void CSV2HTML(string CSVFile)
        {
            Logger.record("[CSV2HTML]: HTML Report building", "Session", "info");
            bool exDrRetry = false;

            string htmlFile = CSVFile; htmlFile = htmlFile.Replace(".csv", ".htm");
            string htmlFileFull = workingDir + htmlFile;

            string[] thisLine = new string[columnHeaders.Split(',').Length];

            do
            {
                exDrRetry = false;
                try
                {
                    string t = "th";
                    string tableLine = ""; string noteImage = ""; string noteRtf = "";
                    longstrings.htmlstrings.html_title = sessionFile;

                    File.Delete(htmlFileFull);
                    File.WriteAllText(htmlFileFull, longstrings.htmlstrings.a_html_header); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.c_javascript); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.d_style); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.g_html_body1); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, "<h1>Session Report | Powered by <a href=\"http://testing.gershon.info/reporter/\">Rapid Reporter</a></h1><br />"); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.i_toggle_auto); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.j_html_bodytable1); Thread.Sleep(150);
                    foreach (string line in File.ReadAllLines(workingDir + CSVFile))
                    {
                        if ("" == line) continue; // Some files have empty lines in it, we don't want to process these lines.
                        noteImage = ""; noteRtf = ""; // We clean this variables in order not to carry the last ones from the last iteration
                        thisLine = line.Split(',');

                        // Dealing with screenshot attachments (if they exist).
                        if (thisLine.Length > 4)
                        {
                            if (File.Exists(workingDir + thisLine[4]))
                            {
                                noteImage = "<a href=\"" + thisLine[4] + "\" target=\"_blank\"><img src=\"" + thisLine[4] + "\"></a>";
                            }
                            else noteImage = thisLine[4];
                        } noteImage += "&nbsp;";

                        // Dealing with the RTF note attachments (if they exist).
                        if(thisLine.Length > 5)
                        {
                            if (File.Exists(workingDir + thisLine[5]))
                            {
                                noteRtf = "<a href=\"" + thisLine[5] + "\" target=\"_blank\">" + thisLine[5] + "</a>";
                            }
                            else noteRtf = thisLine[5];
                        } noteRtf += "&nbsp;";
                        
                        tableLine =
                            "<tr class=\""+thisLine[2]+"\"> <"+t+">" + thisLine[0] +
                            "</"+t+"><"+t+">"+ thisLine[1] +
                            "</"+t+"><"+t+" class=\"notetype\">"+ thisLine[2] +
                            "</"+t+"><"+t+">"+ thisLine[3].Replace("\"","") +
                            "</"+t+"><"+t+">"+ noteImage +
                            "</"+t+"><"+t+">"+ noteRtf +
                            "</"+t+"></tr>\n";
                        File.AppendAllText(htmlFileFull, tableLine); Thread.Sleep(150);

                        t = "td";
                    }
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.m_html_bodytable2); Thread.Sleep(150);
                    File.AppendAllText(htmlFileFull, longstrings.htmlstrings.p_html_footer); Thread.Sleep(150);
                }
                catch (Exception ex)
                {
                    Logger.record("[CSV2HTML]: EXCEPTION reached - Session Report file could not be saved (" + htmlFile + ")", "Session", "error");
                    exDrRetry = Logger.FileErrorMessage(ex, "CSV to HTML", htmlFile);
                }
            } while (exDrRetry);
            Logger.record("[CSV2HTML]: HTML Report built, done.", "Session", "info");
            MessageBox.Show("Rapid Reporter has finished the process of transformation to HTML.\nFile created: " + htmlFile, "Rapid Reporter -tohtml transformation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}


