using DavidIntraDayReport.Model;
using DavidIntraDayReport.Processors;
using Moq;
using NUnit.Framework.Constraints;
using Services;

namespace DavidIntraDayReport.Tests
{
    public class Tests
    {
        [SetUp]
        public void CheckReportDirCorrectedWhenDoesntExist()
        {
        }

        [Test]
        public void CheckCorrectTradesAggregatedByPeriodNum()
        {
            var ps = new Mock<IPowerService>();

            // setup expected trades and volumes
            var expectedTrade = PowerTrade.Create(DateTime.Now, 3);
            var expectedTrade2 = PowerTrade.Create(DateTime.Now, 3);
            var expectedTrade3 = PowerTrade.Create(DateTime.Now, 3);

            expectedTrade.Periods[0].Volume = 1;
            expectedTrade.Periods[1].Volume = 10;
            expectedTrade.Periods[2].Volume = 100;

            expectedTrade2.Periods[0].Volume = 1;
            expectedTrade2.Periods[1].Volume = 11;
            expectedTrade2.Periods[2].Volume = 100;

            expectedTrade3.Periods[0].Volume = 1;
            expectedTrade3.Periods[1].Volume = 13;
            expectedTrade3.Periods[2].Volume = 100;

            var expectedTrades = new List<PowerTrade>();
            expectedTrades.Add(expectedTrade);
            expectedTrades.Add(expectedTrade2);
            expectedTrades.Add(expectedTrade3);

            ps.Setup(x => x.GetTrades(It.IsAny<DateTime>())).Returns(expectedTrades.AsEnumerable);

            var tradePositions = PowerTradeProcessor.GetTradePositions(ps.Object, null).ToList();

            Assert.AreEqual(3, tradePositions.Count);
            Assert.AreEqual(3, tradePositions[0].Volume);
            Assert.AreEqual(34, tradePositions[1].Volume);
            Assert.AreEqual(300, tradePositions[2].Volume);
        }

        [Test]
        public void CheckPeriodNumbersConvertedToCorrectDateTimes()
        {
            var ps = new Mock<IPowerService>();

            // setup expected trades and volumes
            var expectedTrade = PowerTrade.Create(DateTime.Now, 3);
            var expectedTrade2 = PowerTrade.Create(DateTime.Now, 3);
            var expectedTrade3 = PowerTrade.Create(DateTime.Now, 3);


            var expectedTrades = new List<PowerTrade>();
            expectedTrades.Add(expectedTrade);
            expectedTrades.Add(expectedTrade2);
            expectedTrades.Add(expectedTrade3);

            ps.Setup(x => x.GetTrades(It.IsAny<DateTime>())).Returns(expectedTrades.AsEnumerable);

            var tradePositions = PowerTradeProcessor.GetTradePositions(ps.Object, null).ToList();

            Assert.AreEqual(3, tradePositions.Count);
            Assert.AreEqual(DateTime.Today.AddDays(-1).AddHours(23), tradePositions[0].LocalTime);
            Assert.AreEqual(DateTime.Today, tradePositions[1].LocalTime);
            Assert.AreEqual(DateTime.Today.AddHours(1), tradePositions[2].LocalTime);
        }

        [Test]
        public void CheckWhenNoTradesEmptyEnumerableReturned()
        {
            var ps = new Mock<IPowerService>();
            ps.Setup(x => x.GetTrades(It.IsAny<DateTime>())).Returns(new List<PowerTrade>().AsEnumerable);

            var tradePositions = PowerTradeProcessor.GetTradePositions(ps.Object, null).ToList();

            Assert.AreEqual(0, tradePositions.Count);
        }
    }
}