using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RVIP3Classes;

namespace RVIP3Client
{
    class Client
    {
        private static readonly int Port;
        private static readonly Random Rnd;
        static Client()
        {
            Port = 23456;
            Rnd = new Random(Guid.NewGuid().GetHashCode());
        }
        private static void Main(string[] args)
        {
            while (true)
            {
                SendRequest();
                Thread.Sleep(100);
            }
        }
        private static void SendRequest()
        {            
            List<Detail> lst = new List<Detail>{
                new Detail("motor", 8,true),
                new Detail("transmission", 5,false),
                new Detail("brake", 4,false),
                new Detail("detail№4", 5,false),
                new Detail("detail№5", 7,false),
            };
            for (int i = 0; i < lst.Count; i++)
            {
                if (!lst[i].have) {
                    try
                    {
                        var ipHost = Dns.GetHostEntry("localhost");
                        var ipAddr = ipHost.AddressList[0];
                        var ipEndPoint = new IPEndPoint(ipAddr, Port);
                        var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        sender.Connect(ipEndPoint);
                        var detail = BinarySerializer<Detail>.Serialize(lst[i]);
                        var lenDetail = BitConverter.GetBytes(detail.Length);
                        sender.Send(lenDetail);
                        sender.Send(detail);
                        var inDetail = new byte[BitConverter.ToInt32(lenDetail, 0)];
                        sender.Receive(inDetail);
                        Detail currentDetail= BinarySerializer<Detail>.Deserialize(inDetail);
                        if (currentDetail.have)
                        {
                            lst[i].have = true;
                            Console.WriteLine("Деталь " + lst[i].name + " Успешно доставлена");
                            installation(lst[i]);
                            sender.Close();
                        }
                        else {
                            Console.WriteLine("Что-то пошло не так");
                        }

                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex);
                        Console.ReadKey();
                    }
                }
                else
                {
                    installation(lst[i]);
                }
            }
        }
        public static void installation(Detail detail)
        {
            System.Console.WriteLine(System.DateTime.Now + "  " + "  Начинаем Монтаж детали " + detail.name);
            System.Threading.Thread.Sleep(detail.installation_time);
            System.Console.WriteLine(System.DateTime.Now + "  " + "  Выполнен монтаж детали " + detail.name + " За " + detail.installation_time + " миллисекунд");
        }
    }
}
