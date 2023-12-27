using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Mesut.DiscountManager.Domains;
using Nop.Plugin.Mesut.DiscountManager.Services.Abstract;

namespace Nop.Plugin.Mesut.DiscountManager.Services;

public class DiscountZipCodeRangeService : IDiscountZipCodeRangeService
{
    #region Fields
    private readonly IRepository<DiscountZipCodeRange> _repository;
    private readonly IStaticCacheManager _staticCacheManager;
    #endregion

    #region Ctor
    public DiscountZipCodeRangeService(IRepository<DiscountZipCodeRange> repository, IStaticCacheManager staticCacheManager)
    {
        _repository = repository;
        _staticCacheManager = staticCacheManager;
    }
    #endregion

    #region Methods

    public async Task AddAsync(DiscountZipCodeRange discountZipCodeRange)
    {
        if (discountZipCodeRange == null)
            throw new ArgumentNullException(nameof(discountZipCodeRange));

        await _repository.InsertAsync(discountZipCodeRange);
        await _staticCacheManager.RemoveByPrefixAsync(DiscountManagerConstants.PrefixCacheKey);
    }

    public async Task AddAsync(IEnumerable<DiscountZipCodeRange> discountZipCodeRanges)
    {
        if (discountZipCodeRanges == null)
            throw new ArgumentNullException(nameof(discountZipCodeRanges));

        await _repository.InsertAsync(discountZipCodeRanges.ToList());
        await _staticCacheManager.RemoveByPrefixAsync(DiscountManagerConstants.PrefixCacheKey);
    }

    public async Task DeleteAsync(DiscountZipCodeRange discountZipCodeRange)
    {
        if (discountZipCodeRange == null)
            throw new ArgumentNullException(nameof(discountZipCodeRange));

        await _repository.DeleteAsync(discountZipCodeRange);
        await _staticCacheManager.RemoveByPrefixAsync(DiscountManagerConstants.PrefixCacheKey);
    }

    public async Task DeleteAsync(IEnumerable<DiscountZipCodeRange> discountZipCodeRanges)
    {
        if (discountZipCodeRanges == null)
            throw new ArgumentNullException(nameof(discountZipCodeRanges));

        await _repository.DeleteAsync(discountZipCodeRanges.ToList());
        await _staticCacheManager.RemoveByPrefixAsync(DiscountManagerConstants.PrefixCacheKey);
    }

    public async Task<DiscountZipCodeRange> GetByIdAsync(int discountZipCodeRangeId)
    {
        if (discountZipCodeRangeId == 0)
            return null;

        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(DiscountManagerConstants.CacheKey, discountZipCodeRangeId), async () =>
            await _repository.GetByIdAsync(discountZipCodeRangeId));
    }

    public async Task<IEnumerable<DiscountZipCodeRange>> GetByRequirementIdAsync(int discountRequirementId)
    {
        if (discountRequirementId == 0)
            return null;

        return await _repository.Table.Where(range => range.DiscountRequirementId == discountRequirementId).ToListAsync();
    }

    public async Task UpdateAsync(DiscountZipCodeRange discountZipCodeRange)
    {
        if (discountZipCodeRange == null)
            throw new ArgumentNullException(nameof(discountZipCodeRange));

        await _repository.UpdateAsync(discountZipCodeRange);
        await _staticCacheManager.RemoveByPrefixAsync(DiscountManagerConstants.PrefixCacheKey);
    }
    #endregion
}
