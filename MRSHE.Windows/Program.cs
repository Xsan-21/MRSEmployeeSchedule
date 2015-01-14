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
            Parse.ParseClient.Initialize(Properties.Settings.Default.EmailUserName, Properties.Settings.Default.EmailPassword);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
