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
    }

    let private getTokenExpiry(config: IConfigurationRoot) =
        let tokenExpiry = config.Item("TokenExpiryDays")
        let ti = DateTime.Now.AddDays(Double.Parse(tokenExpiry))
        new Nullable<DateTime>(ti)

    let private getSecretTokenKey(config: IConfigurationRoot) =
        config.Item("TokenKey")

    let private getSigningCredentials(config: IConfigurationRoot) =
        let tokenKey = config |> getSecretTokenKey
        let key = SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
        SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)

    let getTokenSettings =
        let configuration = buildConfig()
        { SigningCredentials = getSigningCredentials(configuration);
          TokenExpiry = getTokenExpiry(configuration)}


