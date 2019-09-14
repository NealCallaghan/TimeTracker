namespace TimeTracker.Domain


open TimeTrackerDomain
open QueryModels
open TimeTracker.Common.Errors
open TimeTracker.Common.CommonTypes

module Workflow =
    
    
    type ValidateUserLoginModel = UserLoginModel           -> Result<UserLogin, Error>
    type ValidateExistingUser   = Result<UserLogin, Error> -> Result<User, Error>
    type ValidateToken          = Result<User, Error>      -> Result<string, Error>

    let private (|ModelIsValid|EmailError|PasswordError|BothInError|) input =
        match input with
        | (Ok email, Ok password) -> ModelIsValid (email, password)
        | (Error error, Ok _)  -> EmailError error
        | (Ok _, Error error) -> PasswordError error
        | (Error emailError, Error passwordError) -> BothInError (emailError, passwordError)

    let toUserLogin (userLoginModel:UserLoginModel) =
        let emailResult = String50.Create userLoginModel.UserName |> Result.bind(fun x -> Email.Create(x))
        let passwordResult = String50.Create userLoginModel.Password
        match emailResult, passwordResult with 
        | ModelIsValid (email, password) -> { Email = email ; Password = password } |> Ok
        | EmailError error  -> error |> Error
        | PasswordError error -> error |> Error
        | BothInError (emailError, passwordError) -> ValidationError "Check User email and password" |> Error  

    let ValidateUserLoginModel: ValidateUserLoginModel =
        (fun x -> x |> toUserLogin)

