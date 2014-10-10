using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.ExternalServices
{
    public static class Configuration
    {
        internal static string PostgresDbConnection
        {
            get { return Properties.Settings.Default.PostgresDbConection; }
        }

        public static string StoreLocation
        {
            get { return Properties.Settings.Default.StoreLocation; }
            set { Properties.Settings.Default.StoreLocation = value; Properties.Settings.Default.Save(); }
        }
    }
}
