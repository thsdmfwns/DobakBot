using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
   public enum SlotResult
    {
        Error,
        Lose,
        Win,
        JackPot,
    }

    public enum SlotCard
    {
        None,
        Orange,
        Grape,
        Cherry,
        Card,
        Gate,
        Bell,
        Max,
    }

}
