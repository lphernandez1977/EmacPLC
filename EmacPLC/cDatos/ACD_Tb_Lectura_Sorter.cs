using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Agregadas
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace EmacPLC
{
    public class ACD_Tb_Lectura_Sorter
    {
        //cRegistroErr RegLogError = new cRegistroErr();

        public cTb_Lectura_Cartones Inserta_Lectura_Cartones(cTb_Lectura_Cartones oTb_Lectura_Cartones)
        {
            cTb_Lectura_Cartones oLecturaCartones = new cTb_Lectura_Cartones();
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("sp_Insert_Lectura_Cartones", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pCartonLPN", System.Data.SqlDbType.VarChar, 20).Value = oTb_Lectura_Cartones.CartonLPN;
                cmd.Parameters.Add("@pStatusUpdFlag", System.Data.SqlDbType.Bit).Value = oTb_Lectura_Cartones.StatusUpdFlag;
                            
                //parametros de salida
                cmd.Parameters.Add("@pDestino", SqlDbType.VarChar,3).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pLinea", SqlDbType.VarChar,3).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pDtd", SqlDbType.VarChar, 7).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                cnx.Open();
                res = cmd.ExecuteNonQuery();

                oLecturaCartones.Store = cmd.Parameters["@pDestino"].Value.ToString();
                oLecturaCartones.Lane = Convert.ToInt32(cmd.Parameters["@pLinea"].Value);
                oLecturaCartones.DtdSalida = cmd.Parameters["@pDtd"].Value.ToString();
                oLecturaCartones.Result = cmd.Parameters["@Resultado"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                return oLecturaCartones;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        //public string Editar_Tb_Usuarios(cTb_Usuarios oTb_Usuarios)
        //{
        //    int res = 0;
        //    try
        //    {
        //        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
        //        SqlCommand cmd = new SqlCommand("sp_Modifica_Usuarios", cnx);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        cmd.Parameters.Add("@pNom_Usuario", System.Data.SqlDbType.VarChar, 20).Value = oTb_Usuarios.Nom_Usuario;
        //        cmd.Parameters.Add("@pPass", System.Data.SqlDbType.VarChar, 20).Value = oTb_Usuarios.Contrasenia;
        //        cmd.Parameters.Add("@pNivel", System.Data.SqlDbType.Int).Value = oTb_Usuarios.Nivel;
        //        cmd.Parameters.Add("@pEstado", System.Data.SqlDbType.Bit).Value = oTb_Usuarios.Estado_Usuario;

        //        //parametros de salida
        //        cmd.Parameters.Add("@Pgi_Salida", SqlDbType.Int).Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add("@pMensaje_Error", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add("@pMensaje_Ok", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
        //        cnx.Open();
        //        res = cmd.ExecuteNonQuery();

        //        int Salida = Convert.ToInt32(cmd.Parameters["@Pgi_Salida"].Value);
        //        string MensajeOK = cmd.Parameters["@pMensaje_Ok"].Value.ToString();
        //        string MensajeErr = cmd.Parameters["@pMensaje_Error"].Value.ToString();

        //        cnx.Close();
        //        cnx.Dispose();

        //        if (Salida == 1)
        //        {
        //            return "1";
        //        }
        //        else
        //        {
        //            return MensajeErr;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message.ToString();
        //    }
        //}

        public string Modificar_CartonLPN(int _Linea, string _CartonLPN)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("sp_Modifica_CartonLPN", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pCarton", System.Data.SqlDbType.VarChar, 13).Value = _CartonLPN;
                cmd.Parameters.Add("@pLinea", System.Data.SqlDbType.Int).Value = _Linea;

                //parametros de salida
                cmd.Parameters.Add("@pSalida", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pMensaje", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@pMensaje_Ok", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt32(cmd.Parameters["@pSalida"].Value);
                string MensajeOK = cmd.Parameters["@pMensaje"].Value.ToString();
                //string MensajeErr = cmd.Parameters["@pMensaje_Error"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                if (Salida == 1)
                {
                    return "1";
                }
                else
                {
                    return MensajeOK;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string Modificar_CartonLPN_Recirc(Int32 _CartonLPN, int _Lin_Recirc)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("sp_Modifica_CartonLPN_Recirc", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pLectura", System.Data.SqlDbType.Float).Value = _CartonLPN;
                cmd.Parameters.Add("@pLineaRecirc", System.Data.SqlDbType.Int).Value = _Lin_Recirc;

                //parametros de salida
                cmd.Parameters.Add("@pSalida", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pMensaje", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@pMensaje_Ok", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt32(cmd.Parameters["@pSalida"].Value);
                string MensajeOK = cmd.Parameters["@pMensaje"].Value.ToString();
                //string MensajeErr = cmd.Parameters["@pMensaje_Error"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                if (Salida == 1)
                {
                    //RegLogError.RegistroLog(MensajeOK);
                    return "1";
                }
                else
                {
                    //RegLogError.RegistroLog(MensajeOK);
                    return MensajeOK;
                }

            }
            catch (Exception ex)
            {                
                return ex.Message.ToString();
            }
        }

        public DataSet Consultar_CartonLPN_Recirc(Int32 _CartonLPN)
        {
            DataSet ds = new DataSet();
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_CONSULTA_RECIRC", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pLectura", System.Data.SqlDbType.Float).Value = _CartonLPN;
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

        public string Modificar_CartonLPN_FullLine(int _Linea, string _CartonLPN)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("sp_Modifica_CartonLPN_Full", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pCarton", System.Data.SqlDbType.VarChar, 13).Value = _CartonLPN;
                cmd.Parameters.Add("@pLinea", System.Data.SqlDbType.Int).Value = _Linea;

                //parametros de salida
                cmd.Parameters.Add("@pSalida", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pMensaje", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@pMensaje_Ok", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt32(cmd.Parameters["@pSalida"].Value);
                string MensajeOK = cmd.Parameters["@pMensaje"].Value.ToString();
                //string MensajeErr = cmd.Parameters["@pMensaje_Error"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                if (Salida == 1)
                {
                    return "1";
                }
                else
                {
                    return MensajeOK;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public DataSet Listado_CartonesLeidos()
        {
            DataSet ds = new DataSet();
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("sp_Lectura_Cartones", con))
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
       
        public cTb_Lectura_Cartones Listado_SP_SELECCIONA_SALIDA_FEDEX(cTb_Lectura_Cartones oCartones)
        {
            string MensajeSp = string.Empty;

            //leerRegistro As Action(Of SqlDataReader)
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_SELECCIONA_SALIDA_FEDEX", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@in_KeyBul", System.Data.SqlDbType.VarChar, 50).Value = oCartones.CartonLPN;
    
                        //abrimos conexion
                        con.Open();

                        //ejecuto query
                        SqlDataReader dataReader = cmd.ExecuteReader();

                        //Instanciamos al objeto Eproducto para llenar sus propiedades
                        cTb_Lectura_Cartones _cTb_Lectura_Cartones = new cTb_Lectura_Cartones();

                        if (dataReader.HasRows)
                        {
                            //Preguntamos si el DataReader fue devuelto con datos
                            while (dataReader.Read())
                            {
                                {
                                    _cTb_Lectura_Cartones.Lane = Convert.ToInt32(dataReader["Anden_Destino"].ToString());
                                    _cTb_Lectura_Cartones.DtdSalida = dataReader["Desc_Destino"].ToString();
                                }
                            }

                            dataReader.Close();

                            return _cTb_Lectura_Cartones;
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
                //RegLogError.RegistroLog(ex.Message.ToString() + " Error generar salidas fedex");
                return null;
            }
        }

        public cTb_Lectura_Cartones Listado_SP_SELECCIONA_SALIDA_FASA(cTb_Lectura_Cartones oCartones)
        {
            string MensajeSp = string.Empty;

            //leerRegistro As Action(Of SqlDataReader)
            try
            {
                //Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString()))
                {
                    //asignar los valores proporcionados a estos parámetros sobre la base de orden de los parámetros
                    using (SqlCommand cmd = new SqlCommand("SP_SELECCIONA_SALIDA_FASA", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@in_KeyBul", System.Data.SqlDbType.VarChar, 50).Value = oCartones.CartonLPN;

                        //abrimos conexion
                        con.Open();

                        //ejecuto query
                        SqlDataReader dataReader = cmd.ExecuteReader();

                        //Instanciamos al objeto Eproducto para llenar sus propiedades
                        cTb_Lectura_Cartones _cTb_Lectura_Cartones = new cTb_Lectura_Cartones();

                        if (dataReader.HasRows)
                        {
                            //Preguntamos si el DataReader fue devuelto con datos
                            while (dataReader.Read())
                            {
                                {
                                    _cTb_Lectura_Cartones.Lane = Convert.ToInt32(dataReader["Tobogan"].ToString());
                                    _cTb_Lectura_Cartones.DtdSalida = dataReader["Descripcion"].ToString();
                                }
                            }

                            dataReader.Close();

                            return _cTb_Lectura_Cartones;
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
                //RegLogError.RegistroLog(ex.Message.ToString() + " Error generar salidas fedex");
                return null;
            }
        }

        public string Inserta_Lectura_CartonesFedex(cENT_Vl_Cbc_Fdx oTb_Lectura_Cartones)
        {
            cTb_Lectura_Cartones oLecturaCartones = new cTb_Lectura_Cartones();
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("SP_INSERTA_TABLA_FEDEX_VL_CBC", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_LECTURA", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.lectura;
                cmd.Parameters.Add("@in_TIPLEC", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.tiplec;
                cmd.Parameters.Add("@in_EXPEDICION", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.expedicion;
                cmd.Parameters.Add("@in_KEYUSR", System.Data.SqlDbType.VarChar, 50).Value = oTb_Lectura_Cartones.keyuser;
                cmd.Parameters.Add("@in_KEYDEL", System.Data.SqlDbType.VarChar, 50).Value = oTb_Lectura_Cartones.keydel;
                cmd.Parameters.Add("@in_KILOS", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.kilos;
                cmd.Parameters.Add("@in_LARGO", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.largo;
                cmd.Parameters.Add("@in_ANCHO", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.ancho;
                cmd.Parameters.Add("@in_ALTO", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.alto;
                cmd.Parameters.Add("@in_VOLUMEN", System.Data.SqlDbType.Float).Value = Math.Round(oTb_Lectura_Cartones.volumen,3);
                cmd.Parameters.Add("@in_KEYRCO", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.keyrco;
                cmd.Parameters.Add("@in_KEYBUL", System.Data.SqlDbType.VarChar,50).Value = oTb_Lectura_Cartones.keybul;
                cmd.Parameters.Add("@in_RAMPAE", System.Data.SqlDbType.VarChar, 50).Value = oTb_Lectura_Cartones.rampae;
                cmd.Parameters.Add("@in_RAMPAS", System.Data.SqlDbType.VarChar,50).Value = oTb_Lectura_Cartones.rampas;
                cmd.Parameters.Add("@in_ID_UBI", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.id_ubi;
                cmd.Parameters.Add("@in_ST_INTERFACE", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.st_interface;
                cmd.Parameters.Add("@in_CRITERIO", System.Data.SqlDbType.VarChar, 50).Value = oTb_Lectura_Cartones.criterio;
                cmd.Parameters.Add("@in_HOJAREP", System.Data.SqlDbType.Float).Value = oTb_Lectura_Cartones.hojarep;
                cmd.Parameters.Add("@in_NOM_DESTINO", System.Data.SqlDbType.VarChar, 100).Value = oTb_Lectura_Cartones.Localidad_Destino;
              
                //parametros de salida
                cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@MensajeError", SqlDbType.VarChar,200).Direction = ParameterDirection.Output;

                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int resul = Convert.ToInt32(cmd.Parameters["@Resultado"].Value.ToString());
                string MensajeError = cmd.Parameters["@MensajeError"].Value.ToString();

                cnx.Close();
                cnx.Dispose();

                if (resul == 1)
                {
                    return "1";
                }
                else
                {
                    //RegLogError.RegistroLog(MensajeError + " error insertar sql");
                    return "0";           
                }               
            }
            catch (Exception ex)
            {
                //RegLogError.RegistroLog(ex.Message.ToString() + " error insertar sql");
                return ex.Message.ToString();
            }

        }

        public string Modificar_Lectura_CartonFedex(int _Linea, Int32 _Lectura)
        {
            int res = 0;
            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnString"].ToString());
                SqlCommand cmd = new SqlCommand("sp_Modifica_CartonLPN", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pLectura", System.Data.SqlDbType.Float).Value = _Lectura;
                cmd.Parameters.Add("@pLinea", System.Data.SqlDbType.Int).Value = _Linea;

                //parametros de salida
                cmd.Parameters.Add("@pSalida", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@pMensaje_Error", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@pMensaje_Ok", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cnx.Open();
                res = cmd.ExecuteNonQuery();

                int Salida = Convert.ToInt32(cmd.Parameters["@pSalida"].Value);
                string MensajeOK = cmd.Parameters["@pMensaje_Error"].Value.ToString();
              
                cnx.Close();
                cnx.Dispose();

                if (Salida == 1)
                {
                    return "1";
                }
                else
                {
                    return MensajeOK;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
