namespace TimeTracker.Services

open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open TimeTracker.Domain.TimeTrackerDomain
open TimeTracker.Domain.QueryModels

module TokenService =    

    let permissionToClaim (permissionName:PermissionName) =
        let permission = match permissionName with PermissionName permission -> permission
        Claim(ClaimTypes.Name, permission, ClaimValueTypes.String, "TimeTracker.api")

    let getClaimsForUser (userGroups:UserGroups) =
        userGroups |> List.collect(fun x -> x.UserGroupPermissions)
                   |> List.map(fun x -> x.PermissionName)
                   |> List.distinct
                   |> List.map(permissionToClaim)
    
    let getLoginToken (user:User) =
        let claims = getClaimsForUser user.UserGroups
        let tokenSettings = AppConfiguration.getTokenSettings
        let tokenString = JwtSecurityToken( 
                           issuer = "TimeTracker.api", 
                           audience = "TimeTrackerUsers", 
                           claims = claims, 
                           expires = tokenSettings.TokenExpiry, 
                           signingCredentials = tokenSettings.SigningCredentials)
        JwtSecurityTokenHandler().WriteToken tokenString |> Result.Ok

        
        

