namespace Criipto.NugetSample

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Criipto.Configuration.AuthenticationBuilderExtensions

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration
        
        

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllersWithViews().AddRazorRuntimeCompilation() |> ignore
        services.AddRazorPages() |> ignore
        let authbuilder = 
            services.AddAuthorization()
                    .Configure<CookiePolicyOptions>(fun (options : CookiePolicyOptions) -> 
                        options.CheckConsentNeeded <- fun _ -> true
                        options.MinimumSameSitePolicy <- SameSiteMode.None
                    ).AddAuthentication(fun options ->
                        options.DefaultScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                        options.DefaultChallengeScheme <- OpenIdConnectDefaults.AuthenticationScheme
                    ).AddCookie()
        authbuilder.AddOpenIdConnect(this.Configuration, fun options ->
            let validationParameters = Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            validationParameters.NameClaimType <- "name"
            options.TokenValidationParameters <- validationParameters
        ) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member __.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts() |> ignore
        app.UseHttpsRedirection()
           .UseStaticFiles()
           .UseCookiePolicy()
           .UseRouting()
           .UseAuthentication()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllerRoute(
                    name = "default",
                    pattern = "{controller=Home}/{action=Index}/{id?}") |> ignore
                endpoints.MapRazorPages() |> ignore) |> ignore

    member val Configuration : IConfiguration = null with get, set
