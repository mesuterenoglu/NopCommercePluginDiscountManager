using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Mesut.DiscountManager.Domains;

namespace Nop.Plugin.Mesut.DiscountManager.Migrations;

[NopMigration("2023/12/25 12:00:00", "Nop.Plugin.Mesut.DiscountManager schema", MigrationProcessType.Installation)]
public class DiscountManagerSchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<DiscountZipCodeRange>();
    }
}
