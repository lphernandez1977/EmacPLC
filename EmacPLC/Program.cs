using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EmacPLC
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {

//#if DEBUG
//            EmacPLC myservice = new EmacPLC();
//            myservice.OnDebug();
//            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
//#else

//#endif

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new EmacPLC() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
