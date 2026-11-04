# Garage App

A console application in **C# / .NET 10** that simulates a parking garage.
You can park, remove, find and search vehicles. The garage is a **generic collection**
(`Garage<T> where T : Vehicle`) that implements `IEnumerable<T>`, so it works with
`foreach` and LINQ.

## Features

- Create a garage with a user-defined number of parking spaces
- Optionally populate the garage with test vehicles at start
- Park and remove vehicles (Car, Motorcycle, Bus, Boat, Airplane)
- List all parked vehicles
- List vehicle types and how many of each are parked
- Find a vehicle by registration number (`ABC123`, `abc123` and `AbC123` all match)
- Search by any combination of properties, e.g. *all black vehicles with four wheels*
- Search on vehicle-specific properties (fuel type, seats, cylinder volume, engines, length)
- Several named garages: create, switch, and always see which one you are working in
- Registration numbers are unique across all garages
- Vehicles take different space (motorcycle 1/3, car 1, bus 2, boat 2, airplane 3);
  only vehicle types that fit are shown when parking
- Registration number format validated with a regular expression (`ABC123` or `ABC12E`)
- Clear feedback when something succeeds or fails, and why
- Robust input handling: the app never crashes on invalid input

## Architecture

```
ConsoleUI (IUI)  →  GarageHandler (IHandler)  →  Garage<Vehicle>  →  Vehicle (IVehicle)
  user input          logic and LINQ queries       stores vehicles      Car, Motorcycle,
  and output          no Console here              protects its rules   Bus, Boat, Airplane
```

- **`Garage<T>`**: generic collection with a capacity set in the constructor.
  Implements `IEnumerable<T>` with `yield return`.
- **`GarageHandler`**: the layer between the UI and the garage. The UI never talks to
  `Garage<T>` directly. Uses LINQ (`GroupBy`, `Where`) for counting and searching.
- **`ConsoleUI`**: all interaction with the user: menus, input and messages.
- **Interfaces** (`IUI`, `IHandler`, `IVehicle`): the UI depends on contracts, not on
  concrete classes.

## Project structure

```
garage-app/
├── GarageApp.slnx
├── src/GarageApp/
│   ├── Interfaces/        IHandler, IUI, IVehicle
│   ├── Vehicles/          Vehicle (abstract), Car, Motorcycle, Bus, Boat, Airplane, FuelType, VehicleSize
│   ├── Garage.cs          Garage<T>
│   ├── GarageHandler.cs
│   ├── GarageSummary.cs
│   ├── ConsoleUI.cs
│   ├── ParkResult.cs
│   └── Program.cs
└── tests/GarageApp.Tests/
    ├── GarageTests.cs     tests for the public methods of Garage<T>
    ├── VehicleTests.cs    tests for registration number validation and vehicle space
    └── GarageHandlerTests.cs tests for multiple garages
```

## Getting started

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download)

**Visual Studio:** open `GarageApp.slnx` and press **F5**.

**Command line:**

```bash
dotnet run --project src/GarageApp
```

## Running the tests

**Visual Studio:** *Test → Test Explorer → Run All*

**Command line:**

```bash
dotnet test
```

63 unit tests (xUnit), written with Arrange / Act / Assert and named
`MethodName_StateUnderTest_ExpectedBehavior`.

## Roadmap

- [x] Multiple garages (e.g. hangar, motorcycle garage) and switching between them
- [x] Vehicles of different sizes (car 1 space, boat 2, airplane 3, motorcycle 1/3)
- [ ] Save and load the garage to a JSON file (async file I/O)
- [ ] Garage size from configuration (`appsettings.json`)
- [ ] Garage made of parking spots
- [ ] Web API version of the garage

## Tech stack

C# · .NET 10 · LINQ · xUnit · Git
