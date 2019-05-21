using Autofac;
using Jimu;
using Jimu.Client;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Dapper;
//using MySql.Data.MySqlClient;

namespace ApiGateway.Modules.Auth
{
    public class RoleAuthMiddleware
    {
        private readonly ClientRequestDel _next;
        private readonly RoleAuthOptions _options;
        public RoleAuthMiddleware(ClientRequestDel next, RoleAuthOptions options)
        {
            _options = options;
            _next = next;
        }

        public Task<JimuRemoteCallResultData> InvokeAsync(RemoteCallerContext context)
        {
            if (_options != null && context.Service.ServiceDescriptor.EnableAuthorization)
            {
                var hasRight = false;
                var empid = 0;
                if (context.PayLoad != null && context.PayLoad.Items != null && context.PayLoad.Items.Count > 0)
                {
                    empid = Convert.ToInt32(context.PayLoad.Items["userid"]);
                }
                var url = context.Service.ServiceDescriptor.RoutePath;
                //                using (var cnn = new MySqlConnection(_options.ConnectionString))
                //                {
                //                    cnn.Open();
                //                    var sql = $@"
                //select count(0)
                //from {DbNameHelper.Instance.SystemDb}.SysRoleMenuPoint rmp
                //join {DbNameHelper.Instance.SystemDb}.SysErpPostRole epr on epr.RoleId = rmp.RoleId
                //join {DbNameHelper.Instance.SystemDb}.SysMenuPoint mp on mp.Code = rmp.PointCode and mp.MenuCode = rmp.MenuCode and mp.Url = @url
                //join {DbNameHelper.Instance.ErpDb}.bas_emp emp on emp.empid = @empid and emp.postid = epr.PostId
                //";
                //                    hasRight = cnn.QueryFirst<int>(sql, new { empid, url }) > 0;
                //                }
                if (!hasRight)
                {
                    var result = new JimuRemoteCallResultData
                    {
                        ErrorMsg = "Unauthorized",
                        ErrorCode = "401"
                    };
                    return Task.FromResult(result);
                }


            }
            return _next(context);
        }
    }
}
