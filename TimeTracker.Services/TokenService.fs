namespace TimeTracker.Services

open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open TimeTracker.Domain.TimeTrackerDomain

module TokenService =    
    
    let userEmailClaim (userEmail:UserEmail) =
        let email = match userEmail with UserEmail email -> email
        [Claim(JwtRegisteredClaimNames.Email, email)]

    let permissionToClaim (permissionName:PermissionName) =
        let permission = match permissionName with PermissionName permission -> permission
        Claim(ClaimTypes.Name, permission, ClaimValueTypes.String, "TimeTracker.api")

    let getClaimsForUser (userGroups:UserGroups) =
        userGroups |> List.collect(fun x -> x.UserGroupPermissions)
                   |> List.map(fun x -> x.PermissionName)
                   |> List.distinct
                   |> List.map(permissionToClaim)
    
    let getLoginToken (user:User) =
        let claims = List.concat([getClaimsForUser user.UserGroups;userEmailClaim user.UserEmail])
        let tokenSettings = AppConfiguration.getTokenSettings
        let tokenString = JwtSecurityToken( 
                           issuer = "TimeTracker.api", 
                           audience = "TimeTrackerUsers", 
                           claims = claims,
                           expires = tokenSettings.TokenExpiry, 
                           signingCredentials = tokenSettings.SigningCredentials)
        JwtSecurityTokenHandler().WriteToken tokenString |> Result.Ok

        
        

