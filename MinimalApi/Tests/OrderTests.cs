using Microsoft.AspNetCore.Http.HttpResults;
using ModelsLibrary;
using OrderApi;
using Tests.Helper;

namespace Tests
{
    public class OrderTests
    {
        [Fact]
        public async void CreateOrder()
        {
            //Arrange
            int userId = 1;
            string address = "Rua Antonio Cardoso da Fonte";
            var item = new OrderItemModel()
            {
                Id = 3,
                OrderId = 350,
                Name = "apple_juice",
                Price = 20
            };
            var order = new OrderModel(userId, address);
            order.AddItems(item);

            await using var context = new MockDb().CreateDbContext();

            //Act
            var result = OrderEndPointsV1.AddOrder(order, context);

            //Asserts
            Assert.IsType<Created<OrderModel>>(result);

            Assert.NotNull(order.Address);
            Assert.NotEmpty(context.Orders);
            Assert.Collection(context.Orders, order =>
            {
                Assert.Equal(userId, order.UserId);
                Assert.Equal(address, order.Address);
                Assert.True(order.OrderItems.Any());
            });

            
            
            
            

        }
    }
}