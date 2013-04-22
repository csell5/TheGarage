﻿using System;

namespace SignalR.Sample
{
    public class FromServerToClientData
    {
        public DateTime Now { get; set; }
        public int Integer { get; set; }
        public string Text { get; set; }
        public string Command { get; set; }
    }

    public class FromClientToServerData
    {
        public string Text { get; set; }
        public string Command { get; set; }
    }
}