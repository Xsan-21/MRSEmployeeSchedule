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
            if (string.IsNullOrEmpty(MRSES.Core.Configuration.StoreLocation))
                new Forms.FormSelectStore().ShowDialog();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
