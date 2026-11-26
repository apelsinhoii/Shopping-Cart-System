using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;
using Xunit;

namespace Shopping_Cart_System.Tests.Properties
{


    public class RepositoryProperties
    {
        // P5 · Тестування за допомогою оракула.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void SaveCart_ThroughService_MatchesDirectSave(
            CartItem[] items)
        {
            var repo1 = new InMemoryCartRepository();
            var repo2 = new InMemoryCartRepository();

            var service = new CartService(repo1);

            var cart1 = new ShoppingCart();
            var cart2 = new ShoppingCart();

            foreach (var item in items)
            {
                cart1.Add(item.Product, item.Quantity);
                cart2.Add(item.Product, item.Quantity);
            }

            service.SaveCart(cart1);
            repo2.Save(cart2);

            Assert.Single(repo1.All());
            Assert.Single(repo2.All());

            var savedCart1 = repo1.All().Single();
            var savedCart2 = repo2.All().Single();

            Assert.Equal(savedCart2.Items.Count, savedCart1.Items.Count);
            Assert.Equal(savedCart2.TotalPrice(), savedCart1.TotalPrice());

            Assert.Equal(
                savedCart2.Items.OrderBy(i => i.Product.Id)
                    .Select(i => (i.Product, i.Quantity)),
                savedCart1.Items.OrderBy(i => i.Product.Id)
                    .Select(i => (i.Product, i.Quantity)));
        }
    }
}
