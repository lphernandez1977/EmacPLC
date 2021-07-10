using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EmacPLC
{
    public class LGN_Insertar_Lecturas
    {
        
        ACD_Tb_Lectura_Sorter _ACD_Tb_Lectura_Sorter = new ACD_Tb_Lectura_Sorter();

        public cTb_Lectura_Cartones Inserta_Lecturas_Cartones(cTb_Lectura_Cartones oTb_Lectura_Cartones)
        {
            cTb_Lectura_Cartones ocTb_Lectura_Cartones = new cTb_Lectura_Cartones();
            string res = string.Empty;

            ocTb_Lectura_Cartones = _ACD_Tb_Lectura_Sorter.Inserta_Lectura_Cartones(oTb_Lectura_Cartones);

            return ocTb_Lectura_Cartones;
        }

        public string Modificar_CartonLPN(int _Linea, Int32 _CartonLPN)
        {      
            string res = string.Empty;

            res = _ACD_Tb_Lectura_Sorter.Modificar_Lectura_CartonFedex(_Linea, _CartonLPN);

            return res;
        }

        public string Modificar_CartonLPN_Recirc(Int32 _CartonLPN, int _LineaRecirc)
        {
            string res = string.Empty;

            res = _ACD_Tb_Lectura_Sorter.Modificar_CartonLPN_Recirc(_CartonLPN, _LineaRecirc);

            return res;
        }

        public DataSet Consultar_CartonLPN_Recirc(Int32 _CartonLPN)
        {
            DataSet ds = new DataSet();

            ds = _ACD_Tb_Lectura_Sorter.Consultar_CartonLPN_Recirc(_CartonLPN);

            return ds;
        }

        public string Modificar_CartonLPN_FullLine(int _Linea, string _CartonLPN)
        {
            string res = string.Empty;

            res = _ACD_Tb_Lectura_Sorter.Modificar_CartonLPN_FullLine(_Linea, _CartonLPN);

            return res;
        }

        public DataSet Listado_CartonesLeidos()
        {
            DataSet ds = new DataSet();

            ds = _ACD_Tb_Lectura_Sorter.Listado_CartonesLeidos();

            return ds;
        }


        public cTb_Lectura_Cartones Listado_SP_SELECCIONA_SALIDA_FEDEX(cTb_Lectura_Cartones oCartones)
        {
            cTb_Lectura_Cartones oSalida = new cTb_Lectura_Cartones();

            oSalida = _ACD_Tb_Lectura_Sorter.Listado_SP_SELECCIONA_SALIDA_FEDEX(oCartones);

            return oSalida;
        }

        public cTb_Lectura_Cartones Listado_SP_SELECCIONA_SALIDA_FASA(cTb_Lectura_Cartones oCartones)
        {
            cTb_Lectura_Cartones oSalida = new cTb_Lectura_Cartones();

            oSalida = _ACD_Tb_Lectura_Sorter.Listado_SP_SELECCIONA_SALIDA_FASA(oCartones);

            return oSalida;
        }

        public string Inserta_Lecturas_CartonesFedex(cENT_Vl_Cbc_Fdx oTb_Lectura_Cartones)
        {
            cTb_Lectura_Cartones ocTb_Lectura_Cartones = new cTb_Lectura_Cartones();
            string res;

            res = _ACD_Tb_Lectura_Sorter.Inserta_Lectura_CartonesFedex(oTb_Lectura_Cartones);

            return res;
        }
            
    }
}
