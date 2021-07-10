using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
//using System.Data.OracleClient;
using System.Configuration;
using System.Data.OleDb;
//using Oracle.DataAccess.Client; 

namespace EmacPLC
{
    public class ConexionOracle
    {
        public string StrOra()
        {            
            string conexion = ConfigurationManager.ConnectionStrings["cnnString_Oracle"].ToString();
            return conexion;
        }
    }
}
