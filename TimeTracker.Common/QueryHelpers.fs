namespace TimeTracker.Common

module QueryHelpers =
    open TimeTracker.Common.Errors
    open TimeTracker.Common.CommonTypes
    
    let PerformQuery (query: Queryable<_>) enumerator =
        try
            query |> enumerator |> Ok
        with
        | Failure exceptionMessage -> DatabaseError exceptionMessage |> Error

