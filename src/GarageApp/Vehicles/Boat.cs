using System;

namespace GarageApp.Vehicles
{
    public class Boat : Vehicle
    {
        public double Length { get; }

        public Boat(string registrationNumber, string color, double length)
            : base(registrationNumber, color, 0)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");

            Length = length;
        }

        public override string ToString()
        {
            return base.ToString() + $"  Length: {Length:0.0} m";
        }
    }
}