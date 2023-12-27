using FluentValidation;
using Nop.Plugin.Mesut.DiscountManager.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Mesut.DiscountManager.Validators;

public class ZipCodeRangeModelValidator : BaseNopValidator<ZipCodeRangeModel>
{
    public ZipCodeRangeModelValidator(ILocalizationService localizationService)
    {
         RuleFor(x=>x)
            .Must((configuration, zipcoderange) =>
            {
                return zipcoderange.MaxValue > 0 &&
                        zipcoderange.MinValue > 0 &&
                        zipcoderange.MaxValue > zipcoderange.MinValue;
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.IsNotValid"));
    }
}
