using Nop.Core;

namespace Nop.Plugin.Mesut.DiscountManager.Domains;

public class DiscountZipCodeRange : BaseEntity
{
    public int DiscountRequirementId { get; set; }
    public int ZipCodeRangeMin { get; set; }
    public int ZipCodeRangeMax { get; set; }
}
