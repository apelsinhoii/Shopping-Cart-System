using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;
using Xunit;
using FsCheck;
using FluentAssertions;

namespace Shopping_Cart_System.Tests.Properties
{
    public class ItemProperties
    {
        // P2 · Інваріант.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void CartItem_Quantity_IsAlwaysPositive(
            CartItem item, int amount)
        {
            Assert.True(item.Quantity > 0);

            item.Increase(amount);

            Assert.True(item.Quantity > 0);

            item.Decrease(amount);

            Assert.True(item.Quantity > 0);
        }

        // P1 · Туди й назад.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Increase_ThenDecrease_RestoresQuantity(
            CartItem item, int amount)
        {
            int initialQuantity = item.Quantity;

            item.Increase(amount);

            Assert.True(item.Quantity > initialQuantity);

            item.Decrease(amount);

            Assert.Equal(initialQuantity, item.Quantity);
        }

        // P7 · Легко перевірити — з передумовою When.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Decrease_ReducesQuantityCorrectly(
            CartItem item, int amount)
        {
            int initialQuantity = item.Quantity;

            bool applicable = amount < initialQuantity;
            bool ok = true;

            if (applicable)
            {
                item.Decrease(amount);

                int removedQuantity = initialQuantity - item.Quantity;

                ok = removedQuantity == amount
                     && item.Quantity > 0;
            }

            return ok.When(applicable);
        }

        // P2 · Інваріант — стан не змінюється після некоректної дії.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public void Increase_InvalidAmount_Throws(
            CartItem item, NegativeInt amount)
        {
            int initialQuantity = item.Quantity;

            Action act = () => item.Increase(amount.Get);

            act.Should().ThrowExactly<InvalidOperationException>();

            item.Quantity.Should().Be(initialQuantity);

            Action zeroAct = () => item.Increase(0);

            zeroAct.Should().ThrowExactly<InvalidOperationException>();

            item.Quantity.Should().Be(initialQuantity);
        }

        // P2 · Інваріант — стан не змінюється після некоректної дії.
        // Перевірка винятку з передумовою When.
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Decrease_TooMuch_Throws(
            CartItem item, int amount)
        {
            int initialQuantity = item.Quantity;

            bool applicable = amount >= initialQuantity;
            bool ok = true;

            if (applicable)
            {
                Action act = () => item.Decrease(amount);

                act.Should().ThrowExactly<InvalidOperationException>();

                ok = item.Quantity == initialQuantity;
            }

            return ok.When(applicable);
        }
    }
}
