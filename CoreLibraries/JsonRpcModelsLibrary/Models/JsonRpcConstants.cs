using System;

namespace JsonRpcModelsLibrary.Models
{
    public class JsonRpcConstants
    {
        public const String Version = "2.0";

        public const int ParseErrorCode = -32700;
        public const int InvalidRequestErrorCode = -32600;
        public const int MethodNotFoundErrorCode = -32601;
        public const int InvalidParamsErrorCode = -32602;
        public const int InternalErrorCode = -32603;

        public const String ParseErrorMessage = "Parse error.";
        public const String InvalidRequestErrorMessage = "Invalid Request.";
        public const String MethodNotFoundErrorMessage = "Method not found.";
        public const String InvalidParamsErrorMessage = "Invalid params.";
        public const String InternalErrorMessage = "Internal error.";

        public const uint StreamPrefixSize = 4;
    }
    public enum JsonRpcStreamMode
    {
        ObjectSizePrefixed,
        Unknown
    }
}
