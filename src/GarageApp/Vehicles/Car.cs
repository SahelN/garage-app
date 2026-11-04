namespace GarageApp.Vehicles
{
    public class Car : Vehicle
    {
        public FuelType FuelType { get; }
        
        public override int SpaceInThirds
        {
            get { return VehicleSize.Car; }
        }

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