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
    public class ACD_TipoDespacho
    {
        //cRegistroErr oError = new cRegistroErr();

        public string Inserta_TB_TipoDespacho(cEnt_TB_Tipo_Despacho oTipoDespacho)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("SP_Inserta_TB_Tipo_Despacho", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_CodTipDespacho", System.Data.SqlDbType.Int).Value = oTipoDespacho.CodTipDespacho;
                cmd.Parameters.Add("@in_NomEstado", System.Data.SqlDbType.VarChar, 20).Value = oTipoDespacho.NomEstado;
                cmd.Parameters.Add("@in_UsuarioRegistro", System.Data.SqlDbType.VarChar, 20).Value = oTipoDespacho.UsuarioRegistro;

                
                //parametros de salida
                cmd.Parameters.Add("@out_Mensaje_Operacion", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@out_Mensaje_ErrorSql", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt16(cmd.Parameters["@out_Mensaje_Operacion"].Value.ToString());
                string MensajeErr = cmd.Parameters["@out_Mensaje_ErrorSql"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

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
                
                return ex.Message.ToString();
            }

        }

        public DataSet Listado_TipoDespacho()
        {
            DataSet ds = new DataSet();
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_Listado_TB_Tipo_Despacho", con))
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
            catch (SqlException ex)
            {
                return null;
            }
        }

        public string Actualiza_Tipo_Despacho(cEnt_TB_Tipo_Despacho oTipoDespacho)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("SP_Actualiza_TB_Tipo_Despacho", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_Id", System.Data.SqlDbType.Int).Value = oTipoDespacho.CodEstado;
                cmd.Parameters.Add("@in_CodTipDespacho", System.Data.SqlDbType.Int).Value = oTipoDespacho.CodTipDespacho;
                cmd.Parameters.Add("@in_NomEstado", System.Data.SqlDbType.VarChar, 20).Value = oTipoDespacho.NomEstado;
                cmd.Parameters.Add("@in_UsuarioRegistro", System.Data.SqlDbType.VarChar, 20).Value = oTipoDespacho.UsuarioRegistro;


                //parametros de salida
                cmd.Parameters.Add("@out_Mensaje_Operacion", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@out_Mensaje_ErrorSql", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt16(cmd.Parameters["@out_Mensaje_Operacion"].Value.ToString());
                string MensajeErr = cmd.Parameters["@out_Mensaje_ErrorSql"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

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
                
                return ex.Message.ToString();
            }

        }
    }
}
