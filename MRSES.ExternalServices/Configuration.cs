namespace MRSES.ExternalServices
{
    public struct Configuration
    {
        public static string Business { internal get; set; }

        public static System.DateTime LastSyncDate
        {
            get { return Properties.Settings.Default.LastSyncDate; }
            set { Properties.Settings.Default.LastSyncDate = value; Properties.Settings.Default.Save(); }
        }
    }
}
