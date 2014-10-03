using System.Linq;
using System.Collections.Generic;
using NodaTime;
using MRSES.Core.Entities;

namespace MRSES.Core.Shared
{
    public struct ComboBoxFunctions
    {
        static public List<LocalDate> FillWeekSelectorComboBox()
        {           
            var week =  WorkWeek.CurrentWeek();
            var weeksGenerated = WorkWeek.GetNextFourWeeksFrom(week);
            return weeksGenerated.ToList();            
        }

        // TODO put here the employee fill for comboboxes
    }
}
