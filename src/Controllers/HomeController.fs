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

    member private this.ClaimsIdentity with get() = this.User.Identity :?> ClaimsIdentity
    member private this.ValueOrElse claimsName =
        match this.ClaimsIdentity.FindFirst(fun c -> c.Type = claimsName) with
        null -> None
        | c -> Some c.Value
    member private this.TryFindClaimsValue possibleClaimsName =
        possibleClaimsName
        |> Seq.map(this.ValueOrElse)    
        |> Seq.tryFind Option.isSome
        |> Option.map Option.get

    [<Authorize>]
    member this.Index () =
        let name = 
            match this.ValueOrElse "name" with
            None -> "Jane Doe"
            | Some n -> n
        let accountsList = Statements.generate name 100
        let emailAddress = 
            [ClaimTypes.Email;"email"]
            |> this.TryFindClaimsValue
        let dateofbirth = 
            [ClaimTypes.DateOfBirth;"birthdate"]
            |> this.TryFindClaimsValue
        let profileImage = this.ValueOrElse "picture"
        let usr = 
            {
                Name = name
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

    [<Authorize>]
    member this.List(id) =
        let name = 
            match this.ValueOrElse "name" with
            None -> "Jane Doe"
            | Some n -> n
        let accountsList = Statements.generate name 100
        let account = 
            match accountsList |> List.tryFind(fun a -> a.Name = id) with
            Some a -> a
            | None -> 
                printfn "%s not found in %A" id (accountsList |> List.map(fun a -> a.Name))
                failwith "Account not found"
        this.View(account)
    member _.Callback() =
        RedirectToActionResult("Index", "ControllerName",obj())

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })
