using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface IBusiness
    {
        string ObjectID { get; set; }
        string Name { get; set; }
        string FirstDayOfWeek { get; set; }
    }

    public struct Business : IBusiness
    {
        public string ObjectID { get; set; }
        public string Name { get; set; }
        public string FirstDayOfWeek { get; set; }

        public Business(string name, string firstDayOfWeek)
        {
            ObjectID = string.Empty;
            Name = name;
            FirstDayOfWeek = firstDayOfWeek;
        }
    }
}
