using DavidIntraDayReport.Model;
using DavidIntraDayReport.Processors;
using Services;
using static Services.PowerService;

namespace DavidIntraDayReport
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly WorkerOptions _options;
        private readonly PowerService _powerService;
        private readonly int _intervalMs;

        public Worker(ILogger<Worker> logger, WorkerOptions options)
        {
            _logger = logger;
            _options = options;
            _intervalMs = (int)options.IntervalMinutes * 1000 * 60;
            _powerService = new PowerService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}. Interval {interval} minutes", DateTimeOffset.Now, _options.IntervalMinutes);

                if (!Directory.Exists(_options.ReportPath))
                {
                    Directory.CreateDirectory(_options.ReportPath);
                }

                _logger.LogInformation($"Retrieving Trade Data..");
                var tradePositions = PowerTradeProcessor.GetTradePositions(_powerService, _logger);

                if (!tradePositions.Any())
                {
                    _logger.LogWarning("No trades retrieved. Skipping this export.");
                }
                else
                {
                    ExportTradePositions(tradePositions);
                }

                await Task.Delay(_intervalMs, stoppingToken);
            }
        }

        private void ExportTradePositions(IEnumerable<TradePosition> tradePositions)
        {
            string fullPath = GetExportFilename();

            _logger.LogInformation("Exporting Trade Positions...");

            var lines = new List<string>();
            lines.Add($"{_options.LocalTimeHeaderName}{_options.CSVSeparator}{_options.VolumeHeaderName}");

            foreach (var pos in tradePositions)
            {
                lines.Add($"{pos.LocalTime.ToString("HH:mm")}{_options.CSVSeparator}{pos.Volume}");
            }

            File.WriteAllLines(fullPath, lines.ToArray());

            _logger.LogInformation("Export Complete.");
        }

        private string GetExportFilename()
        {
            var filename = _options.ReportFilename.Replace("YYYYMMDD", DateTime.Now.ToString("yyyyMMdd")).Replace("HHMM", DateTime.Now.ToString("HHmm"));
            var fullPath = Path.Combine(_options.ReportPath, filename);

            _logger.LogInformation($"Report Filepath: {fullPath}");

            // design choice to replace already existing file (same date and time) with same filename
            if (!File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return fullPath;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service starting..");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopping..");
            return base.StopAsync(cancellationToken);
        }
    }
}