using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using OrderBookProject.Models;
using OrderBookProject.Utilities;
using OrderBookProject.OrderBooks;

namespace OrderBookProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string resourceDir = Path.Combine(projectRoot, "Resources");

            Directory.CreateDirectory(resourceDir);

            string inputFilePath = Path.Combine(resourceDir, "ticks.raw");
            string outputFilePath = Path.Combine(resourceDir, "output.csv");

            Console.WriteLine("Reading orders...");
            List<Order> orders = FileUtility.ReadOrdersFromFile(inputFilePath);
            Console.WriteLine($"Loaded {orders.Count} ticks.");

            var orderBook = new OrderBook();
            var stopwatch = new Stopwatch();

            Console.WriteLine("Processing order book.");

            long totalMicroseconds = 0;

            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];

                stopwatch.Restart();
                orderBook.ProcessOrder(order);
                stopwatch.Stop();

                long tickTimeMicroseconds = stopwatch.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                totalMicroseconds += tickTimeMicroseconds;

                //Console.WriteLine($"Tick {i + 1}/{orders.Count} processed in {tickTimeMicroseconds} µs");
            }

            double avgPerTick = totalMicroseconds / (double)orders.Count;

            Console.WriteLine($"Total order book build time: {totalMicroseconds:N0} µs");
            Console.WriteLine($"average time per tick:       {avgPerTick:N2} µs");

            Console.WriteLine("Writing output...");
            FileUtility.WriteOrdersToFile(outputFilePath, orders, orderBook);

            Console.WriteLine("Done:");
            Console.WriteLine(outputFilePath);
        }
    }
}