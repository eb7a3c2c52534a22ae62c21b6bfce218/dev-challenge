using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wool.DevChallenge.Api.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortOption
    {
        Low,
        High,
        Ascending,
        Descending,
        Recommended
    }
}