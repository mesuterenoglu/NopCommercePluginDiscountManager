using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Mesut.DiscountManager.Services;
using Nop.Plugin.Mesut.DiscountManager.Services.Abstract;

namespace Nop.Plugin.Mesut.DiscountManager.Infrastracture
{
    public class NopStartup : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDiscountZipCodeRangeService, DiscountZipCodeRangeService>();
            services.AddScoped<IDiscountRequirementService, DiscountRequirementService>();
        }

        public int Order => 3000;
    }
}
