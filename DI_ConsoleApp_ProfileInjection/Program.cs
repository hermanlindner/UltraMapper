using DI_ConsoleApp_ProfileInjection;
using DI_ConsoleApp_ProfileInjection.ClassesToMap;
using DI_ConsoleApp_ProfileInjection.ClassesToMap.Company;
using DI_ConsoleApp_ProfileInjection.ClassesToMap.User;
using DI_ConsoleApp_ProfileInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
// Create the service container
var builder = new ServiceCollection()
    .AddSingleton<MyApplication>()
    .AddSingletonUltraMapper( typeof( Program ).Assembly )
    .BuildServiceProvider();
var app = builder.GetRequiredService<MyApplication>();

ConvertedFrom from = new ConvertedFrom
{
    User = new FromUser { MyId = 45, Name = "Its me" },
    Company = new FromCompany { CompanyType = 2, CompanyName = "MyCompany" }
};
var mapped = app.ConvertEntry( from );

Console.WriteLine( $"UserName {mapped.User_MapTo.UserName}" );
Console.WriteLine( $"UserId {mapped.User_MapTo.Id}" );
Console.WriteLine( $"CompanyName {mapped.Company_MapTo.Name}" );
Console.WriteLine( $"CompanyType {mapped.Company_MapTo.CompanyType}" );
