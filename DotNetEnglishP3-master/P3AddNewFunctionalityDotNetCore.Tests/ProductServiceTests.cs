﻿using Xunit;
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
            var products = _productService.GetAllProducts();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(testProducts.Count, products.Count);
            Assert.Equal(testProducts[1].Id, products[1].Id);
            Assert.Equal(testProducts[1].Name, products[1].Name);
        }

        [Fact]
        public void GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            //Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Test product" };
            _productRepositoryMock.Setup(repo => repo.GetAllProducts()).Returns(new List<Product> { expectedProduct });

            //Act
            var actualProduct = _productService.GetProductById(productId);

            //Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.Id, actualProduct.Id);
            Assert.Equal(expectedProduct.Name, actualProduct.Name);
        }

        [Fact]
        public void GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _productRepositoryMock.Setup(repo => repo.GetAllProducts()).Returns(new List<Product>());

            // Act
            var actualProduct = _productService.GetProductById(productId);

            // Assert
            Assert.Null(actualProduct);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
        {
            //Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Test Product" };
            _productRepositoryMock.Setup(repo => repo.GetProduct(productId)).ReturnsAsync(expectedProduct);

            //Act
            var actualProduct = await _productService.GetProduct(productId);

            //Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.Id, actualProduct.Id);
            Assert.Equal(expectedProduct.Name,actualProduct.Name);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNull_WhenProductDoesntExist()
        {
            //Arrange
            var productId = 1;
            _productRepositoryMock.Setup(repo => repo.GetProduct(productId)).ReturnsAsync((Product)null);

            //Act
            var actualProduct = await (_productService.GetProduct(productId));

            //Assert
            Assert.Null(actualProduct);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
        {
            new Product { Id = 1, Quantity = 10, Price = 100.0, Name = "Product 1", Description = "Description 1", Details = "Details 1" },
            new Product { Id = 2, Quantity = 20, Price = 200.0, Name = "Product 2", Description = "Description 2", Details = "Details 2" }
        };
            _productRepositoryMock.Setup(repo => repo.GetProduct()).ReturnsAsync(expectedProducts);

            // Act
            var actualProducts = await _productService.GetProduct();

            // Assert
            Assert.NotNull(actualProducts);
            Assert.Equal(expectedProducts.Count, actualProducts.Count);
            Assert.Equal(expectedProducts[0].Id, actualProducts[0].Id);
            Assert.Equal(expectedProducts[0].Name, actualProducts[0].Name);
        }
    }
}