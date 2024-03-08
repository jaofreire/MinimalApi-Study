using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary
{
    public class OrderModel
    {
        public OrderModel(int userId, string? address)
        {
            UserId = userId;
            Address = address;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Address { get; set; }
        public List<OrderItemModel> OrderItems { get; set; } = new();

        public void AddItems(OrderItemModel item)
        {
            OrderItems.Add(item);
        }
    }
}
