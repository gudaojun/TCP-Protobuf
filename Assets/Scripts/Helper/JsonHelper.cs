using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

public class JsonHelper 
{
    //LitJson
    /*public static string ToJson(object x)
    {
        string str = JsonMapper.ToJson(x);
        return str;
    }

    public static T ToObject<T>(string x)
    {
        return JsonMapper.ToObject<T>(x);
    }

    public static T ToObject<T>(byte[] b)
    {
        string x = Encoding.UTF8.GetString(b, 0, b.Length);
        return ToObject<T>(x);
    }*/
    
    public static string ToJson(object obj)
    {
        string json=JsonConvert.SerializeObject(obj);
        return json;
    }

    public static T ToObject<T>(string str)
    { 
        T obj=JsonConvert.DeserializeObject<T>(str);
        return obj;
    }

    public static T ToObject<T>(byte[] b)
    {
        string str=Encoding.UTF8.GetString(b, 0, b.Length);
        return ToObject<T>(str);
    }
}
