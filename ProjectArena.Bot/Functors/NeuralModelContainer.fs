namespace ProjectArena.Bot.Functors
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.BotMongoContext

type NeuralModelContainer =
    private {
        Id: string
        mutable NeuralModel: NeuralModel option
        Lazy: bool
        Connection: IMongoConnection
    }

    static member Pack (configuration: Configuration) id =
        {
            Id = id
            NeuralModel = None
            Lazy = configuration.LazyNeuralModels
            Connection = configuration.Storage
        }

    member this.GetId () = this.Id

    member this.Bind (func: NeuralModel -> NeuralModelContainer) = async {
        let! model = this.Unpack()
        return func (model)
    }

    member this.Unpack () = async {
        match this.NeuralModel with
        | Some model -> return model
        | None ->
            let! model = BotContext(this.Connection).NeuralModels.GetOneAsync(fun m -> m.Id = this.Id) |> Async.AwaitTask
            if not this.Lazy then
                this.NeuralModel <- Some model
            return model
    }