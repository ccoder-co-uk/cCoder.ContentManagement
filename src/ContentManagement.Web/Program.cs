// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace ContentManagement.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(
                args: args);

        builder.Services.AddContentManagementApplication(
            builder: builder);

        WebApplication app = builder.Build();

        app.UseContentManagementApplication()
            .Run();
    }
}