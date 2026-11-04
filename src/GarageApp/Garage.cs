using GarageApp.Vehicles;
using System.Collections;

namespace GarageApp
{
    // T must be a Vehicle (or a subclass of Vehicle).
    public class Garage<T> : IEnumerable<T> where T : Vehicle
    {
        // The parked vehicles.
        private readonly List<T> _vehicles = new List<T>();

        public int Capacity { get; }
        public string Name { get; }

        // Number of vehicles (not space)
        public int ParkedCount
        {
            get { return _vehicles.Count; }
        }

        private int CapacityInThirds
        {
            get { return Capacity * VehicleSize.ThirdsPerSpace; }
        }

        private int UsedThirds
        {
            get { return _vehicles.Sum(v => v.SpaceInThirds); }
        }

        public int FreeThirds
        {
            get { return CapacityInThirds - UsedThirds; }
        }

        // For display only, e.g. 2.33 used spaces
        public double UsedSpaces
        {
            get { return (double)UsedThirds / VehicleSize.ThirdsPerSpace; }
        }

        public double FreeSpaces
        {
            get { return (double)FreeThirds / VehicleSize.ThirdsPerSpace; }
        }

        // Full = no space at all left
        public bool IsFull
        {
            get { return FreeThirds <= 0; }
        }

        public Garage(int capacity, string name = "Garage")
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Garage name is required.", nameof(name));

            Capacity = capacity;
            Name = name.Trim();
        }

        // Is there room for a vehicle that needs this much space?
        public bool HasRoomFor(int spaceInThirds)
        {
            return spaceInThirds <= FreeThirds;
        }

        // Parks a vehicle if there is room and the registration number is not already used.
        public ParkResult Park(T vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (IsFull)
                return ParkResult.GarageFull;

            // There is space left, but not enough for this vehicle
            if (!HasRoomFor(vehicle.SpaceInThirds))
                return ParkResult.NotEnoughSpace;

            // Vehicle already stores the registration number in upper case,
            // so we can compare directly with ==
            bool alreadyParked = _vehicles.Any(v => v.RegistrationNumber == vehicle.RegistrationNumber);
            if (alreadyParked)
                return ParkResult.DuplicateRegistrationNumber;

            _vehicles.Add(vehicle);
            return ParkResult.Success;
        }

        // Finds a vehicle by registration number. ABC123, abc123 and AbC123 all match.
        public T? FindByRegistrationNumber(string registrationNumber)
        {
            string normalizedNumber = NormalizeRegistrationNumber(registrationNumber);

            return _vehicles.FirstOrDefault(v => v.RegistrationNumber == normalizedNumber);
        }

        // Removes a vehicle. Returns false if no vehicle has that registration number.
        public bool Remove(string registrationNumber)
        {
            T? vehicle = FindByRegistrationNumber(registrationNumber);

            if (vehicle == null)
                return false;

            return _vehicles.Remove(vehicle);
        }

        // Returns the vehicles one at a time.
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T vehicle in _vehicles)
            {
                yield return vehicle;
            }
        }

        // Required by the non-generic IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // One place for the rule "ignore spaces and upper/lower case"
        private static string NormalizeRegistrationNumber(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                return string.Empty;

            return registrationNumber.Trim().ToUpperInvariant();
        }
    }
}