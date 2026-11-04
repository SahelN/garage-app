namespace GarageApp.Interfaces
{
    public interface IVehicle
    {
        string RegistrationNumber { get; }
        string Color { get; }
        int NumberOfWheels { get; }
    }
}