using System;
using System.Windows.Forms;

namespace MRSES.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Parse.ParseInstallation();
            Parse.ParseClient.Initialize(Properties.Settings.Default.ParseApplicationID, Properties.Settings.Default.ParseDotNetKey);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        /// <summary>
        /// Gets the current store that is using this application.
        /// </summary>
        static public string StoreLocation { get { return Properties.Settings.Default.StoreLocation; } }
    }
}
