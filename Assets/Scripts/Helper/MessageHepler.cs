using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;

public class MessageHepler : MonoBehaviour
{
    private static MessageHepler instance = new MessageHepler();
    public static MessageHepler Instance => instance;
    byte[] data = new byte[4096];
    int mesgLenth = 0;

    /// <summary>
    /// 从传进来的数据进行分包处理
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="length"></param>
    public void CopyToData(byte[] buffer, int length)
    {
        Array.Copy(buffer,0,data,mesgLenth,length);
        mesgLenth += length;
        Handel();
    }
    
    private void Handel()
    {
        //数据包传过来大小+消息ID+包体byte[]
            
        if (mesgLenth>=8)
        {
            byte[] _size = new byte[4];
            Array.Copy(data, 0, _size, 0, 4);
            //获取到包体大小
            int size=BitConverter.ToInt32(_size, 0);

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

                if (mesgLenth>_length) //是否超出本包长度
                {
                    for (int i = 0; i < mesgLenth-_length; i++)
                    {
                        data[i] = data[_length + i];
                    }
                }

                mesgLenth -= _length;
                Console.WriteLine($"收到客户端请求:{id}");
                switch (id)
                {
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


    public void SendToServer(int id, string str)
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
        Debug.Log("Josn byte长度："+send_buff.Length);
        Client.Instance.Send(send_buff);

    }
    
    public void SendToServer(int id, byte[] body)
    {
        //4个字节 发送内容大小
        
        //发送包体大小 长度+id+内容
        byte[] send_buff = new byte[body.Length + 8];
        int size = body.Length;
        var _size = BitConverter.GetBytes(size);
        var _id = BitConverter.GetBytes(id);

        Array.Copy(_size, 0, send_buff, 0, 4);
        Array.Copy(_id, 0, send_buff, 4, 4);
        Array.Copy(body, 0, send_buff, 8, body.Length);
        Debug.Log("Protobuf byte长度："+send_buff.Length);
        Client.Instance.Send(send_buff);

    }

    //发送注册请求 1001
    public void SendRegisterMsg(string account, string email, string pwd)
    {
        RegisterMsgC2S msg = new RegisterMsgC2S();
        msg.account = account;
        msg.email = email;
        msg.password = pwd;
        var str=JsonHelper.ToJson(msg);
        SendToServer(1001,str);
    }
    public Action<RegisterMsgS2C> rigisterMsgHandel;
    //处理注册请求
    private void RigisterMesHandel(byte[] body)
    {
        var str=Encoding.UTF8.GetString(body);
        RegisterMsgS2C msg = JsonHelper.ToObject<RegisterMsgS2C>(str);
        rigisterMsgHandel?.Invoke(msg);
        
    }

    //发送登录请求 1002
    public void SendLoginMsg(string account, string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();
        msg.account = account;
        msg.password = pwd;
        var str=JsonHelper.ToJson(msg);
        SendToServer(1002,str);        
    }

    public Action<LoginMsgS2C> loginHandel;
    //处理登录请求
    private void LoginMsgHandel(byte[] body)
    {
        var str=Encoding.UTF8.GetString(body);
        LoginMsgS2C msg = JsonHelper.ToObject<LoginMsgS2C>(str);
        loginHandel?.Invoke(msg);
    }

    public void SendChatMsg(string account, string chat)
    {
        ChatMsgC2S msg = new ChatMsgC2S();
        msg.player=account;
        msg.msg=chat;
        msg.type = 0;

        var str = JsonHelper.ToJson(msg);
        SendToServer(1003, str);
    }

    public Action<ChatMsgS2C> chatHandel;
    //聊天业务
    private void ChatMsgHandel(byte[] body)
    {
        var str=Encoding.UTF8.GetString(body);
        ChatMsgS2C msg = JsonHelper.ToObject<ChatMsgS2C>(str);
        chatHandel?.Invoke(msg);
    }
    
}


//1001
public class RegisterMsgC2S
{    public string account;
    public string password;
    public string email;
}

public class RegisterMsgS2C
{    public string account;
    public string password;
    public string email;
    public int result; //0成功 1已经被注册
}
//1002
public class LoginMsgC2S
{
    public string account;
    public string password;
}

public class LoginMsgS2C
{    public string account;
    public string password;
    public int result; //0成功1失败

}
//1003
//服务器转发给所有在线的客户端
public class ChatMsgC2S
{    public string player;
    public string msg;
    public int type;//0全部
}
public class ChatMsgS2C
{    public string player;
    public string msg;
    public int type;//0全部
}