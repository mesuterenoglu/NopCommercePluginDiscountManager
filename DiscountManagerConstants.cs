using Nop.Core.Caching;

namespace Nop.Plugin.Mesut.DiscountManager;

public static class DiscountManagerConstants
{
    public static string CountryCode => "NL";

    /// <summary>
    /// The system name of the discount requirement rule
    /// </summary>
    public static string SystemName => "Mesut.DiscountManager";

    /// <summary>
    /// Gets the cache key for configuration
    /// </summary>
    /// <remarks>
    /// {0} : configuration identifier
    /// </remarks>
    public static CacheKey CacheKey => new("Nop.Plugin.Mesut.DiscountManager-{0}", PrefixCacheKey);

    /// <summary>
    /// Gets the prefix key to clear cache
    /// </summary>
    public static string PrefixCacheKey => "Nop.Plugin.Mesut.DiscountManager";
}
