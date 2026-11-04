using GarageApp.Vehicles;

namespace GarageApp.Interfaces
{
    public interface IHandler
    {
        bool HasGarage { get; }
        int Capacity { get; }
        int ParkedCount { get; }
        int FreeSpaces { get; }
        bool IsFull { get; }

        string CurrentGarageName { get; }

        bool GarageNameExists(string name);
        void CreateGarage(string name, int capacity);
        bool SelectGarage(string name);
        List<GarageSummary> GetGarageSummaries();
        string? FindGarageNameFor(string registrationNumber);
        int SeedVehicles();

        ParkResult Park(Vehicle vehicle);
        bool Remove(string registrationNumber);
        Vehicle? FindByRegistrationNumber(string registrationNumber);

        List<Vehicle> GetAllVehicles();
        Dictionary<string, int> GetVehicleTypeCounts();

        List<Vehicle> Search(
            string? vehicleType = null,
            string? color = null,
            int? numberOfWheels = null,
            FuelType? fuelType = null,
            int? numberOfSeats = null,
            int? cylinderVolume = null,
            int? numberOfEngines = null,
            double? length = null);
    }
}