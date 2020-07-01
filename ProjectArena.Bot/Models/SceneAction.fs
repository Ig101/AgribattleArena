namespace ProjectArena.Bot.Models.Neural

type SceneAction =
    | Wait
    | Move of x:int * y:int
    | Cast of name:string * x:int * y:int
