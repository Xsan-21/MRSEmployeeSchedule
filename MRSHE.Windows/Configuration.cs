using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.Windows
{
    internal class Configuration
    {
        public static string EmailUserName
        {
            get { return Properties.Settings.Default.EmailUserName; }
        }

        public static string EmailPassword
        {
            get { return Properties.Settings.Default.EmailPassword; }
        }
    }
}
