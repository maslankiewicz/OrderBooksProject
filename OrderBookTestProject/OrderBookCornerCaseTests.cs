using NUnit.Framework;
using OrderBookProject.Models;
using OrderBookProject.OrderBooks;

namespace OrderBookProject.Tests
{
    [TestFixture]
    public class OrderBookCornerCaseTests
    {
        private OrderBook book;

        [SetUp]
        public void Setup()
        {
            book = new OrderBook();
        }

        [Test]
        public void ModifyOrderThatDoesNotExist_ShouldAddIt()
        {
            var order = new Order
            {
                Side = '1',
                Action = 'M',
                OrderId = 1001,
                Price = 123,
                Qty = 5
            };

            book.ProcessOrder(order);

            var (price, qty, count) = book.GetBestBid();
            Assert.AreEqual(123, price);
            Assert.AreEqual(5, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void RemoveOrderThatDoesNotExist_ShouldDoNothing()
        {
            var order = new Order
            {
                Side = '2',
                Action = 'D',
                OrderId = 999,
                Price = 1000,
                Qty = 1
            };

            Assert.DoesNotThrow(() => book.ProcessOrder(order));
            var (price, qty, count) = book.GetBestAsk();
            Assert.AreEqual(0, price);
            Assert.AreEqual(0, qty);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void AddThenModifyOrderWithDifferentPrice_ShouldMoveIt()
        {
            // Order is first added at one price, then moved to another
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 200, Price = 100, Qty = 5 });
            book.ProcessOrder(new Order { Side = '1', Action = 'M', OrderId = 200, Price = 120, Qty = 10 });

            var (price, qty, count) = book.GetBestBid();
            Assert.AreEqual(120, price);
            Assert.AreEqual(10, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ClearBook_ShouldRemoveAllOrdersOnSide()
        {
            // Ensure that clearing BID side doesn't affect ASK side
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 1 });
            book.ProcessOrder(new Order { Side = '2', Action = 'A', OrderId = 2, Price = 200, Qty = 1 });

            book.ProcessOrder(new Order { Side = '1', Action = 'F' });

            var (bPrice, _, _) = book.GetBestBid();
            var (aPrice, _, _) = book.GetBestAsk();

            Assert.AreEqual(0, bPrice);     // BID side cleared
            Assert.AreEqual(200, aPrice);   // ASK side unaffected
        }

        [Test]
        public void BestBid_ShouldAggregateQuantitiesCorrectly()
        {
            // Test multiple orders at same price
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 2 });
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 2, Price = 100, Qty = 3 });
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 3, Price = 100, Qty = 5 });

            var (price, qty, count) = book.GetBestBid();
            Assert.AreEqual(100, price);
            Assert.AreEqual(10, qty);   // 2 + 3 + 5
            Assert.AreEqual(3, count);  // 3 orders
        }

        [Test]
        public void AllOrdersRemoved_ShouldReturnEmptyBestBidAsk()
        {
            // Book starts full and ends empty after removals
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 10, Price = 100, Qty = 1 });
            book.ProcessOrder(new Order { Side = '2', Action = 'A', OrderId = 20, Price = 200, Qty = 1 });

            book.ProcessOrder(new Order { Side = '1', Action = 'D', OrderId = 10, Price = 100, Qty = 0 });
            book.ProcessOrder(new Order { Side = '2', Action = 'D', OrderId = 20, Price = 200, Qty = 0 });

            var (bPrice, _, _) = book.GetBestBid();
            var (aPrice, _, _) = book.GetBestAsk();

            Assert.AreEqual(0, bPrice);
            Assert.AreEqual(0, aPrice);
        }

        [Test]
        public void ValidOrderAtPriceZero_ShouldAppearInOutput()
        {
            // Orders at price 0 are allowed (edge case)
            book.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 5000, Price = 0, Qty = 1 });

            var (bPrice, _, _) = book.GetBestBid();
            Assert.AreEqual(0, bPrice);   // Should not be blank or filtered
        }
    }
}