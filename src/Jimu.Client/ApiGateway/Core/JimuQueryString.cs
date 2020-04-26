using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace Jimu.Client.ApiGateway.Core
{
    [ModelBinder(BinderType = typeof(JimuQueryStringModelBinder))]
    public class JimuQueryString
    {
        public string Query { get; set; }

        public IDictionary<string, string> Collection { get; set; }
        public JimuQueryString(string query)
        {
            Query = query;
            var group = query.TrimStart('?')
               .Split('&', StringSplitOptions.RemoveEmptyEntries)
               .Select(x => x.Split('='))
               .Where(x => x.Length == 2)
               .GroupBy(x => x[0], x => x[1]).ToList()
               ;

            //Collection = query.TrimStart('?')
            //   .Split('&', StringSplitOptions.RemoveEmptyEntries)
            //   .Select(x => x.Split('='))
            //   .Where(x => x.Length == 2)
            //   .ToDictionary(x => x[0], x => x[1]);
            //.ToLookup(x => x[0], x => x[1], StringComparer.OrdinalIgnoreCase);
            //Parser(Collection);
            Collection = Parser(group);
            //Collection = HttpUtility.ParseQueryString(query);
        }
        /// <summary>
        /// compatible with OpenAPI3 query parameter of object
        /// </summary>

        private IDictionary<string, string> Parser(List<IGrouping<string, string>> list)
        {
            IDictionary<string, string> kvs = new Dictionary<string, string>();

            foreach (var g in list)
            {
                // is an array
                if (g.Count() > 1)
                {
                    using (TextWriter tw = new StringWriter())
                    using (JsonWriter jw = new JsonTextWriter(tw))
                    {
                        jw.WriteStartArray();
                        foreach (var item in g)
                        {
                            jw.WriteValue(HttpUtility.UrlDecode(item));
                        }
                        jw.WriteEndArray();
                        kvs[g.Key] = tw.ToString();
                    }
                }
                else
                {

                    var v = g.First();
                    var form = v.Split(',');
                    if (!v.Contains(',')
                        || form.Length % 2 != 0
                        || IsValidJson(HttpUtility.UrlDecode(v)))
                    {
                        kvs[g.Key] = HttpUtility.UrlDecode(v);
                        continue;
                    }

                    using (TextWriter tw = new StringWriter())
                    using (JsonWriter jw = new JsonTextWriter(tw))
                    {
                        jw.WriteStartObject();
                        var i = 0;
                        while (i < form.Length)
                        {
                            jw.WritePropertyName(form[i]);
                            jw.WriteValue(HttpUtility.UrlDecode(form[++i]));
                            ++i;
                        }
                        jw.WriteEndObject();
                        kvs[g.Key] = tw.ToString();
                    }

                }
            }
            return kvs;
        }
        //private void Parser(IDictionary<string, string> kvs)
        //{
        //    foreach (var k in kvs.Keys.ToList())
        //    {
        //        var v = kvs[k];
        //        var form = v.Split(',');
        //        if (!v.Contains(',')
        //            || form.Length % 2 != 0
        //            || IsValidJson(HttpUtility.UrlDecode(v)))
        //        {
        //            kvs[k] = HttpUtility.UrlDecode(v);
        //            continue;
        //        }

        //        using (TextWriter tw = new StringWriter())
        //        using (JsonWriter jw = new JsonTextWriter(tw))
        //        {
        //            jw.WriteStartObject();
        //            var i = 0;
        //            while (i < form.Length)
        //            {
        //                jw.WritePropertyName(form[i]);
        //                jw.WriteValue(HttpUtility.UrlDecode(form[++i]));
        //                ++i;
        //            }
        //            jw.WriteEndObject();
        //            kvs[k] = tw.ToString();
        //        }

        //    }
        //}

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    //Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    //Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
