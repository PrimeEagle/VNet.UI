using System.Collections;
using System.Reflection;

namespace VNet.UI.Avalonia.ReflectionStrategies
{
    public class EnumerableOfComplexTypeReflectionStrategy : IReflectionStrategy
    {
        private readonly IReflectionStrategy _complexTypeStrategy = new ComplexTypeReflectionStrategy();

        public bool CanReflect(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type) || type == typeof(string))
            {
                return false;
            }

            if (!type.IsGenericType) return false;
            var itemType = type.GetGenericArguments()[0];

            return _complexTypeStrategy.CanReflect(itemType);
        }

        public object? ReflectProperty(object instance, PropertyInfo property)
        {
            if (property.GetValue(instance) is not IEnumerable value) return null;

            var reflectedItems = new List<object>();
            foreach (var item in value)
            {
                if (item == null)
                {
                    reflectedItems.Add("null");
                    continue;
                }

                if (_complexTypeStrategy.CanReflect(item.GetType()))
                {
                    var reflectedItem = _complexTypeStrategy.ReflectProperty(item, item.GetType().GetProperty("This", BindingFlags.Instance | BindingFlags.Public));
                    reflectedItems.Add(reflectedItem ?? "null");
                }
                else
                {
                    reflectedItems.Add(item);
                }
            }

            return reflectedItems;
        }
    }
}