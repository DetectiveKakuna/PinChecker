using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PinChecker.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ShopName
{
    [EnumMember(Value = "Potato Pins")]
    PotatoPins,
}