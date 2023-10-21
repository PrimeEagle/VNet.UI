using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using VNet.Configuration;
using VNet.Configuration.Attributes;
using VNet.UI.Avalonia.Common;
using VNet.UI.Avalonia.Common.CategoryDefinitions;
using VNet.UI.Avalonia.Common.PropertyDefinitions;
using VNet.UI.Avalonia.Common.PropertyEditors;
using VNet.UI.Avalonia.Common.ReflectionStrategies;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable SuggestBaseTypeForParameter
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning disable CA1822
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8603 // Possible null reference return.

namespace VNet.UI.Avalonia.Controls
{
    public partial class PreferenceTabs : UserControl
    {
        private readonly Dictionary<Type, IPropertyDefinition> _propertyDefinitions = new();
        private readonly Dictionary<string, IPropertyEditor> _propertyEditors = new();
        private readonly StackPanel _propertyPanel = new() { Orientation = Orientation.Vertical };
        private List<IGrouping<ICategoryDefinition, PropertyInfo>> _sortedCategorizedProperties = new List<IGrouping<ICategoryDefinition, PropertyInfo>>();
        private object? _currentObject;
        private List<PropertyInfo> _originalProperties;
        private HashSet<object> _visitedObjects = new HashSet<object>();


        public bool Recursive { get; set; } = true;
        public CategorizationType CategoryType { get; set; } = CategorizationType.ByClass;
        public CategorySortingMode CategorySorting { get; set; } = CategorySortingMode.None;
        public PropertySortingMode PropertySorting { get; set; } = PropertySortingMode.Alphabetical;
        public static readonly DirectProperty<PreferenceTabs, object> CurrentObjectProperty =
            AvaloniaProperty.RegisterDirect<PreferenceTabs, object>(
                nameof(CurrentObject),
                o => o.CurrentObject,
                (o, v) => o.CurrentObject = v);
        public int RecursionDepth { get; set; } = 0;
        public object? CurrentObject
        {
            get => _currentObject;
            set
            {
                if (_currentObject == value) return;
                if (_currentObject is INotifyPropertyChanged oldNotifyPropertyChanged)
                {
                    oldNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }

                SetAndRaise(CurrentObjectProperty, ref _currentObject, value);

                if (value is INotifyPropertyChanged newNotifyPropertyChanged)
                {
                    newNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;
                }

                ReflectObject(value);
            }
        }

        public static readonly StyledProperty<ObservableCollection<string>> TabNamesProperty =
            AvaloniaProperty.Register<PreferenceTabs, ObservableCollection<string>>(nameof(TabNames), null, true, BindingMode.TwoWay);

        public ObservableCollection<string> TabNames
        {
            get => GetValue(TabNamesProperty);
            set => SetValue(TabNamesProperty, value);
        }

        public PreferenceTabs()
        {
            InitializeComponent();
           // Content = new ScrollViewer { Content = _propertyPanel };
        }

        public void RegisterPropertyDefinition(Type dataType, IPropertyDefinition definition)
        {
            _propertyDefinitions[dataType] = definition;
        }

        public void ReflectObject(object objectToReflect)
        {
            _propertyPanel.Children.Clear();

            // Get all properties of the object.
            var properties = objectToReflect.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            // Clear the visited objects.
            _visitedObjects.Clear();

            if (Recursive)
            {
                // Start recursive property fetching, filtering out ISettings properties from being added to the list.
                properties = GetPropertiesRecursively(objectToReflect, properties, 1);
            }

            // Assign the list of properties that don't implement ISettings.
            _originalProperties = properties;

            // Generate UI or other representations for the properties.
            GeneratePropertyItems(objectToReflect, properties);
        }

        private List<PropertyInfo> GetPropertiesRecursively(object obj, List<PropertyInfo> properties, int currentDepth)
        {
            if (RecursionDepth != 0 && currentDepth >= RecursionDepth)
            {
                return properties;
            }

            // Preventing recursion on the same object.
            if (!_visitedObjects.Add(obj))
            {
                return new List<PropertyInfo>(); // Object already visited.
            }

            var allProperties = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                // Check if the property is a class but not a string (primitive types are already excluded).
                var propertyType = property.PropertyType;
                if (propertyType.IsPrimitive || propertyType == typeof(string)) continue;

                // Get the property value.
                var propertyValue = property.GetValue(obj);
                if (propertyValue == null || _visitedObjects.Contains(propertyValue)) continue;

                // If the property type implements any interface (excluding ISettings), we add it to the list but do not recurse into it.
                bool isInterface = propertyType.GetInterfaces().Any();
                bool isSettings = typeof(ISettings).IsAssignableFrom(propertyType);

                if (isInterface && !isSettings)
                {
                    // It's a non-Settings interface; add it but don't recurse.
                    allProperties.Add(property);
                }
                else if (!isSettings) // For non-Settings types, continue the recursion.
                {
                    allProperties.Add(property);

                    // For recursion, get the properties of the current property.
                    var subProperties = propertyValue.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                    allProperties.AddRange(GetPropertiesRecursively(propertyValue, subProperties, currentDepth + 1));
                }
                // If it's ISettings, don't add it to the list but still recurse into it.
                else if (isSettings)
                {
                    var subProperties = propertyValue.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                    allProperties.AddRange(GetPropertiesRecursively(propertyValue, subProperties, currentDepth + 1));
                }
            }

            return allProperties;
        }



        private IEnumerable<PropertyInfo> GetSortedProperties(object targetObject)
        {
            return targetObject.GetType().GetProperties().OrderBy(p => p.Name);
        }

        public void SearchAndDisplayProperties(string searchTerm)
        {
            var properties = GetSortedProperties(_currentObject).Where(prop => prop.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            _propertyPanel.Children.Clear();
            GeneratePropertyItems(_currentObject, properties);
        }

        public void ClearSearch()
        {
            if (_currentObject == null || _originalProperties == null) return;
            _propertyPanel.Children.Clear();
            GeneratePropertyItems(_currentObject, _originalProperties);
        }

        private void GeneratePropertyItems(object targetObject, IEnumerable<PropertyInfo> properties)
        {
            _propertyPanel.Children.Clear();
            _propertyEditors.Clear();

            var categorizedProperties = properties.GroupBy(GetPropertyCategory);
            var sortedCategorizedProperties = CategorySorting switch
            {
                CategorySortingMode.Alphabetical =>
                    categorizedProperties.OrderBy(categoryGroup => categoryGroup.Key.Name),

                CategorySortingMode.DisplayOrder =>
                    categorizedProperties.OrderBy(categoryGroup => categoryGroup.Key.DisplayOrder),

                _ => categorizedProperties
            };

            foreach (var categoryGroup in sortedCategorizedProperties)
            {
                IEnumerable<PropertyInfo> sortedProperties = PropertySorting switch
                {
                    PropertySortingMode.Alphabetical => categoryGroup.OrderBy(x => x.Name),
                    PropertySortingMode.DisplayOrder => categoryGroup.OrderBy(GetPropertyDisplayOrder),
                    _ => categoryGroup
                };

                foreach (var property in sortedProperties)
                {
                    var canUpdateModelFromControl = property.GetCustomAttribute<OnlyUpdateControlFromModelAttribute>() == null;
                    var canUpdateControlFromModel = property.GetCustomAttribute<OnlyUpdateModelFromControlAttribute>() == null;

                    if (!canUpdateModelFromControl && !canUpdateControlFromModel)
                    {
                        continue;
                    }

                    if (!_propertyDefinitions.TryGetValue(property.PropertyType, out var definition))
                    {
                        continue;
                    }

                    var editor = definition.CreateEditor();

                    if (canUpdateControlFromModel)
                    {
                        var propertyValue = property.GetValue(targetObject);
                        editor.Value = propertyValue;
                    }

                    _propertyEditors[property.Name] = editor;
                    editor.ValueChanged -= OnEditorValueChanged;

                    if (canUpdateModelFromControl)
                    {
                        editor.ValueChanged += OnEditorValueChanged;
                    }

                    AddEditorToPropertyGrid(editor, property);
                }
            }

            var _sortedCategorizedProperties = new ObservableCollection<string>(sortedCategorizedProperties.Select(x => x.Key.Name).ToList());
        }
        
        private void OnEditorValueChanged(object sender, EventArgs e)
        {
            var editor = (IPropertyEditor)sender;
            var propertyName = editor.Tag as string;
            var property = _currentObject?.GetType().GetProperty(propertyName);

            if (property == null) return;
            if (property.GetCustomAttribute<OnlyUpdateControlFromModelAttribute>() != null) return;
            if (!property.CanWrite)
            {
                return;
            }

            var newValue = editor.Value;
            property.SetValue(_currentObject, newValue);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = sender.GetType().GetProperty(args.PropertyName);

            if (property == null) return;
            if (property.GetCustomAttribute<OnlyUpdateModelFromControlAttribute>() != null) return;
            if (!_propertyEditors.TryGetValue(args.PropertyName, out var editor)) return;

            var currentValue = property.GetValue(sender);
            editor.Value = currentValue;
        }

        private void AddEditorToPropertyGrid(IPropertyEditor editor, PropertyInfo property)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            var label = new TextBlock { Text = property.Name, Width = 150 };
            panel.Children.Add(label);

            var editorControl = editor.GetEditorControl();
            panel.Children.Add(editorControl);

            _propertyPanel.Children.Add(panel);
        }

        private ICategoryDefinition GetPropertyCategory(PropertyInfo property)
        {
            var categoryAttribute = property.GetCustomAttribute<CategoryAttribute>();

            switch (CategoryType)
            {
                case CategorizationType.ByClass:
                    return new CategoryByClassName(property.DeclaringType?.Name ?? "Unknown");

                case CategorizationType.ByAttribute:
                    if (categoryAttribute != null)
                    {
                        return new CategoryByAttribute(categoryAttribute.Category);
                    }
                    break;

                case CategorizationType.None:
                default:
                    return new Uncategorized();
            }

            return new Uncategorized();
        }

        private int GetPropertyDisplayOrder(PropertyInfo property)
        {
            var displayOrderAttr = property.GetCustomAttribute<DisplayOrderAttribute>();
            return displayOrderAttr?.Order ?? int.MaxValue;
        }

        public void RegisterPrimitiveTypes()
        {
            var reflectionStrategy = new SimpleTypeReflectionStrategy();
            var editor = new TextBoxPropertyEditor();

            var types = new[]
            {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),
                typeof(string)
            };

            foreach (var type in types)
            {
                var propertyDefinition = new PropertyDefinition
                {
                    Editor = editor,
                    ReflectionStrategy = reflectionStrategy
                };

                RegisterPropertyDefinition(type, propertyDefinition);
            }
        }
    }
}