namespace GarageApp.Vehicles
{
    public static class VehicleSize
    {
        public const int ThirdsPerSpace = 3;

        public const int Motorcycle = 1;   // 1/3 space
        public const int Car = 3;          // 1 space
        public const int Bus = 6;          // 2 spaces
        public const int Boat = 6;         // 2 spaces
        public const int Airplane = 9;     // 3 spaces

        public static int GetSpaceInThirds(string vehicleType)
        {
            switch (vehicleType)
            {
                case "Motorcycle": return Motorcycle;
                case "Car": return Car;
                case "Bus": return Bus;
                case "Boat": return Boat;
                case "Airplane": return Airplane;
                default: throw new ArgumentException($"Unknown vehicle type: {vehicleType}", nameof(vehicleType));
            }
        }

        // Shows spaces for the user: 1 -> "1/3", 3 -> "1", 4 -> "1 1/3", 6 -> "2"
        public static string FormatAsSpaces(int thirds)
        {
            int wholeSpaces = thirds / ThirdsPerSpace;
            int restThirds = thirds % ThirdsPerSpace;

            if (restThirds == 0)
                return wholeSpaces.ToString();

            if (wholeSpaces == 0)
                return $"{restThirds}/3";

            return $"{wholeSpaces} {restThirds}/3";
        }
    }
}
