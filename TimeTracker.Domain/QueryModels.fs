namespace TimeTracker.Domain

(*
This module contains mappings of our domain types to something that user/client will see.
Since JSON and a lot of popular languages now do not support Discriminated Unions, which
we heavily use in our domain, we have to convert our domain types to something represented
by common types.
*)
module QueryModels =        
    
    [<CLIMutable>]
    type public UserLoginModel = {
        UserName: string
        Password: string
    }

