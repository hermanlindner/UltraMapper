using UltraMapper;
using UltraMapper.ConfigurationProfiles;

namespace DI_ConsoleApp_ProfileInjection.ClassesToMap.User
{
    public class UserMappingProfile : IMappingProfile
    {
        public void Configure( Configuration config )
        {
            config.MapTypes<FromUser, ToUser>()
                .MapMember( x => x.Name, y => y.UserName )
                .MapMember( x => x.MyId, y => y.Id, s => s.ToString() );
        }
    }
}
