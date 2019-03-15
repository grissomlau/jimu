using System.Collections.Generic;

namespace Jimu.Client.Auth
{
    public class JwtAuthorizationContext
    {
        public string Error { get; private set; }
        public string ErrorDescription { get; private set; }
        public bool IsRejected { get; private set; }

        public string UserName { get; }
        public string Password { get; }

        public RemoteCallerContext RemoteInvokeMessage { get; }


        private Dictionary<string, object> Payload { get; }

        public JwtAuthorizationContext(JwtAuthorizationOptions options, RemoteCallerContext remoteInvokeMessage)
        {
            Options = options;
            Payload = options.GetPayload();
            RemoteInvokeMessage = remoteInvokeMessage;
            if (remoteInvokeMessage.Paras.ContainsKey("username"))
                UserName = remoteInvokeMessage.Paras["username"] + "";
            if (remoteInvokeMessage.Paras.ContainsKey("password"))
                Password = remoteInvokeMessage.Paras["password"] + "";
            Payload.Add("username", UserName);
        }
        //public Dictionary<string, string> Claims { get; }

        public JwtAuthorizationOptions Options { get; }
        public void Rejected(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
            IsRejected = true;
        }

        public void AddClaim(string name, string value)
        {
            Payload[name] = value;
        }

        public Dictionary<string, object> GetPayload()
        {
            return Payload;
        }
    }

}
