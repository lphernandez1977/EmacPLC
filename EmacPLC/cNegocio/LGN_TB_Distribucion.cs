using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EmacPLC
{
    public class LGN_TB_Distribucion
    {
        ACD_TB_Distribucion _ACD_TB_Distribucion = new ACD_TB_Distribucion();

        public DataSet Listado_Jornadas_EnProduccion()
        {
            DataSet ds = new DataSet();
            ds = _ACD_TB_Distribucion.Listado_Jornadas_EnProduccion();
            return ds;
        }

        public string Crear_Jornada_Automaticas()
        {
            string res;
            res = _ACD_TB_Distribucion.Crear_Jornada_Automaticas();
            return res;
        }

    }
}
