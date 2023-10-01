using UltraMapper;
using UltraMapper.ConfigurationProfiles;

namespace DI_ConsoleApp_ProfileInjection.ClassesToMap.Company
{
    public class CompanyMappingProfile : IMappingProfile
    {
        public void Configure( Configuration config )
        {
            config.MapTypes<FromCompany, ToCompany>()
                .MapMember( x => x.CompanyName, y => y.Name )
                .MapMember( x => x.CompanyType, y => y.CompanyType, x => MapCompanyType( x ) );
        }

        public string MapCompanyType( int type )
        {
            switch( type )
            {
                case 1: return "Right company type";
                case 2: return "My other type";
            }
            return "Unknown type";
        }

    }
}
