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
            Configuration.ValidateConfiguration();
            Parse.ParseClient.Initialize(Configuration.ParseApplicationId, Configuration.ParseDotNetId);
            Core.Configuration.DbConnection = Configuration.DBConnection;
            Core.Configuration.Business = Configuration.Business;
            Core.Configuration.Location = Configuration.Location;
            Core.Configuration.FirstDayOfWeek = Configuration.FirstDayOfWeek;
            Core.Configuration.ReportFolderLocation = Configuration.ReportFolderLocation;
            Core.Configuration.AccessKey = Configuration.AccessKey;
            Core.Configuration.CultureInfo = Configuration.CultureInfo;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
