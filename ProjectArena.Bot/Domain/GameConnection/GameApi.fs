module ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Models.Configuration
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
    }

let private post<'Output> url : Async<'Output option> =
    async {
        let jsonSettings = JsonSerializerSettings();
        jsonSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
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
    }

let authorize (configuration: ApiConfiguration) =
    async {
        let mutable content = None
        let request = {
            Email = configuration.Login
            Password = configuration.Password
        }
        while content = None do
            let! result = postBody<SignInRequestDto, unit> (sprintf "%s/api/auth/signin" configuration.Host, request)
            match result with
            | None ->
                printfn "Unsuccessful authorization call. Try again after 5 seconds"
                do! Task.Delay 5000
            | _ -> content <- result
        return content.Value
    }

let enqueue (host: string) =
    async {
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
