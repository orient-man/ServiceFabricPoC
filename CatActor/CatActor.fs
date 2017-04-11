namespace CatActor

open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors.Runtime
open CatActor.Interfaces

/// <remarks>
/// This class represents an actor.
/// Every ActorID maps to an instance of this class.
/// The StatePersistence attribute determines persistence and replication of actor state:
///  - Persisted: State is written to disk and replicated.
///  - Volatile: State is kept in memory only and replicated.
///  - None: State is kept in memory only and not replicated.
/// </remarks>
[<StatePersistence(StatePersistence.Persisted)>]
type CatActor internal (actorService, actorId) =
    inherit Actor(actorService, actorId)

    /// <summary>
    /// This method is called whenever an actor is activated.
    /// An actor is activated the first time any of its methods are invoked.
    /// </summary>
    override this.OnActivateAsync() =
        ActorEventSource.Current.ActorMessage(this, "Actor activated.")

        // The StateManager is this actor's private state store.
        // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
        // Any serializable object can be saved in the StateManager.
        // For more information, see https://aka.ms/servicefabricactorsstateserialization
        this.StateManager.TryAddStateAsync("count", 0) :> Task

    interface ICatActor with
        member this.GetCountAsync(cancellationToken) =
            this.StateManager.GetStateAsync<int>("count", cancellationToken)

        member this.SetCountAsync(count, cancellationToken) =
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            this.StateManager
                .AddOrUpdateStateAsync(
                    "count",
                    count,
                    (fun key value -> if count > value then count else value),
                    cancellationToken) :> Task