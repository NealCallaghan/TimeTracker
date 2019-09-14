namespace TimeTracker.Data

module QueryRepository =
    
    open TimeTracker.Data.StoreConfiguration
    open TimeTracker.Common
    open TimeTracker.Common.Errors
    open TimeTracker.Domain.TimeTrackerDomain

    type LoggedInUser = {
        Email: Email
        UserName: UserName
    }
    
    type IsExistingUser = TimeTrackerContext -> UserLogin -> Result<LoggedInUser, Error>

    let GetLoggedInUser (dataBase: TimeTrackerContext) (userLogin:UserLogin) =
        let userEmail = userLogin.Email.Value
        let query = query {
            for users in dataBase.User do
            where(users.Email = userEmail)
            select(users.Email, users.Name, users.Hash)
        }
        let userHash = Hashing.GeneratePasswordHash userLogin.Password.Value
        let userDetails = QueryHelpers.PerformQuery query (Seq.toList >> List.tryExactlyOne)
        userDetails
        |> Result.bind 
            (fun emailHash 
                -> match emailHash with
                    | Some (_, userName, hash) when hash = userHash -> { Email = userLogin.Email; UserName = UserName(userName) } |> Ok
                    | Some (_, _, _) -> ValidationError "Incorrect Password" |> Error
                    | None -> ValidationError "Incorrect Email" |> Error)
        
    
    let private getPermission (perms) = //: string * string * string
        let (_, permissionName, permissionDescription) = perms
        { PermissionName = PermissionName(permissionName); 
          PermissionDescription = PermissionDescription(permissionDescription) }

    let private getUserGroups (userGroupsData) = //: list<string * seq<string * string * string>>
        userGroupsData 
        |> List.map(
            fun (groupNameString, rawPermssions) -> 
                let groupName = UserGroupName(groupNameString)
                let userGroupPermissions = rawPermssions |> Seq.map(getPermission) |> Seq.toList
                { UserGroupName = groupName; UserGroupPermissions = userGroupPermissions })

    let getUserGroupsForUser (database:TimeTrackerContext) (loggedInUser:LoggedInUser) =
        let query = 
                query {
            for user in database.User do
            join userGroupAssignment in database.Usergroupassignment 
                on (user.Id = userGroupAssignment.Userid)
            join userGroup in database.Usergroup 
                on (userGroupAssignment.Usergroupid = userGroup.Id)
            join userGroupPermission in database.Usergroupperm 
                on (userGroup.Id = userGroupPermission.Usergroupid)
            join permission in database.Perm 
                on (userGroupPermission.Permid = permission.Id)

            where(user.Email = loggedInUser.Email.Value)
            select(userGroup.Name, permission.Name, permission.Description)
        }
        QueryHelpers.PerformQuery query 
                    (Seq.groupBy(fun (groupName, _, _) -> groupName) 
                    >> Seq.toList 
                    >> getUserGroups)

    let getUserWithGroups (database:TimeTrackerContext) (loggedInUser:LoggedInUser) =
        let userGroups = getUserGroupsForUser database loggedInUser
        match userGroups with
        | Ok userGroup -> { UserEmail = UserEmail(loggedInUser.Email.Value); 
                            UserName = loggedInUser.UserName; 
                            UserGroups = userGroup } |> Ok
        | Error e -> e |> Error

       

    

