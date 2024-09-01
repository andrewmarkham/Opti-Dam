using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OptiDAM.Services;
using OptiDAM.Tags;
using OptiDAM.Graph;

namespace OptiDAM.Initilization
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJhooseOptiDamExtensions(this IServiceCollection services,
                IConfiguration configuration
                )
        {
            services.Configure<ImageDimensionOption>(configuration.GetSection(ImageDimensionOption.ImageDimensions));
            services.Configure<OptiDamGraphOption>(configuration.GetSection(OptiDamGraphOption.OptiDamGraph));

            services.AddSingleton<IOptiDamService, OptiDamService>();
            services.AddSingleton<IOptiDamFolderManager, OptiDamFolderManager>();
            services.AddSingleton<IOptiDamAuthService, OptiDamAuthService>();
            services.AddSingleton<IOptiDamGraphService, OptiDamGraphService>();
            services.AddSingleton<IImageHelper, ImageHelper>();
            
            services.AddHttpClient<IOptiDamService, OptiDamService>(client =>
            {
                client.BaseAddress = new Uri("https://api.cmp.optimizely.com/v3/");
            });
            services.AddHttpClient<IOptiDamAuthService, OptiDamAuthService>( client =>
            {
                client.BaseAddress = new Uri("https://accounts.newscred.com/");
            });

            return services;
        }
    }
}
