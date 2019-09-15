namespace TimeTracker.Services

open Microsoft.IdentityModel.Tokens
open System
open System.Text

module AppConfiguration =
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions
    open System.IO      
    open TimeTracker.Data.StoreConfiguration

    let private buildConfig() =
        Configuration.ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build()

    let private getMySqlRepositorySettings(config: IConfigurationRoot) =
        { ConnectionString = config.GetConnectionString("Timetracker") }

    let GetTimeTrackerDatabase =
        buildConfig() |> getMySqlRepositorySettings |> getTimeTrackerContext

    type TokenSettings = {
        SigningCredentials: SigningCredentials
        TokenExpiry: Nullable<DateTime>
        TokenIssuer: string
        TokenAudience: string
    }

    let private getTokenAppSettings(config:IConfigurationRoot) =
        config.Item("TokenKey"), 
        config.Item("TokenExpiryDays"),
        config.Item("TokenIssuer"),
        config.Item("TokenAudience")

    let private getTokenExpiry(expiryDays:string) =
        let ti = DateTime.Now.AddDays(Double.Parse(expiryDays))
        new Nullable<DateTime>(ti)

    let private getSigningCredentials(tokenKey:string) =
        let key = SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
        SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)

    let getTokenSettings =
        let configuration = buildConfig()
        let (key, expiryDays, issuer, audience) = getTokenAppSettings(configuration)
        { SigningCredentials = getSigningCredentials(key);
          TokenExpiry = getTokenExpiry(expiryDays);
          TokenIssuer = issuer;
          TokenAudience = audience }


