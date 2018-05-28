using System;

namespace Jimu.ApiGateway.Model
{
    public class HttpStatusCodeException : Exception
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = @"application/json";
        public HttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }
        public HttpStatusCodeException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
