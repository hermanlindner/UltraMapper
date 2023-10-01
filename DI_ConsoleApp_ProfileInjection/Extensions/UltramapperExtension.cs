using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UltraMapper.ConfigurationProfiles;

namespace DI_ConsoleApp_ProfileInjection.Extensions
{
    public static class UltramapperExtension
    {
        public static IServiceCollection AddSingletonUltraMapper( this IServiceCollection services, params Assembly[] assemblies )
        {
            return RegisterMappingProfiles( services, assemblies ).AddSingletonUltraMapper();
        }

        public static IServiceCollection AddSingletonUltraMapper( this IServiceCollection services )
        {
            return services.AddSingleton( sp =>
            {
                var profiles = sp.GetServices<IMappingProfile>();
                return MappingConfigExtensions.CreateProfileBasedMapper( profiles );
            } );
        }

        private static IServiceCollection RegisterMappingProfiles( IServiceCollection services, Assembly[] assemblies )
        {
            var interfaceType = typeof( IMappingProfile );
            var types = assemblies
                .SelectMany( s => s.GetTypes() )
                .Where( p => interfaceType.IsAssignableFrom( p ) );
            foreach( var type in types )
            {
                services = services.AddTransient( interfaceType, type );
            }
            return services;
        }
    }
}
