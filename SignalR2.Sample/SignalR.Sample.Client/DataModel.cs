using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR.Sample.Client
{
    public class FromServerToClientData
    {
        public DateTime Now { get; set; }
        public int Integer { get; set; }
        public string Text { get; set; }
    }

    public class FromClientToServerData
    {
        public string Text { get; set; }
    }
}