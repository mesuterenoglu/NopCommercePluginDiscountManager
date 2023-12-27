using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Mesut.DiscountManager.Models;

public class ConfigurationModel
{
    public int DiscountId { get; set; }
    public int DiscountRequirementId { get; set; }
    [NopResourceDisplayName("Plugins.Mesut.DiscountManager.Fields.ZipCodeRanges")]
    public ZipCodeRangeModel[] ZipCodeRanges { get; set; }
}
