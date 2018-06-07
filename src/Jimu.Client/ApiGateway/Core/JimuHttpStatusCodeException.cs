using System;

namespace Jimu.Client.ApiGateway
{
    public class JimuHttpStatusCodeException : Exception
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = @"application/json";
        public JimuHttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }
        public JimuHttpStatusCodeException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
