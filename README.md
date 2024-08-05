# Inter.DeviceKeeper
The center of the Devices domain, this service federates access to Devices.  It does so by exposing a web api and a grpc (contract [here](https://github.com/InterMW/Inter.Device/blob/main/device.proto)).  To further support the grpc, a C# package encapuslates it, depending on the Common and the Domain packages.

|Name|Nuget|
|-|-|
|Device.GrpcClient|[![Device.GrpcClient](https://img.shields.io/nuget/v/Device.GrpcClient.svg)](https://www.nuget.org/packages/Device.GrpcClient/)
|Device.Domain|[![Device.Domain](https://img.shields.io/nuget/v/Device.Domain.svg)](https://www.nuget.org/packages/Device.Domain/)
|Device.Common|[![Device.Common](https://img.shields.io/nuget/v/Device.Common.svg)](https://www.nuget.org/packages/Device.Common/)|

# How to run

Clone this repository and run with `dotnet run --project Application/Application.csproj`.

There is a demo project for testing changes.  To use, run the above command to start the server, then (in a separate window) run `dotnet run --project Demo`.

## General information

This project requires dotnet 8 sdk to run (install link [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)).

This project uses the MelbergFramework nuget package, please see [its github repo](https://github.com/Joseph-Melberg/https://github.com/MelbergFramework) for more info.

## Required Infrastructure
|Product|Details|Database Install Link|
|-|-|-|
|Couchbase| Update the Couchbase section with your url and a user that has Full Admin rights.| Docker installation guide for Couchbase [here](https://docs.couchbase.com/server/current/install/getting-started-docker.html)|
