using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Plugin.Mesut.DiscountManager.Services.Abstract;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Mesut.DiscountManager;

public class DiscountManagerBase : BasePlugin, IDiscountRequirementRule
{
    #region Fields
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly ICountryService _countryService;
    private readonly IDiscountService _discountService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly ILocalizationService _localizationService;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly DiscountManagerSettings _settings;
    private readonly IAddressService _addressService;
    private readonly IDiscountZipCodeRangeService _discountZipCodeRangeService;
    #endregion

    #region Ctor
    public DiscountManagerBase(
        IActionContextAccessor actionContextAccessor,
        IDiscountService discountService,
        ILocalizationService localizationService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        ICountryService countryService,
        DiscountManagerSettings settings,
        IAddressService addressService,
        IDiscountZipCodeRangeService discountZipCodeRangeService)
    {
        _webHelper = webHelper;
        _countryService = countryService;
        _settingService = settingService;
        _discountService = discountService;
        _urlHelperFactory = urlHelperFactory;
        _localizationService = localizationService;
        _actionContextAccessor = actionContextAccessor;
        _settings = settings;
        _addressService = addressService;
        _discountZipCodeRangeService = discountZipCodeRangeService;
    }
    #endregion

    #region Methods
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        //invalid by default
        var result = new DiscountRequirementValidationResult();

        if (request.Customer is null)
            return result;

        var billingAddress = await _addressService.GetAddressByIdAsync(request.Customer.BillingAddressId.Value);
        if (billingAddress is null)
            return result;

        if (_settings.CountryId != billingAddress.CountryId)
            return result;

        if (!int.TryParse(billingAddress.ZipPostalCode.Substring(0, 4), out int zipDigits))
            return result;

        var ranges = await _discountZipCodeRangeService.GetByRequirementIdAsync(request.DiscountRequirementId);

        if (ranges is null)
            return result;

        foreach (var range in ranges)
        {
            if (range.ZipCodeRangeMin <= zipDigits && zipDigits <= range.ZipCodeRangeMax)
            {
                result.IsValid = true;
                return result;
            }
        }

        return result;
    }

    public string GetConfigurationUrl(int discountId, int? discountRequirementId)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        return urlHelper.Action(
            "Configure",
            "DiscountManager",
            new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
    }

    public override async Task InstallAsync()
    {
        //Define settings
        var nl = await _countryService.GetCountryByTwoLetterIsoCodeAsync(DiscountManagerConstants.CountryCode);
        await _settingService.SaveSettingAsync(new DiscountManagerSettings { CountryId = nl.Id });

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Mesut.DiscountManager.Fields.CountryId"] = 
                "This rule can only applied in Netherlands",
            ["Plugins.Mesut.DiscountManager.Fields.CountryId.Hint"] = 
                "This rule can only applied in Netherlands",
            ["Plugins.Mesut.DiscountManager.Fields.ZipCodeRanges"] = "Zip Code Ranges",
            ["Plugins.Mesut.DiscountManager.Fields.ZipCodeRanges.Hint"] =
                "In this field, you can enter the first 4 digits of the zip codes valid in the Netherlands (1000-9999).",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.DiscountId.Required"] = 
                "Discount Id can not be empty.",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.DiscountId.IsNotValid"] = 
                "Discount Id needs to be valid.",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.DiscountRequirementId.Required"] = 
                "Discount Requirement Id can not be empty.",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.Required"] = 
                "At least one zip code range has to be defined.",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.IsNotValid"] = 
                "The entered values must be greater than 0. The max value must be greater than the min value.",
            ["Plugins.DiscountRules.Mesut.DiscountManager.Fields.ZipCodeRanges.Between1000And9999"] =
                "Minimum and maximum values needs to be between 1000 and 9999.",

        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<DiscountManagerSettings>();

        //discount requirements
        var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
            .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountManagerConstants.SystemName);
        foreach (var discountRequirement in discountRequirements)
        {
            await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Mesut.DiscountManager");

        await base.UninstallAsync();
    }
    #endregion
}
