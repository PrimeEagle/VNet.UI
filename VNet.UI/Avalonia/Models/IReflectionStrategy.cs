using System.Reflection;

namespace VNet.UI.Avalonia.Models;

public interface IReflectionStrategy
{
    bool CanReflect(Type type);
    object ReflectProperty(object instance, PropertyInfo property);
}