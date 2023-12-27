using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Mesut.DiscountManager.Domains;
using Nop.Plugin.Mesut.DiscountManager.Models;
using Nop.Plugin.Mesut.DiscountManager.Services.Abstract;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Mesut.DiscountManager.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class DiscountManagerController : BasePluginController
{
    #region Fields

    private readonly IDiscountService _discountService;
    private readonly IPermissionService _permissionService;
    private readonly IDiscountZipCodeRangeService _discountZipCodeRangeService;
    private readonly IDiscountRequirementService _discountRequirementService;

    #endregion

    #region Ctor

    public DiscountManagerController(IDiscountService discountService,
        IPermissionService permissionService,
        IDiscountZipCodeRangeService discountZipCodeRangeService,
        IDiscountRequirementService discountRequirementService)
    {
        _discountService = discountService;
        _permissionService = permissionService;
        _discountZipCodeRangeService = discountZipCodeRangeService;
        _discountRequirementService = discountRequirementService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
            return Content("Access denied");

        //load the discount
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("Discount could not be loaded");

        var discountRequirement = discountRequirementId is null || discountRequirementId <= 0 ?
            await _discountRequirementService.GetZipCodeRequirementByDiscountIdAsync(discountId) :
            await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.Value);

        if (discountRequirement is null)
        {
            return View("~/Plugins/Mesut.DiscountManager/Views/Configure.cshtml",
                new ConfigurationModel
                {
                    DiscountRequirementId = 0,
                    DiscountId = discountId,
                    ZipCodeRanges = new ZipCodeRangeModel[] { new ZipCodeRangeModel { } }
                });
        }

        var ranges = await _discountZipCodeRangeService.GetByRequirementIdAsync(discountRequirement.Id);

        ranges ??= new List<DiscountZipCodeRange>();

        var model = new ConfigurationModel
        {
            DiscountRequirementId = discountRequirement.Id,
            DiscountId = discountId,
            ZipCodeRanges = ranges.Select(x => new ZipCodeRangeModel { Id = x.Id, MinValue = x.ZipCodeRangeMin, MaxValue = x.ZipCodeRangeMax }).ToArray(),
        };

        return View("~/Plugins/Mesut.DiscountManager/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
            return Content("Access denied");

        if (ModelState.IsValid)
        {
            //load the discount
            var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
            if (discount is null)
                return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

            //get the discount requirement
            var discountRequirement = await _discountRequirementService.GetZipCodeRequirementByDiscountIdAsync(model.DiscountId);

            //the discount requirement does not exist, so create a new one
            if (discountRequirement is null)
            {
                discountRequirement = new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    DiscountRequirementRuleSystemName = DiscountManagerConstants.SystemName
                };

                await _discountService.InsertDiscountRequirementAsync(discountRequirement);
            }

            if (model.ZipCodeRanges is null)
                return Ok(new { Errors = new[] { "Zip code ranges needs to be added" } });

            await HandleRanges(discountRequirement.Id, model.ZipCodeRanges);

            return Ok(new { NewRequirementId = discountRequirement.Id });
        }

        return Ok(new
        {
            Errors = ModelState.Values.
                                    SelectMany(x =>
                                    x.Errors
                                    .Select(e => e.ErrorMessage))
        });
    }

    #endregion

    #region Utilities
    public async Task HandleRanges(int discountRequirementId, IEnumerable<ZipCodeRangeModel> models)
    {
        var ranges = await _discountZipCodeRangeService.GetByRequirementIdAsync(discountRequirementId);

        if (ranges is null)
        {
            await _discountZipCodeRangeService.AddAsync(
                    models.Select(range =>
                    new DiscountZipCodeRange
                    {
                        DiscountRequirementId = discountRequirementId,
                        ZipCodeRangeMin = range.MinValue,
                        ZipCodeRangeMax = range.MaxValue
                    }));

        }

        // ranges needs to be updated
        var rangesUpdatedToBe = models.Where(x => x.Id > 0);
        // ranges to be deleted
        var rangesToBeDeleted = ranges.Where(x => !rangesUpdatedToBe.Select(range => range.Id).Contains(x.Id));
        // new ranges 
        var newRanges = models.Where(x => x.Id == 0);

        //Update existing ranges
        foreach (var rangeModel in rangesUpdatedToBe)
        {
            var range = ranges.First(x => x.Id == rangeModel.Id);
            range.ZipCodeRangeMin = rangeModel.MinValue;
            range.ZipCodeRangeMax = rangeModel.MaxValue;
        }

        if (rangesToBeDeleted.Any())
            await _discountZipCodeRangeService.DeleteAsync(rangesToBeDeleted);

        if (newRanges.Any())
            await _discountZipCodeRangeService.AddAsync(newRanges.Select(range =>
                    new DiscountZipCodeRange
                    {
                        DiscountRequirementId = discountRequirementId,
                        ZipCodeRangeMin = range.MinValue,
                        ZipCodeRangeMax = range.MaxValue
                    }));
    }
    #endregion
}
