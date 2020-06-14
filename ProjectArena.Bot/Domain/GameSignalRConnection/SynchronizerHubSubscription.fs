namespace ProjectArena.Bot.Domain.GameSignalRConnection
open Microsoft.AspNetCore.SignalR.Client
open ProjectArena.Bot.Models.Dtos

type SynchronizerHubSubscription<'O> = 
    {
        Function: (string * SynchronizerDto) -> 'O
        MethodName: string
        Connection: HubConnection
    }

type SynchronizerHubSubscription =

    static member Unit methodName (connection:HubConnection) =
         {
            Function = (fun sync ->
                printfn "Get synchronizer: %A" sync
                sync)
            MethodName = methodName
            Connection = connection
        }

    static member Map (func: 'O -> 'T) (sub: SynchronizerHubSubscription<'O>) =
        sub.Connection.Remove(sub.MethodName)
        let newSub =
            {
                Function = sub.Function >> func
                MethodName = sub.MethodName
                Connection = sub.Connection
            }
        newSub.Connection.On(newSub.MethodName, newSub.Function >> ignore) |> ignore
        newSub
