using System.Reflection;

namespace VNet.UI.Avalonia.Models;

public class SimpleTypeReflectionStrategy : IReflectionStrategy
{
    public bool CanReflect(Type type)
    {
        return type.IsPrimitive || type == typeof(string);
    }

    public object ReflectProperty(object instance, PropertyInfo property)
    {
        return property.GetValue(instance);
    }
}