module TimeTracker.Api.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open TimeTracker.Domain.QueryModels
open TimeTracker.Services.LoginApi
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Microsoft.IdentityModel.Tokens
open TimeTracker.Services.AppConfiguration

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "TimeTracker.Api" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "TimeTracker.Api" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------
type SimpleClaim = { Type: string; Value: string }

let authorize =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

let getType =
    fun() ->
    async {
        return "someType"
    }

let showClaims =
    fun (func: unit -> Async<string>) (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let claims = ctx.User.Claims 
            let! s = func()
            let simpleClaims = Seq.map (fun (i : Claim) -> {Type = s; Value = i.Value}) claims
            return! json simpleClaims next ctx
        }
        


//let resultToHttpResponseAsync asyncWorkflow : HttpHandler =
//    fun next ctx ->
//    task {
//        let! result = asyncWorkflow |> Async.StartAsTask
//        let responseFn =
//            match result with
//            | Ok ok -> json ok |> Successful.ok
//            | Error e -> json e
//        return! responseFn next ctx
//    }

let resultToHttpResponse re : HttpHandler =
    fun next ctx ->
    let responseFn =
        match re with
        | Ok ok -> json ok |> Successful.ok
        | Error e -> json e
    responseFn next ctx

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
    htmlView view

let indexHandlerAsync (name : string) =
    task {
        let greetings = sprintf "Hello %s, from Giraffe!" name
        let model     = { Text = greetings }
        let view      = Views.index model
        return htmlView view
        }

let bindJsonForRoute<'a> r f = routeCi r >=> bindJson<'a> f

//let handleLogin (login : UserLoginModel) =
//    async {
//        loginUser(login) |> Async.Start
//        return ""
//    }

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandler
                route "/Claims" >=> authorize >=> showClaims getType
            ]
        POST >=>
            choose [
                bindJsonForRoute "/Login" 
                    (fun loginModel -> loginUser(loginModel) |> resultToHttpResponse)  
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseAuthentication()
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let jwtBearerOptions (cfg : JwtBearerOptions) =
    let settings = getTokenSettings
    cfg.IncludeErrorDetails <- true
    cfg.TokenValidationParameters <- TokenValidationParameters (
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = settings.TokenIssuer,
        ValidAudience = settings.TokenAudience,
        IssuerSigningKey = settings.SigningCredentials.Key
    )

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Action<JwtBearerOptions> jwtBearerOptions) |> ignore    

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0