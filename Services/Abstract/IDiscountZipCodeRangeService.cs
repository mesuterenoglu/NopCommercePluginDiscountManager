using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Mesut.DiscountManager.Domains;

namespace Nop.Plugin.Mesut.DiscountManager.Services.Abstract;

public interface IDiscountZipCodeRangeService
{
    Task AddAsync(DiscountZipCodeRange discountZipCodeRange);
    Task AddAsync(IEnumerable<DiscountZipCodeRange> discountZipCodeRanges);
    Task DeleteAsync(DiscountZipCodeRange discountZipCodeRange);
    Task DeleteAsync(IEnumerable<DiscountZipCodeRange> discountZipCodeRanges);
    Task<DiscountZipCodeRange> GetByIdAsync(int discountZipCodeRangeId);
    Task<IEnumerable<DiscountZipCodeRange>> GetByRequirementIdAsync(int discountRequirementId);
    Task UpdateAsync(DiscountZipCodeRange discountZipCodeRange);
}
