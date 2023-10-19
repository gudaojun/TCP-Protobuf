using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Helper
{
   public  class ProtobufHelper
    {
        public static byte[] ToBytes(object message)
        {
            return ((Google.Protobuf.IMessage)message).ToByteArray();
        }

        /*public static void ToStream(object message, MemoryStream stream)
        {
            ((Google.Protobuf.IMessage) message).WriteTo(stream);
        }*/
        public static T ToObject<T>(byte[] bytes) where T : Google.Protobuf.IMessage
        {
            var message = Activator.CreateInstance<T>();
            message.MergeFrom(bytes);
            return message;
        }
    }
}
