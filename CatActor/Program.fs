module CatActor.Program

open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors.Runtime

[<EntryPoint>]
let main _ =
    try
        let awaiter =
            ActorRuntime
                .RegisterActorAsync<CatActor>(fun context actorType -> ActorService(context, actorType))
                .GetAwaiter()

        awaiter.GetResult()

        Thread.Sleep(Timeout.Infinite)
    with ex ->
        ActorEventSource.Current.ActorHostInitializationFailed(ex.ToString())
        reraise ()

    0 // return an integer exit code