using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;

namespace MessengerServer
{
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        public static List<Socket> sockets = new List<Socket>();
        public static List<string> messages = new List<string> ();
        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            
            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                Thread Listen = new Thread(Listener);
                Listen.Start();
                while (true)
                {
                    Connect(listenSocket);
                    
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void Answer(string message)
        {


            for (int i = 0; i < sockets.Count; i++)
            {
                byte[] data = new byte[256];
                data = Encoding.Unicode.GetBytes(message);
                try
                {


                    sockets[i].Send(data);
                }
                catch
                {
                    sockets.Remove(sockets[i]);
                    continue;
                }
            }
                
            
            
        }
        public static void Listener()
        {

         
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < sockets.Count; i++)
                    {

                        int bytes = 0; // количество полученных байтов
                        byte[] data = new byte[256]; // буфер для получаемых данных
                    try
                    {
                        
                        bytes = sockets[i].Receive(data);
                    }
                    catch
                    {
                        sockets.Remove(sockets[i]);
                        continue;
                    }
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        Console.WriteLine(builder.ToString());

                }
                    
                    Answer(builder.ToString());
                }
            
            
        }
        public static void Connect(Socket listenSocket)
        {
            Socket handler = listenSocket.Accept();
            sockets.Add(handler);
        }
    }
}

