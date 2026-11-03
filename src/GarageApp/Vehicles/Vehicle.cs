using System;
using System.Text.RegularExpressions;

namespace GarageApp.Vehicles
{
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; }
        public string Color { get; }
        public int NumberOfWheels { get; }

        // Swedish format: 3 letters, 2 digits, then a letter or a digit (ABC123, ABC12E).
        // ^ and $ make sure the WHOLE text matches, not just a part of it.
        private static readonly Regex RegistrationNumberPattern =
            new Regex("^[A-Z]{3}[0-9]{2}[A-Z0-9]$", RegexOptions.IgnoreCase);

        public static bool IsValidRegistrationNumber(string? registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                return false;

            return RegistrationNumberPattern.IsMatch(registrationNumber.Trim());
        }

        protected Vehicle(string registrationNumber, string color, int numberOfWheels)
        {
            if (!IsValidRegistrationNumber(registrationNumber))
                throw new ArgumentException(
                    "Registration number must be 3 letters, 2 digits and a letter or digit, e.g. ABC123 or ABC12E.",
                    nameof(registrationNumber));

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