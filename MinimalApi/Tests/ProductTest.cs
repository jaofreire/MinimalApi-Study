using ModelsLibrary;
using ProductsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helper;

namespace Tests
{
    public class ProductTest
    {
        [Fact]
        public async void TestRoutes()
        {
            //Arrange
            int id = 1;
            string name = "apple_juice";

            var product = new ProductModel()
            {
                Id = id,
                Name = name 
            };

            var validator = new ProductsValidator();

            await using var context = new MockDb().CreateDbContext();


            //Act
            var result = RouteExtension.RegisterNewProduct(product, context, validator);


            //Asserts
            await Assert.IsType<Task<ProductModel>>(result);
        }
    }
}
