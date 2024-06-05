using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models;


namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IStringLocalizer<ProductService>> _localizerMock;
        private readonly Mock<ICart> _cartMock;
        
        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _localizerMock = new Mock<IStringLocalizer<ProductService>>();
            _cartMock = new Mock<ICart>();
            _productService = new ProductService(
                _cartMock.Object,
                _productRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _localizerMock.Object);
        }

        [Fact]
        public void GetAllProducts_ReturnAllProducts()
        {
            // Arrange
            var testProducts = new List<Product> 
            {
                new Product { Id = 1, Quantity = 20, Price = 55.45, Name = "Enceinte Stéréo", Description ="Haut-parleurs", Details = "Haute qualité"},
                new Product { Id = 2, Quantity = 75, Price = 99, Name = "Livre", Description = "Livre rare", Details = "1ére édition"}
            };
            _productRepositoryMock.Setup(repo => repo.GetAllProducts()).Returns(testProducts);

            // Act
            var oneProductTest = _productService.GetAllProducts();

            // Assert
            Assert.NotNull(oneProductTest);
            Assert.Equal(testProducts.Count, oneProductTest.Count);
            Assert.Equal(testProducts[1].Id, oneProductTest[1].Id);
            Assert.Equal(testProducts[1].Name, oneProductTest[1].Name);
        }

    }
}