module ProjectArena.Bot.Domain.GameConnection.GameApi
open HttpFs.Client
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Text
open Hopac
open System
open ProjectArena.Bot.Models.Dtos
open System.Threading.Tasks

let private postBody<'Input, 'Output> (url, body: 'Input) : Async<'Output option> =
    async {
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

let private post<'Output> url : Async<'Output option> =
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

let private get<'Output> url : Async<'Output option> =
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

let authorize (login: string) (password: string) (host: string) = async {
    let mutable content = None
    let request = {
        Email = login
        Password = password
    }
    while content = None do
        let! result = postBody<SignInRequestDto, unit> (sprintf "%s/api/auth/signin" host, request)
        match result with
        | None ->
            printfn "Unsuccessful authorization call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let enqueue (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = post<unit> (sprintf "%s/api/queue" host)
        match result with
        | None ->
            printfn "Unsuccessful enqueue call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}

let getSceneSynchronizer (host: string) (sceneId: Guid) =
    get<SynchronizerDto> (sprintf "%s/api/battle/%s" host (sceneId.ToString()))

let getUserInfo (host: string) = async {
    let mutable content = None
    while content = None do
        let! result = get<UserDto> (sprintf "%s/api/user" host)
        match result with
        | None ->
            printfn "Unsuccessful get user call. Try again after 5 seconds"
            do! Task.Delay 5000
        | _ -> content <- result
    return content.Value
}