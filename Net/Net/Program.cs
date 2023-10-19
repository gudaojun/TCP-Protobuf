
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Net;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer tcpServer = new TCPServer();
            tcpServer.Start();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
