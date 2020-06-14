namespace ProjectArena.Bot.Domain.GameSignalRConnection
open Microsoft.AspNetCore.SignalR.Client
open ProjectArena.Infrastructure.Models.Battle.Synchronization

type UnitHubSubscription<'O> = 
    {
        Function: unit -> 'O
        MethodName: string
        Connection: HubConnection
    }

type UnitHubSubscription =

    static member Unit methodName (connection:HubConnection) =
         {
            Function = (fun sync -> sync)
            MethodName = methodName
            Connection = connection
        }

    static member Map (func: 'O -> 'T) (sub: UnitHubSubscription<'O>) =
        sub.Connection.Remove(sub.MethodName)
        let newSub =
            {
                Function = sub.Function >> func
                MethodName = sub.MethodName
                Connection = sub.Connection
            }
        newSub.Connection.On(newSub.MethodName, newSub.Function >> ignore) |> ignore
        newSub
