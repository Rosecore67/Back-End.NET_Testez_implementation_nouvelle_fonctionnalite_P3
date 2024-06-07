using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Xunit;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductViewModelTests
    {
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
            return validationResults;
        }

        [Fact]
        public void ProductViewModel_ShouldRequireName()
        {
            // Arrange
            var model = new ProductViewModel { Price = "10.00", Stock = "5" };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Name") && vr.ErrorMessage == "ErrorMissingName");
        }

        [Fact]
        public void ProductViewModel_ShouldRequireStock()
        {
            // Arrange
            var model = new ProductViewModel { Name = "Test Product", Price = "10.00" };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Stock") && vr.ErrorMessage == "ErrorMissingStock");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("-5")]
        [InlineData("0")]
        public void ProductViewModel_ShouldValidateStock(string stock)
        {
            // Arrange
            var model = new ProductViewModel { Name = "Test Product", Price = "10.00", Stock = stock };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Stock"));
        }

        [Fact]
        public void ProductViewModel_ShouldRequirePrice()
        {
            // Arrange
            var model = new ProductViewModel { Name = "Test Product", Stock = "5" };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Price") && vr.ErrorMessage == "ErrorMissingPrice");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("-10.00")]
        [InlineData("0")]
        [InlineData("10.001")]
        public void ProductViewModel_ShouldValidatePrice(string price)
        {
            // Arrange
            var model = new ProductViewModel { Name = "Test Product", Price = price, Stock = "5" };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Price"));
        }

        [Fact]
        public void ProductViewModel_ValidModel_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Details = "Test Details",
                Price = "10.00",
                Stock = "5"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Empty(validationResults);
        }
    }
}
