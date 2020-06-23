namespace ProjectArena.Bot.Models.Neural

type MagnifyingActionNeuronType =
    | Proceed of x:int * y:int
    | Wait
