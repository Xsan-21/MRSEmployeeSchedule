namespace MRSES.Core
{
    public static class Configuration
    {
        internal static string PostgresDbConnection
        {
            get { return Properties.Settings.Default.PostgresDBConnection; }
        }

        public static string FirstDayOfWeek 
        { 
            get { return Properties.Settings.Default.FirstDayOfWeek; }
            set { Properties.Settings.Default.FirstDayOfWeek = value; Properties.Settings.Default.Save(); } 
        }

        public static string StoreLocation
        {
            get { return Properties.Settings.Default.StoreLocation; }
            set { Properties.Settings.Default.StoreLocation = value; Properties.Settings.Default.Save(); } 
        }

        public static string CultureInfo
        {
            get { return Properties.Settings.Default.CultureInfo; }
        }
    }
}
