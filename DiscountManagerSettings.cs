using Nop.Core.Configuration;

namespace Nop.Plugin.Mesut.DiscountManager;

public class DiscountManagerSettings : ISettings
{
    public int CountryId { get; set; }
}
