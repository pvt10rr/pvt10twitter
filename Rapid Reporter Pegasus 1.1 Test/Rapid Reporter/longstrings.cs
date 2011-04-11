namespace longstrings
{
	public static class helpstrings
	{
		// Help is:
		// 0 - Normal help inside application
		// 1 - -help help
		// 2 - -report help
		// 3 - -tohtml help
		public static string[] helpstring = new string[4] {"","","", ""};
		static helpstrings()
		{

			//****************************************/
			helpstring[0] = @"Welcome to Rapid Reporter!

Basic usage:
--------------
1) Run the application from your working folder (no installation necessary!)
	o   All the report files and attachments will be created in this directory.
2) Enter your name and your charter.
3) Start entering notes.
	o   Every single note you write in the main window is saved automatically.
	o   You can change the type of the note easily by pressing the up/down arrows, on
	    the fly .
4) Add screenshots to the current note by pressing the button with the 'camera' icon.
	o   By pressing SHIFT while clicking the button you can edit the screenshot (crop
	    or highlight a part)
5) You can add an extended text to the current note by pressing the button with the
	'notebook' icon.
	o   The extended note is useful for tracking error messages, logs, pictures...
	o   The extended note's format is rich text.
	o   Note: This area is persistent, so you can use it as a place for persistent
	    information between notes.
";

			//****************************************/
			helpstring[1] = @"Welcome to Rapid Reporter!

By using the '-help' or '/h' switch, you are presented with the command line arguments
supported by the app:

-help, /h or /?:
		Displays this help.

-report:		Consolidates all the *.csv reports into one report.
		Used for preparing an all encompassing report of the sessions done.
		Warning: At this moment, any file that ends with *.csv will be used in this
		report, except other reports.

-tohtml <FILENAME.CSV>:
		Transforms a CSV file with session information into an HTML file with thumbnails
		and links, to be used during the session review.
		You need to specify which of the CSV files you want to transform.
		This feature can be executed after executing -report, in order to get a single
		file account of all sessions.

-directory <FOLDER PATH>:
		Changes the working folder to the one provided. Will work on normal sessions,
		when doing reports and HTML transformations.
		If the PATH provided is incorrect, the current directory will be used instead.

Any other:	Any other words separated by space given to Rapid Reporter will be used as
		note types during the session instead of the default types. This is useful for
		using your own terminology or attending other context-based necessities.
		For example, 'RapidReporter.exe Note Question Lookup' will start a Rapid Reporter
		session with the note types Note, Question and Lookup available.

A description of the basic usage and operation of Rapid Reporter can be found inside the
About dialog.
";

			//****************************************/
			helpstring[2] = @"Welcome to Rapid Reporter!

By using the '-report' switch, you are consolidating all the *.csv reports into one report.
This is very useful when you are preparing a complete report of the sessions done until now.
The application will generate the new file with the the 'report_' prefix and exit.

Warning: At this moment, any file that ends with *.csv will be used in this report,
except other reports.

A dialog box will pop up alerting you when the consolidation is done.
";

			//****************************************/
            helpstring[3] = @"Welcome to Rapid Reporter!

By using the '-tohtml' switch, you are transforming a CSV file into an HTML file including
a table of the session.
This is useful for having a different view of the session, in addition to the spreadsheet
one (very useful to show managers!). The HTML page can be stylized by using a CSS file
called style.css.

You should provide the CSV file to be used as the next word after this command. For example:
		RapidReporter.exe -tohtml  20100915_100059.csv

The application will generate the new with the same filename as the CSV file and exit.
Image files will be shown as linked thumbnails and the RTF attachments will be hyperlinked
too (embedded RTF notes are poorly supported by browsers).

A dialog box will pop up alerting you when the report transformation is done.
";
		}
	}

	public static class htmlstrings
	{
		// Dynamic
		public static string html_title = "Session Report";

		// Static values
		//  the letter at the beginning of the var name is to hint about their order
		public static string a_html_header = "";
		public static string c_javascript = "";
		public static string d_style = "";
		public static string g_html_body1 = "";
        public static string i_toggle_auto = "";
		public static string j_html_bodytable1 = "";
		public static string m_html_bodytable2 = "";
		public static string p_html_footer = "";
		static htmlstrings()
		{

			//****************************************/
			a_html_header = @"
<html>
<head>
<title>" + html_title + @"</title>
";
            //****************************************/
            c_javascript = @"
<SCRIPT>
    <!--
    function showRow(obj)
    {
        var allTrs = document.getElementsByTagName(""tr"");
        var display2 = ""none"";
        if(true == obj.checked)
        {
            display2 = """";
        }
        for(var i=0; i < allTrs.length ; i++)
        {
            if(""autogenerated"" == allTrs[i].className)
            {
                allTrs[i].style.display = display2;
            }
        }
    }
    //-->
</SCRIPT> 
";

			//****************************************/
			d_style = @"
<style>
H1 {text-align: center;}
table {margin-left: auto; margin-right: auto;}
table tr img {width: 250px; resize-scale: showall;}
table tr.Session {font-weight: bold; background: #ffff99;}
table tr.Bug {background: #FF7878;}
table tr.Test {background: #FFFFE0;}
table tr.Note {background: #00FFFF;}
table tr.Setup {background: #E3E3E3;}
table tr.autogenerated {color: #C0C0C0; font-style: italic;}
table tr.autogenerated img {width: 100px; resize-scale: showall;}
table td.notetype {font-weight: bold;}
</style>
<link rel=""stylesheet"" type=""text/css"" href=""style.css"" />
";

			//****************************************/
			g_html_body1 = @"
</head>
<body>
<div id=""allbody"">
";

            //****************************************/
            i_toggle_auto = @"
<input type=""checkbox"" checked=""true"" id=""1"" onClick=""showRow(this)""/> Show autogenerated rows
";

			//****************************************/
			j_html_bodytable1 = @"
<div id=""aroundtable"">
<table border=""1"">
";

			//****************************************/
			m_html_bodytable2 = @"
</table>
</div>
";

			//****************************************/
			p_html_footer = @"
</div>
</body>
</html>
";
		}
	}
}