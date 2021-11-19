namespace Criipto.NugetSample.Controllers

open System.Diagnostics
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http
open Criipto.NugetSample.Models
open System.Security.Claims

type HomeController (logger : ILogger<HomeController>) =
    inherit Controller()

    
    [<Authorize>]
    member this.Index () =
        let accountsList = Statements.generate "user" 100
        let claimsIdentity = this.User.Identity :?> ClaimsIdentity
        
        let valueOrElse claimsName =
            match claimsIdentity.FindFirst(fun c -> c.Type = claimsName) with
            null -> None
            | c -> Some c.Value
        let tryFindClaimsValue possibleClaimsName =
            possibleClaimsName
            |> Seq.map(valueOrElse)    
            |> Seq.tryFind Option.isSome
            |> Option.map Option.get
        let name = valueOrElse "name"
        let emailAddress = 
            [ClaimTypes.Email;"email"]
            |> tryFindClaimsValue
        let dateofbirth = 
            [ClaimTypes.DateOfBirth;"birthdate"]
            |> tryFindClaimsValue
        let profileImage = valueOrElse "picture"
        let usr = 
            {
                Name = 
                    match name with
                    None -> "Jane Doe"
                    | Some n -> n
                DateOfBirth = 
                    match dateofbirth with
                    None -> ""
                    | Some d -> d
                Email = emailAddress
                ImageUrl = profileImage
                Accounts = accountsList
            } : Models.User
        this.View(usr)

    [<Authorize>]
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
