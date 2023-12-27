using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Plugin.Mesut.DiscountManager.Services.Abstract;

namespace Nop.Plugin.Mesut.DiscountManager.Services;

public class DiscountRequirementService : IDiscountRequirementService
{
    #region Fiedls
    private readonly IRepository<DiscountRequirement> _discountRequirementRepository;
    #endregion

    #region Ctor
    public DiscountRequirementService(IRepository<DiscountRequirement> discountRequirementRepository)
    {
        _discountRequirementRepository = discountRequirementRepository;
    }
    #endregion

    #region Methods
    public async Task<DiscountRequirement> GetZipCodeRequirementByDiscountIdAsync(int discountId)
    {
        return await _discountRequirementRepository.
                        Table.
                        FirstOrDefaultAsync(
                            x => x.DiscountId == discountId &&
                            x.DiscountRequirementRuleSystemName == DiscountManagerConstants.SystemName);
    }
    #endregion
}
