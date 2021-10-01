using Exiled.API.Features;
using ServerManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager.Models
{
    public class UpdateServerModel
    {
        public int ServerPort => Server.Port;
        public ServerState? State { get; set; }
        public int? OnlinePlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public string SLVersion { get; set; }
        public string ExiledVersion { get; set; }
    }
}
