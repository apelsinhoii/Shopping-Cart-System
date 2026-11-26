using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;
using Xunit;
using FsCheck;
using FsCheck.Fluent;


namespace Shopping_Cart_System.Tests.Properties
{
    public class CartProperties
    {
        // P7 · Легко перевірити.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Add_NewProduct_AppearsInCart(
            Product product, int quantity)
        {
            var cart = new ShoppingCart();

            cart.Add(product, quantity);

            Assert.Single(cart.Items);
            Assert.Equal(product, cart.Items.Single().Product);
            Assert.Equal(quantity, cart.Items.Single().Quantity);
        }

        // P4 · Різні шляхи — один результат.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Add_SameProductTwice_CombinesQuantity(
            Product product, int quantity1, int quantity2)
        {
            var cart1 = new ShoppingCart();
            var cart2 = new ShoppingCart();

            cart1.Add(product, quantity1);
            cart1.Add(product, quantity2);

            cart2.Add(product, quantity1 + quantity2);

            Assert.Single(cart1.Items);
            Assert.Single(cart2.Items);

            Assert.Equal(
                cart1.Items.Single().Quantity,
                cart2.Items.Single().Quantity);

            Assert.Equal(cart1.TotalPrice(), cart2.TotalPrice());
        }

        // P1 · Туди й назад.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Remove_ExistingProduct_RemovesIt(
            Product product, int quantity)
        {
            var cart = new ShoppingCart();

            int initialCount = cart.Items.Count;
            decimal initialPrice = cart.TotalPrice();

            cart.Add(product, quantity);

            Assert.Single(cart.Items);

            cart.Remove(product.Id);

            Assert.Equal(initialCount, cart.Items.Count);
            Assert.Equal(initialPrice, cart.TotalPrice());
        }

        // P2 · Інваріант — стан кошика не змінюється після некоректної дії.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Remove_NonExistingProduct_Throws(Product product)
        {
            var cart = new ShoppingCart();

            Action act = () => cart.Remove(product.Id);

            act.Should().ThrowExactly<InvalidOperationException>();

            cart.Items.Should().BeEmpty();
        }

        // P6 · Метаморфічна властивість.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void DoubleQuantity_DoublesTotalPrice(
            Product product, int quantity)
        {
            var cart1 = new ShoppingCart();
            var cart2 = new ShoppingCart();

            cart1.Add(product, quantity);
            cart2.Add(product, quantity * 2);

            Assert.Equal(
                cart1.TotalPrice() * 2,
                cart2.TotalPrice());
        }

        // P2 · Інваріант — вартість кошика з платним товаром додатна.
        // Перевірка з передумовою When.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property PositivePrice_ProducesPositiveTotal(
            Product product, int quantity)
        {
            bool applicable = product.Price > 0;
            bool ok = true;

            if (applicable)
            {
                var cart = new ShoppingCart();

                cart.Add(product, quantity);

                ok = cart.TotalPrice() > 0;
            }

            return ok.When(applicable);
        }
    }
}
