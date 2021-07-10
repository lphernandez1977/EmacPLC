using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmacPLC
{
    public class cEnt_TB_Tipo_Despacho
    {
        public int CodTipDespacho {get;set;}
	    public string UsuarioRegistro {get;set;}
	    public DateTime Fecha_Registo {get;set;}
	    public int CodEstado {get;set;}
        public string NomEstado { get; set; }

        public static int glo_CodTipoDespacho { get; set; }
    }
}
