namespace ProjectArena.Bot.Models.Neural

type ActionNeuronType =
    | Move of x:int * y:int
    | Cast of name:string * x:int * y:int
