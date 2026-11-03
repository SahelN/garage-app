using GarageApp.Vehicles;

namespace GarageApp.Tests
{
    public class VehicleTests
    {
        [Theory]
        [InlineData("ABC123")]   // 3 letters + 3 digits
        [InlineData("ABC12E")]   // last character can be a letter
        [InlineData("abc123")]   // lower case is allowed
        [InlineData("AbC12e")]   // mixed case is allowed
        [InlineData(" ABC123 ")] // spaces around are trimmed
        public void IsValidRegistrationNumber_ValidFormat_ReturnsTrue(string registrationNumber)
        {
            // Act
            bool isValid = Vehicle.IsValidRegistrationNumber(registrationNumber);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]          // empty
        [InlineData("   ")]       // only spaces
        [InlineData("AB1234")]    // only 2 letters
        [InlineData("ABCD12")]    // 4 letters
        [InlineData("ABC1234")]   // too long
        [InlineData("ABC12")]     // too short
        [InlineData("123ABC")]    // wrong order
        [InlineData("ABC 123")]   // space inside
        [InlineData("ÅÄÖ123")]    // Swedish letters are not in A-Z
        [InlineData("XXABC123YY")] // pattern inside other text (that is why we need ^ and $)
        public void IsValidRegistrationNumber_InvalidFormat_ReturnsFalse(string registrationNumber)
        {
            // Act
            bool isValid = Vehicle.IsValidRegistrationNumber(registrationNumber);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidRegistrationNumber_Null_ReturnsFalse()
        {
            // Act
            bool isValid = Vehicle.IsValidRegistrationNumber(null);

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("ABCD12")]
        [InlineData("")]
        [InlineData("123")]
        public void Constructor_InvalidRegistrationNumber_ThrowsArgumentException(string registrationNumber)
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => new Car(registrationNumber, "Black", 4, FuelType.Gasoline));
        }

        [Fact]
        public void Constructor_LowercaseRegistrationNumber_StoresUppercase()
        {
            // Arrange + Act
            Car car = new Car("abc12e", "Black", 4, FuelType.Gasoline);

            // Assert
            Assert.Equal("ABC12E", car.RegistrationNumber);
        }
    }
}