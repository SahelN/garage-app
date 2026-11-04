using GarageApp.Vehicles;

namespace GarageApp.Tests
{
    public class GarageHandlerTests
    {
        [Fact]
        public void CreateGarage_TwoGarages_SecondIsCurrent()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();

            // Act
            handler.CreateGarage("Garage", 5);
            handler.CreateGarage("Hangar", 3);

            // Assert
            Assert.Equal("Hangar", handler.CurrentGarageName);
            Assert.Equal(2, handler.GetGarageSummaries().Count);
        }

        [Fact]
        public void CreateGarage_SameNameDifferentCase_ThrowsArgumentException()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Hangar", 3);

            // Act + Assert
            Assert.Throws<ArgumentException>(() => handler.CreateGarage("hangar", 5));
        }

        [Fact]
        public void SelectGarage_ExistingName_ChangesCurrentGarage()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Garage", 5);
            handler.CreateGarage("Hangar", 3);

            // Act
            bool wasSelected = handler.SelectGarage("garage");

            // Assert
            Assert.True(wasSelected);
            Assert.Equal("Garage", handler.CurrentGarageName);
        }

        [Fact]
        public void SelectGarage_UnknownName_ReturnsFalseAndKeepsCurrent()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Garage", 5);

            // Act
            bool wasSelected = handler.SelectGarage("Harbor");

            // Assert
            Assert.False(wasSelected);
            Assert.Equal("Garage", handler.CurrentGarageName);
        }

        [Fact]
        public void Park_SameRegistrationNumberInOtherGarage_ReturnsDuplicateRegistrationNumber()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Garage", 5);
            handler.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
            handler.CreateGarage("Hangar", 3);

            // Act
            ParkResult result = handler.Park(new Airplane("abc123", "White", 3, 2));

            // Assert
            Assert.Equal(ParkResult.DuplicateRegistrationNumber, result);
            Assert.Equal(0, handler.ParkedCount);
        }

        [Fact]
        public void Park_TwoGarages_VehicleOnlyInCurrentGarage()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Garage", 5);
            handler.CreateGarage("Hangar", 3);

            // Act: Hangar is current
            handler.Park(new Airplane("PLN123", "White", 3, 2));
            handler.SelectGarage("Garage");

            // Assert
            Assert.Equal(0, handler.ParkedCount);
            Assert.Equal("Hangar", handler.FindGarageNameFor("pln123"));
        }

        [Fact]
        public void FindGarageNameFor_UnknownRegistrationNumber_ReturnsNull()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();
            handler.CreateGarage("Garage", 5);

            // Act
            string? garageName = handler.FindGarageNameFor("XYZ999");

            // Assert
            Assert.Null(garageName);
        }

        [Fact]
        public void HasGarage_NoGarageCreated_ReturnsFalse()
        {
            // Arrange
            GarageHandler handler = new GarageHandler();

            // Act + Assert
            Assert.False(handler.HasGarage);
            Assert.Throws<InvalidOperationException>(() => handler.ParkedCount);
        }
    }
}