using System.Text;

namespace Jimu.Core.Protocols
{
    public class RemoteInvokeResultMessage
    {
        public string ExceptionMessage { get; set; }
        public string ErrorMsg { get; set; }
        public string ErrorCode { get; set; }
        public object Result { get; set; }
        public string ResultType { get; set; }

        public string ToErrorString()
        {
            var errorMsg = new StringBuilder();
            errorMsg.Append(!string.IsNullOrEmpty(ErrorCode) ? $"{ErrorCode}," : "");
            errorMsg.Append(!string.IsNullOrEmpty(ErrorMsg) ? $"{ErrorMsg}," : "");
            errorMsg.Append(!string.IsNullOrEmpty(ExceptionMessage) ? $"{ExceptionMessage}," : "");
            return errorMsg.ToString().TrimEnd(',');
        }
    }
}