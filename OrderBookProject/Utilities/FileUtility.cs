using System;
using System.Collections.Generic;
using System.IO;
using OrderBookProject.Models;
using OrderBookProject.OrderBooks;

namespace OrderBookProject.Utilities
{
    public static class FileUtility
    {
        // Reads a binary file (big-endian) and parses each tick into an Order.
        // Assumes full record is always available (no partial reads).
        public static List<Order> ReadOrdersFromFile(string filePath)
        {
            var orders = new List<Order>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var order = new Order
                    {
                        SourceTime = ReadInt64BigEndian(reader),
                        Side = (char)reader.ReadByte(),
                        Action = (char)reader.ReadByte(),
                        OrderId = ReadInt64BigEndian(reader),
                        Price = ReadInt32BigEndian(reader),
                        Qty = ReadInt32BigEndian(reader)
                    };
                    orders.Add(order);
                }
            }
            return orders;
        }
        // Processes and outputs orders with computed B0/A0 state per row.
        public static void WriteOrdersToFile(string filePath, List<Order> orders, OrderBook orderBook)
        {
            using (var writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
            {
                // Header
                writer.WriteLine("SourceTime;Side;Action;OrderId;Price;Qty;B0;BQ0;BN0;A0;AQ0;AN0");

                foreach (var order in orders)
                {
                    orderBook.ProcessOrder(order);

                    var (bPrice, bQty, bCount) = orderBook.GetBestBid();
                    var (aPrice, aQty, aCount) = orderBook.GetBestAsk();

                    // Only blank out values if the count is zero (i.e. no orders at all)
                    string bPriceStr = bCount == 0 ? "" : bPrice.ToString();
                    string bQtyStr = bCount == 0 ? "" : bQty.ToString();
                    string bCountStr = bCount == 0 ? "" : bCount.ToString();

                    string aPriceStr = aCount == 0 ? "" : aPrice.ToString();
                    string aQtyStr = aCount == 0 ? "" : aQty.ToString();
                    string aCountStr = aCount == 0 ? "" : aCount.ToString();

                    string sideStr = order.Side.HasValue ? order.Side.ToString() : "";
                    string actionStr = order.Action.HasValue ? order.Action.ToString() : "";

                    writer.WriteLine($"{order.SourceTime};{sideStr};{actionStr};{order.OrderId};{order.Price};{order.Qty};" +
                                     $"{bPriceStr};{bQtyStr};{bCountStr};{aPriceStr};{aQtyStr};{aCountStr}");
                }
            }
        }
        // This function is added for testing to test the example input file with output sample
        public static void DecodeRawToCsv(string rawFilePath, string outputCsvPath)
        {
            var orders = ReadOrdersFromFile(rawFilePath);

            using (var writer = new StreamWriter(outputCsvPath, false, new System.Text.UTF8Encoding(true)))
            {
                writer.WriteLine("SourceTime;Side;Action;OrderId;Price;Qty");

                foreach (var order in orders)
                {
                    writer.WriteLine($"{order.SourceTime};{order.Side};{order.Action};{order.OrderId};{order.Price};{order.Qty}");
                }
            }
            Console.WriteLine($"Decoded raw binary input to: {outputCsvPath}");
        }

        public static List<Order> ReadOrdersFromDecodedCsv(string filePath)
        {
            var orders = new List<Order>();
            var lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                var parts = lines[i].Split(';');

                if (parts.Length < 6) continue;

                var order = new Order
                {
                    SourceTime = long.Parse(parts[0]),
                    Side = string.IsNullOrWhiteSpace(parts[1]) ? (char?)null : parts[1][0],
                    Action = string.IsNullOrWhiteSpace(parts[2]) ? (char?)null : parts[2][0],
                    OrderId = long.Parse(parts[3]),
                    Price = int.Parse(parts[4]),
                    Qty = int.Parse(parts[5])
                };

                orders.Add(order);
            }

            return orders;
        }
        private static long ReadInt64BigEndian(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        private static int ReadInt32BigEndian(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}