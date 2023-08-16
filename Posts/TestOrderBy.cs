using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrugalCafe.Posts
{
    public class Product
    {
        public string Name;
        public double Price;
    }

    internal class TestOrderBy
    {
        public static void Test()
        {
            var products = new List<Product>();

            products.Add(new Product() { Name = "Milk", Price = 3.14 });
            products.Add(new Product() { Name = "Egg", Price = 4.00 });

            var results = products.OrderBy(item => item.Name).ThenBy(item => item.Price).ToList();

            var results2 = new List<Product>(products);

            results.Sort((x, y) =>
            { 
                int diff = x.Name.CompareTo(y.Name);

                if (diff == 0)
                {
                    diff = x.Price.CompareTo(y.Price);
                }

                return diff; 
            });
        }
    }
}
