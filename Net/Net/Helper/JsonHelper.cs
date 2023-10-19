using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Helper
{
  public static  class JsonHelper
    {
        public static string ToJson(object x)
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
        }
    }
}
