using UltraMapper;
using UltraMapper.ConfigurationProfiles;

namespace DI_ConsoleApp_ProfileInjection.ClassesToMap
{
    public class MainMappingProfile : IMappingProfile
    {
        public void Configure( Configuration config )
        {
            config.MapTypes<ConvertedFrom, ConvertedTo>()
                .MapMember( x => x.User, y => y.User_MapTo )
                .MapMember( x => x.Company, y => y.Company_MapTo );
        }
    }
}
