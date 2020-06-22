module ProjectArena.Bot.Domain.GameConnection.GameApi
open HttpFs.Client
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Text
open Hopac
open System
open ProjectArena.Bot.Models.Dtos
open System.Threading.Tasks
open ProjectArena.Bot.Helpers.MappingHelper
open Microsoft.Extensions.Logging

let private delete<'Output> (logger: ILogger<unit>) (auth: string) url : Async<'Output option> =
    async {
        let jsonSettings = JsonSerializerSettings();
        jsonSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
        try
            use! response = 
                Request.createUrl Delete url
                |> Request.setHeader (ContentType {
                    typ = "application"
                    subtype = "json"
                    charset = Some Encoding.UTF8
                    boundary = None
                })
                |> Request.cookie(Cookie.create("Authorization", auth))
                |> getResponse
                |> Alt.toAsync
            let success = response.statusCode < 300
            match success with
            | true ->
                let! content =
                    Response.readBodyAsString(response)
                    |> Job.toAsync
                return Some (JsonConvert.DeserializeObject<'Output> content)
            | false ->
                logger.LogError (sprintf "Exited delete operation with %d status code." response.statusCode)
                return None
        with
        | e ->
            logger.LogError (sprintf "Exception on delete operation: %A" e.Message)
            return None
    }

let private post<'Output> (logger: ILogger<unit>) (auth: string) url : Async<'Output option> =
    async {
        let jsonSettings = JsonSerializerSettings();
        jsonSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
        try
            use! response = 
                Request.createUrl Post url
                |> Request.setHeader (ContentType {
                    typ = "application"
                    subtype = "json"
                    charset = Some Encoding.UTF8
                    boundary = None
                })
                |> Request.cookie(Cookie.create("Authorization", auth))
                |> getResponse
                |> Alt.toAsync
            let success = response.statusCode < 300
            match success with
            | true ->
                let! content =
                    Response.readBodyAsString(response)
                    |> Job.toAsync
                return Some (JsonConvert.DeserializeObject<'Output> content)
            | false ->
                logger.LogError (sprintf "Exited post operation with %d status code." response.statusCode)
                return None
        with
        | e ->
            logger.LogError (sprintf "Exception on post operation: %A" e.Message)
            return None
    }

let private get<'Output> (logger: ILogger<unit>) (auth: string) url : Async<'Output option> =
    async {
        let jsonSettings = JsonSerializerSettings();
        jsonSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
        try
            use! response = 
                Request.createUrl Get url
                |> Request.setHeader (ContentType {
                    typ = "application"
                    subtype = "json"
                    charset = Some Encoding.UTF8
                    boundary = None
                })
                |> Request.cookie(Cookie.create("Authorization", auth))
                |> getResponse
                |> Alt.toAsync
            let success = response.statusCode < 300
            match success with
            | true ->
                let! content =
                    Response.readBodyAsString(response)
                    |> Job.toAsync
                return Some (JsonConvert.DeserializeObject<'Output> content)
            | false ->
                logger.LogError (sprintf "Exited get operation with %d status code." response.statusCode)
                return None
        with
        | e ->
            logger.LogError (sprintf "Exception on get operation: %A" e.Message)
            return None
    }

let private authorizeInternal (logger: ILogger<unit>) (url, body: 'Input) : Async<string option> = async {
    let jsonSettings = JsonSerializerSettings();
    jsonSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
    let serializedBody = JsonConvert.SerializeObject(body, jsonSettings)
    try
        use! response = 
            Request.createUrl Post url
            |> Request.setHeader (ContentType {
                typ = "application"
                subtype = "json"
                charset = Some Encoding.UTF8
                boundary = None
            })
            |> Request.body (BodyString serializedBody)
            |> getResponse
            |> Alt.toAsync
        let success = response.statusCode < 300
        match success with
        | true ->
            let auth = response.cookies.["Authorization"]
            return Some auth
        | false ->
            logger.LogError (sprintf "Exited post operation with %d status code." response.statusCode)
            return None
    with
    | e ->
        logger.LogError (sprintf "Exception on post operation: %A" e.Message)
        return None
}

let authorize (logger: ILogger<unit>) (login: string) (password: string) (host: string) = async {
    let mutable content = None
    let request = {
        Email = login
        Password = password
    }
    while content = None do
        let! result = authorizeInternal logger (sprintf "%s/api/auth/signin" host, request)
        match result with
        | None ->
            logger.LogError (sprintf "Unsuccessful authorization call. Try again after 5 seconds")
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let enqueue (logger: ILogger<unit>) (auth: string) (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = post<unit> logger auth (sprintf "%s/api/queue" host)
        match result with
        | None ->
            logger.LogError (sprintf "Unsuccessful enqueue call. Try again after 5 seconds")
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let getSceneSynchronizer (logger: ILogger<unit>) (auth: string) (host: string) (sceneId: Guid) =
    get logger auth (sprintf "%s/api/battle/%s" host (sceneId.ToString()))
    |> Async.map ( Option.map mapSynchronizer )

let leaveBattle (logger: ILogger<unit>) (auth: string) (host: string) (sceneId: Guid) =
    delete<unit> logger auth (sprintf "%s/api/battle/%s" host (sceneId.ToString()))

let getUserInfo (logger: ILogger<unit>) (auth: string) (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = get<UserDto> logger auth (sprintf "%s/api/user" host)
        match result with
        | None ->
            logger.LogError (sprintf "Unsuccessful get user call. Try again after 5 seconds")
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}