using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Linq;
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

        private (ProductService, P3Referential) InitializeServices()
        {
            var ctx = new P3Referential(_options, _configuration);
            var productRepository = new ProductRepository(ctx);
            var orderRepository = new OrderRepository(ctx);
            var cart = new Cart();
            var productService = new ProductService(cart, productRepository, orderRepository, _localizer);

            return (productService, ctx);
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
            var (productService, ctx) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product from CREATE integration test");

            int initialCount = await ctx.Product.CountAsync();

            // Act
            productService.SaveProduct(productViewModel);

            // Assert
            Assert.Equal(initialCount + 1, await ctx.Product.CountAsync());

            var product = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            // Clean up
            ctx.Product.Remove(product);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteProduct()
        {
            var (productService, ctx) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product from DELETE integration test");

            productService.SaveProduct(productViewModel);

            var product = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            int initialCount = await ctx.Product.CountAsync();

            // Act
            productService.DeleteProduct(product.Id);

            // Assert
            Assert.Equal(initialCount - 1, await ctx.Product.CountAsync());

            var deletedProduct = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task UpdateProductStock()
        {
            var (productService, ctx) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product for UPDATE integration test", "150", "10");

            // Step 1: Create the product
            productService.SaveProduct(productViewModel);

            var product = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(product);

            // Step 2: Add the product to the cart and update quantities
            var cart = new Cart();
            cart.AddItem(product, 9);
            productService.UpdateProductQuantities();

            // Step 3: Retrieve the product again to verify the update
            var updatedProduct = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
            Assert.NotNull(updatedProduct);
            Assert.Equal(1, updatedProduct.Quantity); // 10 initial stock - 9 removed by cart

            // Clean up
            ctx.Product.Remove(updatedProduct);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductInfo()
        {
            var (productService, ctx) = InitializeServices();
            var productViewModel = CreateTestProductViewModel("Product for GET integration test");

            productService.SaveProduct(productViewModel);

            var product = await ctx.Product.FirstOrDefaultAsync(x => x.Name == productViewModel.Name);
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
            ctx.Product.Remove(product);
            await ctx.SaveChangesAsync();
        }
    }
}
