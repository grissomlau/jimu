using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Jimu.Common.Helper
{
    delegate object TypeConvertDelegate(object instance, Type destinationType);

    class TypeConvertProvider
    {
        public static object Convert(object instance, Type destinationType)
        {
            return GetConvertor().Select(convertor => convertor(instance, destinationType)).FirstOrDefault(result => result != null);
        }

        public static IEnumerable<TypeConvertDelegate> GetConvertor()
        {
            yield return EnumTypeConvert;
            yield return SimpleTypeConvert;
            yield return ComplexTypeConvert;
        }

        private static object EnumTypeConvert(object instance, Type destinationType)
        {
            if (instance == null || !destinationType.GetTypeInfo().IsEnum) return null;
            return Enum.Parse(destinationType, instance.ToString());
        }

        private static object SimpleTypeConvert(object instance, Type destinationType)
        {
            if (instance is IConvertible && typeof(IConvertible).GetTypeInfo().IsAssignableFrom(destinationType))
                return System.Convert.ChangeType(instance, destinationType);
            return null;
        }

        private static object ComplexTypeConvert(object instance, Type destinationType)
        {
            try
            {
                // error ocur when  deserialize Guid with Json，so we use Guid.Parse
                if (destinationType != typeof(Guid))
                    return JsonConvert.DeserializeObject(instance + "", destinationType);
                return Guid.TryParse(instance + "", out var guid)
                    ? guid
                    : JsonConvert.DeserializeObject(instance + "", destinationType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}