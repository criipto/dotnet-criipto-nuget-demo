namespace Criipto.NugetSample

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            ).ConfigureAppConfiguration(fun hostingContext configBuilder ->
                let env = hostingContext.HostingEnvironment.EnvironmentName
                configBuilder
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile(sprintf "appsettings.%s.json" env, optional = true, reloadOnChange = true)
                    .AddUserSecrets<Startup>(true, reloadOnChange = true)
                    .AddEnvironmentVariables()
                    |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()

        exitCode
