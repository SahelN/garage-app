using GarageApp.Vehicles;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GarageApp
{
    // T must be a Vehicle (or a subclass of Vehicle).
    public class Garage<T> : IEnumerable<T> where T : Vehicle
    {
        // The parked vehicles.
        private readonly List<T> _vehicles = new List<T>();

        public int Capacity { get; }

        public int ParkedCount
        {
            get { return _vehicles.Count; }
        }

        public bool IsFull
        {
            get { return ParkedCount >= Capacity; }
        }

        public int FreeSpaces
        {
            get { return Capacity - ParkedCount; }
        }

        public Garage(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");

            Capacity = capacity;
        }

        // Parks a vehicle if there is room and the registration number is not already used.
        public ParkResult Park(T vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (IsFull)
                return ParkResult.GarageFull;

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
