namespace GarageApp.Vehicles
{
    public class Car : Vehicle
    {
        public FuelType FuelType { get; }

        public Car(string registrationNumber, string color, int numberOfWheels, FuelType fuelType)
            : base(registrationNumber, color, numberOfWheels)
        {
            FuelType = fuelType;
        }

        public override string ToString()
        {
            return base.ToString() + $"  Fuel: {FuelType}";
        }
    }
}