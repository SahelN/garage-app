using System;

namespace GarageApp.Vehicles
{
    public class Motorcycle : Vehicle
    {
        public int CylinderVolume { get; }

        public Motorcycle(string registrationNumber, string color, int numberOfWheels, int cylinderVolume)
            : base(registrationNumber, color, numberOfWheels)
        {
            if (cylinderVolume <= 0)
                throw new ArgumentOutOfRangeException(nameof(cylinderVolume), "Cylinder volume must be greater than zero.");

            CylinderVolume = cylinderVolume;
        }

        public override string ToString()
        {
            return base.ToString() + $"  Cylinder volume: {CylinderVolume} cc";
        }
    }
}