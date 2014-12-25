namespace MRSES.ExternalServices
{
    public static class Configuration
    {
        public static string StoreLocation
        {
            get { return Properties.Settings.Default.StoreLocation; }
            set { Properties.Settings.Default.StoreLocation = value; Properties.Settings.Default.Save(); } 
        }
    }
}
