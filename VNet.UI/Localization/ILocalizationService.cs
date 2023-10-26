using System.Globalization;

namespace VNet.UI.Localization;

public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, CultureInfo culture);
}