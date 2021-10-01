using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager.Enums
{
    public enum ServerState : int
    {
        Offline,
        Starting,
        WaitingForPlayers,
        RoundInProgress,
        RoundEnded,
        Restarting
    }
}
