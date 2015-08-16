namespace MRSES.Core
{
    public struct Configuration
    {
        public static string Business
        {
            internal get;
            set;
        }

        public static string DbConnection
        {
            internal get;
            set;            
        }

        public static string AccessKey
        {
            internal get;
            set;
        }

        public static string ReportFolderLocation
        {
            internal get;
            set;
        }

        public static string FirstDayOfWeek
        {
            internal get;
            set;
        }

        public static System.Globalization.CultureInfo CultureInfo
        {
            internal get;
            set;
        }

        internal static System.DateTime LastSyncDate
        {
            get { return Properties.Settings.Default.LastSyncDate; }
            set { Properties.Settings.Default.LastSyncDate = value; Properties.Settings.Default.Save(); }
        }
    }
}
