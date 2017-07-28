using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spacehack.Interfaces
{
    interface IBehavior
    {
        bool Act(Core.Monster monster, Core.CommandSystem commandSystem);
    }
}
