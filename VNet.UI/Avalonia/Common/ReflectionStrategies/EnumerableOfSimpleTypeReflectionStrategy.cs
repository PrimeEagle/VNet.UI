using System.Collections;
using System.Reflection;

namespace VNet.UI.Avalonia.Common.ReflectionStrategies
{
    public class EnumerableOfSimpleTypeReflectionStrategy : IReflectionStrategy
    {
        public bool CanReflect(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }

            if (!type.IsGenericType) return false;
            var itemType = type.GetGenericArguments()[0];

            return itemType.IsPrimitive || itemType == typeof(string);

        }

        public object? ReflectProperty(object instance, PropertyInfo property)
        {
            if (property.GetValue(instance) is not IEnumerable value) return null;

            var items = value.Cast<object?>().Where(item => item != null).ToList();

            return items.Count > 0 ? items : null;
        }
    }
}