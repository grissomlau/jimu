using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.Client.ApiGateway
{
    [ModelBinder(BinderType = typeof(JimuQueryStringModelBinder))]
    public class JimuQueryString
    {
        public string Query { get; set; }

        public NameValueCollection Collection { get; set; }
        public JimuQueryString(string query)
        {
            Query = query;
            Collection = HttpUtility.ParseQueryString(query);
        }
    }
}
