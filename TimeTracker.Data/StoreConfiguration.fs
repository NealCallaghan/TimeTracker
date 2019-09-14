namespace TimeTracker.Data

module StoreConfiguration =
    open FSharp.Data.Sql

    type MySqlRepositorySettings = {
        ConnectionString: string }
        with
        member this.GetConnectionString =
            this.ConnectionString
   
    [<Literal>]
    let private connString = @"Server=localhost;Port=3306;Database=timetracker;User=root;Password=password;Auto Enlist=false; Convert Zero Datetime=true;"
    
    [<Literal>]
    let private dbVendor = Common.DatabaseProviderTypes.MYSQL

    [<Literal>]
    let private resPath = __SOURCE_DIRECTORY__ + "\libraries" //required only at compile time

    [<Literal>]
    let private indivAmount = 1000

    [<Literal>]
    let useOptTypes = true

    
    type sql = SqlDataProvider< 
                ConnectionString = connString,
                DatabaseVendor = dbVendor,
                ResolutionPath = resPath,
                IndividualsAmount = indivAmount,
                UseOptionTypes = true >
    
    
    //This is the compile time context
    //let ctx  = 
    //    sql.GetDataContext()

    //this is the runtime context which can use any connectionstring
    let private getContext (connectionString: string) = 
        sql.GetDataContext(connectionString)


    type TimeTrackerContext = sql.dataContext.timetrackerSchema

    let getTimeTrackerContext (config: MySqlRepositorySettings):TimeTrackerContext =
        let client = getContext config.GetConnectionString
        client.Timetracker


    
