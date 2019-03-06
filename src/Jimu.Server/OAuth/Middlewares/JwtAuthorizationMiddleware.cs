using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jimu.Extension;
using Jose;

namespace Jimu.Server.OAuth
{
    /// <summary>
    /// support jwt middleware
    /// </summary>
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDel _next;
        private readonly JwtAuthorizationOptions _options;
        public JwtAuthorizationMiddleware(RequestDel next, JwtAuthorizationOptions options)
        {
            _options = options;
            _next = next;
        }

        public Task Invoke(RemoteCallerContext context)
        {
            // get jwt token 
            if (!string.IsNullOrEmpty(_options.TokenEndpointPath)
                && context.ServiceEntry == null
                && context.RemoteInvokeMessage.ServiceId == _options.GetServiceId())
            {
                if (_options.CheckCredential == null)
                    throw new Exception("JwtAuthorizationOptions.CheckCredential must be provided");
                JwtAuthorizationContext jwtAuthorizationContext = new JwtAuthorizationContext(_options, context.RemoteInvokeMessage);

                _options.CheckCredential(jwtAuthorizationContext);
                if (jwtAuthorizationContext.IsRejected)
                {
                    return context.Response.WriteAsync(context.TransportMessage.Id, new JimuRemoteCallResultData
                    {
                        ErrorMsg = $"{jwtAuthorizationContext.Error}, {jwtAuthorizationContext.ErrorDescription}",
                        ErrorCode = "400"
                    });
                }

                var payload = jwtAuthorizationContext.GetPayload();
                var token = JWT.Encode(payload, Encoding.ASCII.GetBytes(_options.SecretKey), JwsAlgorithm.HS256);

                var result = new ExpandoObject() as IDictionary<string, object>;
                result["access_token"] = token;
                if (_options.ValidateLifetime)
                {
                    result["expired_in"] = payload["exp"];
                }

                return context.Response.WriteAsync(context.TransportMessage.Id, new JimuRemoteCallResultData
                {
                    Result = result
                });
            }
            // jwt authentication, alse authentication the role

            if (context.ServiceEntry != null && context.ServiceEntry.Descriptor.EnableAuthorization)
            {

                try
                {
                    var pureToken = context.RemoteInvokeMessage.Token;
                    if (pureToken != null && pureToken.Trim().StartsWith("Bearer "))
                    {
                        pureToken = pureToken.Trim().Substring(6).Trim();
                    }
                    var payload = JWT.Decode(pureToken, Encoding.ASCII.GetBytes(_options.SecretKey), JwsAlgorithm.HS256);
                    var payloadObj = JimuHelper.Deserialize(payload, typeof(IDictionary<string, object>)) as IDictionary<string, object>;
                    if (_options.ValidateLifetime)
                    {
                        //var exp = payloadObj["exp"];
                        if (payloadObj == null || ((Int64)payloadObj["exp"]).ToDate() < DateTime.Now)
                        {
                            var result = new JimuRemoteCallResultData
                            {
                                ErrorMsg = "Token is Expired",
                                ErrorCode = "401"
                            };
                            return context.Response.WriteAsync(context.TransportMessage.Id, result);

                        }
                    }
                    var serviceRoles = context.ServiceEntry.Descriptor.Roles;
                    if (!string.IsNullOrEmpty(serviceRoles))
                    {
                        var serviceRoleArr = serviceRoles.Split(',');
                        var roles = payloadObj != null && payloadObj.ContainsKey("roles") ? payloadObj["roles"] + "" : "";
                        var authorize = roles.Split(',').Any(role => serviceRoleArr.Any(x => x.Equals(role, StringComparison.InvariantCultureIgnoreCase)));
                        if (!authorize)
                        {
                            var result = new JimuRemoteCallResultData
                            {
                                ErrorMsg = "Unauthorized",
                                ErrorCode = "401"
                            };
                            return context.Response.WriteAsync(context.TransportMessage.Id, result);
                        }
                    }
                    context.RemoteInvokeMessage.Payload = new JimuPayload { Items = payloadObj };
                }
                catch (Exception ex)
                {
                    var result = new JimuRemoteCallResultData
                    {
                        ErrorMsg = $"Token is incorrect, exception is { ex.Message}",
                        ErrorCode = "401"
                    };
                    return context.Response.WriteAsync(context.TransportMessage.Id, result);
                }
                return _next(context);
            }
            // service can be annoymouse request

            return _next(context);
        }
    }
}
