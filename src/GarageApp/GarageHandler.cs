using GarageApp.Interfaces;
using GarageApp.Vehicles;

namespace GarageApp
{
    // The layer between the UI and the garages.
    // Holds several garages and knows which one the user is working in.
    public class GarageHandler : IHandler
    {
        private readonly List<Garage<Vehicle>> _garages = new List<Garage<Vehicle>>();

        // null until the user has created the first garage
        private Garage<Vehicle>? _currentGarage;

        // ---------- Current garage ----------

        public bool HasGarage
        {
            get { return _currentGarage != null; }
        }

        public string CurrentGarageName
        {
            get { return RequireGarage().Name; }
        }

        public int Capacity
        {
            get { return RequireGarage().Capacity; }
        }

        public int ParkedCount
        {
            get { return RequireGarage().ParkedCount; }
        }

        public int FreeSpaces
        {
            get { return RequireGarage().FreeSpaces; }
        }

        public bool IsFull
        {
            get { return RequireGarage().IsFull; }
        }

        // ---------- Several garages ----------

        // Garage names are unique, ignoring upper/lower case
        public bool GarageNameExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string trimmedName = name.Trim();
            return _garages.Any(g => string.Equals(g.Name, trimmedName, StringComparison.OrdinalIgnoreCase));
        }

        // Creates a new garage and makes it the current one
        public void CreateGarage(string name, int capacity)
        {
            if (GarageNameExists(name))
                throw new ArgumentException($"A garage named {name.Trim()} already exists.", nameof(name));

            Garage<Vehicle> garage = new Garage<Vehicle>(capacity, name);
            _garages.Add(garage);
            _currentGarage = garage;
        }

        // Returns false if there is no garage with that name
        public bool SelectGarage(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string trimmedName = name.Trim();
            Garage<Vehicle>? garage = _garages.FirstOrDefault(
                g => string.Equals(g.Name, trimmedName, StringComparison.OrdinalIgnoreCase));

            if (garage == null)
                return false;

            _currentGarage = garage;
            return true;
        }

        // Information about all garages, without giving away the garages themselves
        public List<GarageSummary> GetGarageSummaries()
        {
            return _garages
                .Select(g => new GarageSummary(g.Name, g.Capacity, g.ParkedCount, g.FreeSpaces, g == _currentGarage))
                .ToList();
        }

        // Which garage is the vehicle parked in? null if it is not parked anywhere.
        public string? FindGarageNameFor(string registrationNumber)
        {
            Garage<Vehicle>? garage = _garages.FirstOrDefault(
                g => g.FindByRegistrationNumber(registrationNumber) != null);

            return garage?.Name;
        }

        // ---------- Populate ----------

        // Adds some ready-made vehicles to the current garage.
        // Returns how many were actually parked.
        public int SeedVehicles()
        {
            List<Vehicle> samples = new List<Vehicle>
            {
                new Car("ABC123", "Black", 4, FuelType.Gasoline),
                new Car("DEF456", "Red", 4, FuelType.Electric),
                new Motorcycle("MCP001", "Pink", 3, 600),
                new Bus("BUS100", "Yellow", 6, 50),
                new Boat("BOA001", "White", 8.5),
                new Airplane("PLN123", "White", 3, 2)
            };

            int parkedCount = 0;
            foreach (Vehicle vehicle in samples)
            {
                if (Park(vehicle) == ParkResult.Success)
                    parkedCount++;
            }
            return parkedCount;
        }

        // ---------- Park, remove, find (current garage) ----------

        public ParkResult Park(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            Garage<Vehicle> garage = RequireGarage();

            // Registration numbers must be unique in ALL garages, not only the current one
            if (FindGarageNameFor(vehicle.RegistrationNumber) != null)
                return ParkResult.DuplicateRegistrationNumber;

            return garage.Park(vehicle);
        }

        public bool Remove(string registrationNumber)
        {
            return RequireGarage().Remove(registrationNumber);
        }

        public Vehicle? FindByRegistrationNumber(string registrationNumber)
        {
            return RequireGarage().FindByRegistrationNumber(registrationNumber);
        }

        // ---------- Lists and search (current garage, LINQ) ----------

        // Returns a copy, so the UI cannot get hold of the garage itself
        public List<Vehicle> GetAllVehicles()
        {
            return RequireGarage().ToList();
        }

        // Key = type name, value = how many of that type are parked
        public Dictionary<string, int> GetVehicleTypeCounts()
        {
            return RequireGarage()
                .GroupBy(v => v.GetType().Name)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        // null means "ignore this property"
        public List<Vehicle> Search(
            string? vehicleType = null,
            string? color = null,
            int? numberOfWheels = null,
            FuelType? fuelType = null,
            int? numberOfSeats = null,
            int? cylinderVolume = null,
            int? numberOfEngines = null,
            double? length = null)
        {
            IEnumerable<Vehicle> result = RequireGarage();

            if (!string.IsNullOrWhiteSpace(vehicleType))
            {
                string type = vehicleType.Trim();
                result = result.Where(v => string.Equals(v.GetType().Name, type, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(color))
            {
                string wantedColor = color.Trim();
                result = result.Where(v => string.Equals(v.Color, wantedColor, StringComparison.OrdinalIgnoreCase));
            }

            if (numberOfWheels.HasValue)
            {
                int wheels = numberOfWheels.Value;
                result = result.Where(v => v.NumberOfWheels == wheels);
            }

            if (fuelType.HasValue)
            {
                FuelType fuel = fuelType.Value;
                result = result.Where(v => v is Car car && car.FuelType == fuel);
            }

            if (numberOfSeats.HasValue)
            {
                int seats = numberOfSeats.Value;
                result = result.Where(v => v is Bus bus && bus.NumberOfSeats == seats);
            }

            if (cylinderVolume.HasValue)
            {
                int volume = cylinderVolume.Value;
                result = result.Where(v => v is Motorcycle motorcycle && motorcycle.CylinderVolume == volume);
            }

            if (numberOfEngines.HasValue)
            {
                int engines = numberOfEngines.Value;
                result = result.Where(v => v is Airplane airplane && airplane.NumberOfEngines == engines);
            }

            // double: compare with a small tolerance, never with == (8.5 can be stored as 8.4999999...)
            if (length.HasValue)
            {
                double boatLength = length.Value;
                result = result.Where(v => v is Boat boat && Math.Abs(boat.Length - boatLength) < 0.01);
            }

            return result.ToList();
        }

        // ---------- Helpers ----------

        // One place for "there must be a current garage"
        private Garage<Vehicle> RequireGarage()
        {
            if (_currentGarage == null)
                throw new InvalidOperationException("No garage has been created yet.");

            return _currentGarage;
        }
    }
}