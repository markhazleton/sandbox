using GarageSpark.Services.Mapping;

namespace GarageSpark.Extensions
{
    /// <summary>
    /// Dependency injection extensions for mapping services
    /// </summary>
    public static class MappingServiceExtensions
    {
        /// <summary>
        /// Registers mapping services with the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMappingServices(this IServiceCollection services)
        {
            services.AddScoped<IMappingService, MappingService>();

            return services;
        }
    }
}
