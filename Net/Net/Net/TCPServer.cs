using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    class TCPServer
    {
        //创建一个tpc服务端
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private NetworkStream stream;

        public void Start()
        {
            try
            {
                //创建一个监听器 
                tcpListener = TcpListener.Create(7788);
                tcpListener.Start(500);

               Console.WriteLine("服务端启动成功");
                Accpet();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Client tempClient;
        //监听客户端链接
        public async void Accpet()
        {
            try
            {
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("客户端已链接" + tcpClient.Client.RemoteEndPoint);
                Client client = new Client(tcpClient);
                tempClient = client;
                tempClient.SendToClient(1, "Login");
                Accpet();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tcpListener.Stop();
                throw;
            }
        }
    }
}
