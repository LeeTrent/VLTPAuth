using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VLTPAuth
{
    public class Program
    {
        // public static void Main(string[] args)
        // {
        //     CreateWebHostBuilder(args).Build().Run();
        // }

        // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //     WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>();

      public static void Main(string[] args)
        {
            Console.WriteLine("[Program][Main] Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory() );
            CreateWebHostBuilder(args).Build().Run();
        }

        // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //   WebHost.CreateDefaultBuilder(args)
        //       .ConfigureAppConfiguration((hostingContext, config) =>
        //       {
        //           config.SetBasePath("/Users/leetrent/temp");
        //           config.AddIniFile("config.ini", optional: true, reloadOnChange: true);
        //       })
        //       .UseStartup<Startup>();
    // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    //     WebHost.CreateDefaultBuilder(args)
    //         .ConfigureAppConfiguration((hostingContext, config) =>
    //         {
    //             config.SetBasePath("/Users/leetrent/temp");
    //             config.AddJsonFile("VLTPAuth.json", optional: false, reloadOnChange: true);
    //         })
    //         .UseStartup<Startup>();
    // }

    // export APPSETTINGS_DIRECTORY="/Users/leetrent/temp"
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Environment.GetEnvironmentVariable("APPSETTINGS_DIRECTORY"));
                config.AddJsonFile("VLTPAuth_appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>();
    }
    
}