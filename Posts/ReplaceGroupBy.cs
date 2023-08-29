using System;
using System.Collections.Generic;
using System.Linq;

namespace FrugalCafe.Posts
{
    internal class Order : IEquatable<Order>
    {
        public int CustomerId;
        public double Amount;

        public bool Equals(Order other)
        {
            return this.CustomerId == other.CustomerId;
        }

        public override int GetHashCode()
        {
            return this.CustomerId;
        }
    }

    internal class ReplaceGroupBy
    {
        public static void Test()
        {
            var orders = new List<Order>
            {
                new Order { CustomerId = 1, Amount = 100 },
                new Order { CustomerId = 2, Amount = 50 },
                new Order { CustomerId = 1, Amount = 150 },
                new Order { CustomerId = 3, Amount = 200 },
            };

            var totalAmountPerCustomer = orders.GroupBy(o => o.CustomerId)
                .Select(g => new Order
                {
                    CustomerId = g.Key,
                    Amount = g.Sum(Order => Order.Amount)
                })
                .ToList();

            var result = new HashSet<Order>();

            foreach (var order in orders)
            {
                if (result.TryGetValue(order, out var old))
                {
                    old.Amount += order.Amount;
                }
                else
                {
                    result.Add(new Order { CustomerId = order.CustomerId, Amount = order.Amount });
                }    
            }
         }
    }
}
