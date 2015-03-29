using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    public class StreamContent
    {
        public Byte[] ContentBytes;

        public UInt32 ContentLength;

        protected StreamContent(Byte[] contentBytes)
        {
            Debug.Assert(contentBytes != null);

            ContentBytes = contentBytes;
            ContentLength = (UInt32)contentBytes.Length;
        }
    }

    public class JTokenStreamContent : StreamContent
    {
        public JTokenStreamContent(JToken jToken) : base(Encoding.UTF8.GetBytes(jToken.ToString()))
        {
        }
    }
}
