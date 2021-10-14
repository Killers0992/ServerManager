using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string ApiKey { get; set; } = "INVALID";
        public string ApiEndpoint { get; set; } = "https://exiledplugins.kingsplayground.fun/api";
    }
}
