using System.Linq;
using System.Collections.Generic;
using NodaTime;
using MRSES.Core.Entities;

namespace MRSES.Core.Shared
{
    public struct ComboBoxFunctions
    {
        static public IEnumerable<string> GetCurrentAndNextThreeWeeks()
        { 
            foreach (var week in WorkWeek.GetCurrentAndNextThreeWeeksFrom(WorkWeek.CurrentWeek()))
            {
                yield return week.ToString();
            }            
        }

        // TODO put here the employee fill for comboboxes
    }
}
