using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.Windows
{
    public struct Configuration
    {
        internal static string EmailUserName { get; } = Properties.Settings.Default.EmailUserName;

        internal static string EmailPassword { get; } = Properties.Settings.Default.EmailPassword;

        internal static string SendEmailTo { get; } = Properties.Settings.Default.SendEmailTo;

        internal static string ParseApplicationId { get; } = Properties.Settings.Default.ParseApplicationID; 

        internal static string ParseDotNetId { get; } = Properties.Settings.Default.ParseDotNetKey;
       
        internal static string DBConnection { get; } = Properties.Settings.Default.DBConnection;

        public static string FirstDayOfWeek
        {
            get { return Properties.Settings.Default.FirstDayOfWeek; }
            set { Properties.Settings.Default.FirstDayOfWeek = value; Properties.Settings.Default.Save(); }
        }

        public static System.Globalization.CultureInfo CultureInfo
        {
            get { return new System.Globalization.CultureInfo(Properties.Settings.Default.CultureInfo); }
            set { Properties.Settings.Default.CultureInfo = value.ToString(); Properties.Settings.Default.Save(); }
        }

        public static string ReportFolderLocation
        {
            get { return Properties.Settings.Default.ReportLocation; }
            set { Properties.Settings.Default.ReportLocation = value; Properties.Settings.Default.Save(); }
        }

        public static string Business
        {
            get { return Properties.Settings.Default.Business; }
            set { Properties.Settings.Default.Business = value; Properties.Settings.Default.Save(); }
        }

        public static string Location

        {
            get { return Properties.Settings.Default.Location; }
            set { Properties.Settings.Default.Location = value; Properties.Settings.Default.Save(); }
        }

        internal static bool IsNewInstallation
        {
            get { return Properties.Settings.Default.IsNewInstallation; }
            set { Properties.Settings.Default.IsNewInstallation = value; Properties.Settings.Default.Save(); }
        }

        internal static string AccessKey
        {
            get { return Properties.Settings.Default.AccessKey; }
            set { Properties.Settings.Default.AccessKey = value; Properties.Settings.Default.Save(); }
        }

        internal static string Phone
        {
            get { return Properties.Settings.Default.PhoneNumber; }
            set { Properties.Settings.Default.PhoneNumber = value; Properties.Settings.Default.Save(); }
        }

        public static void ValidateConfiguration()
        {
            if (SettingsAreNotAssigned())
                new Forms.FormConfigureBusiness().ShowDialog();            
        }

        internal static bool SettingsAreNotAssigned()
        {
            return StringIsNullOrEmpty(Business) ||
                StringIsNullOrEmpty(Location) ||
                StringIsNullOrEmpty(FirstDayOfWeek) ||
                StringIsNullOrEmpty(ReportFolderLocation) ||
                StringIsNullOrEmpty(AccessKey);
        }

        static bool StringIsNullOrEmpty(string text)
        {
            return string.IsNullOrEmpty(text);
        }
    }
}
