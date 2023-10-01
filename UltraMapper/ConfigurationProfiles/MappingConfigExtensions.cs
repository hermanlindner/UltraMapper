using System;
using System.Collections.Generic;

namespace UltraMapper.ConfigurationProfiles
{
    public static class MappingConfigExtensions
    {
        public static Mapper CreateProfileBasedMapper(IEnumerable<IMappingProfile> profiles)
        {
            Action<Configuration> action = ( config ) => {
                foreach ( var profile in profiles )
                {
                    profile.Configure(config);
                }
            };
            var mapper = new Mapper( action );
            return mapper;
        } 
    }
}
