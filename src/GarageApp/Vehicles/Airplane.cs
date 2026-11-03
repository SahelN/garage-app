using System;

namespace GarageApp.Vehicles
{
    public class Airplane : Vehicle
    {
        public int NumberOfEngines { get; }

        public Airplane(string registrationNumber, string color, int numberOfWheels, int numberOfEngines)
            : base(registrationNumber, color, numberOfWheels)
        {
            if (numberOfEngines <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfEngines), "Number of engines must be greater than zero.");

            NumberOfEngines = numberOfEngines;
        }

        public override string ToString()
        {
            return base.ToString() + $"  Engines: {NumberOfEngines}";
        }
    }
}