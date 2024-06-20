using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class IntegrationTestsProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<ProductService> _localizer;
        private readonly DbContextOptions<P3Referential> _options;

        public IntegrationTestsProductService()
        {
            _options = new DbContextOptionsBuilder<P3Referential>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;Trust Server Certificate=True;MultipleActiveResultSets=true")
                .Options;
        }

        private (ProductService, P3Referential, Cart) InitializeServices()
        {
            var dbContext = new P3Referential(_options, _configuration);
            var productRepository = new ProductRepository(dbContext);
            var orderRepository = new OrderRepository(dbContext);
            var cart = new Cart();
            var productService = new ProductService(cart, productRepository, orderRepository, _localizer);

            return (productService, dbContext, cart);
        }

        private ProductViewModel CreateTestProductViewModel(string name, string price = "150", string stock = "1")
        {
            return new ProductViewModel
            {
                Name = name,
                Description = "Description",
                Details = "Detail",
                Stock = stock,
                Price = price
            };
        }

        [Fact]
        public async Task SaveNewProduct()
        {
            var (productService, dbContext, cart) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product from CREATE integration test");

            int initialCount = await dbContext.Product.CountAsync();

            // Act
            productService.SaveProduct(productViewModel);

            // Assert
            Assert.Equal(initialCount + 1, await dbContext.Product.CountAsync());

            var product = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            // Clean up
            dbContext.Product.Remove(product);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteProduct()
        {
            var (productService, dbContext, cart) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product from DELETE integration test");

            productService.SaveProduct(productViewModel);

            var product = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            int initialCount = await dbContext.Product.CountAsync();

            // Act
            productService.DeleteProduct(product.Id);

            // Assert
            Assert.Equal(initialCount - 1, await dbContext.Product.CountAsync());

            var deletedProduct = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task UpdateProductStock()
        {
            // Arrange
            var (productService, dbContext, cart) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product for UPDATE integration test", "150", "10");

            // Step 1: Create the product
            productService.SaveProduct(productViewModel);

            var product = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            // Act
            // Step 2: Add the product to the cart and update quantities
            cart.AddItem(product, 9);
            productService.UpdateProductQuantities();

            // Assert
            // Step 3: Retrieve the product again to verify the update
            var updatedProduct = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(updatedProduct);
            Assert.Equal(1, updatedProduct.Quantity); // 10 initial stock - 9 removed by cart

            // Clean up
            dbContext.Product.Remove(updatedProduct);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductInfo()
        {
            var (productService, dbContext, cart) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product for GET integration test");

            productService.SaveProduct(productViewModel);

            var product = await dbContext.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            // Act
            var productInfo = productService.GetProductByIdViewModel(product.Id);

            // Assert
            Assert.NotNull(productInfo);
            Assert.Equal(product.Name, productInfo.Name);
            Assert.Equal(product.Description, productInfo.Description);
            Assert.Equal(product.Details, productInfo.Details);
            Assert.Equal(product.Quantity.ToString(), productInfo.Stock);
            Assert.Equal(product.Price.ToString(System.Globalization.CultureInfo.InvariantCulture), productInfo.Price);

            // Clean up
            dbContext.Product.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }
}
