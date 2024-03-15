using Azure.Core;
using Azure.Identity;
using AzureDigitalTwinsPlatformAPIs.Interfaces;
using AzureDigitalTwinsPlatformAPIs.Services;

namespace AzureDigitalTwinsPlatformAPIs
{
    public static class HostModule
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISpaceDigitalTwinService, SpaceDigitalTwinService>();
            services.AddSingleton<ISensorDigitalTwinService, SensorDigitalTwinService>();
            return services;
        }

        public static IServiceCollection ConfigureTokenCredential(this IServiceCollection services)
        {
            services.AddSingleton<TokenCredential>(ConfigureChainedTokenCredential);
            return services;
        }

        private static ChainedTokenCredential ConfigureChainedTokenCredential(IServiceProvider provider)
        {
            var managedIdentityCredential = new ManagedIdentityCredential();
            var azureCliCredential = new AzureCliCredential();
            var interactiveBrowserCredential = new InteractiveBrowserCredential();

            var chainedTokenCredential = new ChainedTokenCredential(managedIdentityCredential, azureCliCredential, interactiveBrowserCredential);
            return chainedTokenCredential;
        }
    }
}