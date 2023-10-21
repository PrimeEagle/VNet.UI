using System.Reflection;

namespace VNet.UI.Avalonia.Common.ReflectionStrategies
{
    public class EnumReflectionStrategy : IReflectionStrategy
    {
        public bool CanReflect(Type type)
        {
            return type.IsEnum;
        }

        public object? ReflectProperty(object instance, PropertyInfo property)
        {
            if (!property.PropertyType.IsEnum || !property.CanRead)
            {
                return null;
            }

            var value = property.GetValue(instance);

            return value != null ? Enum.GetName(property.PropertyType, value) : null;
        }
    }
}