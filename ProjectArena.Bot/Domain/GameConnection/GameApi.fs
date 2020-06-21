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

let private post<'Output> (auth: string) url : Async<'Output option> =
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
                printfn "Exited post operation with %d status code." response.statusCode
                return None
        with
        | e ->
            printfn "Exception on post operation: %A" e.Message
            return None
    }

let private get<'Output> (auth: string) url : Async<'Output option> =
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
                printfn "Exited get operation with %d status code." response.statusCode
                return None
        with
        | e ->
            printfn "Exception on get operation: %A" e.Message
            return None
    }

let private authorizeInternal (url, body: 'Input) : Async<string option> = async {
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
            printfn "Exited post operation with %d status code." response.statusCode
            return None
    with
    | e ->
        printfn "Exception on post operation: %A" e.Message
        return None
}

let authorize (login: string) (password: string) (host: string) = async {
    let mutable content = None
    let request = {
        Email = login
        Password = password
    }
    while content = None do
        let! result = authorizeInternal (sprintf "%s/api/auth/signin" host, request)
        match result with
        | None ->
            printfn "Unsuccessful authorization call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let enqueue (auth: string) (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = post<unit> auth (sprintf "%s/api/queue" host)
        match result with
        | None ->
            printfn "Unsuccessful enqueue call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let getSceneSynchronizer (auth: string) (host: string) (sceneId: Guid) =
    get<ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto> auth (sprintf "%s/api/battle/%s" host (sceneId.ToString()))
    |> Async.map ( Option.map mapSynchronizer )

let getUserInfo (auth: string) (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = get<UserDto> auth (sprintf "%s/api/user" host)
        match result with
        | None ->
            printfn "Unsuccessful get user call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}