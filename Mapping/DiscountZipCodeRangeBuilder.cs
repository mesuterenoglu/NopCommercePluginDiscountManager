using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Mesut.DiscountManager.Domains;
using Nop.Data.Extensions;

namespace Nop.Plugin.Mesut.DiscountManager.Mapping;

public class DiscountZipCodeRangeBuilder : NopEntityBuilder<DiscountZipCodeRange>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DiscountZipCodeRange.DiscountRequirementId))
            .AsInt32()
            .ForeignKey<DiscountRequirement>();
    }
}
