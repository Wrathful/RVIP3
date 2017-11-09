using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using RVIP3Classes;

namespace RVIP3
{
    internal class Server
    {
        private static Delivery deliver= new Delivery();
        static void Main(string[] args)
        {
            Console.WriteLine("Введите максимальное кол-во обрабатываемых одновременно запросов");
            var threadsCount = int.Parse(Console.ReadLine());
            ThreadPool.SetMaxThreads(threadsCount, threadsCount);
            Start();
        }
        private static void Start()
        {
            var ipHost = Dns.GetHostEntry("localhost");
            var ipAddr = ipHost.AddressList[0];
            var ipEndPoint = new IPEndPoint(ipAddr, 23456);
            var listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipEndPoint);
            listener.Listen(1000);
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                var handler = listener.Accept();
                ThreadPool.QueueUserWorkItem((o) => {
                    deliver.AcceptRequest(handler);
                });
            }
        }
    }
    class Delivery
    {
        int delivery_time;
        public static int currentThreads = 0;
        private static int countThread = 1;
        Random rnd = new Random();
        public Delivery()
        {
            this.delivery_time = rnd.Next(500, 1000);
        }

        public void AcceptRequest(Socket socket) {
            try {
                var detsize= new byte[4];
                socket.Receive(detsize);
                var length = BitConverter.ToInt32(detsize, 0);
                var detail = new byte[length];
                socket.Receive(detail);
                Detail det = BinarySerializer<Detail>.Deserialize(detail);
                deliver(det);
                var outdet = BinarySerializer<Detail>.Serialize(det);
                socket.Send(outdet);
            }
            catch(Exception ex){
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        public void deliver(Detail detail)
        {
            int thisThreadnumber = countThread;
            countThread++;
            System.Console.WriteLine(System.DateTime.Now + "  Поток №" + thisThreadnumber + " начался" + "  " + "Запрошена доставка детали " + detail.name);
            System.Threading.Thread.Sleep(delivery_time);
            detail.have = true;
            System.Console.WriteLine(System.DateTime.Now + "  " + "Деталь " + detail.name + " доставили спустя  " + delivery_time + " миллисекунд" + "  Поток №" + thisThreadnumber + " окончился");
        }
    }
}
