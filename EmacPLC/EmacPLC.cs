using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using S7.Net;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Reflection;


namespace EmacPLC
{
    public partial class EmacPLC : ServiceBase
    {
        Plc oPLC = null;
        cPLC sPLC = new cPLC();
        cConexionIP sScanner = new cConexionIP();
        cDispositivos oDirecciones = new cDispositivos();
        cVarGlobales oVarGlobales = new cVarGlobales();
        cRegistroErr oError = new cRegistroErr();
        Timer Tempo = null;

        cEnt_Meson oMeson = new cEnt_Meson();
        cENT_Vl_Cbc_Fdx ocENT_Vl_Cbc_Fdx = new cENT_Vl_Cbc_Fdx();
        cSalidasTroll oSalidas = new cSalidasTroll();
        cRecirculado oRecirc = new cRecirculado();
        LGN_Oracle lgn_oracle = new LGN_Oracle();
        LGN_TipoDespacho _lgn_tipodespacho = new LGN_TipoDespacho();
        LGN_Insertar_Lecturas _LGN_Insertar_Lecturas = new LGN_Insertar_Lecturas();
        LGN_TB_Distribucion _lgn_Tb_Distribucion = new LGN_TB_Distribucion();

        public EmacPLC()
        {
            InitializeComponent();

            Tempo = new Timer();

            //Eventos = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("EmacPLC"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "EmacPLC", "Application");
            }
            Eventos.Source = "EmacPLC";
            Eventos.Log = "Application";
        }

        //public void OnDebug()
        //{
        //    OnStart(null);
        //}

        protected override void OnStart(string[] args)
        {

            Direcciones();
            ConectarPLC();
            ConectarSCAN();
            TipoDespacho();

            //asigno esas variables
            sPLC.IP = oDirecciones.IpPLC;
            sScanner.IP = oDirecciones.IpScanner;
            sScanner.PUERTO = oDirecciones.PortScanner;

            //oError.RegistroLog("Paso por funcion escuchar al inicio");

            //invoco al evento escuchar TCP IP
            sScanner.Escuchar += oConexionIP_Escuchar;

            //tipos de despacho 
            TipoDespacho();

            //cargar jornada por defecto
            ListadoJornadaEnProduccion();

            this.Tempo.Enabled = true;
            this.Tempo.Interval = 3000;
            this.Tempo.Elapsed += new ElapsedEventHandler(Temporizador_Elapsed);
            this.Tempo.Start();

            Eventos.WriteEntry("Servicio Iniciado en forma correcta" + DateTime.Now.ToString());

            //oError.RegistroLog("Servicio Iniciado en forma correcta");

        }

        protected override void OnStop()
        {
            if (cVarGlobales.EstConePLC)
            {
                oPLC.Close();
            }

            if (cVarGlobales.EstConeSCAN)
            {
                sScanner.Desconectar_TCPClient();
            }

            //detengo y elimino el timer
            Tempo.Stop();
            Tempo.Dispose();

            Eventos.WriteEntry("Servicio Detenido en forma correcta" + DateTime.Now.ToString());

        }

        void Temporizador_Elapsed(object sender, ElapsedEventArgs e)
        {
            oError.RegistroLog("inicio temporizador");

            Tempo.Stop();

            try
            {                
                if ((sPLC.IsAvailable) != true)
                {
                    cVarGlobales.EstConePLC = false;
                    ConectarPLC();
                    LeerEtiquetasDerivadas();
                    LeerEtiquetasRecirculadas(); 
                }
                else 
                {
                    if (cVarGlobales.EstConePLC) 
                    { 
                        LeerEtiquetasDerivadas(); 
                        LeerEtiquetasRecirculadas(); 
                    }                
                }

                if ((sScanner.IsAvailable) != true)
                {
                    cVarGlobales.EstConeSCAN = false;
                    ConectarSCAN();
                }

            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion Ciclica");
            }
           
            Tempo.Start();

            //oError.RegistroLog("Fin temporizador");
        }

        void Direcciones()
        {
            try
            {
                oDirecciones.IpPLC = ConfigurationManager.AppSettings["IpPCL"].ToString();
                oDirecciones.PortPLC = Convert.ToInt16(ConfigurationManager.AppSettings["PortPLC"].ToString());
                oDirecciones.RackPLC = Convert.ToInt16(ConfigurationManager.AppSettings["RackPLC"].ToString());
                oDirecciones.SlotPLC = Convert.ToInt16(ConfigurationManager.AppSettings["SlotPLC"].ToString());
                oDirecciones.NumDb = Convert.ToInt16(ConfigurationManager.AppSettings["NumDb"].ToString());
                oDirecciones.IpScanner = ConfigurationManager.AppSettings["IpScann"].ToString();
                oDirecciones.PortScanner = Convert.ToInt16(ConfigurationManager.AppSettings["PortScann"].ToString());
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString());
            }
        }

        void ConectarPLC()
        {
            try
            {
                sPLC.IP = oDirecciones.IpPLC;

                if (sPLC.IsAvailable)
                {
                    oPLC = new Plc(CpuType.S71200, oDirecciones.IpPLC, oDirecciones.RackPLC, oDirecciones.SlotPLC);
                    oPLC.Open();

                    cVarGlobales.EstConePLC = oPLC.IsConnected;

                    if (cVarGlobales.EstConePLC)
                    {
                        oVarGlobales.MensajePLC = "PLC Conectado " + oDirecciones.IpPLC;
                        Eventos.WriteEntry(oVarGlobales.MensajePLC);
                    }
                    else
                    {
                        oVarGlobales.MensajePLC = "PLC Desconectado" + oDirecciones.IpPLC;
                        Eventos.WriteEntry(oVarGlobales.MensajePLC);
                    }
                }
                else
                {
                    cVarGlobales.EstConePLC = false;
                    oVarGlobales.MensajePLC = "PLC Desconectado" + oDirecciones.IpPLC;
                    Eventos.WriteEntry(oVarGlobales.MensajePLC);
                }
            }
            catch (Exception ex)
            {
                cVarGlobales.EstConePLC = false;
                oVarGlobales.MensajePLC = ex.Message.ToString();
                Eventos.WriteEntry(oVarGlobales.MensajePLC);
            }
        }

        void ConectarSCAN()
        {
            try
            {
                sScanner.IP = oDirecciones.IpScanner;
                sScanner.PUERTO = oDirecciones.PortScanner;

                if (sScanner.IsAvailable)
                {
                    if (sScanner.Conectar_TCPClient(oDirecciones.IpScanner, oDirecciones.PortScanner))
                    {
                        cVarGlobales.EstConeSCAN = true;
                        oVarGlobales.MensajeSCAN = "Scanner Conectado " + oDirecciones.IpScanner;
                        Eventos.WriteEntry(oVarGlobales.MensajeSCAN);
                    }
                    else
                    {
                        cVarGlobales.EstConeSCAN = false;
                        oVarGlobales.MensajeSCAN = "Scanner Desconectado " + oDirecciones.IpScanner;
                        Eventos.WriteEntry(oVarGlobales.MensajeSCAN);
                    }
                }
                else
                {
                    cVarGlobales.EstConeSCAN = false;
                    oVarGlobales.MensajeSCAN = "Scanner Desconectado " + oDirecciones.IpScanner;
                    Eventos.WriteEntry(oVarGlobales.MensajeSCAN);
                }

            }
            catch (Exception ex)
            {
                oVarGlobales.MensajeSCAN = ex.Message.ToString();
                Eventos.WriteEntry(oVarGlobales.MensajeSCAN);
            }

        }

        void oConexionIP_Escuchar(string datos)
        {
            int in_Variable = 0;
            string Lectura = datos.Trim().TrimStart();

            char[] MyChar = { '\u0002', '\u0003', '\u0010', '\u0013', '\r', '\n' };

            string str = Lectura.TrimStart(MyChar).TrimEnd(MyChar);

            //VARIABLE TIPO DESPACHO
            in_Variable = cVarGlobales.CodTipoDespacho;

            if (Clasifica_Salida_Fedex(str.TrimEnd(MyChar), in_Variable) == "1")
            {
                Lectura = string.Empty;
            }
            else
            {
                //Eventos.WriteEntry(ex.Message.ToString());                
            }
        }

        public string Clasifica_Salida_Fedex(string in_CartonLPN, int Tipo_Lectura)
        {
            char[] MyChar = { '\u0002', '\u0003', '\u0010', '\u0013', '\r', '\n', ';' };

            cTb_Lectura_Cartones oCartones = new cTb_Lectura_Cartones();
            cTb_Lectura_Cartones oSalifdaCartonesFedex = new cTb_Lectura_Cartones();
            LGN_Insertar_Lecturas negCartones = new LGN_Insertar_Lecturas();
            LGN_Oracle _LGN_Oracle = new LGN_Oracle();
            cCorrelativoDB oCorrelativo = new cCorrelativoDB();
            LGN_Insertar_Lecturas _LGN_Insertar_Lecturas = new LGN_Insertar_Lecturas();

            long _corr = 0;
            string resSql;
            string[] resultado;
            string anio, mes, dia, hora, min, seg;

            //******************************************************************
            //matriz  divido la matriz
            //******************************************************************
            resultado = in_CartonLPN.Split(MyChar);

            //******************************************************************
            //Descomposicion codigo barras
            //******************************************************************                            
            if (resultado[0].ToString().Length == 27)
            {
                if (resultado[0].ToString() != "NoRead")
                {
                    oMeson.Depot_From = resultado[0].ToString().Substring(0, 3).Trim();
                    oMeson.Depot_To = resultado[0].ToString().Substring(3, 3).Trim();
                    oMeson.Zona = resultado[0].ToString().Substring(6, 4).Trim();
                    oMeson.Producto = resultado[0].ToString().Substring(10, 2).Trim();
                }
            }

            //******************************************************************
            //distribucion de la matriz 
            //******************************************************************
            oMeson.CartonLPN = resultado[0].ToString().Trim();
            oCartones.CartonLPN = oMeson.CartonLPN;

            anio = resultado[1].ToString();
            mes = resultado[2].ToString();
            dia = resultado[3].ToString();
            hora = resultado[4].ToString();
            min = resultado[5].ToString();
            seg = resultado[6].ToString();

            oMeson.Kilos = Convert.ToDecimal(resultado[7].ToString());//gramos (k*1)/1000
            oMeson.Largo = Convert.ToDecimal(resultado[8].ToString());//mm (mm*1)/1000
            oMeson.Ancho = Convert.ToDecimal(resultado[9].ToString());//mm (mm*1)/1000
            oMeson.Alto = Convert.ToDecimal(resultado[10].ToString());//mm (mm*1)/1000
            oMeson.Volumen = oMeson.Largo * oMeson.Ancho * oMeson.Alto;

            //******************************************************************
            //Salida cajas FEDEX -- FASA
            //******************************************************************
            if (resultado[0].ToString().Length == 27)
            {
                oSalifdaCartonesFedex = negCartones.Listado_SP_SELECCIONA_SALIDA_FEDEX(oCartones);
            }
            else
            {
                oSalifdaCartonesFedex = null;
                //oSalifdaCartonesFedex = negCartones.Listado_SP_SELECCIONA_SALIDA_FASA(oCartones);
            }

            //******************************************************************
            //valido salidas 
            //******************************************************************
            if (oSalifdaCartonesFedex != null)
            {
                oCartones.Lane = oSalifdaCartonesFedex.Lane;
                oCartones.Store = oSalifdaCartonesFedex.DtdSalida;
            }
            else
            {
                oCartones.Lane = 99;
                oCartones.Store = "No tiene destino asignado";
                //Eventos.WriteEntry(oMeson.CartonLPN + " sin destino asignado");                                
            }

            //******************************************************************
            //CALCULAR CORRELATIVO
            //******************************************************************
            oCorrelativo = lgn_oracle.Listado_Correlativo_VL_CBC_SQL();

            if (oCorrelativo == null)
            {
                //RegErrores.RegistroLog(" Error Leer correlativo");
                _corr = 0;
            }
            else
            {
                _corr = Convert.ToInt64(oCorrelativo._Corr_Base_Datos);
            }

            //LblLectura.Text = _corr.ToString();

            //******************************************************************
            //valores en pantalla                      
            //******************************************************************
            if (oCartones.CartonLPN.Length == 27)
            {
            }
            else
            {
                int a = 27;
                int res = 0;
                int var = 0;
                var sb = new System.Text.StringBuilder();

                //tamaño del string
                var = oMeson.CartonLPN.Length;

                //cantidad de ceros para agregar
                res = a - var;

                for (int i = 0; i < res; i++)
                {
                    sb.AppendLine("0");
                }

                string unir = sb.ToString().Replace("\r\n", string.Empty);

                oMeson.CartonLPN = unir + oMeson.CartonLPN;

                oCartones.CartonLPN = oMeson.CartonLPN;
            }

            //*******************************************************************
            //estructura de insercion
            //*******************************************************************
            ocENT_Vl_Cbc_Fdx.lectura = _corr;
            ocENT_Vl_Cbc_Fdx.tiplec = Tipo_Lectura;
            ocENT_Vl_Cbc_Fdx.fecha = DateTime.Now;
            ocENT_Vl_Cbc_Fdx.expedicion = 0;
            ocENT_Vl_Cbc_Fdx.keyuser = ConfigurationManager.AppSettings["KEYUSER"].ToString();
            ocENT_Vl_Cbc_Fdx.keydel = "001";
            ocENT_Vl_Cbc_Fdx.kilos = (Convert.ToDouble(resultado[7])) / 1000;
            ocENT_Vl_Cbc_Fdx.largo = (Convert.ToDouble(resultado[8]) * 1) / 1000;
            ocENT_Vl_Cbc_Fdx.ancho = (Convert.ToDouble(resultado[9]) * 1) / 1000;
            ocENT_Vl_Cbc_Fdx.alto = (Convert.ToDouble(resultado[10]) * 1) / 1000;
            ocENT_Vl_Cbc_Fdx.volumen = ocENT_Vl_Cbc_Fdx.largo * ocENT_Vl_Cbc_Fdx.ancho * ocENT_Vl_Cbc_Fdx.alto;
            ocENT_Vl_Cbc_Fdx.keybul = oCartones.CartonLPN;
            ocENT_Vl_Cbc_Fdx.rampae = ConfigurationManager.AppSettings["KEYUSER"].ToString();
            ocENT_Vl_Cbc_Fdx.rampas = oCartones.Lane.ToString();
            ocENT_Vl_Cbc_Fdx.fechae = DateTime.Now;
            ocENT_Vl_Cbc_Fdx.fechas = DateTime.Now;//CUANDO FUE DERIVADA EN LA SALIDA
            ocENT_Vl_Cbc_Fdx.f_sistema = DateTime.Now;
            ocENT_Vl_Cbc_Fdx.id_ubi = 0;
            ocENT_Vl_Cbc_Fdx.fecha_e = DateTime.Now;
            ocENT_Vl_Cbc_Fdx.st_interface = 1;
            ocENT_Vl_Cbc_Fdx.criterio = "1";
            ocENT_Vl_Cbc_Fdx.hojarep = 0;
            ocENT_Vl_Cbc_Fdx.Localidad_Destino = oCartones.Store;
            //*******************************************************************

            try
            {
                if (oMeson.CartonLPN.Length == 27)
                {
                    if (EscribirEntero(Convert.ToInt16(ocENT_Vl_Cbc_Fdx.rampas), Convert.ToInt16(resultado[8])))
                    {
                        if (EscribirTracking(Convert.ToInt32(ocENT_Vl_Cbc_Fdx.lectura)))
                        {
                        }
                        else
                        {
                        }
                    }


                    //*******************************************************************
                    //inserto registros en tabla SQL
                    resSql = _LGN_Insertar_Lecturas.Inserta_Lecturas_CartonesFedex(ocENT_Vl_Cbc_Fdx);
                    //*******************************************************************

                    if (resSql == "1")
                    {

                        Eventos.WriteEntry("Carton insertado en SQL " + oCartones.CartonLPN + " en la salida " + ocENT_Vl_Cbc_Fdx.rampas);
                    }
                    else
                    {
                        Eventos.WriteEntry("No inserto en SQL " + oCartones.CartonLPN);
                    }

                    //limpio matriz codigo barras
                    Array.Clear(resultado, 0, resultado.Length);

                    return "1";
                }

                if (resultado[0].ToString() == "NoRead")
                {
                    //RegErrores.RegistroLog("Etiqueta no fue leida");
                    oCartones.Lane = 99;

                    //escribo destino plc
                    if (EscribirEntero(Convert.ToInt16(oCartones.Lane), Convert.ToInt16(resultado[8])))
                    {
                        //RegErrores.RegistroLog("Error Lectura " + oMeson.CartonLPN + " derivada " + oCartones.Lane.ToString());
                    }
                    else
                    {
                        //RegErrores.RegistroLog("Error Funcion EscribirEntero caja " + oMeson.CartonLPN + " derivada " + oCartones.Lane.ToString());
                    }

                    //inserto registros en tabla SQL
                    resSql = _LGN_Insertar_Lecturas.Inserta_Lecturas_CartonesFedex(ocENT_Vl_Cbc_Fdx);

                    //limpio matriz codigo barras
                    Array.Clear(resultado, 0, resultado.Length);

                    return "0";
                }

                //limpio matriz codigo barras
                Array.Clear(resultado, 0, resultado.Length);

                return "1";
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " Error Lectura Scanner");
                return "0";
            }
        }

        public bool EscribirString(string texto)
        {
            try
            {
                int dbnumber = 54;
                int startadrr = 0;
                string input = texto;
                byte[] databytes = Types.String.ToByteArray(input);
                List<byte> values = new List<byte>();
                byte medida = (byte)input.Length;
                byte medidaactual = (byte)input.Length;

                values.Add(medida);
                values.Add(medidaactual);
                values.AddRange(databytes);

                oPLC.WriteBytes(DataType.DataBlock, dbnumber, startadrr, values.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion EscribirString");
                return false;
            }
        }

        public bool EscribirEntero(short valor, short _in_Largo_Caja)
        {
            if (_in_Largo_Caja > 650)
            {
                valor = 10;
            }

            if ((valor == 0) && (_in_Largo_Caja < 650))
            {
                valor = 8;
            }


            try
            {
                short db1IntVariable = valor;
                short db1InVariable2 = _in_Largo_Caja;

                //ESCRIBE SALIDA CAJA
                oPLC.Write("DB5.DBW0", db1IntVariable.ConvertToUshort());

                oPLC.Write("DB5.DBW2", db1IntVariable.ConvertToUshort());

                //ESCRIBE LARGO CAJa
                oPLC.Write("DB2.DBW316", db1InVariable2.ConvertToUshort());
                return true;
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion EscribirEntero");
                return false;
            }
        }

        public bool EscribirTracking(Int32 valor)
        {
            try
            {
                Int32 db1IntVariable = valor;
                oPLC.Write("DB54.DBD0", db1IntVariable.ConvertToUInt());

                oPLC.Write("DB54.DBD4", db1IntVariable.ConvertToUInt());
                return true;
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion EscribirEntero Tracking");
                return false;
            }

        }

        private void LeerEtiquetasDerivadas()
        {
            cSalidasTroll oSalidas = new cSalidasTroll();
            try
            {
                int NumberOfDB = 51;

                char[] MyChar = { '\u0002', '\u0003', '\u0010', '\u0013', '\n', '\r', '\0', '\v', '[', ']', '\b', '\t', '\x020' };

                byte[] Derivadasresult1 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4010, 4);
                byte[] Derivadasresult2 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4014, 4);
                byte[] Derivadasresult3 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4018, 4);
                byte[] Derivadasresult4 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4022, 4);
                byte[] Derivadasresult5 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4026, 4);
                byte[] Derivadasresult6 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4030, 4);
                byte[] Derivadasresult7 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4034, 4);
                byte[] Derivadasresult8 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4038, 4);
                byte[] Derivadasresult9 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4042, 4);
                byte[] Derivadasresult10 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4046, 4);

                oSalidas.CartonSalida01 = Types.DInt.FromByteArray(Derivadasresult1);
                oSalidas.CartonSalida02 = Types.DInt.FromByteArray(Derivadasresult2);
                oSalidas.CartonSalida03 = Types.DInt.FromByteArray(Derivadasresult3);
                oSalidas.CartonSalida04 = Types.DInt.FromByteArray(Derivadasresult4);
                oSalidas.CartonSalida05 = Types.DInt.FromByteArray(Derivadasresult5);
                oSalidas.CartonSalida06 = Types.DInt.FromByteArray(Derivadasresult6);
                oSalidas.CartonSalida07 = Types.DInt.FromByteArray(Derivadasresult7);
                oSalidas.CartonSalida08 = Types.DInt.FromByteArray(Derivadasresult8);
                oSalidas.CartonSalida09 = Types.DInt.FromByteArray(Derivadasresult9);
                oSalidas.CartonSalida10 = Types.DInt.FromByteArray(Derivadasresult10);

                string res = string.Empty;
                PropertyInfo[] properties = typeof(cSalidasTroll).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //así obtenemos el nombre del atributo
                    string NombreAtributo = property.Name;

                    //así obtenemos el valor del atributo
                    string Valor = property.GetValue(oSalidas).ToString();

                    string conver = string.Empty;

                    conver = Regex.Replace(Valor, @"[^\w\s.!@$%^&*()\-\/]+", "");

                    int vLinea = Convert.ToInt32(NombreAtributo.Substring(12, 2));

                    //if (conver != string.Empty)
                    if (conver != "0")
                    {
                        res = _LGN_Insertar_Lecturas.Modificar_CartonLPN(vLinea, Convert.ToInt32(conver));
                    }
                }

                //limpio las variables de recepcion de datos
                oSalidas = new cSalidasTroll();

                Array.Clear(Derivadasresult1, 0, Derivadasresult1.Length);
                Array.Clear(Derivadasresult2, 0, Derivadasresult2.Length);
                Array.Clear(Derivadasresult3, 0, Derivadasresult3.Length);
                Array.Clear(Derivadasresult4, 0, Derivadasresult4.Length);
                Array.Clear(Derivadasresult5, 0, Derivadasresult5.Length);
                Array.Clear(Derivadasresult6, 0, Derivadasresult6.Length);
                Array.Clear(Derivadasresult7, 0, Derivadasresult7.Length);
                Array.Clear(Derivadasresult8, 0, Derivadasresult8.Length);
                Array.Clear(Derivadasresult9, 0, Derivadasresult9.Length);
                Array.Clear(Derivadasresult10, 0, Derivadasresult10.Length);
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion LeerEtiquetasDerivadas");
            }

        }

        private void LeerEtiquetasRecirculadas()
        {
            cRecirculado oRecirc = new cRecirculado();
            DataSet ds = new DataSet();
            try
            {
                int NumberOfDB = 51;
                char[] MyChar = { '\u0002', '\u0003', '\u0010', '\u0013', '\n', '\r', '\0', '\v' };

                byte[] recirc1 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4058, 4);
                byte[] recirc2 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4062, 4);
                byte[] recirc3 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4066, 4);
                byte[] recirc4 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4070, 4);
                byte[] recirc5 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4074, 4);
                byte[] recirc6 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4078, 4);
                byte[] recirc7 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4082, 4);
                byte[] recirc8 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4086, 4);
                byte[] recirc9 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4090, 4);
                byte[] recirc10 = oPLC.ReadBytes(DataType.DataBlock, NumberOfDB, 4094, 4);

                oRecirc.CartonRecirc01 = Types.DInt.FromByteArray(recirc1);
                oRecirc.CartonRecirc02 = Types.DInt.FromByteArray(recirc2);
                oRecirc.CartonRecirc03 = Types.DInt.FromByteArray(recirc3);
                oRecirc.CartonRecirc04 = Types.DInt.FromByteArray(recirc4);
                oRecirc.CartonRecirc05 = Types.DInt.FromByteArray(recirc5);
                oRecirc.CartonRecirc06 = Types.DInt.FromByteArray(recirc6);
                oRecirc.CartonRecirc07 = Types.DInt.FromByteArray(recirc7);
                oRecirc.CartonRecirc08 = Types.DInt.FromByteArray(recirc8);
                oRecirc.CartonRecirc09 = Types.DInt.FromByteArray(recirc9);
                oRecirc.CartonRecirc10 = Types.DInt.FromByteArray(recirc10);

                string res = string.Empty;

                PropertyInfo[] properties = typeof(cRecirculado).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //así obtenemos el nombre del atributo
                    string NombreAtributo = property.Name;

                    //así obtenemos el valor del atributo
                    string Valor = property.GetValue(oRecirc).ToString();

                    string conver = string.Empty;

                    conver = Regex.Replace(Valor, @"[^\w\s.!@$%^&*()\-\/]+", "");

                    int vLinea = Convert.ToInt32(NombreAtributo.Substring(12, 2));

                    if (conver != "0")
                    {
                        res = _LGN_Insertar_Lecturas.Modificar_CartonLPN_Recirc(Convert.ToInt32(conver), vLinea);
                    }
                }

                oRecirc = new cRecirculado();
            }
            catch (Exception ex)
            {
                Eventos.WriteEntry(ex.Message.ToString() + " funcion LeerEtiquetasRecirculadas");
            }

        }

        void TipoDespacho()
        {
            DataSet dsTipo = new DataSet();           
            try
            {
                dsTipo = _lgn_tipodespacho.Listado_TipoDespacho();

                if (dsTipo != null)
                {
                    foreach (DataRow fila in dsTipo.Tables[0].Rows)
                    {
                        cEnt_TB_Tipo_Despacho.glo_CodTipoDespacho = Convert.ToInt32(fila[0]);
                    }
                }
                else
                {
                    Eventos.WriteEntry("Se debe agregar tipo despacho a la tabla");
                }
            }
            catch (Exception ex) 
            {
                Eventos.WriteEntry(ex.Message.ToString() + " Funcion Valor Tipo Despacho");            
            }
        }

        void ListadoJornadaEnProduccion()
        {
            DataSet dsdatos = new DataSet();
            string res = string.Empty;

            //lleno dataset
            dsdatos = _lgn_Tb_Distribucion.Listado_Jornadas_EnProduccion();

            //VALIDO QUE NO EXISTAN REGISTROS DE TABLA JORNADA PRODUCCION
            if (dsdatos.Tables[0].Rows.Count == 0)
            {
                res = _lgn_Tb_Distribucion.Crear_Jornada_Automaticas();

                if (res == "1")
                {
                    //RegErrores.RegistroLog(cTB_Distribucion.oMensajes);
                }
                else
                {
                    Eventos.WriteEntry(cTB_Distribucion.oMensajes + " Funcion Listado Jornada En Produccion");                       
                }
            }
        }
    }
}
