using System.Collections;
using System.Reflection;

namespace VNet.UI.Avalonia.Common.ReflectionStrategies
{
    public class EnumerableOfEnumReflectionStrategy : IReflectionStrategy
    {
        public bool CanReflect(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type) || type == typeof(string)) return false;
            if (!type.IsGenericType) return false;
            var itemType = type.GetGenericArguments()[0];

            return itemType.IsEnum;
        }

        public object? ReflectProperty(object instance, PropertyInfo property)
        {
            if (property.GetValue(instance) is not IEnumerable value) return null;
            var reflectedEnums = new List<string>();

            foreach (var item in value)
            {
                if (item == null)
                {
                    reflectedEnums.Add("null");
                    continue;
                }

                var enumName = Enum.GetName(item.GetType(), item);
                reflectedEnums.Add(enumName ?? "Unknown");
            }

            return reflectedEnums;
        }
    }
}