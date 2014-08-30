using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    internal class StreamCommand
    {
        public Byte[] ContentBytes;

        public UInt32 ContentLength
        {
            get { return (UInt32)ContentBytes.Length; }
        }

        protected StreamCommand()
        {
            ContentBytes = null;
        }
    }

    internal class StringStreamCommand : StreamCommand
    {
        public String Contents { get; set; }

        protected StringStreamCommand(String contents)
        {
            Contents = contents;
            ContentBytes = Encoding.UTF8.GetBytes(contents);
        }

        public override string ToString()
        {
            return Contents;
        }
    }

    internal class JObjectStreamCommand : StringStreamCommand
    {
        public JObject JsonContents { get; set; }

        protected JObjectStreamCommand(JObject jObject)
            : base(jObject.ToString())
        {
            JsonContents = jObject;
            ContentBytes = Encoding.UTF8.GetBytes(jObject.ToString());
        }

        public static class Factory
        {
            public static JObjectStreamCommand Create(JObject jObject)
            {
                return new JObjectStreamCommand(jObject);
            }

            public static JObjectStreamCommand Create(String jsonString)
            {
                return Create(JObject.Parse(jsonString));
            }

            public static JObjectStreamCommand Create(byte[] bytes)
            {
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                return Create(messageString);
            }
        }
    }
}
