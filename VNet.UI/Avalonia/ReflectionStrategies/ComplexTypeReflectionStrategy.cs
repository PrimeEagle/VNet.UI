using System.Collections;
using System.Reflection;

namespace VNet.UI.Avalonia.ReflectionStrategies
{
    public class ComplexTypeReflectionStrategy : IReflectionStrategy
    {
        private readonly IReflectionStrategy _simpleTypeStrategy = new SimpleTypeReflectionStrategy();
        private readonly IReflectionStrategy _enumerableStrategy = new EnumerableOfSimpleTypeReflectionStrategy();

        public bool CanReflect(Type type)
        {
            return !type.IsPrimitive && type != typeof(string) && !type.IsEnum && type != typeof(IEnumerable);
        }

        public object? ReflectProperty(object instance, PropertyInfo property)
        {
            var value = property.GetValue(instance);
            if (value == null) return null;

            if (_simpleTypeStrategy.CanReflect(property.PropertyType))
            {
                return _simpleTypeStrategy.ReflectProperty(instance, property);
            }
            else if (_enumerableStrategy.CanReflect(property.PropertyType))
            {
                return _enumerableStrategy.ReflectProperty(instance, property);
            }

            var reflectedProperties = new Dictionary<string, object>();
            var properties = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                object? reflectedValue;

                if (CanReflect(prop.PropertyType))
                {
                    reflectedValue = ReflectProperty(value, prop);
                }
                else
                {
                    reflectedValue = _simpleTypeStrategy.CanReflect(prop.PropertyType) ?
                                        _simpleTypeStrategy.ReflectProperty(value, prop) :
                                     _enumerableStrategy.CanReflect(prop.PropertyType) ?
                                        _enumerableStrategy.ReflectProperty(value, prop) :
                                        null;
                }

                if (reflectedValue != null)
                {
                    reflectedProperties[prop.Name] = reflectedValue;
                }
            }

            return reflectedProperties;
        }
    }
}