namespace TimeTracker.Domain

module TimeTrackerDomain =

    open TimeTracker.Common.CommonTypes
    open TimeTracker.Common.Errors
    
    type ProjectName = ProjectName of string
    type ProjectDecription = ProjectDecription of string
        
    type Project = {
        ProjectName: ProjectName
        ProjectDescription: ProjectDecription
    }

    type CategorySet =
    | Basic
    | Development

    type SubProjectName = SubProjectName of string

    type SubProject = {
        Project: Project
        SubProjectName: SubProjectName
        CategorySet: CategorySet
    }

    type CategoryName = CategoryName of string

    type Category = {
        CategoryName: CategoryName
        CategorySet: CategorySet
    }

    type PermissionName = PermissionName of string
    type PermissionDescription = PermissionDescription of string

    type Permission = {
        PermissionName: PermissionName
        PermissionDescription: PermissionDescription
    }

    type UserGroupPermissions = Permission List

    type UserGroupName = UserGroupName of string

    type UserGroup = {
        UserGroupName: UserGroupName
        UserGroupPermissions: UserGroupPermissions
    }

    type UserEmail = UserEmail of string
    type UserName = UserName of string
    type UserGroups = UserGroup List

    type User = {
        UserEmail: UserEmail
        UserName: UserName
        UserGroups: UserGroups
    }

    //type EntryYear = EntryYear of int
    //type EntryWeek = EntryWeek of int
    //type EntryTime = EntryTime of DateTime
    //type EntryDaysSpent = EntryDaysSpent of int

    //type Entry = {
    //    User: User
    //    SubProject: SubProject
    //    Category: Category
    //    EntryYear: EntryYear
    //    EntryWeek: EntryWeek
    //    EntryTime: EntryTime
    //    EntryDaysSpent: EntryDaysSpent
    //}
    
    //type WeekFromAndTo = WeekFromAndTo of string
    //type TotalDays = TotalDays of int

    //type Week = {
    //    Year: EntryYear
    //    Week: EntryWeek
    //    WeekFromAndTo: WeekFromAndTo
    //    TotalDays: TotalDays
    //}

    ////////////////Login

    let private isValidEmail (str:string) =
        str.ToLower().Contains "@blueprism.com"

    type Email private (value: string) =
        member x.Value = value
        static member Create(validString: String50) =
            if(isValidEmail validString.Value) then Email(validString.Value) |> Ok
            else ValidationError "Username must be a Blue Prism Email!" |> Error

    type UserLogin = {
        Email: Email
        Password: String50
    }
    
    type JwtSettings = {
        Key: string
        Issuer: string
        ExpiryDays: string
    }

    //"JwtKey": "n9evM0nP2Ey8Vule2uYx+g==",
    //"JwtIssuer": "http://localhost/",
    //"JwtExpiryDays": 0.02,
            

