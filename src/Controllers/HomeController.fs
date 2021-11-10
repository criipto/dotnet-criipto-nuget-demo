namespace Criipto.NugetSample.Controllers

open System.Diagnostics
open Microsoft.AspNetCore.Authorization

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open Criipto.NugetSample.Models

type HomeController (logger : ILogger<HomeController>) =
    inherit Controller()

    [<Authorize>]
    member this.Index () =
        this.View()

    member this.Privacy () =
        this.View()

    member this.Callback() =
        RedirectToActionResult("Index", "ControllerName",obj())

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })
