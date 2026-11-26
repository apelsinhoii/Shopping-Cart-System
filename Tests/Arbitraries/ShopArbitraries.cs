using FsCheck;
using FsCheck.Fluent;
using Shopping_Cart_System.Domain;
using System;

namespace Shopping_Cart_System.Tests.Arbitraries
{
    // Зразки з проєкту лекції: FuelTankGen (залежні значення), VehicleGen (генератор з генератора).
    // Кошик (ShoppingCart) генератора не потребує: створіть new ShoppingCart() у тесті й додайте
    // товари з масиву Product[] — FsCheck побудує масив сам, як Vehicle[] у тестах автопарку.
    public static class ShopArbitraries
    {
        // Непорожній рядок для Id і назви.
        // Підказка: Gen.Choose(1, 10).Select(n => $"P{n}") або Gen.Elements("Apple", "Milk", ...).
        // Невеликий набір Id робить повтори частими — це потрібно для перевірки повторного Add.
        private static Gen<string> NonEmptyAlphaString()
        {
            return Gen.Choose(1, 10)
              .Select(n => $"P{n}");
        }

        // Product: Id і Name — непорожні, Price >= 0 (decimal), напр. Gen.Choose(0, 100_000).Select(c => c / 100m).
        public static Arbitrary<Product> Product()
        {
            var gen =
                from id in NonEmptyAlphaString()
                from name in NonEmptyAlphaString()
                from price in Gen.Choose(0, 100_000).Select(c => c / 100m)
                select new Product(id, name, price);
           
            return Arb.From(gen);
        }

        // Кількість для конструктора CartItem, Add та Increase: завжди >= 1.
        public static Arbitrary<int> Quantity()
        {
            var gen = Gen.Choose(1, 100);

            return Arb.From(gen);
        }

        // CartItem = Product + кількість (як VehicleGen будується з FuelTankGen):
        // використайте Product().Generator і Quantity().Generator.
        public static Arbitrary<CartItem> CartItem()
        {
            var gen =
                from product in Product().Generator
                from quantity in Quantity().Generator
                select new CartItem(product, quantity);

            return Arb.From(gen);
        }
    }
}
