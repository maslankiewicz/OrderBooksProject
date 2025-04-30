using NUnit.Framework;
using System.Collections.Generic;
using OrderBookProject.Models;
using OrderBookProject.OrderBooks;

namespace OrderBookProject.Tests
{
    [TestFixture]
    public class OrderBookTests
    {
        private OrderBook orderBook;

        [SetUp]
        public void Setup()
        {
            orderBook = new OrderBook();
        }

        [Test]
        public void AddOrder_ShouldAppearInBestBid()
        {
            var order = new Order
            {
                Side = '1',
                Action = 'A',
                OrderId = 1,
                Price = 100,
                Qty = 5
            };

            orderBook.ProcessOrder(order);
            var (price, qty, count) = orderBook.GetBestBid();

            Assert.AreEqual(100, price);
            Assert.AreEqual(5, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void AddMultipleOrders_ShouldKeepBestBid()
        {
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 5 });
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 2, Price = 105, Qty = 2 });

            var (price, qty, count) = orderBook.GetBestBid();
            Assert.AreEqual(105, price);
            Assert.AreEqual(2, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void RemoveOrder_ShouldUpdateBestBid()
        {
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 5 });
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 2, Price = 105, Qty = 2 });

            orderBook.ProcessOrder(new Order { Side = '1', Action = 'D', OrderId = 2, Price = 105, Qty = 0 });

            var (price, qty, count) = orderBook.GetBestBid();
            Assert.AreEqual(100, price);
            Assert.AreEqual(5, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ClearOrderBook_ShouldWipeSide()
        {
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 1 });
            orderBook.ProcessOrder(new Order { Side = '2', Action = 'A', OrderId = 2, Price = 110, Qty = 1 });

            orderBook.ProcessOrder(new Order { Side = '1', Action = 'F' });

            var (bPrice, _, _) = orderBook.GetBestBid();
            var (aPrice, _, _) = orderBook.GetBestAsk();

            Assert.AreEqual(0, bPrice); // wiped bid side
            Assert.AreEqual(110, aPrice); // ask remains
        }

        [Test]
        public void UpdateOrder_ShouldReplaceQty()
        {
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'A', OrderId = 1, Price = 100, Qty = 5 });
            orderBook.ProcessOrder(new Order { Side = '1', Action = 'M', OrderId = 1, Price = 100, Qty = 3 });

            var (_, qty, _) = orderBook.GetBestBid();
            Assert.AreEqual(3, qty);
        }

        [Test]
        public void GetBestAsk_ShouldReturnLowestAsk()
        {
            orderBook.ProcessOrder(new Order { Side = '2', Action = 'A', OrderId = 1, Price = 110, Qty = 1 });
            orderBook.ProcessOrder(new Order { Side = '2', Action = 'A', OrderId = 2, Price = 105, Qty = 1 });

            var (price, qty, count) = orderBook.GetBestAsk();
            Assert.AreEqual(105, price);
            Assert.AreEqual(1, qty);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void EmptyBook_ShouldReturnZeroForBestBidAsk()
        {
            var (bPrice, bQty, bCount) = orderBook.GetBestBid();
            var (aPrice, aQty, aCount) = orderBook.GetBestAsk();

            Assert.AreEqual(0, bPrice);
            Assert.AreEqual(0, bQty);
            Assert.AreEqual(0, bCount);
            Assert.AreEqual(0, aPrice);
            Assert.AreEqual(0, aQty);
            Assert.AreEqual(0, aCount);
        }
    }
}