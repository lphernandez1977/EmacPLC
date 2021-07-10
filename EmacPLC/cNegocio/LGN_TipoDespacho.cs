using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EmacPLC
{
    public class LGN_TipoDespacho
    {
        ACD_TipoDespacho _ACD_TipoDespacho = new ACD_TipoDespacho();
 
        public string Inserta_Tipo_Despacho(cEnt_TB_Tipo_Despacho oTiposDespacho)
        {     
            string res = string.Empty;

            res = _ACD_TipoDespacho.Inserta_TB_TipoDespacho(oTiposDespacho);

            return res;
        }

        public DataSet Listado_TipoDespacho()
        {
            DataSet ds = new DataSet();

            ds = _ACD_TipoDespacho.Listado_TipoDespacho();

            return ds;
        }

        public string Actualiza_Tipo_Despacho(cEnt_TB_Tipo_Despacho oTiposDespacho)
        {
            string res = string.Empty;

            res = _ACD_TipoDespacho.Actualiza_Tipo_Despacho(oTiposDespacho);

            return res;
        }
    }
}
