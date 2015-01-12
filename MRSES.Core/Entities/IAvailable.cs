using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface IAvailable
    {
        System.Threading.Tasks.Task<bool> CanDoTheTurnAsync(string employeeName, ITurn turn);
    }
}
