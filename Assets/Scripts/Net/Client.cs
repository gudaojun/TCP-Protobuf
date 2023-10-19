using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Client
{
    private static Client instance = new Client();
    public static Client Instance => instance;
    private TcpClient _client;

    public void Start()
    {
        _client = new TcpClient();
        Connect();
    }

    public async void Connect()
    {
        try
        {
            await _client.ConnectAsync("127.0.0.1", 7788);
            Debug.Log("TCP链接成功");
            Receive();
            /*PlayerData playerData=new PlayerData();
            playerData.userName = "1";
            playerData.password = "2";
            playerData.email = "3";
            var json=JsonHelper.ToJson(playerData);*/
            //  MessageHepler.Instance.SendToServer(1,json);
            //相同的数据Json 字节长度50几，Protobuf 字节长度17 
            PlayerData playerData = new PlayerData();
            playerData.UserName = "1";
            playerData.Password = "2";
            playerData.Email = "3";
            var probuf=ProtobufHelper.ToBytes(playerData);
            MessageHepler.Instance.SendToServer(1,probuf);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _client.Close();
            throw;
        }
    }

    public async void Receive()
    {
        while (_client.Connected)
        {
            try
            {
                if (_client.GetStream().DataAvailable)
                {
                    byte[] buffer = new byte[4096];
                    int length = await _client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        var str = Encoding.UTF8.GetString(buffer, 0, length);
                        Debug.Log(str);

                        MessageHepler.Instance.CopyToData(buffer, length);
                    }
                    else
                    {
                        _client.Close();
                    }
                }
                else
                {
                    await Task.Delay(100); // 添加延迟以避免过度消耗CPU资源
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _client.Close();
                throw;
            }
        }
    }

    public async void Send(byte[] data)
    {
        try
        {
            await _client.GetStream().WriteAsync(data, 0, data.Length);
            Debug.Log("发送成功");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _client.Close();
            throw;
        }
    }
}