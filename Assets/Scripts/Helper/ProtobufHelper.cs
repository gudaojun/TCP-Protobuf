
    using System;
    using System.IO;
    using Google.Protobuf;

    public class ProtobufHelper
    {
        public static byte[] ToBytes(object message)
        {
            return ((Google.Protobuf.IMessage) message).ToByteArray();
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
