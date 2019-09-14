open System
open System.Linq

//#I @"C:\Dev\TimeTracker\src\TimeTracker.Data\bin"
//#r @"Google.Protobuf.dll"
//#r @"MySql.Data.dll"
//#r @"FSharp.Data.SqlProvider.dll"

#I @"C:\Users\Neal Callaghan\.nuget\packages"
#r @"fsharp.data\3.1.1\lib\netstandard2.0\FSharp.Data.dll"
#r @"mysql.data\8.0.11\lib\netstandard2.0\MySql.Data.dll"
#r @"sqlprovider\1.1.66\lib\netstandard2.0\FSharp.Data.SqlProvider.dll"
#r @"google.protobuf\3.5.1\lib\netstandard1.0\Google.Protobuf.dll"


open FSharp.Data.Sql

[<Literal>]
let connString = @"Server=localhost;Port=3306;Database=timetracker;User=root;Password=password;Auto Enlist=false; Convert Zero Datetime=true;"

[<Literal>]
let dbVendor = Common.DatabaseProviderTypes.MYSQL

[<Literal>]
let resPath = __SOURCE_DIRECTORY__ + "\libraries" //required only at compile time

[<Literal>]
let indivAmount = 1000

[<Literal>]
let useOptTypes = true

type sql = SqlDataProvider< ConnectionString = connString, DatabaseVendor = dbVendor, ResolutionPath = resPath, IndividualsAmount = 1000, UseOptionTypes = true>

let ctx = sql.GetDataContext()

//let q = 
//    query {
//        for user in ctx.Timetracker.User do
//        //exists (user.Email = "neal.callaghan@blueprism.com")
//        //where(user.Email = "neal.callaghan@blueprism.com")
//        select(user.Email) //user.Hash
//    }
//    |> Seq.toList
let q = 
    query {
        for user in ctx.Timetracker.User do
        join userGroupAssignment in ctx.Timetracker.Usergroupassignment 
            on (user.Id = userGroupAssignment.Userid)
        join userGroup in ctx.Timetracker.Usergroup 
            on (userGroupAssignment.Usergroupid = userGroup.Id)
        join userGroupPermission in ctx.Timetracker.Usergroupperm 
            on (userGroup.Id = userGroupPermission.Usergroupid)
        join permission in ctx.Timetracker.Perm 
            on (userGroupPermission.Permid = permission.Id)
        //exists (user.Email = "neal.callaghan@blueprism.com")
        
        where(user.Email = "david.sutcliffe@blueprism.com")
        
        select(userGroup.Name, permission.Name, permission.Description)
    }
    |> Seq.toList |> Seq.groupBy(fun (x, _, _)  -> x) |> Seq.toList


//select 
//	u.name
//   ,ug.name
//   ,p.name
//   ,p.fullname
//   ,p.description
//from timetracker.user u 
//join timetracker.usergroupassignment ua on ua.userid = u.id
//join timetracker.usergroup ug on ug.id = ua.usergroupid 
//join timetracker.usergroupperm up on up.usergroupid = ug.id
//join timetracker.perm p on p.id = up.permid


let doStuff (qu:IQueryable<'T>) =
    qu |> Seq.toList

let myStuff = doStuff q    

let userDetails = 
    query {
        for users in ctx.Timetracker.User do
        where(users.Email = "neal.callaghan@blueprism.co")
        select(users.Email, users.Hash)
    }
    |> Seq.toList |> List.tryExactlyOne

//let users = 
//    ctx.Timetracker.User.Individuals.``1``

//let res =
//        query {
//            for p in ctx.Timetracker.Project do
//            join sp in ctx.Timetracker.Subproject
//                on (p.Id = sp.Projectid)
//            select (p, sp)
//        }
//        |> Seq.toList