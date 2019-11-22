using System;

namespace Jimu.Client.ApiGateway.Core
{
    public class JimuHttpStatusCodeException : Exception
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = @"application/json";
        public string Path { get; set; }
        public JimuHttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }
        public JimuHttpStatusCodeException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public JimuHttpStatusCodeException(int statusCode, string message, string path) : base(message)
        {
            StatusCode = statusCode;
            Path = path;
        }
    }
}
