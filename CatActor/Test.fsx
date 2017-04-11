#r @"bin\debug\CatActor.Interfaces.dll"
#r "System.Runtime.Serialization"
#r "System.ServiceModel"
#I "../packages/"
#r "Microsoft.ServiceFabric/lib/net45/System.Fabric.Strings.dll"
#r "Microsoft.ServiceFabric/lib/net45/System.Fabric.Management.ServiceModel.dll"
#r "Microsoft.ServiceFabric/lib/net45/Microsoft.ServiceFabric.Internal.Strings.dll"
#r "System.ValueTuple/lib/netstandard1.0/System.ValueTuple.dll"
#r "Microsoft.ServiceFabric/lib/net45/System.Fabric.dll"
#r "Microsoft.ServiceFabric/lib/net45/System.Fabric.Management.ServiceModel.XmlSerializers.dll"
#r "Microsoft.ServiceFabric/lib/net45/Microsoft.ServiceFabric.Internal.dll"
#r "Microsoft.ServiceFabric.Data/lib/net45/Microsoft.ServiceFabric.Data.Interfaces.dll"
#r "Microsoft.ServiceFabric.Data/lib/net45/Microsoft.ServiceFabric.Data.dll"
#r "Microsoft.ServiceFabric.FabricTransport.Internal/lib/net45/Microsoft.ServiceFabric.FabricTransport.dll"
#r "Microsoft.ServiceFabric.Services/lib/net45/Microsoft.ServiceFabric.Services.dll"
#r "Microsoft.ServiceFabric.Services.Remoting/lib/net45/Microsoft.ServiceFabric.Services.Remoting.dll"
#r "Microsoft.ServiceFabric.Actors/lib/net45/Microsoft.ServiceFabric.Actors.dll"
open System
open System.Threading
open Microsoft.ServiceFabric.Actors
open Microsoft.ServiceFabric.Actors.Client
open CatActor.Interfaces

let cat = ActorProxy.Create<ICatActor>(ActorId "ServiceFabricPoC", Uri("fabric:/ServiceFabricPoC/CatActorService"))

async {
    let cts = new CancellationTokenSource()
    do! cat.SetCountAsync(1, cts.Token) |> Async.AwaitTask
    do! cat.SetCountAsync(2, cts.Token) |> Async.AwaitTask
    let! count = cat.GetCountAsync(cts.Token) |> Async.AwaitTask
    return sprintf "Count for %s is %d." (cat.GetActorId().GetStringId()) count
} |> Async.RunSynchronously