using GarageApp.Interfaces;
using GarageApp.Vehicles;

namespace GarageApp
{
    public class ConsoleUI : IUI
    {
        private readonly IHandler _handler;

        private static readonly List<string> VehicleTypes = new List<string>
        {
            "Car", "Motorcycle", "Bus", "Boat", "Airplane"
        };

        public ConsoleUI(IHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            _handler = handler;
        }

        public void Run()
        {
            Console.WriteLine("=== Garage App ===");
            Console.WriteLine();

            // A garage must exist before the menu is shown
            int capacity = ReadInt("How many parking spaces should the garage have? (1-1000): ", 1, 1000);
            _handler.CreateGarage(capacity);
            Console.WriteLine($"A garage with {capacity} parking spaces was created.");
            Console.WriteLine();

            // Optional test vehicles
            if (ReadYesNo("Do you want to add some test vehicles? (y/n): "))
            {
                int added = _handler.SeedVehicles();
                Console.WriteLine($"{added} test vehicles were added.");
            }

            bool running = true;
            while (running)
            {
                PrintMenu();
                int choice = ReadInt("Choose an option: ", 0, 6);
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        ListAllVehicles();
                        break;
                    case 2:
                        ListVehicleTypes();
                        break;
                    case 3:
                        ParkVehicle();
                        break;
                    case 4:
                        RemoveVehicle();
                        break;
                    case 5:
                        FindVehicle();
                        break;
                    case 6:
                        SearchVehicles();
                        break;
                    case 0:
                        running = false;
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            Console.WriteLine("Goodbye!");
        }

        private void PrintMenu()
        {
            string parkText = _handler.IsFull ? "Park a vehicle (no free spaces, garage is full)" : "Park a vehicle";

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine($" GARAGE  ({_handler.ParkedCount} of {_handler.Capacity} spaces used, {_handler.FreeSpaces} free)");
            Console.WriteLine("========================================");
            Console.WriteLine(" 1. List all parked vehicles");
            Console.WriteLine(" 2. List vehicle types and counts");
            Console.WriteLine($" 3. {parkText}");
            Console.WriteLine(" 4. Remove a vehicle");
            Console.WriteLine(" 5. Find a vehicle by registration number");
            Console.WriteLine(" 6. Search vehicles by properties");
            Console.WriteLine(" 0. Exit");
            Console.WriteLine("----------------------------------------");
        }

        // ---------- Menu actions ----------
        private void ListAllVehicles()
        {
            Console.WriteLine("Parked vehicles in the garage:");
            PrintVehicles(_handler.GetAllVehicles());
        }

        private void ListVehicleTypes()
        {
            Dictionary<string, int> typeCounts = _handler.GetVehicleTypeCounts();

            if (typeCounts.Count == 0)
            {
                Console.WriteLine("The garage is empty.");
                return;
            }

            Console.WriteLine("Vehicle types in the garage:");
            foreach (KeyValuePair<string, int> pair in typeCounts)
            {
                Console.WriteLine($"  {pair.Key,-11} {pair.Value}");
            }
        }

        private void ParkVehicle()
        {
            // Check early so the user doesn't type everything for nothing
            if (_handler.IsFull)
            {
                Console.WriteLine("The garage is full. Remove a vehicle first.");
                return;
            }

            Console.WriteLine("Vehicle types:");
            for (int i = 0; i < VehicleTypes.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {VehicleTypes[i]}");
            }
            int typeChoice = ReadInt($"Choose vehicle type (1-{VehicleTypes.Count}): ", 1, VehicleTypes.Count);
            string vehicleType = VehicleTypes[typeChoice - 1];

            string registrationNumber = ReadRegistrationNumber("Registration number (e.g. ABC123): ");

            // Check early: no need to ask for color and wheels if the number is taken
            if (_handler.FindByRegistrationNumber(registrationNumber) != null)
            {
                Console.WriteLine($"A vehicle with registration number {registrationNumber.ToUpperInvariant()} is already parked.");
                return;
            }

            string color = ReadRequiredString("Color: ");

            Vehicle vehicle;
            try
            {
                switch (vehicleType)
                {
                    case "Car":
                        int carWheels = ReadInt("Number of wheels (0-20): ", 0, 20);
                        FuelType fuelType = ReadFuelType();
                        vehicle = new Car(registrationNumber, color, carWheels, fuelType);
                        break;
                    case "Motorcycle":
                        int motorcycleWheels = ReadInt("Number of wheels (0-20): ", 0, 20);
                        int cylinderVolume = ReadInt("Cylinder volume in cc (1-10000): ", 1, 10000);
                        vehicle = new Motorcycle(registrationNumber, color, motorcycleWheels, cylinderVolume);
                        break;
                    case "Bus":
                        int busWheels = ReadInt("Number of wheels (0-20): ", 0, 20);
                        int numberOfSeats = ReadInt("Number of seats (1-200): ", 1, 200);
                        vehicle = new Bus(registrationNumber, color, busWheels, numberOfSeats);
                        break;
                    case "Boat":
                        double length = ReadPositiveDouble("Length in meters: ");
                        vehicle = new Boat(registrationNumber, color, length);
                        break;
                    case "Airplane":
                        int airplaneWheels = ReadInt("Number of wheels (0-20): ", 0, 20);
                        int numberOfEngines = ReadInt("Number of engines (1-10): ", 1, 10);
                        vehicle = new Airplane(registrationNumber, color, airplaneWheels, numberOfEngines);
                        break;
                    default:
                        Console.WriteLine($"Unknown vehicle type: {vehicleType}");
                        return;
                }
            }
            catch (ArgumentException ex)
            {
                // Safety net: the Vehicle constructors validate their input
                Console.WriteLine($"Could not create the vehicle: {ex.Message}");
                return;
            }

            ParkResult result = _handler.Park(vehicle);
            PrintParkResult(result, vehicle);
        }

        private void RemoveVehicle()
        {
            string registrationNumber = ReadRequiredString("Registration number of the vehicle to remove: ");

            if (_handler.Remove(registrationNumber))
                Console.WriteLine($"{registrationNumber.ToUpperInvariant()} was removed from the garage.");
            else
                Console.WriteLine($"No vehicle with registration number {registrationNumber.ToUpperInvariant()} was found.");
        }

        private void FindVehicle()
        {
            string registrationNumber = ReadRequiredString("Registration number: ");
            Vehicle? vehicle = _handler.FindByRegistrationNumber(registrationNumber);

            if (vehicle != null)
            {
                Console.WriteLine("Vehicle found:");
                Console.WriteLine($"  {vehicle}");
            }
            else
            {
                Console.WriteLine($"No vehicle with registration number {registrationNumber.ToUpperInvariant()} was found.");
            }
        }

        private void SearchVehicles()
        {
            Console.WriteLine("Search vehicles. Leave a field empty to ignore it.");
            Console.WriteLine();

            Console.WriteLine("Common properties:");
            string? vehicleType = ReadOptionalVehicleType(VehicleTypes);
            string? color = ReadOptionalString("  Color: ");

            // Boats always have 0 wheels, so the question is pointless for boats
            int? numberOfWheels = null;
            if (vehicleType != "Boat")
                numberOfWheels = ReadOptionalInt("  Number of wheels: ");

            // Only ask for the specific properties that can match the chosen type
            Console.WriteLine();
            Console.WriteLine("Specific properties:");

            FuelType? fuelType = null;
            int? numberOfSeats = null;
            int? cylinderVolume = null;
            int? numberOfEngines = null;
            double? length = null;

            if (vehicleType == null || vehicleType == "Car")
                fuelType = ReadOptionalFuelType();

            if (vehicleType == null || vehicleType == "Bus")
                numberOfSeats = ReadOptionalInt("  Number of seats (Bus): ");

            if (vehicleType == null || vehicleType == "Motorcycle")
                cylinderVolume = ReadOptionalInt("  Cylinder volume in cc (Motorcycle): ");

            if (vehicleType == null || vehicleType == "Airplane")
                numberOfEngines = ReadOptionalInt("  Number of engines (Airplane): ");

            if (vehicleType == null || vehicleType == "Boat")
                length = ReadOptionalPositiveDouble("  Length in meters (Boat): ");

            List<Vehicle> result = _handler.Search(
                vehicleType: vehicleType,
                color: color,
                numberOfWheels: numberOfWheels,
                fuelType: fuelType,
                numberOfSeats: numberOfSeats,
                cylinderVolume: cylinderVolume,
                numberOfEngines: numberOfEngines,
                length: length);

            Console.WriteLine();
            Console.WriteLine($"Search result ({result.Count} found):");
            PrintVehicles(result);
        }

        // ---------- Output helpers ----------
        private static void PrintVehicles(List<Vehicle> vehicles)
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("  No vehicles found.");
                return;
            }

            foreach (Vehicle vehicle in vehicles)
            {
                Console.WriteLine($"  {vehicle}");
            }
        }

        private static void PrintParkResult(ParkResult result, Vehicle vehicle)
        {
            switch (result)
            {
                case ParkResult.Success:
                    Console.WriteLine($"{vehicle.RegistrationNumber} was parked in the garage.");
                    break;
                case ParkResult.GarageFull:
                    Console.WriteLine($"Could not park {vehicle.RegistrationNumber}: the garage is full.");
                    break;
                case ParkResult.DuplicateRegistrationNumber:
                    Console.WriteLine($"Could not park {vehicle.RegistrationNumber}: a vehicle with that registration number is already parked.");
                    break;
            }
        }

        // ---------- Input helpers (never crash on bad input) ----------
        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;

                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }

        private static double ReadPositiveDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (double.TryParse(input, out double value) && value > 0)
                    return value;

                Console.WriteLine("Please enter a number greater than zero.");
            }
        }

        private static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.WriteLine("This field is required.");
            }
        }

        private static string? ReadOptionalString(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return null;

            return input.Trim();
        }

        // Numbered list instead of free text, so a typo like "Cars" can't happen
        private static string? ReadOptionalVehicleType(List<string> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                Console.WriteLine($"    {i + 1}. {types[i]}");
            }

            while (true)
            {
                Console.Write($"  Vehicle type (1-{types.Count}, empty = all): ");
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (input == string.Empty)
                    return null;

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= types.Count)
                    return types[choice - 1];

                Console.WriteLine($"  Please enter a number between 1 and {types.Count}, or leave it empty.");
            }
        }

        private static int? ReadOptionalInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    return null;

                if (int.TryParse(input, out int value) && value >= 0)
                    return value;

                Console.WriteLine("Please enter a whole number, or leave it empty.");
            }
        }

        private static double? ReadOptionalPositiveDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    return null;

                if (double.TryParse(input, out double value) && value > 0)
                    return value;

                Console.WriteLine("  Please enter a number greater than zero, or leave it empty.");
            }
        }

        private static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

                if (input == "y" || input == "yes")
                    return true;
                if (input == "n" || input == "no")
                    return false;

                Console.WriteLine("Please answer y or n.");
            }
        }

        private static FuelType ReadFuelType()
        {
            Console.WriteLine("Fuel type: 1. Gasoline  2. Diesel  3. Electric");
            int choice = ReadInt("Choose fuel type: ", 1, 3);

            switch (choice)
            {
                case 1:
                    return FuelType.Gasoline;
                case 2:
                    return FuelType.Diesel;
                default:
                    return FuelType.Electric;
            }
        }

        private static FuelType? ReadOptionalFuelType()
        {
            while (true)
            {
                Console.Write("  Fuel type (Car) 1. Gasoline  2. Diesel  3. Electric: ");
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (input == string.Empty)
                    return null;

                switch (input)
                {
                    case "1":
                        return FuelType.Gasoline;
                    case "2":
                        return FuelType.Diesel;
                    case "3":
                        return FuelType.Electric;
                }

                Console.WriteLine("  Please enter 1, 2 or 3, or leave it empty.");
            }
        }

        // Asks again until the registration number has the right format
        private static string ReadRegistrationNumber(string prompt)
        {
            while (true)
            {
                string input = ReadRequiredString(prompt);

                if (Vehicle.IsValidRegistrationNumber(input))
                    return input;

                Console.WriteLine("Invalid format. Use 3 letters, 2 digits and a letter or digit, e.g. ABC123 or ABC12E.");
            }
        }
    }
}
