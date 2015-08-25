namespace MRSES.Core
{
    public struct Configuration
    {
        public static string Business
        {
            internal get;
            set;
        }

        public static string Location
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
    }
}
