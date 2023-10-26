using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Options;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
#pragma warning disable CA2208


namespace VNet.UI.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _baseResourceName;


        public LocalizationService(IOptions<LocalizationOptions> options, Assembly resourceAssembly)
        {
            _baseResourceName = options.Value.ResourceBaseName;
            
            if (string.IsNullOrWhiteSpace(_baseResourceName))
            {
                throw new ArgumentException("Base resource name cannot be null or whitespace.", nameof(_baseResourceName));
            }

            if (resourceAssembly == null)
            {
                throw new ArgumentNullException(nameof(resourceAssembly));
            }

            _resourceManager = new ResourceManager(_baseResourceName, resourceAssembly);
        }

        public string GetString(string key)
        {
            return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? throw new ArgumentException($"No resource found with key '{key}'.");
        }

        public string GetString(string key, CultureInfo culture)
        {
            return _resourceManager.GetString(key, culture) ?? throw new ArgumentException($"No resource found with key '{key}'.");
        }
    }
}