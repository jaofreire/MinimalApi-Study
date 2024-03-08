using Microsoft.AspNetCore.Http.HttpResults;
using ModelsLibrary;

namespace OrderApi
{
    public static  class OrderEndPointsV1
    {
        public static Created<OrderModel> AddOrder(OrderModel newOrder, ApiDbContext db)
        {
             db.Orders.Add(newOrder);
             db.SaveChanges();

            return TypedResults.Created($"/order/{newOrder.Id}", newOrder);
        }

    }
}
