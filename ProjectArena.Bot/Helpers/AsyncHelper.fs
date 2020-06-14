module ProjectArena.Bot.Helpers.AsyncHelper
open System
open System.Threading.Tasks
open System.Threading

let rand = Random()

let randomSleep =
    let ms = rand.Next(100,1000)
    Thread.Sleep ms