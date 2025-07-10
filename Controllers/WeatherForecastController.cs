using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIServer.Net.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly Meter meter = new Meter("WebAPIServer.Net", "1.0");
        private static readonly Counter<long> pendingCount = meter.CreateCounter<long>("WeatherForecast_Metric_Gantry.Net");

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            CounterMetricTest();

            _logger.LogInformation("Begin WeatherForecast Get");

            WeatherForecast[] forecast = default;
            var rng = new Random();
            forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("WeatherForecasts generated {count}: {forecasts}", forecast.Length, forecast);
            _logger.LogError($"This error is intentionally generated..");
            _logger.LogInformation("End WeatherForecast Get");

            return forecast;
        }

        public void CounterMetricTest()
        {
            _logger.LogError($"CounterMetricTest started");
            int counter = 1;
            for (int i = 1; i <= 5; i++)
            {
                pendingCount.Add(5 - i, new KeyValuePair<string, object>("HostNameUpdate", "10.137.246.20"), new KeyValuePair<string, object>("PluginNameUpdate", "ReturnUtilityPlugIn"));
                counter++;
            }
            _logger.LogError($"CounterMetricTest completed");
        }

        [HttpGet]
        [Route("httprequest")]
        public string GetData()
        {
            string response;
            using (var client = new HttpClient())
            {
                response = client.GetStringAsync("http://google.com").Result;
            }

            return response.Substring(0, 2000);
        }
    }
}
