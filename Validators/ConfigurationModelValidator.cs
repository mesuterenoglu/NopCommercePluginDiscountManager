using FluentValidation;
using Nop.Plugin.Mesut.DiscountManager.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Mesut.DiscountManager.Validators;

public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
{
    public ConfigurationModelValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.DiscountId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Mesut.DiscountManager.Fields.DiscountId.Required"))
            .GreaterThanOrEqualTo(1)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Mesut.DiscountManager.Fields.DiscountId.IsNotValid"));

        RuleForEach(x => x.ZipCodeRanges)
            .NotNull()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.Required"))
            .Must((configuration, zipCodeRange) =>
            {
                return zipCodeRange is not null &&
                       1000 <= zipCodeRange.MinValue &&
                       zipCodeRange.MinValue < zipCodeRange.MaxValue &&
                       zipCodeRange.MaxValue <= 9999;
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.Between1000And9999"));
    }
}
