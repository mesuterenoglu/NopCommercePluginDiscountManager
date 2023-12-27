using System.Threading.Tasks;
using Nop.Core.Domain.Discounts;

namespace Nop.Plugin.Mesut.DiscountManager.Services.Abstract;

public interface IDiscountRequirementService
{
    Task<DiscountRequirement> GetZipCodeRequirementByDiscountIdAsync(int discountId);
}
