namespace MRSES.Core
{
    public static class Configuration
    {
        public static string FirstDayOfWeek 
        { 
            get { return Properties.Settings.Default.FirstDayOfWeek; }
            set { Properties.Settings.Default.FirstDayOfWeek = value; Properties.Settings.Default.Save(); } 
        }
    }
}
