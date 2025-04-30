using System.Collections.Generic;
using System.Linq;
using OrderBookProject.Models;

namespace OrderBookProject.OrderBooks
{
    public class OrderBook
    {
        private readonly SortedDictionary<int, SortedDictionary<long, Order>> bidOrders =
            new SortedDictionary<int, SortedDictionary<long, Order>>(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        private readonly SortedDictionary<int, SortedDictionary<long, Order>> askOrders =
            new SortedDictionary<int, SortedDictionary<long, Order>>();

        public void ProcessOrder(Order order)
        {
            if (!order.Action.HasValue)
                return; 

            switch (order.Action.Value)
            {
                case 'Y':
                case 'F':
                    if (order.Side.HasValue)
                        ClearOrderBook(order.Side.Value);
                    break;

                case 'A':
                case 'M':
                    AddOrUpdateOrder(order);
                    break;

                case 'D':
                    RemoveOrder(order);
                    break;
            }
        }

        private void ClearOrderBook(char side)
        {
            if (side == '1') bidOrders.Clear();
            else if (side == '2') askOrders.Clear();
        }

        private void AddOrUpdateOrder(Order order)
        {
            var book = order.Side == '1' ? bidOrders : askOrders;

            if (!book.TryGetValue(order.Price, out var priceLevel))
            {
                priceLevel = new SortedDictionary<long, Order>();
                book[order.Price] = priceLevel;
            }

            priceLevel[order.OrderId] = order;
        }

        private void RemoveOrder(Order order)
        {
            var book = order.Side == '1' ? bidOrders : askOrders;

            if (book.TryGetValue(order.Price, out var priceLevel))
            {
                priceLevel.Remove(order.OrderId);

                if (priceLevel.Count == 0)
                    book.Remove(order.Price);
            }
        }

        public (int Price, int Qty, int Count) GetBestBid()
        {
            if (!bidOrders.Any()) return (0, 0, 0);

            var bestPrice = bidOrders.First().Key;
            var ordersAtPrice = bidOrders[bestPrice].Values;

            int qty = ordersAtPrice.Sum(o => o.Qty);
            int count = ordersAtPrice.Count();

            return (bestPrice, qty, count);
        }

        public (int Price, int Qty, int Count) GetBestAsk()
        {
            if (!askOrders.Any()) return (0, 0, 0);

            var bestPrice = askOrders.First().Key;
            var ordersAtPrice = askOrders[bestPrice].Values;

            int qty = ordersAtPrice.Sum(o => o.Qty);
            int count = ordersAtPrice.Count();

            return (bestPrice, qty, count);
        }
    }
}
