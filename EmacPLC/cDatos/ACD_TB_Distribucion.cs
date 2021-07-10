using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EmacPLC
{
    public class ACD_TB_Distribucion
    {
        public DataSet Listado_Jornadas_EnProduccion()
        {
            DataSet ds = new DataSet();
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_Listado_Jornadas_En_Produccion", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //cmd.Parameters.Add("@pRut", System.Data.SqlDbType.Float).Value = rutsta;

                        //abrimos conexion
                        con.Open();

                        //Ejecutamos Procedimiento                        
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        //llenar el conjunto de datos utilizando los valores predeterminados para los nombres de DataTable, etc 
                        adapter.Fill(ds);

                        //Cierre conexion
                        con.Close();
                        //retorna los datos
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string Crear_Jornada_Automaticas()
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("SP_CREAR_JORNADAS_AUTOMATICAS", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.Add("@in_CorrJornada", System.Data.SqlDbType.Int).Value = loc_corr;
                //cmd.Parameters.Add("@in_Usuario", System.Data.SqlDbType.VarChar, 50).Value = in_usuario;

                //parametros de salida
                cmd.Parameters.Add("@out_Mensaje_Operacion", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@out_Mensaje_ErrorSql", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt16(cmd.Parameters["@out_Mensaje_Operacion"].Value.ToString());
                string MensajeErr = cmd.Parameters["@out_Mensaje_ErrorSql"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                cTB_Distribucion.oMensajes = MensajeErr;

                if (Salida == 1)
                {

                    return "1";
                }
                else
                {
                    return MensajeErr;
                }

            }
            catch (Exception ex)
            {
                cTB_Distribucion.oMensajes = ex.Message.ToString();

                return ex.Message.ToString();
            }

        }



    }
}
