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

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

//Agregadas
using System.Data.SqlClient;


namespace EmacPLC
{
    public class ACD_Oracle
    {
        
        public string Inserta_VL_CBC_ORACLE(cENT_Vl_Cbc_Fdx oVl_Cbc)
        {
            string sql;
            sql = "INSERT INTO VL_CBC( " +
                                        "LECTURA," + "TIPLEC," + "FECHA," + "EXPEDICION," + "KEYUSR," +
                                        "KEYDEL," + "KILOS," + "LARGO," + "ANCHO," + "ALTO," +
                                        "VOLUMEN," + "KEYRCO," + "KEYBUL," + "RAMPAE," + "RAMPAS," +
                                        "FECHAE," + "FECHAS," + "F_SISTEMA," + "ID_UBI," + "FECHA_E," +
                                        "ST_INTERFACE, " + "CRITERIO," + "HOJAREP" + ")" +
            "VALUES (" +
                                        oVl_Cbc.lectura + "," + oVl_Cbc.tiplec + "," + "SYSDATE" + "," + oVl_Cbc.expedicion + "," + oVl_Cbc.keyusr + "," +
                                        oVl_Cbc.keydel + "," + oVl_Cbc.kilos.ToString().Replace(',', '.') + "," + oVl_Cbc.largo.ToString().Replace(',', '.') + "," + oVl_Cbc.ancho.ToString().Replace(',', '.') + "," + oVl_Cbc.alto.ToString().Replace(',', '.') + "," +
                                        Math.Round(oVl_Cbc.volumen, 3).ToString().Replace(',', '.') + "," + oVl_Cbc.keyrco + "," + oVl_Cbc.keybul + "," + oVl_Cbc.rampae + "," + oVl_Cbc.rampas + "," +
                                        "SYSDATE" + "," + "SYSDATE" + "," + "SYSDATE" + "," + oVl_Cbc.id_ubi + "," + "SYSDATE" + "," +
                                        oVl_Cbc.st_interface + "," + oVl_Cbc.criterio + "," + oVl_Cbc.hojarep + ")";


            string conexion = ConfigurationManager.ConnectionStrings["cnnString_Oracle"].ToString();
            ConexionOracle cn = new ConexionOracle();
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conexion;

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            con.Open();
            OracleCommand cmd_ora = con.CreateCommand();
            OracleTransaction trn_oracle;
            trn_oracle = con.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd_ora.Transaction = trn_oracle;

            try
            {
                cmd_ora.CommandText = sql;
                cmd_ora.ExecuteNonQuery();
                trn_oracle.Commit();

                //CIERRO Y ELIMINO ELEMENTO CONEXION
                con.Close();
                cmd_ora.Dispose();
                con.Dispose();

                //RegErrores.RegistroLog("query ejecutado bien");

                return "1";
            }
            catch (Exception ex)
            {
                trn_oracle.Rollback();

                //CIERRO Y ELIMINO ELEMENTO CONEXION
                con.Close();
                cmd_ora.Dispose();
                con.Dispose();
                return ex.Message.ToString();
            }
        }

        public cCorrelativoDB Listado_Correlativo_VL_CBC_SQL()
        {
            string MensajeSp = string.Empty;

            //leerRegistro As Action(Of SqlDataReader)
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_SELECT_CORR_FEDEX", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //cmd.Parameters.Add("@in_KeyBul", System.Data.SqlDbType.VarChar, 50).Value = oCartones.CartonLPN;

                        //abrimos conexion
                        con.Open();

                        //ejecuto query
                        SqlDataReader dataReader = cmd.ExecuteReader();

                        //Instanciamos al objeto Eproducto para llenar sus propiedades
                        cCorrelativoDB _cCorrelativoDB = new cCorrelativoDB();

                        if (dataReader.HasRows)
                        {
                            //Preguntamos si el DataReader fue devuelto con datos
                            while (dataReader.Read())
                            {
                                {
                                    _cCorrelativoDB._Corr_Base_Datos = Convert.ToInt32(dataReader["Corr"].ToString());

                                }
                            }

                            dataReader.Close();

                            return _cCorrelativoDB;
                        }
                        else
                        {
                            //se define que si no hay datos envia null
                            return null;
                        }

                    }

                    //return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    
      
    }
}
