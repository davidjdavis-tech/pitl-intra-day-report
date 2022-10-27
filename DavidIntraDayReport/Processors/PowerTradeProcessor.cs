using DavidIntraDayReport.Model;
using Microsoft.VisualBasic;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidIntraDayReport.Processors
{
    public static class PowerTradeProcessor
    {
        public static IEnumerable<TradePosition> GetTradePositions(IPowerService powerService, ILogger logger)
        {
            try
            {
                var trades = powerService.GetTrades(DateTime.Today);
                var p = trades.SelectMany(x => x.Periods)
                .GroupBy(x => x.Period)
                .Select(g => new TradePosition()
                {
                    LocalTime = DateTime.Today.AddDays(-1).AddHours(22 + g.Key),
                    Volume = g.Sum(p => p.Volume)
                });

                return p;
            }
            catch (Exception e)
            {
                logger.LogError($"Error in PowerService.GetTrades: {e.Message}");
                return Enumerable.Empty<TradePosition>();
            }
        }
    }
}
