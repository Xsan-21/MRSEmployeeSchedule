namespace MRSES.Core
{
    public static class Configuration
    {
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

        public static string ParseApplicationID
        {
            get { return Properties.Settings.Default.ParseApplicationID; }
        }

        public static string ParseDotNetKey
        {
            get { return Properties.Settings.Default.ParseDotNetKey; }
        } 
    }
}
