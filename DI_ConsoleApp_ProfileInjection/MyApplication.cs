using DI_ConsoleApp_ProfileInjection.ClassesToMap;
using UltraMapper;

namespace DI_ConsoleApp_ProfileInjection
{
    public class MyApplication
    {
        private readonly Mapper _ultramapper;

        public MyApplication(Mapper ultramapper )
        {
            _ultramapper = ultramapper;
        }

        public ConvertedTo ConvertEntry(ConvertedFrom before )
        {
            return _ultramapper.Map<ConvertedTo>( before );
        }
    }
}
