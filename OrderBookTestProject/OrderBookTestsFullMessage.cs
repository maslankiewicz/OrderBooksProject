using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using OrderBookProject.Models;
using OrderBookProject.OrderBooks;
using OrderBookProject.Utilities;

namespace OrderBookProject.Tests
{
    [TestFixture]
    public class OrderBookTestsFullMessage
    {
        private string resourceDir;
        private string inputDecodedCsv;
        private string expectedResultCsv;
        private string actualResultCsv;

        [SetUp]
        public void SetUp()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            resourceDir = Path.Combine(baseDir, "Resources");

            inputDecodedCsv = Path.Combine(resourceDir, "ticks_sample.csv");
            expectedResultCsv = Path.Combine(resourceDir, "ticks_result_sample.csv");
            actualResultCsv = Path.Combine(resourceDir, "ticks_result_from_decoded.csv");
        }

        [Test]
        public void TestDecodedCsvProducesCorrectResult()
        {
            // Ensure files exist
            Assert.IsTrue(File.Exists(inputDecodedCsv), $"Missing input: {inputDecodedCsv}");
            Assert.IsTrue(File.Exists(expectedResultCsv), $"Missing expected output: {expectedResultCsv}");

            var orders = FileUtility.ReadOrdersFromDecodedCsv(inputDecodedCsv);
            var orderBook = new OrderBook();

            FileUtility.WriteOrdersToFile(actualResultCsv, orders, orderBook);

            var expectedLines = File.ReadAllLines(expectedResultCsv);
            var actualLines = File.ReadAllLines(actualResultCsv);

            Assert.AreEqual(expectedLines.Length, actualLines.Length, "Line count mismatch.");

            for (int i = 0; i < expectedLines.Length; i++)
            {
                string expected = expectedLines[i].Trim();
                string actual = actualLines[i].Trim();
                Assert.AreEqual(expected, actual, $"Mismatch at line {i + 1}:\nExpected: {expected}\nActual:   {actual}");
            }
        }
    }
}