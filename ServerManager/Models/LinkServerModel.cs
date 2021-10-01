using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager.Models
{
    public class LinkServerModel
    {
        public Guid Token { get; set; }
        public int ServerPort { get; set; }
    }
}
