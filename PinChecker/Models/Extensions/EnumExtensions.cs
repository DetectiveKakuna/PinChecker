using PinChecker.Models.Enums;
using System.Text.RegularExpressions;

namespace PinChecker.Models.Extensions;

/// <summary>
/// Provides extension methods for enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Parses a text value into the corresponding ShopStatus enum value.
    /// Removes all whitespace and uses case-insensitive comparison.
    /// </summary>
    /// <param name="text">The text to parse</param>
    /// <returns>The corresponding ShopStatus enum value</returns>
    public static ShopStatus ToShopStatus(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return ShopStatus.Unknown;

        string normalized = new string([.. text.Where(c => !char.IsWhiteSpace(c))]).ToLowerInvariant();

        return normalized switch
        {
            "available" => ShopStatus.Available,
            "soldout" => ShopStatus.SoldOut,
            _ => ShopStatus.Unknown
        };
    }
}