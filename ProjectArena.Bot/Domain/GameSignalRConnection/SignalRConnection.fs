module ProjectArena.Bot.Domain.GameSignalRConnection.SignalRConnection
open Microsoft.AspNetCore.SignalR.Client

let openConnection (url:string) =
    let connection =
        HubConnectionBuilder()
            .WithUrl(url)
            .Build
    () |> connection
