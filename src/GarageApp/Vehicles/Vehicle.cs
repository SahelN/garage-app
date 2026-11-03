using System;

namespace GarageApp.Vehicles
{
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; }
        public string Color { get; }
        public int NumberOfWheels { get; }

        protected Vehicle(string registrationNumber, string color, int numberOfWheels)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Registration number is required.", nameof(registrationNumber));

            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Color is required.", nameof(color));

            if (numberOfWheels < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfWheels), "Number of wheels cannot be negative.");

            RegistrationNumber = registrationNumber.Trim().ToUpperInvariant();
            Color = color.Trim();
            NumberOfWheels = numberOfWheels;
        }

        public override string ToString()
        {
            return $"{GetType().Name,-11} {RegistrationNumber,-8} Color: {Color,-8} Wheels: {NumberOfWheels}";
        }
    }
}