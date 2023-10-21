using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using VNet.Configuration.Attributes;
using VNet.UI.Avalonia.Common;
using VNet.UI.Avalonia.Common.CategoryDefinitions;
using VNet.UI.Avalonia.Common.PropertyDefinitions;
using VNet.UI.Avalonia.Common.PropertyEditors;

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
    public partial class PropertyGrid : UserControl
    {
        private readonly Dictionary<Type, IPropertyDefinition> _propertyDefinitions = new();
        private readonly Dictionary<string, IPropertyEditor> _propertyEditors = new();
        private readonly StackPanel _propertyPanel = new() { Orientation = Orientation.Vertical };
        private object? _currentObject;
        private List<PropertyInfo> _originalProperties;


        public CategorySortingMode CategorySorting { get; set; } = CategorySortingMode.None;
        public PropertySortingMode PropertySorting { get; set; } = PropertySortingMode.Alphabetical;
        public static readonly DirectProperty<PropertyGrid, object> CurrentObjectProperty =
            AvaloniaProperty.RegisterDirect<PropertyGrid, object>(
                nameof(CurrentObject),
                o => o.CurrentObject,
                (o, v) => o.CurrentObject = v);

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

        public PropertyGrid()
        {
            InitializeComponent();
            Content = new ScrollViewer { Content = _propertyPanel };
        }

        public void RegisterPropertyDefinition(Type dataType, IPropertyDefinition definition)
        {
            _propertyDefinitions[dataType] = definition;
        }

        public void ReflectObject(object objectToReflect)
        {
            _propertyPanel.Children.Clear();
            _originalProperties = objectToReflect.GetType().GetProperties().ToList();
            GeneratePropertyItems(objectToReflect, _originalProperties);
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
                CategorySortingMode.Alphabetical => categorizedProperties.OrderBy(x => x.Key), // x.Key is the category name itself if it's a string
                // The line below won't work as expected because we don't have a 'DisplayOrder' field in the string.
                // You need a more complex category representation to support ordering by display order.
                //CategorySortingMode.DisplayOrder => categorizedProperties.OrderBy(x => x.Key.DisplayOrder), 
                _ => categorizedProperties
            };

            foreach (var categoryGroup in sortedCategorizedProperties)
            {
                IEnumerable<PropertyInfo> sortedProperties = PropertySorting switch
                {
                    PropertySortingMode.Alphabetical => categoryGroup.OrderBy(x => x.Name),
                    // If you want to sort by display order, you would need a mechanism (like a custom attribute or a mapping table) to determine the order.
                    //PropertySortingMode.DisplayOrder => categoryGroup.OrderBy(x => /* you need an ordering mechanism here */),
                    _ => categoryGroup,
                };

                foreach (var property in sortedProperties)
                {
                    if (!_propertyDefinitions.TryGetValue(property.PropertyType, out var definition))
                        continue;

                    var editor = definition.CreateEditor();
                    var reflectedValue = definition.ReflectionStrategy.ReflectProperty(targetObject, property);

                    editor.Value = reflectedValue;
                    _propertyEditors[property.Name] = editor;

                    editor.Tag = property.Name;
                    editor.ValueChanged -= OnEditorValueChanged;
                    editor.ValueChanged += OnEditorValueChanged;

                    AddEditorToPropertyGrid(editor, property);
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = sender.GetType().GetProperty(args.PropertyName);
            if (property == null) return;

            var onlyUpdateModel = property.GetCustomAttribute<OnlyUpdateModelFromControlAttribute>() != null;
            if (onlyUpdateModel) return;

            if (!_propertyEditors.TryGetValue(args.PropertyName, out var editor)) return;

            var currentValue = property.GetValue(sender);
            editor.Value = currentValue;
        }

        private void OnEditorValueChanged(object sender, EventArgs e)
        {
            var editor = (IPropertyEditor)sender;
            var propertyName = editor.Tag as string;

            var property = _currentObject?.GetType().GetProperty(propertyName);
            if (property == null) return;

            var onlyUpdateControl = property.GetCustomAttribute<OnlyUpdateControlFromModelAttribute>() != null;
            if (onlyUpdateControl) return;

            if (!property.CanWrite)
            {
                return;
            }

            var newValue = editor.Value;
            property.SetValue(_currentObject, newValue);
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

        private string GetPropertyCategory(PropertyInfo property)
        {
            var categoryAttribute = property.GetCustomAttribute<CategoryAttribute>();

            if (categoryAttribute != null)
            {
                return categoryAttribute.Category;
            }

            return property.DeclaringType?.Name ?? "Default";
        }
        
        private int GetPropertyDisplayOrder(PropertyInfo property)
        {
            var orderAttribute = property.GetCustomAttribute<DisplayOrderAttribute>();

            return orderAttribute?.Order ?? int.MaxValue;
        }
    }
}