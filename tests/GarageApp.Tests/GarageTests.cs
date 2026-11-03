using GarageApp;
using GarageApp.Vehicles;

namespace GarageApp.Tests
{
    public class GarageTests
    {
        // Helper: creates a garage that accepts all vehicle types
        private static Garage<Vehicle> CreateGarage(int capacity)
        {
            return new Garage<Vehicle>(capacity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_CapacityZeroOrNegative_ThrowsArgumentOutOfRangeException(int capacity)
        {
            // Act + Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new Garage<Vehicle>(capacity));
        }

        [Fact]
        public void Constructor_ValidCapacity_GarageIsEmpty()
        {
            // Arrange + Act
            Garage<Vehicle> garage = CreateGarage(5);

            // Assert
            Assert.Equal(5, garage.Capacity);
            Assert.Equal(0, garage.ParkedCount);
            Assert.Equal(5, garage.FreeSpaces);
            Assert.False(garage.IsFull);
        }

        [Fact]
        public void Park_EmptyGarage_ReturnsSuccess()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(3);
            Car car = new Car("ABC123", "Black", 4, FuelType.Gasoline);

            // Act
            ParkResult result = garage.Park(car);

            // Assert
            Assert.Equal(ParkResult.Success, result);
            Assert.Equal(1, garage.ParkedCount);
        }

        [Fact]
        public void Park_DifferentVehicleTypes_AllAreParked()
        {
            // Arrange: Garage<Vehicle> accepts every subclass of Vehicle
            Garage<Vehicle> garage = CreateGarage(5);

            // Act
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
            garage.Park(new Motorcycle("MCP001", "Pink", 2, 600));
            garage.Park(new Bus("BUS100", "Yellow", 6, 50));
            garage.Park(new Boat("BOA001", "White", 8.5));
            garage.Park(new Airplane("PLN123", "White", 3, 2));

            // Assert
            Assert.Equal(5, garage.ParkedCount);
            Assert.True(garage.IsFull);
        }

        [Fact]
        public void Park_FullGarage_ReturnsGarageFull()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(1);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

            // Act
            ParkResult result = garage.Park(new Car("DEF456", "Red", 4, FuelType.Diesel));

            // Assert
            Assert.Equal(ParkResult.GarageFull, result);
            Assert.Equal(1, garage.ParkedCount);
        }

        [Fact]
        public void Park_SameRegistrationNumberDifferentCase_ReturnsDuplicateRegistrationNumber()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(3);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

            // Act: Vehicle stores "abc123" as "ABC123"
            ParkResult result = garage.Park(new Motorcycle("abc123", "Red", 2, 600));

            // Assert
            Assert.Equal(ParkResult.DuplicateRegistrationNumber, result);
            Assert.Equal(1, garage.ParkedCount);
        }

        [Fact]
        public void Park_Null_ThrowsArgumentNullException()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(3);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => garage.Park(null!));
        }

        [Fact]
        public void Remove_ParkedVehicle_ReturnsTrueAndFreesSpace()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(2);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

            // Act
            bool wasRemoved = garage.Remove("ABC123");

            // Assert
            Assert.True(wasRemoved);
            Assert.Equal(2, garage.FreeSpaces);
        }

        [Fact]
        public void Remove_LowercaseRegistrationNumber_ReturnsTrue()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(2);
            garage.Park(new Bus("BUS100", "Yellow", 6, 50));

            // Act
            bool wasRemoved = garage.Remove("bus100");

            // Assert
            Assert.True(wasRemoved);
            Assert.Equal(0, garage.ParkedCount);
        }

        [Fact]
        public void Remove_UnknownRegistrationNumber_ReturnsFalse()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(2);

            // Act
            bool wasRemoved = garage.Remove("XYZ999");

            // Assert
            Assert.False(wasRemoved);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("abc123")]
        [InlineData("AbC123")]
        [InlineData(" abc123 ")]
        public void FindByRegistrationNumber_AnyCaseOrSpaces_FindsVehicle(string searched)
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(2);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

            // Act
            Vehicle? found = garage.FindByRegistrationNumber(searched);

            // Assert
            Assert.NotNull(found);
            Assert.Equal("ABC123", found.RegistrationNumber);
        }

        [Theory]
        [InlineData("XYZ999")]
        [InlineData("")]
        [InlineData("   ")]
        public void FindByRegistrationNumber_UnknownOrEmpty_ReturnsNull(string searched)
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(2);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

            // Act
            Vehicle? found = garage.FindByRegistrationNumber(searched);

            // Assert
            Assert.Null(found);
        }

        [Fact]
        public void GetEnumerator_ThreeParkedVehicles_ForeachVisitsAllThree()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(5);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
            garage.Park(new Bus("BUS100", "Yellow", 6, 50));
            garage.Park(new Boat("BOA001", "White", 8.5));

            // Act
            int visited = 0;
            foreach (Vehicle vehicle in garage)
            {
                visited++;
            }

            // Assert
            Assert.Equal(3, visited);
        }

        [Fact]
        public void Garage_IsEnumerable_LinqWorksOnGarage()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(5);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
            garage.Park(new Car("DEF456", "Red", 4, FuelType.Electric));
            garage.Park(new Bus("BUS100", "Black", 6, 50));

            // Act: LINQ works because Garage<T> implements IEnumerable<T>
            int blackVehicles = garage.Count(v => v.Color == "Black");

            // Assert
            Assert.Equal(2, blackVehicles);
        }

        [Fact]
        public void FreeSpaces_TwoParkedInGarageOfFive_ReturnsThree()
        {
            // Arrange
            Garage<Vehicle> garage = CreateGarage(5);
            garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
            garage.Park(new Car("DEF456", "Red", 4, FuelType.Diesel));

            // Act
            int freeSpaces = garage.FreeSpaces;

            // Assert
            Assert.Equal(3, freeSpaces);
            Assert.False(garage.IsFull);
        }
    }
}