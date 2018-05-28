using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Updraft.ApiGateway.Utils
{
    public class MyQueryStringValueProvider : QueryStringValueProvider
    {
        public MyQueryStringValueProvider(BindingSource bindingSource, IQueryCollection values, CultureInfo culture) : base(bindingSource, values, culture)
        {
        }

        public MyQueryStringValueProvider(IQueryCollection query) : base(BindingSource.Query, query, CultureInfo.InvariantCulture)
        {

        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            //if (_key != null && _key != key)
            //{
            //    return result;
            //}

            //if (result != ValueProviderResult.None && result.Values.Any(x => x.IndexOf(_separator, StringComparison.OrdinalIgnoreCase) > 0))
            //{
            //    var splitValues = new StringValues(result.Values
            //        .SelectMany(x => x.Split(new[] { _separator }, StringSplitOptions.None)).ToArray());
            //    return new ValueProviderResult(splitValues, result.Culture);
            //}

            return result;
        }
    }
}
