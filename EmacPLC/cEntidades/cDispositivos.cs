using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmacPLC
{
    public class cDispositivos
    {
        public string IpPLC { get; set; }
        public Int16 PortPLC { get; set; }
        public Int16 RackPLC { get; set; }
        public Int16 SlotPLC { get; set; }
        public int NumDb { get; set; }
        public string PtoCOM { get; set; }
        public string IpScanner { get; set; }
        public Int16 PortScanner { get; set; }
        public string IpBascula { get; set; }
        public Int16 PortBascula { get; set; }
        public int Tiempo { get; set; }
    }
}
