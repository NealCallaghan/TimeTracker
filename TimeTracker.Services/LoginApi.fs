namespace TimeTracker.Services

open TimeTracker.Data
open TimeTracker.Domain.Workflow

module LoginApi =

    let private GetLoggedInUser userLogin = 
        QueryRepository.GetLoggedInUser AppConfiguration.GetTimeTrackerDatabase userLogin

    let private GetUserWithGroups userLogin =
        QueryRepository.getUserWithGroups AppConfiguration.GetTimeTrackerDatabase userLogin

    let private ValidateExistingUser: ValidateExistingUser =
        fun userLoginResult -> 
            userLoginResult 
            |> Result.bind GetLoggedInUser
            |> Result.bind GetUserWithGroups

    let private GetTokenString: ValidateToken =
        fun userResult ->
            userResult
            |> Result.bind TokenService.getLoginToken

    let loginUser =
        ValidateUserLoginModel >> ValidateExistingUser >> GetTokenString

        
