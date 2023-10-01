using DI_ConsoleApp_ProfileInjection.ClassesToMap.Company;
using DI_ConsoleApp_ProfileInjection.ClassesToMap.User;

namespace DI_ConsoleApp_ProfileInjection.ClassesToMap
{
    public class ConvertedTo
    {
        public ToUser User_MapTo { get; set; }   
        public ToCompany Company_MapTo { get; set; } 
    }
}
