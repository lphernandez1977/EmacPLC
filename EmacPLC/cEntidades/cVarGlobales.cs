using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmacPLC
{
    public class cVarGlobales
    {
        public string MensajePLC { get; set; }
        public string MensajeSCAN { get; set; }

        public static bool EstConePLC { get; set; }
        public static bool EstConeSCAN { get; set; }

        public static int  CodTipoDespacho { get; set; }
    }
}
