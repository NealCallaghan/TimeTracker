namespace TimeTracker.Services

open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open TimeTracker.Domain.TimeTrackerDomain

module TokenService =    
    
    let userEmailClaim (userEmail:UserEmail) =
        let email = match userEmail with UserEmail email -> email
        [Claim(JwtRegisteredClaimNames.Email, email)]

    let permissionToClaim (issuer:string) (permissionName:PermissionName) =
        let permission = match permissionName with PermissionName permission -> permission
        Claim(ClaimTypes.Name, permission, ClaimValueTypes.String, issuer)

    let getClaimsForUser (userGroups:UserGroups) permissionToClaimFunc =
        userGroups |> List.collect(fun x -> x.UserGroupPermissions)
                   |> List.map(fun x -> x.PermissionName)
                   |> List.distinct
                   |> List.map(permissionToClaimFunc)
    
    let getLoginToken (user:User) =
        let tokenSettings = AppConfiguration.getTokenSettings
        let permToClaimFunc = permissionToClaim tokenSettings.TokenIssuer
        let claims = List.concat([getClaimsForUser user.UserGroups permToClaimFunc;
                                  userEmailClaim user.UserEmail])
        let tokenSettings = AppConfiguration.getTokenSettings
        let tokenString = JwtSecurityToken( 
                           issuer = tokenSettings.TokenIssuer, 
                           audience = tokenSettings.TokenAudience, 
                           claims = claims,
                           expires = tokenSettings.TokenExpiry, 
                           signingCredentials = tokenSettings.SigningCredentials)
        JwtSecurityTokenHandler().WriteToken tokenString |> Result.Ok

        
        

