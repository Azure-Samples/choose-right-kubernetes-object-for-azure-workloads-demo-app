using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerDemoApp.Models
{
    public class IncomingRequest
    {
        public DateTime AddedOn { get; set; }
        public string ConnectionID { get; set; }
        public string IP { get; set; }
        public string CartItem { get; set; }
    }
}
