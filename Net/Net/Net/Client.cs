using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Net.Helper;
namespace Server.Net
{
    class Client
    {
        private TcpClient _client;

        public Client(TcpClient tcpClient)
        {
            _client = tcpClient;
            Receive();
        }

        byte[] data = new byte[4096];
        int mesgLenth = 0;
        public async void Receive()
        {
            while (_client.Connected)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int length = await _client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        //  Console.WriteLine("接收到的内容" + Encoding.UTF8.GetString(buffer));                               
                        Array.Copy(buffer, 0, data, mesgLenth, length);
                        mesgLenth += length;
                        Handel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

        }

        private void Handel()
        {
            //数据包传过来大小+消息ID+包体byte[]

            if (mesgLenth >= 8)
            {
                byte[] _size = new byte[4];
                Array.Copy(data, 0, _size, 0, 4);
                //获取到包体大小
                int size = BitConverter.ToInt32(_size, 0);

                var _length = 8 + size;
                if (mesgLenth >= _length)
                {
                    //获取ID
                    byte[] _id = new byte[4];
                    Array.Copy(data, 4, _id, 0, 4);
                    int id = BitConverter.ToInt32(_id, 0);

                    //获取包体
                    byte[] body = new byte[size];
                    Array.Copy(data, 8, body, 0, size);
                    Console.WriteLine(Encoding.UTF8.GetString(body));
                    if (mesgLenth > _length) //是否超出本包长度
                    {
                        for (int i = 0; i < mesgLenth - _length; i++)
                        {
                            data[i] = data[_length + i];
                        }
                    }

                    mesgLenth -= _length;
                    Console.WriteLine($"收到客户端请求:{id}");
                    switch (id)
                    {
                        case 1:
                            ProtobufHandel(body);
                            break;
                        case 1001://注册
                            RigisterMesHandel(body);
                            break;

                        case 1002://登录
                            LoginMsgHandel(body);
                            break;
                        case 1003://聊天
                            ChatMsgHandel(body);
                            break;
                    }
                }
            }
        }

        private void ProtobufHandel(byte[] body)
        {
            PlayerData data=ProtobufHelper.ToObject<PlayerData>(body);
            Console.WriteLine("从客户端发来的Protobuf数据" + data.UserName + data.Password + data.Email);
        }

        //聊天业务
        private void ChatMsgHandel(byte[] body)
        {

        }

        //处理登录请求
        private void LoginMsgHandel(byte[] body)
        {

        }

        //处理注册请求
        private void RigisterMesHandel(byte[] body)
        {

        }

        public async void Send(byte[] data)
        {
            try
            {
                await _client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _client.Close();
                throw;
            }
        }

        //发送到客户端进行粘包处理
        public void SendToClient(int id, string str)
        {
            //4个字节 发送内容大小
            var body = Encoding.UTF8.GetBytes(str);
            //发送包体大小 长度+id+内容
            byte[] send_buff = new byte[body.Length + 8];
            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id = BitConverter.GetBytes(id);

            Array.Copy(_size, 0, send_buff, 0, 4);
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body, 0, send_buff, 8, body.Length);
            Send(send_buff);

        }
    }
}
