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
    }
}
