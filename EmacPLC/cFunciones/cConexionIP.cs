using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace EmacPLC
{    

    public class cConexionIP
    {
        cRegistroErr oError = new cRegistroErr();

        TcpClient tcpclnt = new TcpClient();
        Thread tcpThd;
        public delegate void DatosRecibidos(string datos);
        public event DatosRecibidos Escuchar;
        Stream stm;

        public string IP
        { get; set; }

        public Int16 PUERTO
        { get; set; }

        public bool IsAvailable
        {
            get
            {
                Ping ping = new Ping();
                PingReply result = ping.Send(IP,300);
                if (result.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
        }

        public void TCPServer()
        {
            try
            {
                TcpListener myList = new TcpListener(IPAddress.Any, 8010);
                myList.Start();

                Console.WriteLine("Server running at port 8001...");
                Console.WriteLine("Waiting for a connection...");

                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                byte[] b = new byte[100];
                int k = s.Receive(b);
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(b[i]));

                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));
                Console.WriteLine("\nSent Acknowledgement");
                /* clean up */
                s.Close();
                Console.Read();
                myList.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
                Console.Read();
            }
        }

        public bool Conectar_TCPClient(string in_IP, int in_Puerto)
        {
            try
            {
                ThreadStart Delegado = new ThreadStart(LeerSocket);

                tcpclnt = new TcpClient();
                tcpThd = new Thread(Delegado);

                tcpclnt.Connect(in_IP, in_Puerto);
                stm = tcpclnt.GetStream();
                tcpThd.Start();

                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Desconectar_TCPClient()
        {
            try
            {
                tcpclnt.Close();                
                tcpThd.Abort();
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        private void LeerSocket()
        {
            oError.RegistroLog("ingreso escuchar");

            var evento = Escuchar;
            //if (evento != null)
            //{
                byte[] BufferDeLectura = new byte[100];
                while (true)
                {
                    try
                    {                       
                        stm.Read(BufferDeLectura, 0, BufferDeLectura.GetLength(0));                                              
                        Escuchar(Encoding.ASCII.GetString(BufferDeLectura));                        
                    }
                    catch (Exception ex)
                    {
                        oError.RegistroLog(ex.Message.ToString());                                 
                        return;
                    }
                }
            //}            
        }
              
    }
}
