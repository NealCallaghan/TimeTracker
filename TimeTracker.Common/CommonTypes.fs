namespace TimeTracker.Common



module CommonTypes =
    open System.Linq
    open Errors

    [<CLIMutable>]
    type public UserLoginModel = {
        UserName: string
        Password: string
    }

    type String50 private (value:string) =
        member x.Value = value
        static member Create(value: string) =
            match value with
            | (""|null) -> ValidationError "string must not be null or empty" |> Error
            | s when s.Length > 50 -> ValidationError "string must be under 50 characters" |> Error
            | s -> String50 s |> Ok

    
    type Queryable<'T> = IQueryable<'T>