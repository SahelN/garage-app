using GarageApp.Vehicles;

namespace GarageApp
{
    // The layer between the UI and the garage.
    public class GarageHandler
    {
        private Garage<Vehicle>? _garage;

        public bool HasGarage
        {
            get { return _garage != null; }
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

        public void CreateGarage(int capacity)
        {
            // Garage<T> validates the capacity and throws if it is wrong
            _garage = new Garage<Vehicle>(capacity);
        }

        // Adds some ready-made vehicles. Returns how many were actually parked
        public int SeedVehicles()
        {
            Garage<Vehicle> garage = RequireGarage();

            List<Vehicle> samples = new List<Vehicle>
            {
                new Car("ABC123", "Black", 4, FuelType.Gasoline),
                new Car("DEF456", "Red", 4, FuelType.Electric),
                new Motorcycle("MCP001", "Pink", 3, 600),
                new Bus("BUS100", "Yellow", 6, 50),
                new Boat("BOAT01", "White", 8.5),
                new Airplane("SEABC1", "White", 3, 2)
            };

            int parkedCount = 0;
            foreach (Vehicle vehicle in samples)
            {
                if (garage.Park(vehicle) == ParkResult.Success)
                    parkedCount++;
            }
            return parkedCount;
        }

        public ParkResult Park(Vehicle vehicle)
        {
            return RequireGarage().Park(vehicle);
        }

        public bool Remove(string registrationNumber)
        {
            return RequireGarage().Remove(registrationNumber);
        }

        public Vehicle? FindByRegistrationNumber(string registrationNumber)
        {
            return RequireGarage().FindByRegistrationNumber(registrationNumber);
        }

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

        // null means "ignore this property".
        public List<Vehicle> Search(string? vehicleType, string? color, int? numberOfWheels)
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

            return result.ToList();
        }

        // One place for "there must be a garage". The UI should check HasGarage first,
        // but the handler protects itself anyway.
        private Garage<Vehicle> RequireGarage()
        {
            if (_garage == null)
                throw new InvalidOperationException("No garage has been created yet.");

            return _garage;
        }
    }
}